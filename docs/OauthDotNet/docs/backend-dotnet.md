---
title: Backend (.NET) setup
---

The backend is an ASP.NET Core minimal API that uses cookie auth as the default scheme and Google as the challenge scheme.

## Prerequisites
- .NET 8/10 SDK installed
- `Authentication:Google` values available (ClientId, ClientSecret, CallbackPath)

## Configure secrets (recommended)
Use user-secrets to avoid committing secrets:

```bash
cd backend
dotnet user-secrets init
dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>"
dotnet user-secrets set "Authentication:Google:ClientSecret" "<client-secret>"
dotnet user-secrets set "Authentication:Google:CallbackPath" "/api/auth/google-callback"
```

`appsettings.Development.json` can also hold these values, but keep secrets out of source control.

## Key configuration (Program.cs)
The current setup:

```csharp
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
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // change to Always in production
    options.Cookie.Name = "auth_demo_cookie";
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
    options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
    options.CallbackPath = builder.Configuration["Authentication:Google:CallbackPath"] ?? "/api/auth/google-callback";
    options.SaveTokens = true;
});
```

### Middleware order
```csharp
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
```
Keep `UseAuthentication` before the mapped endpoints.

### Auth endpoints
- `/api/auth/login?returnUrl=<url>` → challenges Google and returns to `returnUrl` (defaults to `http://localhost:5173/`).
- `/api/auth/logout` → signs out the cookie.
- `/api/auth/me` → returns `{ name, email }` when authenticated; 401 otherwise.

### Security notes
- In production set `options.Cookie.SecurePolicy = CookieSecurePolicy.Always` and ensure `baseUrl` uses HTTPS.
- Add your deployed frontend origin to CORS and to Google allowed origins.
- Keep callback paths consistent between Google Console and `Authentication:Google:CallbackPath`.
- If hosting behind a reverse proxy, ensure `X-Forwarded-*` headers are forwarded so HTTPS is detected correctly.
