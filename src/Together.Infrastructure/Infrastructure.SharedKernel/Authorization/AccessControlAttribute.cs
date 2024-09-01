using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.SharedKernel.Authorization;

public sealed class AccessControlAttribute : AuthorizeAttribute
{
    private const string PolicyPrefix = "Together";
    
    public string Permission { get; }

    public AccessControlAttribute(string permission)
    {
        Permission = permission;
        SetPolicy();
    }

    private void SetPolicy()
    {
        Policy = $"{PolicyPrefix}.{Permission}";
    }
}