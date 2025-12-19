using System.Threading.Tasks;
using authDemoApi.Repositories;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace authDemoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthRepository authRepository, ILogger<AuthController> logger)
        {
            _authRepository = authRepository;
            _logger = logger;
        }

        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login([FromQuery] string? returnUrl)
        {
            var properties = _authRepository.BuildAuthenticationProperties(returnUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _authRepository.SignOutAsync(HttpContext);
            return Ok();
        }

        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var userInfo = _authRepository.GetCurrentUser(User);
            if (userInfo is null)
            {
                _logger.LogInformation("Unauthenticated user attempted to access profile details.");
                return Unauthorized();
            }

            return Ok(userInfo);
        }
    }
}
