﻿using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ShopListApp.Interfaces.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ShopListApp.Infrastructure;

public class JwtTokenManager(IConfiguration config) : ITokenManager
{
    private readonly IConfiguration _config = config;

    public string GenerateAccessToken(UserDto user)
    {
        var tokenConfig = _config.GetSection("TokenConfiguration");
        var issuer = "https://localhost:7101";
        var audience = "https://localhost:7101";
        var secretKey = Environment.GetEnvironmentVariable("SecretJwtKey") 
            ?? tokenConfig.GetValue<string>("SecretKey") 
            ?? string.Empty;
        var jwtExpireMinutes = tokenConfig.GetValue<int>("AccessTokenExpirationMinutes");
        var jwtExpireDate = DateTime.Now.AddMinutes(jwtExpireMinutes);
        var symmKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(symmKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,   
            claims: claims, 
            expires: jwtExpireDate, 
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateHashRefreshToken()
    {
        var refreshToken = RandomNumberGenerator.GetBytes(64);
        using (var sha256 = SHA256.Create())
        {
            var bytes = sha256.ComputeHash(refreshToken);
            return Convert.ToBase64String(bytes);
        }
    }

    public string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

    public int GetRefreshTokenExpirationDays() => _config.GetValue<int>("TokenConfiguration:RefreshTokenExpirationDays");

    public string? GetHashRefreshToken(string refreshToken)
    {
        try
        {
            var bytes = Convert.FromBase64String(refreshToken);
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
        catch
        {
            return null;
        }
    }
}
