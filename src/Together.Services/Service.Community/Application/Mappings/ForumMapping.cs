using AutoMapper;
using Service.Community.Application.Features.FeatureForum.Commands;
using Service.Community.Application.Features.FeatureForum.Responses;
using Service.Community.Domain.Aggregates.ForumAggregate;

namespace Service.Community.Application.Mappings;

public sealed class ForumMapping : Profile
{
    public ForumMapping()
    {
        CreateMap<CreateForumCommand, Forum>();
        
        CreateMap<Forum, CreateForumResponse>();

        CreateMap<Forum, ForumViewModel>();
        
        CreateMap<Forum, GetForumResponse>();
    }
}