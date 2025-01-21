using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APISocMed.Models;
using APISocMed.Interfaces;
using AutoMapper;
using APISocMed.Models.DTOs;

namespace APISocMed.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        //access appsettings.json
        public AuthService(IConfiguration configuration, IUserRepository userRepository, IMapper mapper, IRefreshTokenRepository refreshTokenRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _mapper = mapper;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<bool> RegisterUserAsync(UserDTO userDTO)
        {
            var userModel = _mapper.Map<User>(userDTO);
            userModel.PasswordHash = encryptSHA256(userDTO.password);
            return await _userRepository.RegisterAsync(userModel);
        }

        public async Task<(bool IsSuccess, string Token)> LoginUserAsync(LoginDTO loginDTO)
        {
            var passwordHash = encryptSHA256(loginDTO.password);
            var user = await _userRepository.AuthenticateUserAsync(loginDTO.email, passwordHash);
            if (user == null) return (false, null);
            var token = getJWT(user);
            return (true, token);
        }

        public async Task<(bool IsSuccess, string Message, string AccessToken, string RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var tokenEntity = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);
            if (tokenEntity == null) return (false, "Invalid or expired refresh token", null, null);

            var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);
            if (user == null) return (false, "User not found", null, null);

            var newAccessToken = getJWT(user);
            tokenEntity.IsUsed = true;
            await _refreshTokenRepository.UpdateRefreshTokenAsync(tokenEntity);

            var newRefreshToken = GenerateRefreshToken();
            await _refreshTokenRepository.SaveRefreshTokenAsync(user.UserId, newRefreshToken);

            return (true, null, newAccessToken, newRefreshToken);
        }

        //hash password
        public string encryptSHA256(string password)
        {
            //hashing
            using (SHA256 sha265Hash = SHA256.Create())
            {
                byte[] bytes = sha265Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                //convert bytes to string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2")); //prints in hexa
                }

                return builder.ToString();
            }           
        }

        public string getJWT(User user)
        {
            //create user info for the token
            var userClaims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim("id", user.UserId.ToString())
            };

            //user credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            //token details
            var jwtConfiguration = new JwtSecurityToken(
                claims: userClaims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(jwtConfiguration);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public int? ValidateJwt(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
            };

            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
                    return int.TryParse(userIdClaim, out var userId) ? userId : null;
                }
                else
                {
                    throw new SecurityTokenException("Invalid token");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token validation failed: {ex.Message}");
                return null;
            }
        }
    }
}
