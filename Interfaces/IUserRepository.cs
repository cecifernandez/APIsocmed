using APISocMed.Models;
using APISocMed.Models.DTOs;

namespace APISocMed.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<User?> AuthenticateUserAsync(string email, string passwordHash);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUserNameAsync(string name);
        Task<User?> UpdateUserAsync(int id, UserDTO user);

        Task SaveSpotifyTokensAsync(int userId, string spotifyId, string refreshToken);
    }
}
