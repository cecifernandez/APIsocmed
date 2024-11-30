using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISocMed.Custom;
using APISocMed.Models;
using APISocMed.Models.DTOs;
using Microsoft.AspNetCore.Authorization;


namespace APISocMed.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [ApiController]
    public class AccessController : ControllerBase
    {
        private readonly SocMedBdContext _socMedBdContext;
        private readonly utilities _utilities;
        public AccessController(SocMedBdContext socMedBdContext, utilities utilities)
        {
            _socMedBdContext = socMedBdContext;
            _utilities = utilities;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult>Register(UserDTO userDTO)
        {
            var userModel = new User
            {
                UserName = userDTO.username,
                UserEmail = userDTO.email,
                PasswordHash = _utilities.encryptSHA256(userDTO.password) 
            };

            await _socMedBdContext.Users.AddAsync(userModel);
            await _socMedBdContext.SaveChangesAsync();

            if (userModel.UserId != 0)
                return StatusCode(StatusCodes.Status200OK, new {isSuccess = true});
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var userExists = await _socMedBdContext.Users
                             .Where(u =>
                                u.UserEmail == loginDTO.email &&
                                u.PasswordHash == _utilities.encryptSHA256(loginDTO.password) 
                             ).FirstOrDefaultAsync();

            if(userExists == null)
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = false, token = "" });
            else
                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true, token = _utilities.getJWT(userExists) });
        }
    }
}
