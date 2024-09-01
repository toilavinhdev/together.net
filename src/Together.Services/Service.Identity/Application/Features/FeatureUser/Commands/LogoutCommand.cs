using Infrastructure.Redis;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;

namespace Service.Identity.Application.Features.FeatureUser.Commands;

public sealed class LogoutCommand : IBaseRequest
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, IRedisService redisService) 
        : BaseRequestHandler<LogoutCommand>(httpContextAccessor)
    {
        protected override Task HandleAsync(LogoutCommand request, CancellationToken ct)
        {
            redisService.KeyDeleteAsync(RedisKeys.Identity<AuthToken>(UserClaimsPrincipal.Id));
            
            Message = "Logout!";
            
            return Task.CompletedTask;
        }
    }
}