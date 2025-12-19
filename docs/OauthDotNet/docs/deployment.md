---
title: Deployment (Advanced)
---

# Deploying to Production

Once your app works locally, you might want to deploy it to a real server. This guide covers the key changes needed.

> **Note:** This is an advanced topic. Make sure everything works locally first!

## Key Differences: Local vs Production

| Aspect | Local Development | Production |
|--------|------------------|------------|
| Protocol | HTTP (localhost) | HTTPS (required!) |
| Cookies | SecurePolicy.None | SecurePolicy.Always |
| URLs | localhost:5173, localhost:5042 | yourdomain.com, api.yourdomain.com |
| Secrets | user-secrets | Environment variables |

## Step 1: Update Cookie Security

In `backend/Program.cs`, change the cookie configuration:

```csharp
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Changed from None!
    options.Cookie.Name = "auth_demo_cookie";
})
```

**Why?** `Secure` cookies only work over HTTPS, which is required in production.

## Step 2: Update CORS Configuration

Add your production URLs:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",     // Keep for local dev
                "https://yourdomain.com"     // Add your production URL
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

Replace `yourdomain.com` with your actual domain.

## Step 3: Update Google Cloud Console

Add your production URLs to Google Console:

### Authorized JavaScript Origins
```
https://yourdomain.com
```

### Authorized Redirect URIs
```
https://api.yourdomain.com/api/auth/google-callback
```

(Use your actual backend URL)

## Step 4: Configure Secrets via Environment Variables

Production servers use environment variables, not user-secrets:

```bash
Authentication__Google__ClientId=your-client-id
Authentication__Google__ClientSecret=your-client-secret
```

**Note:** Double underscores (`__`) replace colons (`:`) in environment variables.

How you set these depends on your hosting platform:
- **Azure:** Application Settings
- **AWS:** Systems Manager Parameter Store or Secrets Manager
- **Docker:** Environment variables in docker-compose.yml
- **Linux server:** Add to systemd service file

## Step 5: Update Frontend URLs

In `frontend/src/App.tsx`, replace `localhost:5042` with your backend URL:

```tsx
// Before (local)
fetch('http://localhost:5042/api/auth/me', { credentials: 'include' })

// After (production)
fetch('https://api.yourdomain.com/api/auth/me', { credentials: 'include' })
```

**Better:** Use environment variables:

Create `frontend/.env.production`:
```
VITE_API_URL=https://api.yourdomain.com
```

Then in your code:
```tsx
fetch(`${import.meta.env.VITE_API_URL}/api/auth/me`, { 
  credentials: 'include' 
})
```

## Example: Deploying to Azure

### Backend (App Service)
1. Publish: `dotnet publish -c Release`
2. Deploy `bin/Release/net8.0/publish/` to Azure
3. Add Application Settings:
   - `Authentication__Google__ClientId`
   - `Authentication__Google__ClientSecret`
4. Ensure HTTPS is enforced

### Frontend (Static Web App or Blob Storage)
1. Build: `npm run build`
2. Deploy `dist/` folder
3. Configure custom domain with HTTPS

## Example: Deploying with Docker

**Backend Dockerfile:**
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY bin/Release/net8.0/publish/ .
ENTRYPOINT ["dotnet", "authDemoApi.dll"]
```

**docker-compose.yml:**
```yaml
services:
  backend:
    build: ./backend
    environment:
      - Authentication__Google__ClientId=${GOOGLE_CLIENT_ID}
      - Authentication__Google__ClientSecret=${GOOGLE_CLIENT_SECRET}
    ports:
      - "5042:8080"
```

## Security Checklist

Before going live, ensure:

- [ ] Using HTTPS everywhere (backend and frontend)
- [ ] Cookie `SecurePolicy` is `Always`
- [ ] Secrets are in environment variables (not in code!)
- [ ] Production URLs added to Google Console
- [ ] CORS only allows your actual frontend domain
- [ ] Backend validates `returnUrl` to prevent open redirects
- [ ] Keep secrets secret (no commits, no logs)

## Common Production Issues

### Cookies Not Working Over HTTPS

**Problem:** Login works but session isn't maintained

**Solutions:**
- Set `SecurePolicy = Always`
- Ensure your backend URL uses HTTPS
- Check that cookies aren't being blocked by browser

### CORS Errors in Production

**Problem:** Frontend can't reach backend

**Solutions:**
- Add exact production URL to CORS (no trailing slash!)
- Keep `AllowCredentials()`
- Check that backend is accessible from frontend domain

### Google Redirect Mismatch

**Problem:** Google shows "redirect_uri_mismatch" in production

**Solutions:**
- Add exact callback URL to Google Console
- Use HTTPS in the callback URL
- Check that the domain and path match exactly

## Monitoring and Logs

In production, enable proper logging:

**appsettings.Production.json:**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore.Authentication": "Information"
    }
  }
}
```

This logs authentication issues without being too verbose.

## Need Help?

Deployment varies greatly by platform. Consult your hosting provider's documentation for:
- How to set environment variables
- How to enable HTTPS
- How to view logs

Focus on getting local development working perfectly first. Production is just a matter of changing URLs and enabling security features!
