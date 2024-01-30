using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Pastebug.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Pastebug.BLL.auth;

public class JwtToken : IJwtToken
{
    private readonly JwtOptions _options;

    public JwtToken(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string Generate(User user)
    {
        var claims = new Claim[] {
            new Claim(JwtRegisteredClaimNames.Email, user.Email.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.Id.ToString()),
        };

        var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey));

        var jwt = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromDays(15)),
                notBefore: DateTime.UtcNow,
                signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
            );

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(jwt);

        return tokenValue;
    }
}
