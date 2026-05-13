using DigitalGoldWallet.API.Dtos.AuthDto;
using DigitalGoldWallet.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalGoldWallet.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> LoginUser(LoginDto dto)
        {
            var result = await _authService.LoginUserAsync(dto);

            return Ok(new
            {
                StatusCode = 200,
                Message = "User login successful",
                Data = result
            });
        }

        [HttpPost("login-vendor")]
        public async Task<IActionResult> LoginVendor(LoginDto dto)
        {
            var result = await _authService.LoginVendorAsync(dto);

            return Ok(new
            {
                StatusCode = 200,
                Message = "Vendor login successful",
                Data = result
            });
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> RegisterUser(RegisterDto dto)
        {
            await _authService.RegisterUserAsync(dto);

            return Ok(new
            {
                StatusCode = 200,
                Message = "User registered successfully"
            });
        }
    }
}