using APISocMed.Models;

namespace APISocMed.Interfaces
{
    public interface IAccessRepository
    {
        Task<bool> RegisterAsync(User user);
        Task<User?> AuthenticateUserAsync(string email, string passwordHash);
    }
}
