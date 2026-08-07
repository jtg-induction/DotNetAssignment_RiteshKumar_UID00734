using RestaurantManagement.Models;
using RestaurantManagement.Services.Configuration;
using System;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantManagement.Services.Auth
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private const int TokenSizeInBytes = 64;

        private readonly IEnvironmentConfigurationService _environmentConfigurationService;

        public RefreshTokenService(
            IEnvironmentConfigurationService environmentConfigurationService)
        {
            _environmentConfigurationService = environmentConfigurationService;
        }
        public RefreshTokenResult CreateRefreshToken(long? userId = null)
        {
            string plainTextToken = GenerateRefreshToken();
            string tokenHash = HashRefreshToken(plainTextToken);

            RefreshToken refreshToken = new RefreshToken
            {
                TokenHash = tokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_environmentConfigurationService.RefreshTokenExpiryDays),
                IsRevoked = false,
                RevokedAt = null
            };

            if (userId.HasValue)
            {
                refreshToken.UserId = userId.Value;
            }

            return new RefreshTokenResult
            {
                PlainTextToken = plainTextToken,
                RefreshTokenEntity = refreshToken
            };
        }

        public bool VerifyRefreshToken(string plainTextToken, string storedHash)
        {
            throw new NotImplementedException();
        }

        public void RevokeRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            refreshToken.RevokedAt = DateTime.UtcNow;
        }

        private string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[TokenSizeInBytes];

            using (RandomNumberGenerator randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(randomBytes);
            }

            return Convert.ToBase64String(randomBytes);
        }

        public string HashRefreshToken(string token)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] tokenBytes = Encoding.UTF8.GetBytes(token);
                byte[] hashBytes = sha256.ComputeHash(tokenBytes);

                StringBuilder builder = new StringBuilder();

                foreach (byte hashByte in hashBytes)
                {
                    builder.Append(hashByte.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
