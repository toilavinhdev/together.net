using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Infrastructure.SharedKernel.Authorization;

public sealed class AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : IAuthorizationPolicyProvider
{
    private const string PolicyPrefix = "Together";
    
    private DefaultAuthorizationPolicyProvider DefaultAuthorizationPolicyProvider { get; } = new(options);
    
    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => DefaultAuthorizationPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => DefaultAuthorizationPolicyProvider.GetFallbackPolicyAsync();

    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var pieces = policyName.Split(".");
        var permission = pieces[1];
        
        if (!policyName.StartsWith(PolicyPrefix, StringComparison.OrdinalIgnoreCase) || pieces.Length != 2)
            return await DefaultAuthorizationPolicyProvider.GetPolicyAsync(policyName);
        
        var builder = new AuthorizationPolicyBuilder();
        builder.AddRequirements(new AuthorizationRequirement(permission));

        return builder.Build();
    }
}