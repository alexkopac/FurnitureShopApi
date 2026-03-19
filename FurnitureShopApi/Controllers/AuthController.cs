using FurnitureShopApi.DTOs;
using FurnitureShopApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FurnitureShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto dto) => Ok(new { Message = await _authService.RegisterAsync(dto) });

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserDto dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == "Invalid credentials")
                return Unauthorized(token);
            return Ok(new { Token = token });
        }
    }
}