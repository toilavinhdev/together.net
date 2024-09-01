using AutoMapper;
using Service.Community.Application.Features.FeatureForum.Responses;
using Service.Community.Application.Features.FeatureTopic.Commands;
using Service.Community.Application.Features.FeatureTopic.Responses;
using Service.Community.Domain.Aggregates.TopicAggregate;

namespace Service.Community.Application.Mappings;

public sealed class TopicMapping : Profile
{
    public TopicMapping()
    {
        CreateMap<CreateTopicCommand, Topic>();
        
        CreateMap<Topic, CreateTopicResponse>();
        
        CreateMap<Topic, GetTopicResponse>();

        CreateMap<Topic, TopicViewModel>()
            .ForMember(viewModel => viewModel.PostCount, cfg => cfg
                .MapFrom(topic => topic.Posts!.LongCount()))
            .ForMember(viewModel => viewModel.ReplyCount, cfg => cfg
                .MapFrom(topic => topic.Posts!.SelectMany(p => p.Replies!).LongCount()));
    }
}