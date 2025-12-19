using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using authDemoApi.Models;

namespace authDemoApi.Repositories
{
    public interface IAuthRepository
    {
        AuthenticationProperties BuildAuthenticationProperties(string? returnUrl);
        Task SignOutAsync(HttpContext httpContext);
        UserInfo? GetCurrentUser(ClaimsPrincipal user);
    }
}
