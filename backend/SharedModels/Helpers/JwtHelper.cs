using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SharedModels.Helpers;

public class JwtHelper
{
    private string _secretKey;
    private string _issuer;
    private string _audience;

    public JwtHelper(IConfiguration configuration)
    {
        var section = configuration.GetSection("JwtSettings");
        _secretKey = section["SecretKey"];
        _issuer = section["Issuer"];
        _audience = section["Audience"];
    }


    public string GenerateJwtToken(string username, string userId)
    {
        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new Exception("JWT secret key is not configured.");
        }

        if (string.IsNullOrEmpty(_issuer) || string.IsNullOrEmpty(_audience))
        {
            throw new Exception("Issuer or Audience are not configured.");
        }
        var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: new List<Claim>
            {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.NameIdentifier, userId )
            },
            expires: DateTime.Now.AddHours(1),
            signingCredentials: credentials
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}
