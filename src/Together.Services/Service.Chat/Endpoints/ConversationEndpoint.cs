using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Chat.Application.Features.FeatureConversation.Commands;
using Service.Chat.Application.Features.FeatureConversation.Queries;
using Service.Chat.Application.Features.FeatureConversation.Responses;

namespace Service.Chat.Endpoints;

public sealed class ConversationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/conversation").WithTags("Chat");

        group.MapGet("/get", GetConversation);
        
        group.MapGet("/list", ListConversation);
        
        group.MapPost("/create", CreateConversation);
        
    }
    
    [Authorize]
    private static Task<BaseResponse<ConversationViewModel?>> GetConversation(ISender sender,
        [AsParameters] GetConversationQuery query) => sender.Send(query);
    
    [Authorize]
    private static Task<BaseResponse<ListConversationResponse>> ListConversation(ISender sender,
        [AsParameters] ListConversationQuery query) => sender.Send(query);

    [Authorize]
    private static Task<BaseResponse<Guid>> CreateConversation(ISender sender,
        CreateConversationCommand command) => sender.Send(command);
}