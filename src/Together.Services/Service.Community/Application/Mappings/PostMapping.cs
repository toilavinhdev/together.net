using AutoMapper;
using Service.Community.Application.Features.FeaturePost.Commands;
using Service.Community.Application.Features.FeaturePost.Responses;
using Service.Community.Domain.Aggregates.PostAggregate;

namespace Service.Community.Application.Mappings;

public sealed class PostMapping : Profile
{
    public PostMapping()
    {
        CreateMap<Post, CreatePostResponse>();
        
        CreateMap<VotePostCommand, PostVote>();
    }
}