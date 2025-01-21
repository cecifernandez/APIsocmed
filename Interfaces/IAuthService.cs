using APISocMed.Models;
using APISocMed.Models.DTOs;

namespace APISocMed.Interfaces
{
    public interface IAuthService
    {
        string encryptSHA256(string password);
        string getJWT(User user);
        string GenerateRefreshToken();
        int? ValidateJwt(string token);
        Task<bool> RegisterUserAsync(UserDTO userDTO);
        Task<(bool IsSuccess, string Token)> LoginUserAsync(LoginDTO loginDTO);
        Task<(bool IsSuccess, string Message, string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken);
    }
}
