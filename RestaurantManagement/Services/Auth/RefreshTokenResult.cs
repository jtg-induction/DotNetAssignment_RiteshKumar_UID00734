using RestaurantManagement.Models;

namespace RestaurantManagement.Services.Auth
{
    public class RefreshTokenResult
    {
        public string PlainTextToken { get; set; }

        public RefreshToken RefreshTokenEntity { get; set; }
    }
}