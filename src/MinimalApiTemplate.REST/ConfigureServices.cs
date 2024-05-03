using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
#if Dapper
using TH.DapperIdentity.Extensions;
using TH.DapperIdentity.SqlServer;
#endif
using MinimalApiTemplate.REST.Common.Interfaces;
using MinimalApiTemplate.REST.Entities;
using MinimalApiTemplate.REST.Features.Auth;
#if RabbitMQ
using RabbitMQ.Client;
using MinimalApiTemplate.REST.MessageBroker;
using MinimalApiTemplate.REST.MessageBroker.Consumers;
using MinimalApiTemplate.REST.MessageBroker.Interfaces;
using MinimalApiTemplate.REST.MessageBroker.Messages;
#endif
using MinimalApiTemplate.REST.Persistance;
using MinimalApiTemplate.REST.Services.Email;
#if EF
using Microsoft.EntityFrameworkCore;
#endif

namespace MinimalApiTemplate.REST;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

        services.AddEndpointsApiExplorer()
                .AddSwaggerGen()
                .AddExceptionHandler<GlobalExceptionHandler>()
                .AddProblemDetails()
                .AddSingleton(TimeProvider.System)
                .AddHttpContextAccessor()
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddMediatRServices()
#if EF
                .AddEntityFrameworkServices(configuration)
#endif
                .AddScoped<DatabaseStateManager>()
                .AddIdentityServices(configuration)
                .AddAuthenticationServices(configuration)
#if RabbitMQ
                .AddMessageBrokerAndConsumerServices(configuration)
#endif
                .AddEmailServices(configuration);

        return services;
    }

    private static IServiceCollection AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        return services;
    }
#if EF
    private static IServiceCollection AddEntityFrameworkServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("Default"), b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        return services;
    }
#endif
    private static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtConfig>().Bind(configuration.GetSection(JwtConfig.SectionName));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtConfig>>().Value);
#if Dapper
        services.AddDataProtection();
#endif
        services.AddIdentityCore<User>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Lockout.AllowedForNewUsers = false;
        })
#if EF
        .AddEntityFrameworkStores<AppDbContext>()
#endif
#if Dapper
        .AddDapperStores<SqlServerDbConnectionFactory>(configuration.GetConnectionString("Default"), options =>
        {
            options.AddSqlServerIdentityRepositories<User, int>();
            options.TableNames.UsersTableName = "Users";
            options.TableNames.RolesTableName = "Roles";
            options.TableNames.UserRolesTableName = "UserRoles";
            options.TableNames.UserClaimsTableName = "UserClaims";
            options.TableNames.RoleClaimsTableName = "RoleClaims";
            options.TableNames.UserLoginsTableName = "UserLogins";
            options.TableNames.UserTokensTableName = "UserTokens";
        })
#endif
        .AddDefaultTokenProviders()
        .AddTokenProvider(LoginTokenProvider.ProviderName, typeof(LoginTokenProvider));

        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = configuration["JWT:Issuer"],
                ValidAudience = configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Secret"]))
            };
        })
        .AddCookie();

        //add sign in with google/facebook etc. here

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
        });

        services.AddAuthorization();

        return services;
    }
#if RabbitMQ
    private static IServiceCollection AddMessageBrokerAndConsumerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RabbitMQConfig>().Bind(configuration.GetSection(RabbitMQConfig.SectionName));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<RabbitMQConfig>>().Value);

        services.AddSingleton<IConnectionFactory>(_ => new ConnectionFactory()
        {
            Uri = new Uri(configuration["RabbitMQ:Uri"]),
            ClientProvidedName = "MinimalApiTemplate",
            DispatchConsumersAsync = true
        });

        services.AddSingleton<RabbitMQPersistentConnection>();
        services.AddScoped<RabbitMQInitializer>();
        services.AddScoped<IQueuePublisher, RabbitMQPublisher>();

        services.AddQueueMessageConsumer<EmailConsumer, EmailMessage>();

        return services;
    }
#endif
    private static IServiceCollection AddEmailServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<SendGridConfig>().Bind(configuration.GetSection(SendGridConfig.SectionName));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<SendGridConfig>>().Value);

        services.AddScoped<IEmailService, SendGridEmailService>();

        return services;
    }
}