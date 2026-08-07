namespace RestaurantManagement.Services.Configuration
{
    public interface IEnvironmentConfigurationService
    {
        string JwtSecret { get; }

        int AccessTokenExpiryMinutes { get; }

        int RefreshTokenExpiryDays { get; }
    }
}
