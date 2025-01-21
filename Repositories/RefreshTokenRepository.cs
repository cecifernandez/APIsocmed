using APISocMed.Data;
using APISocMed.Services;
using APISocMed.Interfaces;
using APISocMed.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Security.Cryptography;

namespace APISocMed.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly SocMedBdContext _socMedBdContext;
        public RefreshTokenRepository(SocMedBdContext socMedBdContext)
        {
            _socMedBdContext = socMedBdContext;
        }

        public async Task SaveRefreshTokenAsync(int userId, string refreshToken)
        {
            var token = new RefreshToken
            {
                Token = refreshToken,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _socMedBdContext.RefreshTokens.Add(token);
            await _socMedBdContext.SaveChangesAsync();
        }

        public async Task<RefreshToken?> GetRefreshTokenAsync(string token)
        {
            return await _socMedBdContext.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsUsed && !rt.IsRevoked && rt.ExpiresAt > DateTime.UtcNow);
        }

        public async Task UpdateRefreshTokenAsync(RefreshToken token)
        {
            token.IsUsed = true;
            _socMedBdContext.RefreshTokens.Update(token);
            await _socMedBdContext.SaveChangesAsync();
        }
    }
}
