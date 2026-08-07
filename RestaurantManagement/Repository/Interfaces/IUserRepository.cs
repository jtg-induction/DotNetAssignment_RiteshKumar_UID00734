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

        void Add(User user);

        void Update(User user);

        Task SaveChangesAsync();
    }
}
