using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.Services;
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
        private readonly IPostService _postService;

        public PostController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpGet]
        [Route("Posts")]
        public async Task<IActionResult> Posts()
        {
            var posts = await _postService.GetAllPostsAsync();
            return Ok(posts);
        }

        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> CreatePost([FromBody] PostDTO postDto)
        {
            var userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "User not authenticated." });
            }

            int userId = int.Parse(userIdClaim.Value);

            var (isSuccess, message, createdPost) = await _postService.CreatePostAsync(postDto, userId);

            if (isSuccess)
                return Ok(new { message, createdPost });

            return BadRequest(new { error = "Post not created" });
        }

        [HttpGet]
        [Route("Feed")]
        public async Task<ActionResult> GetFeed()
        {
            var userId = GetCurrentUserId();
            var feed = await _postService.GetFeedAsync(userId);
            return Ok(new { message = "Feed", feed });
        }

        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(int postId)
        {
            int userId = GetCurrentUserId();
            await _postService.DeletePostAsync(postId, userId);

            return Ok();
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

