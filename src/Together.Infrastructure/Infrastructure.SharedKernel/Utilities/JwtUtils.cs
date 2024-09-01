using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Infrastructure.SharedKernel.Extensions;
using Infrastructure.SharedKernel.ValueObjects;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.SharedKernel.Utilities;

public static class JwtUtils
{
    public static string GenerateAccessToken(JwtConfig config, UserClaimsPrincipal claims)
    {
        return GenerateAccessToken(
            config.TokenSigningKey,
            [
                new Claim("id", claims.Id.ToString()),
                new Claim("userName", claims.UserName),
                new Claim("email", claims.Email),
                new Claim("permissions", string.Join(',', claims.Permissions)),
            ],
            DateTime.UtcNow.AddMinutes(config.AccessTokenDurationInMinutes),
            config.Issuer,
            config.Audience);
    }
    
    private static string GenerateAccessToken(string tokenSingingKey, 
        IEnumerable<Claim> claimsPrincipal, 
        DateTime? expireAt = null, 
        string? issuer = null, 
        string? audience = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSingingKey));
        
        var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = issuer,
            Audience = audience,
            IssuedAt = DateTime.Now,
            Subject = new ClaimsIdentity(claimsPrincipal),
            Expires = expireAt,
            SigningCredentials = credential
        };
        
        var handler = new JwtSecurityTokenHandler();
        
        return handler.WriteToken(handler.CreateToken(descriptor));
    }
    
    public static UserClaimsPrincipal DecodeAccessToken(string accessToken)
    {
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
        
        var id = decodedToken.Claims.FirstOrDefault(x => x.Type.Equals("id"))?.Value.ToGuid()
                 ?? throw new NullReferenceException("Claim type 'id' is required");
        var userName = decodedToken.Claims.FirstOrDefault(x => x.Type.Equals("userName"))?.Value
                       ?? throw new NullReferenceException("Claim type 'userName' is required");
        var email = decodedToken.Claims.FirstOrDefault(x => x.Type.Equals("email"))?.Value
                    ?? throw new NullReferenceException("Claim type 'email' is required");
        var permissions = decodedToken.Claims.FirstOrDefault(x => x.Type.Equals("permissions"))?.Value
                    ?? throw new NullReferenceException("Claim type 'permissions' is required");
        
        if (!RegexUtils.EmailRegex.IsMatch(email)) throw new InvalidCastException("Email invalid");
        
        return new UserClaimsPrincipal
        {
            Id = id,
            UserName = userName,
            Email = email,
            Permissions = permissions.Split(",").ToList()
        };
    }
}