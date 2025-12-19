using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using authDemoApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace authDemoApi.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private const string DefaultLocalReturnUrl = "http://localhost:5173/";
        private readonly string _defaultReturnUrl;

        public AuthRepository(IConfiguration configuration)
        {
            _defaultReturnUrl = configuration["Authentication:DefaultReturnUrl"] ?? DefaultLocalReturnUrl;
        }

        public AuthenticationProperties BuildAuthenticationProperties(string? returnUrl)
        {
            var redirectUri = string.IsNullOrWhiteSpace(returnUrl) ? _defaultReturnUrl : returnUrl;
            return new AuthenticationProperties { RedirectUri = redirectUri };
        }

        public async Task SignOutAsync(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        public UserInfo? GetCurrentUser(ClaimsPrincipal user)
        {
            if (user?.Identity is not { IsAuthenticated: true })
            {
                return null;
            }

            var name = user.Identity?.Name ?? string.Empty;
            var email = user.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value
                       ?? user.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            return new UserInfo(name, email);
        }
    }
}
