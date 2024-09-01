namespace Infrastructure.SharedKernel.ValueObjects;

public class AuthToken
{
    public string AccessToken { get; set; } = default!;

    public string RefreshToken { get; set; } = default!;
}