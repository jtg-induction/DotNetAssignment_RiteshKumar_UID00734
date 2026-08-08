using RestaurantManagement.DTOs.Requests;
using RestaurantManagement.DTOs.Responses;
using System.Threading.Tasks;

namespace RestaurantManagement.Services.Users.Interfaces
{
    public interface IUserService
    {
        Task<UserProfileResponse> PatchProfileAsync(
            long userId,
            PatchProfileRequest request);


        Task ChangePasswordAsync(
            long userId,
            ChangePasswordRequest request);


        Task DeactivateAccountAsync(
            long userId);
    }
}
