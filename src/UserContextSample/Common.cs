using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserContextSample;

public static class Settings
{
    public const string SecretKey = "5948695C-B3C3-4C92-979D-C8621100E5EB";
}

public static class CustomClaims
{
    public const string DepartmentId = "departmentId";
}

public static class TokenService
{
    public static string GenerateToken(User user)
    {
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(CustomClaims.DepartmentId, user.DepartmentId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Settings.SecretKey)),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public static class ClaimsPrincipalExtensions
{
    public static Guid GetDepartmentId(this ClaimsPrincipal user) => Guid.Parse(user.FindFirstValue(CustomClaims.DepartmentId));
}
