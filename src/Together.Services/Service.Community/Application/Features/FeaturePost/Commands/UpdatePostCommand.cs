using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeaturePost.Commands;

public sealed class UpdatePostCommand : IBaseRequest
{
    public Guid Id { get; set; }
    
    public Guid TopicId { get; set; }
    
    public Guid? PrefixId { get; set; }
    
    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdatePostCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.TopicId).NotEmpty();
            RuleFor(x => x.Title).NotEmpty();
            RuleFor(x => x.Body).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<UpdatePostCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdatePostCommand request, CancellationToken ct)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, ct); 
            if (post is null) throw new TogetherException(ErrorCodes.Post.PostNotFound);
            
            if (post.PrefixId != request.PrefixId &&
                !await context.Prefixes.AnyAsync(pf => pf.Id == request.PrefixId, ct))
                throw new TogetherException(ErrorCodes.Prefix.PrefixNotFound);

            if (post.TopicId != request.TopicId)
            {
                var topic = await context.Topics.FirstOrDefaultAsync(x => x.Id == request.TopicId, ct);
                if (topic is null) throw new TogetherException(ErrorCodes.Topic.TopicNotFound);
                post.TopicId = request.TopicId;
                post.ForumId = topic.ForumId;
            }

            post.PrefixId = request.PrefixId;
            post.Title = request.Title;
            post.Body = request.Body;
            post.MarkUserModified(UserClaimsPrincipal.Id);

            context.Posts.Update(post);

            await context.SaveChangesAsync(ct);

            Message = "Updated";
        }
    }
}