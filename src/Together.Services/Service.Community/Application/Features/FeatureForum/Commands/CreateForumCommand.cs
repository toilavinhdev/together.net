using FluentValidation;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Service.Community.Application.Features.FeatureForum.Responses;
using Service.Community.Domain;
using Service.Community.Domain.Aggregates.ForumAggregate;

namespace Service.Community.Application.Features.FeatureForum.Commands;

public sealed class CreateForumCommand : IBaseRequest<CreateForumResponse>
{
    public string Name { get; set; } = default!;
    
    public class Validator : AbstractValidator<CreateForumCommand>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) : 
        BaseRequestHandler<CreateForumCommand, CreateForumResponse>(httpContextAccessor)
    {
        protected override async Task<CreateForumResponse> HandleAsync(CreateForumCommand request, CancellationToken ct)
        {
            var forum = request.MapTo<Forum>();
            forum.MarkUserCreated(UserClaimsPrincipal.Id);
            await context.AddAsync(forum, ct);
            await context.SaveChangesAsync(ct);
            Message = "Created";
            return forum.MapTo<CreateForumResponse>();
        }
    }
}