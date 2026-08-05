using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Configuration;
using RestaurantManagement.Enums;
using RestaurantManagement.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RestaurantManagement.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly JwtSettings _jwtSettings;

        public JwtService(JwtSecurityTokenHandler tokenHandler)
        {
            _jwtSettings = LoadJwtSettings();
            _tokenHandler = tokenHandler;
        }

        private JwtSettings LoadJwtSettings()
        {
            return new JwtSettings
            {
                Secret = GetRequiredEnvironmentVariable("JWT_SECRET"),
                Issuer = GetRequiredEnvironmentVariable("JWT_ISSUER"),
                Audience = GetRequiredEnvironmentVariable("JWT_AUDIENCE"),
                AccessTokenExpiryMinutes = int.Parse(GetRequiredEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRY_MINUTES")),
                RefreshTokenExpiryDays = int.Parse(GetRequiredEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS"))
            };
        }

        private string GetRequiredEnvironmentVariable(string variableName)
        {
            string value = Environment.GetEnvironmentVariable(variableName);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    $"Environment variable '{variableName}' is not configured.");
            }

            return value;
        }

        public string GenerateAccessToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            IEnumerable<Claim> claims = CreateClaims(user);

            SigningCredentials signingCredentials = CreateSigningCredentials();

            JwtSecurityToken token = CreateJwtToken(
                claims,
                signingCredentials);

            return _tokenHandler.WriteToken(token);
        }

        private IEnumerable<Claim> CreateClaims(User user)
        {
            return new List<Claim>
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.UserId.ToString()),

                new Claim(
                    ClaimTypes.Role,
                    ((UserRole)user.RoleId).ToString()),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email),

                new Claim(
                    JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString())
            };
        }

        private SigningCredentials CreateSigningCredentials()
        {
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            return new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);
        }

        private JwtSecurityToken CreateJwtToken(
            IEnumerable<Claim> claims,
            SigningCredentials signingCredentials)
        {
            return new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: signingCredentials);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            throw new NotImplementedException();
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}
