using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using MinimalApiTemplate.REST.Common.Interfaces;
using MinimalApiTemplate.REST.Entities;
using MinimalApiTemplate.REST.Enums;
#if RabbitMQ
using MinimalApiTemplate.REST.MessageBroker.Interfaces;
using MinimalApiTemplate.REST.MessageBroker.Interfaces;
using MinimalApiTemplate.REST.MessageBroker.Messages;
#endif

namespace MinimalApiTemplate.REST.Features.Auth.Login;

public record LoginRequest(string Email) : IRequest;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(c => c.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not in correct format");
    }
}

public class LoginRequestHandler : IRequestHandler<LoginRequest>
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;
#if RabbitMQ
    private readonly IQueuePublisher _messagePublisher;
#else
    private readonly IEmailService _emailService;
#endif

#if RabbitMQ
    public LoginRequestHandler(UserManager<User> userManager, IConfiguration configuration, IQueuePublisher messagePublisher)
    {
        _userManager = userManager;
        _configuration = configuration;
        _messagePublisher = messagePublisher;
    }
#else
    public LoginRequestHandler(UserManager<User> userManager, IConfiguration configuration, IEmailService emailService)
    {
        _userManager = userManager;
        _configuration = configuration;
        _emailService = emailService;
    }
#endif

    public async Task Handle(LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            user = new User()
            {
                Email = request.Email,
                UserName = request.Email
            };

            var userCreationResult = await _userManager.CreateAsync(user);
            userCreationResult.ThrowIfNotSuccessful("Error occurred during user creation.");
        }

        var loginConfirmationToken = await _userManager.GenerateUserTokenAsync(user, LoginTokenProvider.ProviderName, LoginTokenProvider.Purpose);
        var confirmationUrl = _configuration["WebApp:LoginConfirmationUrl"].Replace("*email*", user.Email).Replace("*token*", loginConfirmationToken);

#if RabbitMQ
        _messagePublisher.PublishMessage(new EmailMessage()
        {
            Email = request.Email,
            Purpose = EmailPurpose.EmailConfirmation,
            Parameters = new { confirmationUrl }
        });
#else
        await _emailService.SendEmail(request.Email, EmailPurpose.EmailConfirmation, new { confirmationUrl });
#endif
    }
}

