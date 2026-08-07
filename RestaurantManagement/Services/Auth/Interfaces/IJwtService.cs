using System.Security.Claims;
using RestaurantManagement.Models;

namespace RestaurantManagement.Services.Auth
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);

        ClaimsPrincipal ValidateToken(string token);

        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
