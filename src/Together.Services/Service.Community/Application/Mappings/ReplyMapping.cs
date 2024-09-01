using AutoMapper;
using Service.Community.Application.Features.FeatureReply.Commands;
using Service.Community.Application.Features.FeatureReply.Responses;
using Service.Community.Domain.Aggregates.ReplyAggregate;

namespace Service.Community.Application.Mappings;

public sealed class ReplyMapping : Profile
{
    public ReplyMapping()
    {
        CreateMap<CreateReplyCommand, Reply>();
        
        CreateMap<Reply, CreateReplyResponse>();
        
        CreateMap<VoteReplyCommand, ReplyVote>();
    }
}