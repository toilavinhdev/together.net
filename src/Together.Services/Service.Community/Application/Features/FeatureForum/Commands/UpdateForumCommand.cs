using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureForum.Commands;

public sealed class UpdateForumCommand : IBaseRequest
{
    public Guid Id { get; set; }
    
    public string Name { get; set; } = default!;
    
    public class Validator : AbstractValidator<UpdateForumCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Name).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<UpdateForumCommand>(httpContextAccessor)
    {
        protected override async Task HandleAsync(UpdateForumCommand request, CancellationToken ct)
        {
            var forum = await context.Forums.FirstOrDefaultAsync(f => f.Id == request.Id, ct);
            if (forum is null) throw new TogetherException(ErrorCodes.Forum.ForumNotFound);
            forum.Name = request.Name;
            forum.MarkUserModified(UserClaimsPrincipal.Id);
            context.Forums.Update(forum);
            await context.SaveChangesAsync(ct);
            Message = "Updated";
        }
    }
}