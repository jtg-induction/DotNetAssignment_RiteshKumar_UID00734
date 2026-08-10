using RestaurantManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RestaurantManagement.Repository.Interfaces 
{ 
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenHashAsync(string tokenHash);

        Task<List<RefreshToken>> GetAllByUserIdAsync(long userId);

        void Add(RefreshToken refreshToken);

        void Update(RefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}
