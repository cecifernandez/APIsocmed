using APISocMed.Data;
using APISocMed.DomainServices;
using APISocMed.Models.DTOs;
using APISocMed.Models;
using Microsoft.AspNetCore.Mvc;
using APISocMed.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace APISocMed.Repositories
{
    public class AccessRepository : IAccessRepository
    {
        private readonly SocMedBdContext _socMedBdContext;
        private readonly AuthService _authService;
        public AccessRepository(SocMedBdContext socMedBdContext, AuthService authService)
        {
            _socMedBdContext = socMedBdContext;
            _authService = authService;
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
    }
}
