using APISocMed.Data;
using APISocMed.DomainServices;
using APISocMed.Interfaces;
using APISocMed.Models;
using Microsoft.EntityFrameworkCore;

namespace APISocMed.Repositories
{
    public class FollowerRepository : IFollowerRepository
    {
        private readonly SocMedBdContext _socMedBdContext;
        private readonly AuthService _authService;
        public FollowerRepository(SocMedBdContext socMedBdContext, AuthService authService)
        {
            _socMedBdContext = socMedBdContext;
            _authService = authService;
        }
        public async Task<Follower?> GetFollowerRelationshipAsync(int followerId, int followedId)
        {
            return await _socMedBdContext.Followers.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        //current user follower list
        public async Task<IEnumerable<User>> GetFollowerUsersAsync(int userId)
        {
            return await _socMedBdContext.Followers.Where(f => f.FollowerId == userId)
            .Select(f => f.FollowerUser)
            .ToListAsync();
        }

        //current user following list
        public async Task<IEnumerable<User>> GetFollowingUsersAsync(int userId)
        {
            return await _socMedBdContext.Followers.Where(f => f.FollowedId == userId)
            .Select(f => f.FollowerUser)
            .ToListAsync();
        }

        public async Task<bool> IsFollowingAsync(int followerId, int followedId)
        {
            return await _socMedBdContext.Followers.AnyAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);
        }

        public async Task FollowUsersAsync(int followerId, int followedId)
        {
            var isFollowing = await IsFollowingAsync(followerId, followedId);
            if (isFollowing)
            {
                throw new InvalidOperationException("You are already following this user.");
            }

            var follower = new Follower
            {
                FollowerId = followerId,
                FollowedId = followedId
            };

            _socMedBdContext.Followers.Add(follower);
            await _socMedBdContext.SaveChangesAsync();
        }

        public async Task UnfollowUsersAsync(int followerId, int followedId)
        {
            var follower = await _socMedBdContext.Followers
                            .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FollowedId == followedId);

            if (follower == null)
            {
                throw new InvalidOperationException("You are not following this user.");
            }

            _socMedBdContext.Followers.Remove(follower);
            await _socMedBdContext.SaveChangesAsync();
        }
    }
}
