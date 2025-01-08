using APISocMed.Data;
using APISocMed.DomainServices;
using APISocMed.Interfaces;
using APISocMed.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IFollowerRepository _followerRepository;
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        private readonly SpotifyService _spotifyService;

        public UserController(IUserRepository accesRepository, AuthService authService, IMapper mapper, IFollowerRepository followerRepository, SpotifyService spotifyService)
        {
            _userRepository = accesRepository;
            _authService = authService;
            _mapper = mapper;
            _followerRepository = followerRepository;
            _spotifyService = spotifyService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var userModel = await _userRepository.GetUserByIdAsync(id);
            return Ok(userModel);
        }

        [HttpGet]
        [Route("user/{name}")]
        public async Task<IActionResult> GetUserByUsername(string name)
        {
            var userModel = await _userRepository.GetUserByUserNameAsync(name);
            return Ok(userModel);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("follow/{followingId}")]
        public async Task<IActionResult> FollowUser(int followingId)
        {
            var currentUser = GetCurrentUserId();
            if (currentUser == followingId)
                return BadRequest("You cannot follow yourself.");

            await _followerRepository.FollowUsersAsync(currentUser, followingId);
            return Ok("User followed successfully.");
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("unfollow/{followingId}")]
        public async Task<IActionResult> UnfollowUser(int followingId)
        {
            var currentUser = GetCurrentUserId();
            if (currentUser == followingId)
                return BadRequest("You cannot unfollow yourself.");

            await _followerRepository.UnfollowUsersAsync(currentUser, followingId);
            return Ok("User unfollowed successfully.");
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [Route("connectSpotify")]
        public async Task<IActionResult> ConnectSpotify([FromQuery] string code)
        {
            try
            {
                var currentUser = GetCurrentUserId();


                var accessToken = await _spotifyService.ExchangeCodeForToken(code);

                var userProfile = await _spotifyService.GetSpotifyUserId(accessToken.AccessToken);

                await _userRepository.SaveSpotifyTokensAsync(currentUser, userProfile, accessToken.RefreshToken);

                return Ok(new { message = "Spotify account connected successfully", accessToken });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [NonAction]
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims.");
            }

            return userId;
        }
    }
}
