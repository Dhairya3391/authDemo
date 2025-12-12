using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax; // better for localhost
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // allow http during local dev
    options.Cookie.Name = "auth_demo_cookie";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]
        ?? throw new InvalidOperationException("Google ClientId is not configured");
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]
        ?? throw new InvalidOperationException("Google ClientSecret is not configured");
    options.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"] ?? "/api/auth/google-callback";
    options.SaveTokens = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/auth/login", (string? returnUrl, HttpContext context) =>
{
    var redirectUri = string.IsNullOrWhiteSpace(returnUrl)
        ? "http://localhost:5173/"
        : returnUrl;

    var props = new AuthenticationProperties { RedirectUri = redirectUri };
    return Results.Challenge(props, new[] { GoogleDefaults.AuthenticationScheme });
});

app.MapGet("/api/auth/logout", async (HttpContext context) =>
{
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Ok();
});

app.MapGet("/api/auth/me", (HttpContext context) =>
{
    Console.WriteLine($"=== /api/auth/me called ===");
    Console.WriteLine($"IsAuthenticated: {context.User?.Identity?.IsAuthenticated}");
    Console.WriteLine($"Cookies present: {string.Join(", ", context.Request.Cookies.Select(c => c.Key))}");

    if (context.User?.Identity is not { IsAuthenticated: true })
    {
        Console.WriteLine("User not authenticated - returning 401");
        return Results.Unauthorized();
    }

    var name = context.User.Identity?.Name ?? string.Empty;
    var email = context.User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
    Console.WriteLine($"User authenticated: {name}, {email}");
    return Results.Ok(new { name, email });
});

app.Run();
