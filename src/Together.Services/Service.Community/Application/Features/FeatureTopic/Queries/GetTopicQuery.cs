using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureTopic.Responses;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureTopic.Queries;

public sealed class GetTopicQuery(Guid id) : IBaseRequest<GetTopicResponse>
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<GetTopicQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context)
        : BaseRequestHandler<GetTopicQuery, GetTopicResponse>(httpContextAccessor)
    {
        protected override async Task<GetTopicResponse> HandleAsync(GetTopicQuery request, CancellationToken ct)
        {
            var topic = await context.Topics
                .Where(t => t.Id == request.Id)
                .Select(t => t.MapTo<GetTopicResponse>())
                .FirstOrDefaultAsync(ct);

            if (topic is null) throw new TogetherException(ErrorCodes.Topic.TopicNotFound);

            return topic;
        }
    }
}