using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.SharedKernel.Authorization;

public sealed class AuthorizationRequirement(string permission) : IAuthorizationRequirement
{
    public string Permission { get; set; } = permission;
}