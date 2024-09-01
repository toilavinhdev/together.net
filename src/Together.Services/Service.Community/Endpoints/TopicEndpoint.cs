using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Service.Community.Application.Features.FeatureTopic.Commands;
using Service.Community.Application.Features.FeatureTopic.Queries;
using Service.Community.Application.Features.FeatureTopic.Responses;

namespace Service.Community.Endpoints;

public sealed class TopicEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/topic").WithTags("Topic");
        
        group.MapGet("/{topicId:guid}", GetTopic);
        
        group.MapPost("/create", CreateTopic);
        
        group.MapPut("/update", UpdateTopic);
        
        group.MapDelete("/{topicId:guid}", DeleteTopic);
    }
        
    [AccessControl(Policies.Topic.View)]
    private static Task<BaseResponse<GetTopicResponse>> GetTopic(ISender sender, Guid topicId)
        => sender.Send(new GetTopicQuery(topicId));
            
    [AccessControl(Policies.Topic.Create)]
    private static Task<BaseResponse<CreateTopicResponse>> CreateTopic(ISender sender, CreateTopicCommand command)
        => sender.Send(command);
            
    [AccessControl(Policies.Topic.Update)]
    private static Task<BaseResponse> UpdateTopic(ISender sender, UpdateTopicCommand command)
        => sender.Send(command);
            
    [AccessControl(Policies.Topic.Delete)]
    private static Task<BaseResponse> DeleteTopic(ISender sender, Guid topicId)
        => sender.Send(new DeleteTopicCommand(topicId));
}