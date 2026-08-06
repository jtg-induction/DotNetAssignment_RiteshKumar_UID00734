using RestaurantManagement.Models;

namespace RestaurantManagement.Services.Auth
{
    public interface IRefreshTokenService
    {
        RefreshTokenResult CreateRefreshToken();

        bool VerifyRefreshToken(string plainTextToken, string storedHash);

        void RevokeRefreshToken(RefreshToken refreshToken);
    }
}
