using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Service.Community.Application.Features.FeaturePrefix.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PrefixAggregate;

namespace Service.Community.Application.Features.FeaturePrefix.Commands;

public sealed class CreatePrefixCommand : IBaseRequest<CreatePrefixResponse>
{
    public string Name { get; set; } = default!;

    public string Foreground { get; set; } = default!;

    public string Background { get; set; } = default!;
    
    public class Validator : AbstractValidator<CreatePrefixCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Foreground).NotEmpty();
            RuleFor(x => x.Background).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context, IRedisService redisService) 
        : BaseRequestHandler<CreatePrefixCommand, CreatePrefixResponse>(httpContextAccessor)
    {
        protected override async Task<CreatePrefixResponse> HandleAsync(CreatePrefixCommand request, CancellationToken ct)
        {
            var prefix = request.MapTo<Prefix>();
            prefix.MarkUserCreated(UserClaimsPrincipal.Id);

            await context.Prefixes.AddAsync(prefix, ct);

            await context.SaveChangesAsync(ct);

            await redisService.StringSetAsync(RedisKeys.Community<Prefix>(prefix.Id), prefix);
            
            Message = "Created";
            
            return prefix.MapTo<CreatePrefixResponse>();
        }
    }
}