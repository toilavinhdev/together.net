using Infrastructure.SharedKernel.Constants;
using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.SharedKernel.Authorization;

public sealed class AuthorizationHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorizationRequirement requirement)
    {
        if (!context.User.Identity!.IsAuthenticated)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var userId = context.User.Claims.FirstOrDefault(x => x.Type.Equals("id"))?.Value;
        
        var permissions = context.User.Claims.FirstOrDefault(x => x.Type.Equals("permissions"))?.Value;
        
        if (userId is null || !Guid.TryParse(userId, out _) || permissions is null)
        {
            context.Fail();
            return Task.CompletedTask;
        }
        
        var hasPermission = permissions
            .Split(",")
            .Any(x => x == Policies.All || x == requirement.Permission);
        
        if (!hasPermission)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}