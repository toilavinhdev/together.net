using Infrastructure.Redis;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeaturePrefix.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PrefixAggregate;

namespace Service.Community.Application.Features.FeaturePrefix.Queries;

public sealed class ListPrefixQuery : IBaseRequest<List<PrefixViewModel>>
{
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context, IRedisService redisService) 
        : BaseRequestHandler<ListPrefixQuery, List<PrefixViewModel>>(httpContextAccessor)
    {
        protected override async Task<List<PrefixViewModel>> HandleAsync(ListPrefixQuery request, CancellationToken ct)
        {
            var cachedPrefixKeys = await redisService.KeysByPatternAsync(RedisKeys.Community<Prefix>("*"));

            if (cachedPrefixKeys.Count != 0)
            {
                var cachedPrefixes = new List<PrefixViewModel>();
                
                foreach (var cachedPrefixId in cachedPrefixKeys.Select(key => key.Split(":")[1]))
                {
                    var cachedPrefix = await redisService
                        .StringGetAsync<PrefixViewModel>(RedisKeys.Community<Prefix>(cachedPrefixId));
                    if (cachedPrefix is null) continue;
                    cachedPrefixes.Add(cachedPrefix!);
                }

                return cachedPrefixes;
            }
            
            var prefixes = await context.Prefixes.ToListAsync(ct);

            foreach (var prefix in prefixes)
            {
                await redisService.StringSetAsync(RedisKeys.Community<Prefix>(prefix.Id), prefix);
            }
            
            return prefixes
                .Select(prefix => prefix.MapTo<PrefixViewModel>())
                .ToList();
        }
    }
}