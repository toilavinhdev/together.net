using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Service.Identity.Application.Features.FeatureUser.Commands;
using Service.Identity.Application.Features.FeatureUser.Responses;

namespace Service.Identity.Endpoints;

public sealed class AuthEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/v1/auth").WithTags("Auth");
        
        group.MapPost("/sign-in", SignIn);
        
        group.MapPost("/refresh-token", RefreshToken);
        
        group.MapPost("/external", External);
        
        group.MapPost("/sign-up", SignUp);
        
        group.MapPost("/logout", Logout);
        
        group.MapPost("/forgot-password", ForgotPassword);
        
        group.MapPost("/forgot-password/submit", SubmitForgotPasswordToken);
    }

    [AllowAnonymous]
    private static Task<BaseResponse<AuthToken>> SignIn(ISender sender, SignInCommand command)
        => sender.Send(command);
    
    [AllowAnonymous]
    private static Task<BaseResponse<AuthToken>> RefreshToken(ISender sender, RefreshTokenCommand command)
        => sender.Send(command);
    
    [AllowAnonymous]
    private static Task<BaseResponse<SignUpResponse>> SignUp(ISender sender, SignUpCommand command)
        => sender.Send(command);
    
    [AllowAnonymous]
    private static Task<BaseResponse<AuthToken>> External(ISender sender, ExternalAuthCommand command)
        => sender.Send(command);
    
    [AllowAnonymous]
    private static Task<BaseResponse> Logout(ISender sender)
        => sender.Send(new LogoutCommand());
    
    [AllowAnonymous]
    private static Task<BaseResponse> ForgotPassword(ISender sender, ForgotPasswordCommand command)
        => sender.Send(command);
    
    [AllowAnonymous]
    private static Task<BaseResponse> SubmitForgotPasswordToken(ISender sender, SubmitForgotPasswordTokenCommand command)
        => sender.Send(command);
}