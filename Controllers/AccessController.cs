using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.DomainServices;
using APISocMed.Models;
using APISocMed.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using APISocMed.Data;
using APISocMed.Interfaces;


namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly IAccessRepository _accessRepository;
        private readonly AuthService _authService;
        public AccessController(IAccessRepository accesRepository, AuthService authService)
        {
            _accessRepository = accesRepository;
            _authService = authService;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult>Register(UserDTO userDTO)
        {
            var userModel = new User
            {
                UserName = userDTO.username,
                UserEmail = userDTO.email,
                PasswordHash = _authService.encryptSHA256(userDTO.password) 
            };

            bool isSuccess = await _accessRepository.RegisterAsync(userModel);

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
            var user = await _accessRepository.AuthenticateUserAsync(loginDTO.email, passwordHash);

            if (user == null)
            {
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            }

            var token = _authService.getJWT(user);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token });
        }
    }
}
