using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Identity.Application.Features.FeatureUser.Commands;
using Service.Identity.Application.Features.FeatureUser.Queries;
using Service.Identity.Application.Features.FeatureUser.Responses;

namespace Service.Identity.Endpoints;

public sealed class UserEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/user").WithTags("User");
        
        group.MapGet("/me", Me);
        
        group.MapGet("/me/permissions", Permissions);
        
        group.MapGet("/{userId:guid}", GetUser);
        
        group.MapGet("/list", ListUser);

        group.MapPut("/me/upload-avatar", UploadAvatar);
        
        group.MapPut("/me/update-profile", UpdateProfile);
        
        group.MapPut("/me/update-password", UpdatePassword);
    }
    
    [Authorize]
    private static Task<BaseResponse<MeResponse>> Me(ISender sender)
        => sender.Send(new MeQuery());
    
    [Authorize]
    private static Task<BaseResponse<List<string>>> Permissions(ISender sender)
        => sender.Send(new PermissionsQuery());
    
    [AccessControl(Policies.User.Get)]
    private static Task<BaseResponse<GetUserResponse>> GetUser(ISender sender, Guid userId)
        => sender.Send(new GetUserQuery(userId));
    
    [AccessControl(Policies.User.List)]
    private static Task<BaseResponse<ListUserResponse>> ListUser(ISender sender, [AsParameters] ListUserQuery query)
        => sender.Send(query);
    
    [Authorize]
    private static Task<BaseResponse> UpdateProfile(ISender sender, UpdateProfileCommand command)
        => sender.Send(command);
    
    [Authorize]
    private static Task<BaseResponse> UpdatePassword(ISender sender, UpdatePasswordCommand command)
        => sender.Send(command);
    
    [Authorize]
    private static Task<BaseResponse> UploadAvatar(ISender sender, UpdateAvatarCommand command)
        => sender.Send(command);
}