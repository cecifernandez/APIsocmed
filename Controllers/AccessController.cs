using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.Services;
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
        private readonly IAuthService _authService;


        public AccessController(IAuthService authService)
        {
            _authService = authService;

        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult>Register(UserDTO userDTO)
        {
            var result = await _authService.RegisterUserAsync(userDTO);
            if (result)
                return Ok(new { isSuccess = true });
            else
                return StatusCode(StatusCodes.Status500InternalServerError, new { isSuccess = false });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result = await _authService.LoginUserAsync(loginDTO);
            if (result.IsSuccess)
                return Ok(new { isSuccess = true, token = result.Token });
            else
                return Ok(new { isSuccess = false, token = "" });
        }

        [HttpPost]
        [Route("RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result.IsSuccess)
            {
                return Ok(new
                {
                    isSuccess = true,
                    accessToken = result.AccessToken,
                    refreshToken = result.RefreshToken
                });
            }
            else
            {
                return Unauthorized(new { isSuccess = false, message = result.Message });
            }
        }
    }
}
