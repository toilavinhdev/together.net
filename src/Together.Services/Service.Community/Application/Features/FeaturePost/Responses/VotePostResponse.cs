using Service.Community.Domain.Enums;

namespace Service.Community.Application.Features.FeaturePost.Responses;

public class VotePostResponse
{
    public Guid SourceId { get; set; }
    
    public VoteType? Value { get; set; }
    
    public bool IsVoted { get; set; }
}