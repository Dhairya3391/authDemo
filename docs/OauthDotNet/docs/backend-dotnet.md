---
title: Backend Setup
---

# How the Backend Works

## What Actually Happens

Your backend does three things:
1. **Starts the OAuth flow** - Redirects to Google
2. **Receives the callback** - Google redirects back with proof of identity
3. **Creates a session** - Stores user info in a cookie

## Store Your Secrets

```bash
cd backend
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"
```

These identify your app to Google.

## Understanding Program.cs

### Authentication Configuration

```csharp
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
```

**What this means:**
- `DefaultScheme = Cookie` → Sessions are stored in cookies
- `DefaultChallengeScheme = Google` → When we need login, redirect to Google

### Controllers, Repository, and CORS

```csharp
builder.Services.AddControllers();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

app.UseRouting();
app.MapControllers();
```

**Why this matters:**
- `AddControllers()` enables the classic controller-based API style.
- `AddScoped<IAuthRepository, AuthRepository>()` keeps our auth logic in one reusable place.
- `MapControllers()` tells ASP.NET Core to route HTTP requests to controller actions.

```csharp

```csharp
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.Name = "auth_demo_cookie";
})
```

**What this means:**
- `HttpOnly = true` → JavaScript can't read it (prevents XSS attacks)
- `SameSite = Lax` → Cookie sent with navigation (works for redirects)
- The cookie stores: user ID, name, email, expiration time

## One Controller + One Repository

- **AuthController** keeps the endpoints short and easy to read.
- **AuthRepository** hides the cookie and claim details so you only see the basics.
- If you ever change how you store users, you only touch the repository.

```csharp
public class AuthRepository : IAuthRepository
{
    private readonly string _defaultReturnUrl = "http://localhost:5173/";

    public AuthenticationProperties BuildAuthenticationProperties(string? returnUrl)
    {
        var redirect = string.IsNullOrWhiteSpace(returnUrl) ? _defaultReturnUrl : returnUrl;
        return new AuthenticationProperties { RedirectUri = redirect };
    }

    public Task SignOutAsync(HttpContext httpContext)
        => httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

    public UserInfo? GetCurrentUser(ClaimsPrincipal user)
    {
        if (user.Identity is not { IsAuthenticated: true })
            return null;

        var name = user.Identity.Name ?? string.Empty;
        var email = user.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                   ?? user.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

        return new UserInfo(name, email);
    }
}
```

**Easy takeaway:** the controller calls three small helper methods, and those helpers keep all auth logic tidy.
_Tip: you can override `_defaultReturnUrl` via appsettings or user-secrets if you need a different landing page._

### Google Configuration

```csharp
.AddGoogle(options =>
{
    options.ClientId = "...";
    options.ClientSecret = "...";
    options.CallbackPath = "/api/auth/google-callback";
})
```

**What this means:**
- `ClientId/Secret` → Your app's credentials
- `CallbackPath` → Where Google redirects after login
- ASP.NET Core handles the callback automatically

### CORS Configuration

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowCredentials();
    });
});
```

**What this means:**
- Frontend (port 5173) can call backend (port 5042)
- `AllowCredentials()` → Cookies are allowed in cross-origin requests
- Without this, browsers block the requests

## The Three Endpoints

### 1. `/api/auth/login` - Start OAuth

```csharp
[HttpGet("login")]
public IActionResult Login([FromQuery] string? returnUrl)
{
    var properties = _authRepository.BuildAuthenticationProperties(returnUrl);
    return Challenge(properties, GoogleDefaults.AuthenticationScheme);
}
```

**What happens:**
1. User visits: `http://localhost:5042/api/auth/login?returnUrl=http://localhost:5173/`
2. Backend creates OAuth request to Google
3. Redirects user to Google's login page with:
   - Your ClientId
   - Callback URL: `http://localhost:5042/api/auth/google-callback`
   - Requested scopes: profile, email

### 2. `/api/auth/google-callback` - Handle Response

**You don't write this endpoint!** ASP.NET Core's Google middleware handles it automatically.

**What happens:**
1. Google redirects to: `http://localhost:5042/api/auth/google-callback?code=abc123...`
2. Backend sends `code` + `ClientSecret` to Google's token endpoint
3. Google returns an access token
4. Backend uses token to fetch user's profile from Google
5. Backend creates a cookie with user info (name, email)
6. Backend redirects to the `returnUrl`

### 3. `/api/auth/me` - Get Current User

```csharp
[HttpGet("me")]
public IActionResult GetCurrentUser()
{
    var userInfo = _authRepository.GetCurrentUser(User);
    if (userInfo is null)
        return Unauthorized();

    return Ok(userInfo);
}
```

**What happens:**
1. Browser sends request with cookie
2. ASP.NET Core reads cookie and builds the `User` object
3. Repository turns that into a simple `UserInfo` record
4. Return JSON: `{ "name": "John Doe", "email": "john@example.com" }`

### 4. `/api/auth/logout` - End Session

```csharp
[HttpGet("logout")]
public async Task<IActionResult> Logout()
{
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Ok();
}
```

**What happens:**
1. Browser sends request with cookie
2. Backend deletes the cookie
3. User is logged out

## The OAuth Dance

```
Browser              Your Backend           Google
  |                      |                    |
  |--[1] GET /login----->|                    |
  |                      |--[2] Redirect----->|
  |<----[3] Redirect-----------------------|
  |                      |                    |
  |     (User logs in at Google)             |
  |                      |                    |
  |<----[4] Redirect back with code----------|
  |--[5] GET /callback-->|                    |
  |   (with code)        |                    |
  |                      |--[6] Exchange code->|
  |                      |<--[7] Access token-|
  |                      |--[8] Get profile--->|
  |                      |<--[9] User info-----|
  |<----[10] Set cookie--|                    |
  |     & redirect       |                    |
```

**Key points:**
- Steps 1-10 happen automatically
- Your code just triggers `Challenge()` at step 1
- The middleware handles steps 2-10
- User ends up back at your frontend with a cookie

## What's in the Cookie?

The cookie is encrypted and contains:
- **Claims**: name, email, unique user ID from Google
- **Issued time**: when login happened
- **Expiration**: when session expires (default: 14 days)

ASP.NET Core encrypts this with a key stored on your server. The browser can't read or modify it.

## Next: Frontend

Now that you understand what the backend does, see how the frontend interacts with it: [Frontend Integration](frontend-vite)
