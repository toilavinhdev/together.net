using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Service.Community.Application.Features.FeatureForum.Commands;
using Service.Community.Application.Features.FeatureForum.Queries;
using Service.Community.Application.Features.FeatureForum.Responses;

namespace Service.Community.Endpoints;

public sealed class ForumEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/forum").WithTags("Forum");
        
        group.MapGet("/{forumId:guid}", GetForum);
        
        group.MapGet("/list", ListForum);

        group.MapPost("/create", CreateForum);
            
        group.MapPut("/update", UpdateForum);
        
        group.MapDelete("/{forumId:guid}", DeleteForum);
    }
    
    [AccessControl(Policies.Forum.View)]
    private static Task<BaseResponse<GetForumResponse>> GetForum(ISender sender, Guid forumId)
        => sender.Send(new GetForumQuery(forumId));
        
    [AccessControl(Policies.Forum.View)]
    private static Task<BaseResponse<List<ForumViewModel>>> ListForum(ISender sender, [AsParameters] ListForumQuery query)
        => sender.Send(query);
    
    [AccessControl(Policies.Forum.Create)]
    private static Task<BaseResponse<CreateForumResponse>> CreateForum(ISender sender, CreateForumCommand command)
        => sender.Send(command);

    [AccessControl(Policies.Forum.Update)]
    private static Task<BaseResponse> UpdateForum(ISender sender, UpdateForumCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Forum.Delete)]
    private static Task<BaseResponse> DeleteForum(ISender sender, Guid forumId)
        => sender.Send(new DeleteForumCommand(forumId));
}