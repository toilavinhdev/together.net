using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureForum.Responses;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureForum.Queries;

public sealed class GetForumQuery(Guid id) : IBaseRequest<GetForumResponse>
{
    private Guid Id { get; set; } = id;
    
    public class Validator : AbstractValidator<GetForumQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<GetForumQuery, GetForumResponse>(httpContextAccessor)
    {
        protected override async Task<GetForumResponse> HandleAsync(GetForumQuery request, CancellationToken ct)
        {
            var forum = await context.Forums
                .Where(f => f.Id == request.Id)
                .Select(f => f.MapTo<GetForumResponse>())
                .FirstOrDefaultAsync(ct);

            if (forum is null) throw new TogetherException(ErrorCodes.Forum.ForumNotFound);

            return forum;
        }
    }
}