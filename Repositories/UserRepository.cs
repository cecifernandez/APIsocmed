using APISocMed.Data;
using APISocMed.DomainServices;
using APISocMed.Models.DTOs;
using APISocMed.Models;
using Microsoft.AspNetCore.Mvc;
using APISocMed.Interfaces;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace APISocMed.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SocMedBdContext _socMedBdContext;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        public UserRepository(SocMedBdContext socMedBdContext, AuthService authService, IMapper mapper)
        {
            _socMedBdContext = socMedBdContext;
            _authService = authService;
            _mapper = mapper;
        }

        public async Task<bool> RegisterAsync(User user)
        {
            await _socMedBdContext.Users.AddAsync(user);
            await _socMedBdContext.SaveChangesAsync();
            return user.UserId != 0;
        }

        public async Task<User?> AuthenticateUserAsync(string email, string passwordHash)
        {
            return await _socMedBdContext.Users
                .Where(u => u.UserEmail == email && u.PasswordHash == passwordHash)
                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _socMedBdContext.Users
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        public async Task<User?> GetUserByUserNameAsync(string name)
        {
            return await _socMedBdContext.Users
                .Include(u => u.Posts)
                .Include(u => u.Followers)
                .Include(u => u.FollowedBy)
                .FirstOrDefaultAsync(u => u.UserName == name);
        }

        public async Task<User?> UpdateUserAsync(int id, UserDTO user)
        {
            var existingUser = await _socMedBdContext.Users.FindAsync(id);
            if (existingUser == null)
            {
                return null; 
            }

            _mapper.Map(user, existingUser);

            await _socMedBdContext.SaveChangesAsync();
            return existingUser;
        }

        public async Task SaveSpotifyTokensAsync(int userId, string spotifyId, string refreshToken)
        {
            var user = await GetUserByIdAsync(userId);
            if (user != null)
            {
                user.SpotifyId = spotifyId;
                user.SpotifyRefreshToken = refreshToken;
                await _socMedBdContext.SaveChangesAsync();
            }
        }

    }
}
