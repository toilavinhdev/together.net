using Infrastructure.PostgreSQL;
using Infrastructure.SharedKernel.ValueObjects;
using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeatureReply.Responses;

public class ReplyResponse : PaginationResult<ReplyViewModel>
{
    public ReplyViewModel? Parent { get; set; }
    
    public ReplyViewModel? FocusChild { get; set; }
}

public class ReplyViewModel : ModifierTrackingEntity
{
    public Guid PostId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public int Level { get; set; }
    
    public string Body { get; set; } = default!;

    public string CreatedByUserName { get; set; } = default!;
    
    public string? CreatedByAvatar { get; set; }
    
    public long VoteUpCount { get; set; }
    
    public long VoteDownCount { get; set; }
    
    public VoteType? Voted { get; set; }
    
    public long ChildCount { get; set; }
    
    public List<ReplyViewModel>? Children { get; set; }
}