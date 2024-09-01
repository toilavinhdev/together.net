using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureTopic.Commands;

public sealed class DeleteTopicCommand(Guid id) : IBaseRequest
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<DeleteTopicCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<DeleteTopicCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(DeleteTopicCommand request, CancellationToken ct)
        {
            var topic = await context.Topics.FirstOrDefaultAsync(t => t.Id == request.Id, ct);

            if (topic is null) throw new TogetherException(ErrorCodes.Topic.TopicNotFound);

            context.Topics.Remove(topic);

            await context.SaveChangesAsync(ct);

            Message = "Deleted";
        }
    }
}