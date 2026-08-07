using Microsoft.IdentityModel.Tokens;
using RestaurantManagement.Enums;
using RestaurantManagement.Models;
using RestaurantManagement.Services.Configuration;
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
        private readonly IEnvironmentConfigurationService _environmentConfigurationService;

        public JwtService(
            JwtSecurityTokenHandler tokenHandler,
            IEnvironmentConfigurationService environmentConfigurationService)
        {
            _tokenHandler = tokenHandler;
            _environmentConfigurationService = environmentConfigurationService;
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
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_environmentConfigurationService.JwtSecret));

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
                expires: DateTime.UtcNow.AddMinutes(_environmentConfigurationService.AccessTokenExpiryMinutes),
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
