using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repository.Interfaces;

namespace RestaurantManagement.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RestaurantDbContext _context;

        public UserRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(long userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<User> GetByIdWithRoleAsync(long userId)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email == email);
        }

        public async Task<bool> PhoneExistsAsync(string phone)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone);
        }

        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public void Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
