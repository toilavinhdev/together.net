using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PrefixAggregate;

namespace Service.Community.Application.Features.FeaturePrefix.Commands;

public sealed class UpdatePrefixCommand : IBaseRequest
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = default!;

    public string Foreground { get; set; } = default!;

    public string Background { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdatePrefixCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Foreground).NotEmpty();
            RuleFor(x => x.Background).NotEmpty();
        }
    }

    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context, IRedisService redisService)
        : BaseRequestHandler<UpdatePrefixCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdatePrefixCommand request, CancellationToken ct)
        {
            var prefix = await context.Prefixes.FirstOrDefaultAsync(p => p.Id == request.Id, ct);

            if (prefix is null) throw new TogetherException(ErrorCodes.Prefix.PrefixNotFound);

            prefix.Name = request.Name;
            prefix.Background = request.Background;
            prefix.Foreground = request.Foreground;
            prefix.MarkUserModified(UserClaimsPrincipal.Id);

            context.Prefixes.Update(prefix);

            await context.SaveChangesAsync(ct);

            if (await redisService.StringGetAsync<Prefix>(RedisKeys.Community<Prefix>(prefix.Id)) is not null)
            {
                await redisService.StringSetAsync(RedisKeys.Community<Prefix>(prefix.Id), prefix);
            }

            Message = "Updated";
        }
    }
}