using Infrastructure.SharedKernel.Mediator;
using Microsoft.EntityFrameworkCore;
using Service.Community.Application.Features.FeatureForum.Responses;
using Service.Community.Domain;

namespace Service.Community.Application.Features.FeatureForum.Queries;

public class ListForumQuery : IBaseRequest<List<ForumViewModel>>
{
    public bool? IncludeTopics { get; set; }
    
    internal class Handler(IHttpContextAccessor httpContextAccessor, CommunityContext context) 
        : BaseRequestHandler<ListForumQuery, List<ForumViewModel>>(httpContextAccessor)
    {
        protected override async Task<List<ForumViewModel>> HandleAsync(ListForumQuery request, CancellationToken ct)
        {
            var queryable = context.Forums.AsQueryable();

            if (request.IncludeTopics != null && request.IncludeTopics.Value)
            {
                queryable = queryable.Include(f => f.Topics)!
                    .ThenInclude(f => f.Posts)!
                    .ThenInclude(f => f.Replies);
            }
            
            var data = await queryable
                .OrderBy(f => f.CreatedAt)
                .Select(f => new ForumViewModel
                {
                    Id = f.Id,
                    SubId = f.SubId,
                    Name = f.Name,
                    CreatedAt = f.CreatedAt,
                    Topics = request.IncludeTopics != null && request.IncludeTopics.Value 
                        ? f.Topics!.Select(t => new TopicViewModel
                            {
                                Id = t.Id,
                                SubId = t.SubId,
                                ForumId = t.ForumId,
                                ForumName = t.Forum.Name,
                                Name = t.Name,
                                Description = t.Description,
                                CreatedAt = t.CreatedAt,
                                PostCount = t.Posts!.Count,
                                ReplyCount = t.Posts!.SelectMany(p => p.Replies!).LongCount(),
                            }).ToList() 
                        : new List<TopicViewModel>()
                })
                .ToListAsync(ct);

            return data;
        }
    }
}