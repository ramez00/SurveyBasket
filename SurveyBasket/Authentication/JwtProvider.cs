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

    public string? ValidateToken(string token)
    {
        var TokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

        try
        {
            TokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = securityKey,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ClockSkew = TimeSpan.Zero, // To make Token Expired when time Exceed 
            },out SecurityToken _validatedToken);

            var JwtToken = (JwtSecurityToken)_validatedToken;

            return JwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
        }
        catch
        {

            return null;
        }

    }
}
