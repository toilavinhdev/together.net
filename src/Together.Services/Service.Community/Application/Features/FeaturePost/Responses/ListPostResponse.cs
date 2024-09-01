using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.BusinessObjects;
using Infrastructure.SharedKernel.ValueObjects;

namespace Service.Community.Application.Features.FeaturePost.Responses;

public class ListPostResponse : PaginationResult<PostViewModel>
{
    public Dictionary<string, object>? Extra { get; set; }
}

public class PostViewModel : TimeTrackingEntity
{
    public Guid ForumId { get; set; }
    
    public Guid TopicId { get; set; }

    public string TopicName { get; set; } = default!;
    
    public Guid? PrefixId { get; set; }

    public string? PrefixName { get; set; } = default!;
    
    public string? PrefixForeground { get; set; } = default!;
    
    public string? PrefixBackground { get; set; } = default!;
    
    public string Title { get; set; } = default!;

    public string Body { get; set; } = default!;

    public GeneralUser Author { get; set; } = default!;
    
    public long ReplyCount { get; set; }
}