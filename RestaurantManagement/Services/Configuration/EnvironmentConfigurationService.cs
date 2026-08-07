using System;

namespace RestaurantManagement.Services.Configuration
{
    public class EnvironmentConfigurationService : IEnvironmentConfigurationService
    {
        public string JwtSecret =>
            GetRequiredEnvironmentVariable("JWT_SECRET");

        public int AccessTokenExpiryMinutes =>
            int.Parse(GetRequiredEnvironmentVariable("JWT_ACCESS_TOKEN_EXPIRY_MINUTES"));

        public int RefreshTokenExpiryDays =>
            int.Parse(GetRequiredEnvironmentVariable("JWT_REFRESH_TOKEN_EXPIRY_DAYS"));

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
    }
}
