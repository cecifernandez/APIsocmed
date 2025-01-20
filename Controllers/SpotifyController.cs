using APISocMed.Services;
using APISocMed.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Security.Claims;

namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyController : ControllerBase
    {
        private readonly SpotifyService _spotifyService;
        private readonly AuthService _authService;


        public SpotifyController(SpotifyService spotifyService, AuthService authService)
        {
            _spotifyService = spotifyService;
            _authService = authService;
        }

        [HttpGet("authorize")]
        public IActionResult Authorize()
        {
            var url = _spotifyService.GetAuthorizationUrl();
            //return Redirect(url);
            return Ok(new { authorizationUrl = url });
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback([FromQuery] string code)
        {
            var tokenResponse = await _spotifyService.ExchangeCodeForToken(code);
            return Ok(tokenResponse);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserProfile([FromHeader] string accessToken)
        {
            var profile = await _spotifyService.GetUserProfile(accessToken);
            return Ok(profile);
        }
    }
}
