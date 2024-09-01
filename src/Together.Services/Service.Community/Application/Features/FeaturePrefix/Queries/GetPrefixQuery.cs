using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeaturePrefix.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PrefixAggregate;

namespace Service.Community.Application.Features.FeaturePrefix.Queries;

public sealed class GetPrefixQuery(Guid id) : IBaseRequest<PrefixViewModel>
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<GetPrefixQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context, IRedisService redisService)
        : BaseRequestHandler<GetPrefixQuery, PrefixViewModel>(httpContextAccessor)
    {
        protected override async Task<PrefixViewModel> HandleAsync(GetPrefixQuery request, CancellationToken ct)
        {
            var cachedPrefix = await redisService
                .StringGetAsync<Prefix>(RedisKeys.Community<Prefix>(request.Id));
            if (cachedPrefix is not null) return cachedPrefix.MapTo<PrefixViewModel>();
            
            var prefix = await context.Prefixes
                .Where(p => p.Id == request.Id)
                .Select(p => p.MapTo<PrefixViewModel>())
                .FirstOrDefaultAsync(ct);

            if (prefix is null) throw new TogetherException(ErrorCodes.Prefix.PrefixNotFound);
            
            return prefix;
        }
    }
}