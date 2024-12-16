using APISocMed.Models;

namespace APISocMed.Interfaces
{
    public interface IFollowerRepository
    {
        Task<Follower?> GetFollowerRelationshipAsync(int followerId, int followedId);
        Task<bool> IsFollowingAsync(int followerId, int followedId);

        Task<IEnumerable<User>> GetFollowingUsersAsync(int userId);

        Task<IEnumerable<User>> GetFollowerUsersAsync(int userId);

    }
}
