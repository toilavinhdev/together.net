using System.Net;
using FluentValidation;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Exceptions;
using Infrastructure.SharedKernel.Localization;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.SharedKernel.Extensions;

public static class ExceptionHandlerExtensions
{
    public static void UseCoreExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async ctx =>
            {
                var feature = ctx.Features.Get<IExceptionHandlerFeature>();

                if (feature is not null)
                {
                    var exception = feature.Error;
                    await WriteResponseAsync(ctx, exception);
                }
            });
        });
    }
    
    private static async Task WriteResponseAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = (int)GetStatusCode(exception);
        await context.Response.WriteAsJsonAsync(
            new BaseResponse()
            {
                StatusCode = GetStatusCode(exception),
                Errors = GetResponseErrors(exception)
            });
    }

    private static HttpStatusCode GetStatusCode(Exception ex) => ex switch
    {
        UnauthorizedAccessException => HttpStatusCode.Unauthorized,
        TogetherException dEx => dEx.StatusCode,
        ValidationException => HttpStatusCode.BadRequest,
        _ => HttpStatusCode.InternalServerError
    };
    
    private static List<BaseError> GetResponseErrors(Exception ex)
    {
        switch (ex)
        {
            case UnauthorizedAccessException:
                return [
                    new BaseError(
                        ErrorCodes.Server.Unauthorized, 
                        ErrorCodes.Server.Unauthorized)
                ];
            case ValidationException vEx:
                return vEx.Errors
                    .Select(failure =>
                        new BaseError(
                            failure.ErrorCode, 
                            failure.ErrorMessage, 
                            failure.PropertyName))
                    .ToList();
            default:
            {
                var errorCount = 5;
                var errors = new List<BaseError> { ex.ToResponseError() };

                var inner = ex.InnerException;
        
                while (inner is not null && errorCount-- > 0)
                {
                    errors.Add(inner.ToResponseError());
                    inner = inner.InnerException;
                }

                return errors.ToList();
            }
        }
    }

    private static BaseError ToResponseError(this Exception ex)
    {
        var localizationService = LocalizationServiceFactory.GetInstance();
        
        return ex is TogetherException dEx 
            ? new BaseError(dEx.Code, localizationService.Get(dEx.Code), dEx.Parameter)
            : new BaseError(ErrorCodes.Server.InternalServer, ex.Message);
    }
}