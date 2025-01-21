using APISocMed.Models;
using APISocMed.Models.DTOs;

namespace APISocMed.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string name);
        Task FollowUserAsync(int currentUser, int followingId);
        Task UnfollowUserAsync(int currentUser, int followingId);
        Task<string> ConnectSpotifyAsync(int currentUser, string code);
        Task<User> UpdateUserAsync(int id, UserDTO userDto, int currentUser);
    }
}
