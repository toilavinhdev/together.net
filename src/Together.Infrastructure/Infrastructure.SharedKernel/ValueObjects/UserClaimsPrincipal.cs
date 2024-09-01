namespace Infrastructure.SharedKernel.ValueObjects;

public sealed class UserClaimsPrincipal
{
    public Guid Id { get; set; }
    
    public string UserName { get; set; } = default!;

    public string Email { get; set; } = default!;

    public List<string> Permissions { get; set; } = default!;
}