using Infrastructure.SharedKernel.Authorization;
using Infrastructure.SharedKernel.Constants;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using MediatR;
using Service.Community.Application.Features.FeaturePrefix.Commands;
using Service.Community.Application.Features.FeaturePrefix.Queries;
using Service.Community.Application.Features.FeaturePrefix.Responses;

namespace Service.Community.Endpoints;

public sealed class PrefixEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/prefix").WithTags("Prefix");
        
        group.MapGet("/{prefixId:guid}", GetPrefix);
        
        group.MapGet("/list", ListPrefix);

        group.MapPost("/create", CreatePrefix);
            
        group.MapPut("/update", UpdatePrefix);
        
        group.MapDelete("/{prefixId:guid}", DeletePrefix);
    }
    
    [AccessControl(Policies.Prefix.View)]
    private static Task<BaseResponse<PrefixViewModel>> GetPrefix(ISender sender, Guid prefixId)
        => sender.Send(new GetPrefixQuery(prefixId));
    
    [AccessControl(Policies.Prefix.View)]
    private static Task<BaseResponse<List<PrefixViewModel>>> ListPrefix(ISender sender)
        => sender.Send(new ListPrefixQuery());
    
    [AccessControl(Policies.Prefix.Create)]
    private static Task<BaseResponse<CreatePrefixResponse>> CreatePrefix(ISender sender, CreatePrefixCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Prefix.Update)]
    private static Task<BaseResponse> UpdatePrefix(ISender sender, UpdatePrefixCommand command)
        => sender.Send(command);
    
    [AccessControl(Policies.Prefix.Delete)]
    private static Task<BaseResponse> DeletePrefix(ISender sender, Guid prefixId)
        => sender.Send(new DeletePrefixCommand(prefixId));
}