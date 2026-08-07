using System.Threading.Tasks;
using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;

namespace RestaurantManagement.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> SignupAsync(SignupRequest request);

        Task<AuthResponse> LoginAsync(LoginRequest request);

        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);

        Task LogoutAsync(string refreshToken);
    }
}
