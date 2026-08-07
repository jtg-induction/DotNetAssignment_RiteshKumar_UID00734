using System.Threading.Tasks;
using RestaurantManagement.Models;

namespace RestaurantManagement.Repository.Interfaces 
{ 
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetByTokenHashAsync(string tokenHash);

        void Add(RefreshToken refreshToken);

        void Update(RefreshToken refreshToken);

        Task SaveChangesAsync();
    }
}
