using AutoMapper;
using Service.Chat.Application.Features.FeatureMessage.Commands;
using Service.Chat.Application.Features.FeatureMessage.Responses;
using Service.Chat.Domain.Aggregates.MessageAggregate;

namespace Service.Chat.Application.Mappings;

public sealed class MessageMapping : Profile
{
    public MessageMapping()
    {
        CreateMap<SendMessageCommand, Message>();
        
        CreateMap<Message, SendMessageResponse>();
    }
}