using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Utilities;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class SignInCommand : IBaseRequest<AuthToken>
{
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
    
    public class Validator : AbstractValidator<SignInCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().Matches(RegexUtils.EmailRegex);
            RuleFor(x => x.Password).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        IdentityContext context,
        AppSettings appSettings,
        IRedisService redisService) 
        : BaseRequestHandler<SignInCommand, AuthToken>(httpContextAccessor)
    {
        protected override async Task<AuthToken> HandleAsync(SignInCommand request, CancellationToken ct)
        {
            var user = await context.Users
                .Include(u => u.UserRoles)!
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email, ct);
            if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);
            
            if (user.PasswordHash != request.Password.ToSha256())
                throw new TogetherException(ErrorCodes.User.IncorrectPassword);
            
            if (user.Status == UserStatus.Banned)
                throw new TogetherException(ErrorCodes.User.AccountHasBeenBanned);
            
            var tokenModel = new AuthToken
            {
                AccessToken = JwtUtils.GenerateAccessToken(appSettings.JwtConfig, user.MapTo<UserClaimsPrincipal>()),
                RefreshToken = 16.RandomString()
            };

            await redisService.StringSetAsync(
                RedisKeys.Identity<IdentityPrivilege>(user.Id),
                user.MapTo<IdentityPrivilege>());
            
            await redisService.StringSetAsync(
                RedisKeys.Identity<AuthToken>(user.Id), 
                tokenModel,
                TimeSpan.FromDays(appSettings.JwtConfig.RefreshTokenDurationInDays));
            
            Message = "Login successful";

            return tokenModel;
        }
    }
}