using FluentValidation;
using Infrastructure.Mail;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Utilities;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class ForgotPasswordCommand : IBaseRequest
{
    public string Email { get; set; } = default!;
    
    public class Validator : AbstractValidator<ForgotPasswordCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().Matches(RegexUtils.EmailRegex);
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        IdentityContext context, 
        AppSettings appSettings, 
        IRedisService redisService) 
        : BaseRequestHandler<ForgotPasswordCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(ForgotPasswordCommand request, CancellationToken ct)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.Email, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);

            var token = 40.RandomString();
            
            var existedToken = await redisService.StringGetAsync(RedisKeys.IdentityForgotPasswordToken(user.Id));
            
            if (existedToken is not null)
            {
                await redisService.KeyDeleteAsync(RedisKeys.IdentityForgotPasswordToken(user.Id));
            }

            await redisService.StringSetAsync(
                RedisKeys.IdentityForgotPasswordToken(user.Id), 
                token,
                TimeSpan.FromHours(appSettings.ForgotPasswordTokenDurationInHours));
            
            // Thread Pool
            await Task.Run(() =>
            {
                _ = MailProvider.SmtpSendAsync(appSettings.MailConfig, new MailForm
                {
                    To = request.Email,
                    Title = "Together.NET Reset Password",
                    Body = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Templates/forgot-password-template.html")
                        .ReadAllText()
                        .Replace("{{user_name}}", user.UserName)
                        .Replace("{{validate_url}}", new UriBuilder($"{appSettings.Metadata.Host}/auth/forgot-password/submit/{user.Id}/{token}").Uri.ToString())
                        .Replace("{{duration_in_hours}}", appSettings.ForgotPasswordTokenDurationInHours.ToString())
                        .Replace("{{contact_email}}", appSettings.MailConfig.Mail)
                });
            }, ct);

            Message = "Please check your email";
        }
    }
}