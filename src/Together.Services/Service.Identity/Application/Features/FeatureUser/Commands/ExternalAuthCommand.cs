using System.Security.Authentication;
using FluentValidation;
using Google.Apis.Auth;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Enums;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.Utilities;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Domain;
using Service.Identity.Domain.Aggregates.UserAggregate;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class ExternalAuthCommand : IBaseRequest<AuthToken>
{
    public string Credential { get; set; } = default!;
    
    public class Validator : AbstractValidator<ExternalAuthCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Credential).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        IdentityContext context, 
        AppSettings appSettings,
        IRedisService redisService) 
        : BaseRequestHandler<ExternalAuthCommand, AuthToken>(httpContextAccessor)
    {
        protected override async Task<AuthToken> HandleAsync(ExternalAuthCommand request, CancellationToken ct)
        {
            var payload = await VerifyGoogleCredential(request.Credential);
            if (payload is null) throw new InvalidCredentialException();
            
            var user = await context.Users
                .Include(u => u.UserRoles)!
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Email == payload.Email, ct);
            
            if (user is null)
            {
                var memberRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Member", ct);
                if (memberRole is null) throw new ApplicationException("Member role has not been initialized");
                var fullName = payload.Name;
                user = new User
                {
                    Id = Guid.NewGuid(),
                    FullName = fullName,
                    UserName = $"{fullName.Slugify().Replace("-", "")}-{4.RandomString("0123456789")}",
                    Email = payload.Email,
                    PasswordHash = 12.RandomString().ToSha256(),
                    UserRoles = [new UserRole { RoleId = memberRole.Id }],
                    Avatar = payload.Picture,
                    Status = UserStatus.Active,
                    Gender = Gender.Other
                };
                user.MarkCreated();
                await context.Users.AddAsync(user, ct);
                await context.SaveChangesAsync(ct);
            }
            
            var tokenModel = new AuthToken
            {
                AccessToken = JwtUtils.GenerateAccessToken(appSettings.JwtConfig, user.MapTo<UserClaimsPrincipal>()),
                RefreshToken = 20.RandomString()
            };
            
            await redisService.StringSetAsync(
                RedisKeys.Identity<IdentityPrivilege>(user.Id), 
                user.MapTo<IdentityPrivilege>());

            await redisService.StringSetAsync(
                RedisKeys.Identity<AuthToken>(user.Id), 
                tokenModel,
                TimeSpan.FromDays(appSettings.JwtConfig.RefreshTokenDurationInDays));
            
            Message = "External login successful";
            return tokenModel;
        }

        private async Task<string> RandomUserNameAsync()
        {
            while (true)
            {
                var random = 24.RandomString().ToLower();
                if (await context.Users.AnyAsync(u => u.UserName == random)) continue;
                return random;
            }
        }
        
        private async Task<GoogleJsonWebSignature.Payload?> VerifyGoogleCredential(string credential)
        {
            try
            {
                return await GoogleJsonWebSignature.ValidateAsync(
                    credential,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = [appSettings.GoogleConfig.ClientId]
                    });
            }
            catch
            {
                return null;
            }
        }
    }
}