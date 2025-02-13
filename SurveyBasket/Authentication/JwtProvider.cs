using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasket.Authentication;

public class JwtProvider(IOptions<JwtOptions> jwtOption) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = jwtOption.Value;

    public (string token, int expiredIn) CreateToken(ApplicationUser user)
    {
        Claim[] claims = new Claim[] {
            new Claim(JwtRegisteredClaimNames.Sub , user.Id),
            new Claim(JwtRegisteredClaimNames.Email , user.Email!),
            new Claim(JwtRegisteredClaimNames.GivenName , user.FirstName),
            new Claim(JwtRegisteredClaimNames.FamilyName , user.LastName),
            new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString()),
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var signingCredentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);

        var expiredIn = _jwtOptions.ExpiredIn;

        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            signingCredentials:signingCredentials,
            claims: claims,
            expires: DateTime.Today.AddMinutes(expiredIn)
        );

        return (new JwtSecurityTokenHandler().WriteToken(jwt), expiredIn);

    }
}
