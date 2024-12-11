using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.DomainServices;
using APISocMed.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using APISocMed.Data;
using APISocMed.Interfaces;
using APISocMed.Models;
using System.Security.Claims;


namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly IPostRepository _postRepository;
        private readonly AuthService _authService;
        public PostController(IPostRepository postRepository, AuthService authService)
        {
            _postRepository = postRepository;
            _authService = authService;
        }

        [HttpGet]
        [Route("Posts")]
        public async Task<IActionResult> Posts()
        {
            var posts = await _postRepository.GetAllPosts();
            return Ok(posts);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreatePost([FromBody] PostDTO postDto)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Usuario no autenticado." });
            }

            int userId = int.Parse(userIdClaim.Value);

            var (isSuccess, message, createdPost) = await _postRepository.CreatePost(postDto, userId);

            if (isSuccess)
                return Ok(new { message, createdPost });

            return BadRequest(new { error = "Post not created" });
        }
    }
}

