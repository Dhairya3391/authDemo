---
title: Troubleshooting
---

# Common Issues and Solutions

Having trouble? This guide covers the most common problems students encounter.

## Issue: "401 Unauthorized" When Calling /api/auth/me

**Symptoms:**
- Login seems to work, but `/api/auth/me` returns 401
- User info doesn't show up after login

**Causes & Solutions:**

### 1. Cookies aren't being sent
**Check:** Look at the Network tab in browser DevTools. Does the request to `/api/auth/me` have a Cookie header?

**Fix:** Make sure every fetch includes `credentials: 'include'`:
```tsx
fetch('http://localhost:5042/api/auth/me', {
  credentials: 'include'  // Don't forget this!
})
```

### 2. Cookie domain mismatch
**Check:** Are you using `localhost` for both frontend and backend? Or mixing `localhost` and `127.0.0.1`?

**Fix:** Always use `localhost` for both:
- Frontend: `http://localhost:5173`
- Backend: `http://localhost:5042`

### 3. Browser blocking cookies
**Check:** Are you in Incognito mode with strict settings? Some browsers block third-party cookies.

**Fix:**
- Try in a regular browser window
- Disable "Block third-party cookies" in browser settings
- Check browser console for cookie warnings

## Issue: CORS Errors

**Symptoms:**
- Error in console: "Access to fetch has been blocked by CORS policy"
- Network requests show red in DevTools

**Causes & Solutions:**

### 1. Frontend URL not in CORS config
**Check:** What's the exact error message? Does it mention your frontend URL?

**Fix:** In `backend/Program.cs`, make sure your frontend URL is listed:
```csharp
policy.WithOrigins("http://localhost:5173")
      .AllowCredentials();
```

### 2. Missing AllowCredentials
**Check:** Does your CORS configuration include `AllowCredentials()`?

**Fix:** Add it:
```csharp
policy.WithOrigins("http://localhost:5173")
      .AllowAnyHeader()
      .AllowAnyMethod()
      .AllowCredentials();  // Required for cookies!
```

### 3. Backend not running
**Check:** Is the backend actually running? Can you access `http://localhost:5042` directly?

**Fix:** Start the backend: `cd backend && dotnet run`

## Issue: Google Redirect URI Mismatch

**Symptoms:**
- Error from Google: "Error 400: redirect_uri_mismatch"
- Can't complete login flow

**Causes & Solutions:**

### 1. Callback URL not registered
**Check:** What callback URL is shown in the Google error?

**Fix:** Go to Google Cloud Console → Credentials and add the exact URL:
```
http://localhost:5042/api/auth/google-callback
https://localhost:5042/api/auth/google-callback
```

### 2. Typo in callback path
**Check:** Is it `/api/auth/google-callback` exactly? No extra slashes? Correct spelling?

**Fix:** Make sure it matches exactly in:
- Google Cloud Console
- `backend/Program.cs` (in Google options)

### 3. Using wrong protocol
**Check:** Are you accessing via HTTP but registered HTTPS in Google, or vice versa?

**Fix:** Register both in Google Console, or make sure the protocol matches

## Issue: Login Loop (Keeps Redirecting)

**Symptoms:**
- User approves Google login
- Gets redirected back
- Immediately redirected to Google again
- Never actually logs in

**Causes & Solutions:**

### 1. Cookie not being saved
**Check:** In DevTools Application/Storage tab, do you see `auth_demo_cookie` after login?

**Fix:** Check cookie configuration in `backend/Program.cs`:
```csharp
.AddCookie(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;  // Important!
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;  // For local dev
    options.Cookie.Name = "auth_demo_cookie";
})
```

### 2. Callback not completing
**Check:** Look at Network tab. Does `/api/auth/google-callback` complete successfully?

**Fix:** Check browser console and backend terminal for errors

## Issue: Backend Won't Start

**Symptoms:**
- `dotnet run` fails
- Error about missing configuration

**Causes & Solutions:**

### 1. Google credentials not set
**Error:** "Google ClientId is not configured"

**Fix:** Set your secrets:
```bash
cd backend
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"
```

### 2. Port already in use
**Error:** "Address already in use"

**Fix:** 
- Find and stop the other process using port 5042
- Or change the port in `backend/Properties/launchSettings.json`

## Issue: Changes Not Showing Up

**Symptoms:**
- Made code changes but nothing happens
- Old behavior still present

**Solutions:**

### For Backend Changes
```bash
# Stop the backend (Ctrl+C) and restart
dotnet run
```

### For Frontend Changes
- Vite should auto-reload
- If not, refresh the browser (Ctrl+R or Cmd+R)
- Or restart: `npm run dev`

### For Configuration Changes
```bash
# Backend: restart
cd backend
dotnet run

# Frontend: usually auto-reloads, but restart if needed
cd frontend
npm run dev
```

## Getting More Information

### Enable Detailed Logging

In `backend/appsettings.Development.json`, add:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Information",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}
```

This will show detailed authentication logs in your terminal.

### Check Browser DevTools

1. Press **F12** to open DevTools
2. Go to **Console** tab - look for errors
3. Go to **Network** tab - see all requests and responses
4. Go to **Application** (Chrome) or **Storage** (Firefox) - check cookies

### Test Backend Directly

You can test backend endpoints directly:

1. Start the backend
2. Open browser to `http://localhost:5042/api/auth/login`
3. Complete Google login
4. Navigate to `http://localhost:5042/api/auth/me`
5. You should see your user info JSON

## Still Stuck?

If you've tried everything:

1. **Clear everything and start fresh:**
   - Clear browser cookies for localhost
   - Stop both servers
   - Delete `backend/bin` and `backend/obj` folders
   - Run `dotnet restore` in backend
   - Start servers again

2. **Verify your setup:**
   - Google credentials are correct
   - Secrets are set with `dotnet user-secrets list`
   - Both servers are running
   - No typos in URLs or paths

3. **Check the code:**
   - Compare your code to the repository
   - Make sure you haven't accidentally changed important config

## Quick Checklist

When debugging, go through this list:

- [ ] Backend is running on port 5042
- [ ] Frontend is running on port 5173
- [ ] Google credentials are set with user-secrets
- [ ] Callback URL is registered in Google Console
- [ ] CORS includes your frontend URL and `AllowCredentials()`
- [ ] All fetch requests include `credentials: 'include'`
- [ ] Using `localhost` (not `127.0.0.1`) consistently
- [ ] Browser allows cookies
- [ ] No errors in browser console
- [ ] No errors in backend terminal

Good luck! Most issues are simple misconfigurations that are easy to fix once you know what to look for.
