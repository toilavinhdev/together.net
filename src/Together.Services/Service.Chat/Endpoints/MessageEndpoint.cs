using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Chat.Application.Features.FeatureMessage.Commands;
using Service.Chat.Application.Features.FeatureMessage.Queries;
using Service.Chat.Application.Features.FeatureMessage.Responses;

namespace Service.Chat.Endpoints;

public sealed class MessageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/message").WithTags("Message");
        
        group.MapGet("/list", ListMessage);
        
        group.MapPost("/send", SendMessage);
    }
    
    [Authorize]
    private static Task<BaseResponse<ListMessageResponse>> ListMessage(ISender sender, [AsParameters] ListMessageQuery query)
        => sender.Send(query);
    
    [Authorize]
    private static Task<BaseResponse<SendMessageResponse>> SendMessage(ISender sender, SendMessageCommand command)
        => sender.Send(command);
}