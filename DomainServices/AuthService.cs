using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using APISocMed.Models;

namespace APISocMed.DomainServices
{
    public class AuthService
    {
        private readonly IConfiguration _configuration;
        
        //access appsettings.json
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
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
                new Claim(ClaimTypes.Email, user.UserEmail)
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
    }
}
