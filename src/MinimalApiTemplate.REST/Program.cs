using Serilog;
using MinimalApiTemplate.REST;
using MinimalApiTemplate.REST.Persistance;
#if RabbitMQ
using MinimalApiTemplate.REST.MessageBroker;
#endif
#if Dapper
using MinimalApiTemplate.REST.Persistance;
#endif

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, lc) => lc
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(ctx.Configuration));

    builder.Services.AddServices();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        using var scope = app.Services.CreateScope();

        var databaseStateManager = scope.ServiceProvider.GetRequiredService<DatabaseStateManager>();
#if Dapper
        databaseStateManager.ApplyMigrations();
#else
        databaseStateManager.ApplyMigrations().Wait();
#endif

#if RabbitMQ
        var messageBrokerStateManager = scope.ServiceProvider.GetRequiredService<RabbitMQInitializer>();
        messageBrokerStateManager.InitializeExchangesAndQueues();
#endif
    }

    app.UseHttpsRedirection();

    app.UseExceptionHandler();

    app.MapEndpoints();

    app.UseAuthentication();
    app.UseAuthorization();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Failed to start");
}
finally
{
    Log.Information("Shutting down");
    Log.CloseAndFlush();
}
