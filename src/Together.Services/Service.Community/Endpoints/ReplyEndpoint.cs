using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Service.Community.Application.Features.FeatureReply.Commands;
using Service.Community.Application.Features.FeatureReply.Queries;
using Service.Community.Application.Features.FeatureReply.Responses;

namespace Service.Community.Endpoints;

public sealed class ReplyEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/reply").WithTags("Reply");

        group.MapGet("/query", ListReply);
        
        group.MapPost("/create", CreateReply);
        
        group.MapPost("/vote", VoteReply);
        
        group.MapPut("/update", UpdateReply);
        
        group.MapDelete("/{replyId:guid}", DeleteReply);
    }
    
    [AccessControl(Policies.Reply.View)]
    private static Task<BaseResponse<ReplyResponse>> ListReply(ISender sender, [AsParameters] ReplyQuery query)
        => sender.Send(query);
    
    [AccessControl(Policies.Reply.Create)]
    private static Task<BaseResponse<CreateReplyResponse>> CreateReply(ISender sender, CreateReplyCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Reply.Vote)]
    private static Task<BaseResponse<VoteReplyResponse>> VoteReply(ISender sender, VoteReplyCommand command)
        => sender.Send(command);

    [AccessControl(Policies.Reply.Update)]
    private static Task<BaseResponse> UpdateReply(ISender sender, UpdateReplyCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Reply.Delete)]
    private static Task<BaseResponse> DeleteReply(ISender sender, Guid replyId)
        => sender.Send(new DeleteReplyCommand(replyId));
}