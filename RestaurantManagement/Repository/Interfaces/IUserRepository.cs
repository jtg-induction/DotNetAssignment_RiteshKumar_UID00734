using System.Threading.Tasks;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(long userId);

        Task<User> GetByEmailAsync(string email);

        Task<User> GetByIdWithRoleAsync(long userId);

        Task<bool> EmailExistsAsync(string email);

        Task<bool> PhoneExistsAsync(string phone);

        Task<UserAddress> GetAddressByIdForUserAsync(
            long addressId,
            long userId);

        void Add(User user);

        void Update(User user);

        void AddAddress(UserAddress address);

        void UpdateAddress(UserAddress address);

        Task SaveChangesAsync();
    }
}
