using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class SubmitForgotPasswordTokenCommand : IBaseRequest
{
    public Guid UserId { get; set; } = default!;
    
    public string Token { get; set; } = default!;

    public string NewPassword { get; set; } = default!;
    
    public class Validator : AbstractValidator<SubmitForgotPasswordTokenCommand>
    {
        public Validator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.Token).NotEmpty();
            RuleFor(x => x.NewPassword).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, IRedisService redisService, IdentityContext context) 
        : BaseRequestHandler<SubmitForgotPasswordTokenCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(SubmitForgotPasswordTokenCommand request, CancellationToken ct)
        {
            var existedToken = await redisService.StringGetAsync(RedisKeys.IdentityForgotPasswordToken(request.UserId));

            if (existedToken is null || existedToken != request.Token)
                throw new TogetherException(ErrorCodes.User.ForgotPasswordTokenInvalid);
            
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);

            user.PasswordHash = request.NewPassword.ToSha256();
            user.MarkModified();

            context.Users.Update(user);

            await redisService.KeyDeleteAsync(RedisKeys.IdentityForgotPasswordToken(request.UserId));
            
            await context.SaveChangesAsync(ct);

            Message = "Success";
        }
    }
}