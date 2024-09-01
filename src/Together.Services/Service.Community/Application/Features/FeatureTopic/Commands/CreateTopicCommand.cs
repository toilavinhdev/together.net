using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureTopic.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.TopicAggregate;

namespace Service.Community.Application.Features.FeatureTopic.Commands;

public sealed class CreateTopicCommand : IBaseRequest<CreateTopicResponse>
{
    public Guid ForumId { get; set; }
    
    public string Name { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public class Validator : AbstractValidator<CreateTopicCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ForumId).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<CreateTopicCommand, CreateTopicResponse>(httpContextAccessor)
    {
        protected override async Task<CreateTopicResponse> HandleAsync(CreateTopicCommand request, CancellationToken ct)
        {
            if (!await context.Forums.AnyAsync(x => x.Id == request.ForumId, ct))
                throw new TogetherException(ErrorCodes.Forum.ForumNotFound);

            var topic = request.MapTo<Topic>();
            topic.MarkUserCreated(UserClaimsPrincipal.Id);

            await context.Topics.AddAsync(topic, ct);

            await context.SaveChangesAsync(ct);

            Message = "Created";

            return topic.MapTo<CreateTopicResponse>();
        }
    }
}