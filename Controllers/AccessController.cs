using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.DomainServices;
using APISocMed.Models;
using APISocMed.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using APISocMed.Data;
using APISocMed.Interfaces;
using AutoMapper;
using APISocMed.Repositories;


namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        private readonly IRefreshTokenRepository _refreshTokenRepository;


        public AccessController(IUserRepository accesRepository, AuthService authService, IMapper mapper, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = accesRepository;
            _authService = authService;
            _mapper = mapper;
            _refreshTokenRepository = refreshTokenRepository;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult>Register(UserDTO userDTO)
        {
            var userModel = _mapper.Map<User>(userDTO);
            userModel.PasswordHash = _authService.encryptSHA256(userDTO.password);

            bool isSuccess = await _userRepository.RegisterAsync(userModel);

            if (isSuccess)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var passwordHash = _authService.encryptSHA256(loginDTO.password);
            var user = await _userRepository.AuthenticateUserAsync(loginDTO.email, passwordHash);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }

            var token = _authService.getJWT(user);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var tokenEntity = await _refreshTokenRepository.GetRefreshTokenAsync(refreshToken);

            if (tokenEntity == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, message = "Invalid or expired refresh token" });
            }

            var user = await _userRepository.GetUserByIdAsync(tokenEntity.UserId);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, new { isSuccess = false, message = "User not found" });
            }

            var newAccessToken = _authService.getJWT(user);

            tokenEntity.IsUsed = true;
            await _refreshTokenRepository.UpdateRefreshTokenAsync(tokenEntity);

            var newRefreshToken = _authService.GenerateRefreshToken();
            await _refreshTokenRepository.SaveRefreshTokenAsync(user.UserId, newRefreshToken);

            return StatusCode(StatusCodes.Status200OK, new
            {
                isSuccess = true,
                accessToken = newAccessToken,
                refreshToken = newRefreshToken
            });
        }
    }
}
