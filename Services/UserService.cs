using APISocMed.Interfaces;
using APISocMed.Models;
using APISocMed.Models.DTOs;

namespace APISocMed.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly ISpotifyService _spotifyService;

        public UserService(IUserRepository userRepository, IFollowerRepository followerRepository, ISpotifyService spotifyService)
        {
            _userRepository = userRepository;
            _followerRepository = followerRepository;
            _spotifyService = spotifyService;
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string name)
        {
            return await _userRepository.GetUserByUserNameAsync(name);
        }

        public async Task FollowUserAsync(int currentUser, int followingId)
        {
            if (currentUser == followingId)
                throw new InvalidOperationException("You cannot follow yourself.");

            await _followerRepository.FollowUsersAsync(currentUser, followingId);
        }

        public async Task UnfollowUserAsync(int currentUser, int followingId)
        {
            if (currentUser == followingId)
                throw new InvalidOperationException("You cannot unfollow yourself.");

            await _followerRepository.UnfollowUsersAsync(currentUser, followingId);
        }

        public async Task<string> ConnectSpotifyAsync(int currentUser, string code)
        {
            var accessToken = await _spotifyService.ExchangeCodeForToken(code);
            var userProfile = await _spotifyService.GetSpotifyUserId(accessToken.AccessToken);
            await _userRepository.SaveSpotifyTokensAsync(currentUser, userProfile, accessToken.RefreshToken);
            return accessToken.AccessToken;
        }

        public async Task<User> UpdateUserAsync(int id, UserDTO userDto, int currentUser)
        {
            if (id != currentUser)
                throw new UnauthorizedAccessException("You can't update this user.");

            return await _userRepository.UpdateUserAsync(id, userDto);
        }
    }
}
