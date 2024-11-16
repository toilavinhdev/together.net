using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Service.Community.Application.Features.FeaturePost.Commands;
using Service.Community.Application.Features.FeaturePost.Queries;
using Service.Community.Application.Features.FeaturePost.Responses;

namespace Service.Community.Endpoints;

public sealed class PostEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/post").WithTags("Post");
        
        group.MapGet("/list", ListPost);
        
        group.MapGet("/{postId:guid}", GetPost);
        
        group.MapPost("/create", CreatePost);
        
        group.MapPost("/vote", VotePost);

        group.MapPost("/report", ReportPost);

        group.MapPost("/handle-report", HandleReportPost);
        
        group.MapPut("/update", UpdatePost);

        group.MapDelete("/{postId:guid}", DeletePost);
    }

    [AccessControl(Policies.Post.View)]
    private static Task<BaseResponse<ListPostResponse>> ListPost(ISender sender, [AsParameters] ListPostQuery query)
        => sender.Send(query);
    
    [AccessControl(Policies.Post.View)]
    private static Task<BaseResponse<GetPostResponse>> GetPost(ISender sender, Guid postId)
        => sender.Send(new GetPostQuery(postId));
    
    [AccessControl(Policies.Post.Create)]
    private static Task<BaseResponse<CreatePostResponse>> CreatePost(ISender sender, CreatePostCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Post.Vote)]
    private static Task<BaseResponse<VotePostResponse>> VotePost(ISender sender, VotePostCommand command)
        => sender.Send(command);

    [AccessControl(Policies.Post.Report)]
    private static Task<BaseResponse> ReportPost(ISender sender, ReportPostCommand command)
        => sender.Send(command);

    [AccessControl(Policies.Post.HandleReport)]
    private static Task<BaseResponse> HandleReportPost(ISender sender, HandleReportPostCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Post.Update)]
    private static Task<BaseResponse> UpdatePost(ISender sender, UpdatePostCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Post.Delete)]
    private static Task<BaseResponse> DeletePost(ISender sender, Guid postId)
        => sender.Send(new DeletePostCommand(postId));
}