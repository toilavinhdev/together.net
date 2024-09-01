using Infrastructure.PostgreSQL;

namespace Service.Community.Application.Features.FeatureReply.Responses;

public class CreateReplyResponse : TimeTrackingEntity
{
    public Guid PostId { get; set; }
    
    public Guid? ParentId { get; set; }
    
    public int Level { get; set; }
    
    public string Body { get; set; } = default!;
}