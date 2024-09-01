using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeaturePost.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.PostAggregate;

namespace Service.Community.Application.Features.FeaturePost.Commands;

public sealed class CreatePostCommand : IBaseRequest<CreatePostResponse>
{
    public Guid TopicId { get; set; }
    
    public Guid? PrefixId { get; set; }
    
    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;
    
    public class Validator : AbstractValidator<CreatePostCommand>
    {
        public Validator()
        {
            RuleFor(x => x.TopicId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Body).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<CreatePostCommand, CreatePostResponse>(httpContextAccessor)
    {
        protected override async Task<CreatePostResponse> HandleAsync(CreatePostCommand request, CancellationToken ct)
        {
            var topic = await context.Topics.FirstOrDefaultAsync(x => x.Id == request.TopicId, ct);
            if (topic is null) throw new TogetherException(ErrorCodes.Topic.TopicNotFound);

            if (request.PrefixId is not null && !await context.Prefixes.AnyAsync(p => p.Id == request.PrefixId, ct))
                throw new TogetherException(ErrorCodes.Prefix.PrefixNotFound);
            
            var post = new Post
            {
                Id = Guid.NewGuid(),
                TopicId = topic.Id,
                ForumId = topic.ForumId,
                PrefixId = request.PrefixId,
                Title = request.Title,
                Body = request.Body
            };
            post.MarkUserCreated(UserClaimsPrincipal.Id);

            await context.Posts.AddAsync(post, ct);

            await context.SaveChangesAsync(ct);

            Message = "Created";

            return post.MapTo<CreatePostResponse>();
        }
    }
}