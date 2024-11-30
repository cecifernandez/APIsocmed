using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.Custom;
using APISocMed.Models;
using APISocMed.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;


namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly SocMedBdContext _socMedBdContext;
        public PostController(SocMedBdContext socMedBdContext)
        {
            _socMedBdContext = socMedBdContext;
        }

        [HttpGet]
        [Route("Posts")]
        public async Task<IActionResult> Posts()
        {
            var posts = _socMedBdContext.Posts.ToListAsync();

            Console.WriteLine("Post");

            return StatusCode(StatusCodes.Status200OK, new { value = posts });
        }
    }
}
