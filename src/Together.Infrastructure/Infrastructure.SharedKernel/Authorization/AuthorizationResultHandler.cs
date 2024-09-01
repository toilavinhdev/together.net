using System.Net;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Localization;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.SharedKernel.Authorization;

public sealed class AuthorizationResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();
    
    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        var localizationService = LocalizationServiceFactory.GetInstance();
        
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(
                new BaseResponse
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Errors = [
                        new BaseError(
                            ErrorCodes.Server.Forbidden, 
                            localizationService.Get(ErrorCodes.Server.Forbidden))
                    ]
                });
            return;
        }

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}