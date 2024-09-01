using FluentValidation;
using Infrastructure.Redis;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PrefixAggregate;

namespace Service.Community.Application.Features.FeaturePrefix.Commands;

public sealed class DeletePrefixCommand(Guid id) : IBaseRequest
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<DeletePrefixCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context, IRedisService redisService) 
        : BaseRequestHandler<DeletePrefixCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(DeletePrefixCommand request, CancellationToken ct)
        {
            var prefix = await context.Prefixes.FirstOrDefaultAsync(p => p.Id == request.Id, ct);

            if (prefix is null) throw new TogetherException(ErrorCodes.Prefix.PrefixNotFound);

            context.Prefixes.Remove(prefix);

            await context.SaveChangesAsync(ct);

            await redisService.KeyDeleteAsync(RedisKeys.Community<Prefix>(prefix.Id));

            Message = "Deleted";
        }
    }
}