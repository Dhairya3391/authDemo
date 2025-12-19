---
title: Run Locally
---

# Running the Application

## Start Both Servers

**Terminal 1 - Backend:**
```bash
cd backend
dotnet user-secrets set "Authentication:Google:ClientId" "YOUR_CLIENT_ID"
dotnet user-secrets set "Authentication:Google:ClientSecret" "YOUR_SECRET"
dotnet run
```

Should see: `Now listening on: https://localhost:5042`

**Terminal 2 - Frontend:**
```bash
cd frontend
npm install
npm run dev
```

Should see: `Local: http://localhost:5173/`

## Test the Flow

1. Open `http://localhost:5173`
2. Click "Sign in with Google"
3. Approve on Google's page
4. You're back at your app, logged in

## What Actually Happened

Open browser DevTools (F12) and watch the **Network** tab while logging in:

### Step 1: Initial Page Load
```
GET http://localhost:5173/
  → Returns HTML with React app

GET /api/auth/me (credentials: include)
  → Status: 401 Unauthorized
  → No cookie, so backend doesn't know who you are
  → Frontend shows "Sign in" button
```

### Step 2: Click "Sign In"
```
Navigate to: /api/auth/login?returnUrl=http://localhost:5173/

Backend responds: 302 Redirect
Location: https://accounts.google.com/o/oauth2/v2/auth?
  client_id=YOUR_CLIENT_ID
  &redirect_uri=http://localhost:5042/api/auth/google-callback
  &response_type=code
  &scope=openid profile email

Browser follows redirect → Now at Google's page
```

### Step 3: Approve at Google
```
User logs in and clicks "Continue"

Google responds: 302 Redirect
Location: http://localhost:5042/api/auth/google-callback?
  code=4/0AY0e-g7...

Browser follows redirect to your backend
```

### Step 4: Backend Handles Callback
```
GET /api/auth/google-callback?code=4/0AY0e-g7...

Backend:
  1. Sends code + client_secret to Google's token endpoint
  2. Google returns access_token
  3. Backend uses access_token to call Google's userinfo API
  4. Gets: { name: "John Doe", email: "john@example.com", sub: "123456" }
  5. Creates encrypted cookie with this info
  6. Responds: 302 Redirect
     Set-Cookie: auth_demo_cookie=CfDJ8...
     Location: http://localhost:5173/

Browser follows redirect → Back to your frontend with cookie!
```

### Step 5: Frontend Reloads
```
GET http://localhost:5173/
  → Returns HTML

GET /api/auth/me (credentials: include)
  → Cookie: auth_demo_cookie=CfDJ8...
  → Backend reads cookie, extracts user info
  → Status: 200 OK
  → Response: { "name": "John Doe", "email": "john@example.com" }

Frontend shows: "Welcome, John Doe"
```

## Inspect the Cookie

After logging in:
1. Open DevTools → **Application** → **Cookies** → `http://localhost:5173`
2. Find `auth_demo_cookie`
3. Value is encrypted (looks like: `CfDJ8KE...`)
4. Properties:
   - `HttpOnly` = true (JavaScript can't read it)
   - `SameSite` = Lax (sent with navigation)
   - `Secure` = false (allow HTTP in dev)

## Watch Network Requests

Every subsequent request includes the cookie:

```
GET /api/auth/me
Cookie: auth_demo_cookie=CfDJ8...

→ Backend decrypts cookie
→ Extracts claims: name, email, expiration
→ Returns user info
```

## Common Issues

**401 from /api/auth/me after login?**
- Cookie not being sent
- Check DevTools Network tab: is Cookie header present?
- Make sure `credentials: 'include'` in fetch

**"redirect_uri_mismatch" from Google?**
- Callback URL not registered
- Go to Google Console → Add `http://localhost:5042/api/auth/google-callback`

**CORS errors?**
- Backend not allowing frontend origin
- Check Program.cs: `WithOrigins("http://localhost:5173")`

More help: [Troubleshooting](troubleshooting)
