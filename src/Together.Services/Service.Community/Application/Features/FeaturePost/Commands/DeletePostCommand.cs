using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeaturePost.Commands;

public sealed class DeletePostCommand(Guid id) : IBaseRequest
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<DeletePostCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context)
        : BaseRequestHandler<DeletePostCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(DeletePostCommand request, CancellationToken ct)
        {
            var post = await context.Posts.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
            if (post is null) throw new TogetherException(ErrorCodes.Post.PostNotFound);

            context.Posts.Remove(post);

            await context.SaveChangesAsync(ct);
            
            Message = "Deleted";
        }
    }
}