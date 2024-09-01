using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Utilities;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class RefreshTokenCommand : IBaseRequest<AuthToken>
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;
    
    public class Validator : AbstractValidator<RefreshTokenCommand>
    {
        public Validator()
        {
            RuleFor(x => x.AccessToken).NotEmpty();
            RuleFor(x => x.RefreshToken).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        IdentityContext context, 
        AppSettings appSettings,
        IRedisService redisService) 
        : BaseRequestHandler<RefreshTokenCommand, AuthToken>(httpContextAccessor)
    {
        protected override async Task<AuthToken> HandleAsync(RefreshTokenCommand request, CancellationToken ct)
        {
            var claims = JwtUtils.DecodeAccessToken(request.AccessToken);
            
            var user = await context.Users
                .Include(u => u.UserRoles)!
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == claims.Id, ct);
            
            if (user is null) throw new TogetherException(ErrorCodes.User.RefreshTokenFailed, "User not found"); 
            
            // RT hết hạn(TTL Redis = 0) hoặc không khớp
            var existedTokenModel = await redisService
                .StringGetAsync<AuthToken>(RedisKeys.Identity<AuthToken>(user.Id));
            if (existedTokenModel is null || request.RefreshToken != existedTokenModel.RefreshToken) 
                throw new TogetherException(ErrorCodes.User.RefreshTokenFailed, "Invalid refresh token");
            
            var tokenModel = new AuthToken
            {
                AccessToken = JwtUtils.GenerateAccessToken(appSettings.JwtConfig, user.MapTo<UserClaimsPrincipal>()),
                RefreshToken = existedTokenModel.RefreshToken // Giữ nguyên TTL refresh token cũ
            };
            
            // TTL = Duration
            await redisService.StringSetAsync(
                RedisKeys.Identity<AuthToken>(user.Id), 
                tokenModel,
                TimeSpan.FromDays(appSettings.JwtConfig.RefreshTokenDurationInDays));
            
            await redisService.StringSetAsync(
                RedisKeys.Identity<IdentityPrivilege>(user.Id),
                user.MapTo<IdentityPrivilege>());

            return tokenModel;
        }
    }
}