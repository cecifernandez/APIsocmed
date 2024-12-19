using APISocMed.Data;
using APISocMed.Models;

namespace APISocMed.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task SaveRefreshTokenAsync(int userId, string refreshToken);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);

        Task UpdateRefreshTokenAsync(RefreshToken token);
    }
}
