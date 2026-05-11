using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using PetAdopt.BLL.DTOs;
using PetAdopt.BLL.Services.Interfaces;

namespace PetAdopt.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        // Dependency Injection of the Auth Service
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }


        // ----------REGISTER----------


        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            try
            {
                var result = await _authService.RegisterAsync(dto);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ----------LOGIN----------


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);

                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // ----------LOGOUT----------

       
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();

            return Ok("Logged out successfully");
        }
    }
}
