using Infrastructure.Redis;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Service.Identity.Application.Features.FeatureUser.Responses;
using Service.Identity.Domain;

namespace Service.Identity.Application.Features.FeatureUser.Queries;

public sealed class MeQuery : IBaseRequest<MeResponse>
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, 
        IdentityContext context,
        IRedisService redisService) 
        : BaseRequestHandler<MeQuery, MeResponse>(httpContextAccessor)
    {
        protected override async Task<MeResponse> HandleAsync(MeQuery request, CancellationToken ct)
        {
            var cachedUser = await redisService.StringGetAsync<IdentityPrivilege>(
                RedisKeys.Identity<IdentityPrivilege>(UserClaimsPrincipal.Id));

            if (cachedUser is null)
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == UserClaimsPrincipal.Id, ct);
                if (user is null) throw new TogetherException(ErrorCodes.User.UserNotFound);
                cachedUser = user.MapTo<IdentityPrivilege>();
                await redisService.StringSetAsync(RedisKeys.Identity<IdentityPrivilege>(user.Id), cachedUser);
            }

            return cachedUser.MapTo<MeResponse>();
        }
    }
}