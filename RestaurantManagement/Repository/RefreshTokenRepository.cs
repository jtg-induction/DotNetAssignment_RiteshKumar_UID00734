using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RestaurantManagement.Data;
using RestaurantManagement.Models;
using RestaurantManagement.Repository.Interfaces;

namespace RestaurantManagement.Repository
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly RestaurantDbContext _context;

        public RefreshTokenRepository(RestaurantDbContext context)
        {
            _context = context;
        }

        public async Task<RefreshToken> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == tokenHash);
        }

        public void Add(RefreshToken refreshToken)
        {
            _context.RefreshTokens.Add(refreshToken);
        }

        public void Update(RefreshToken refreshToken)
        {
            _context.Entry(refreshToken).State = EntityState.Modified;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
