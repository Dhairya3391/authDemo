---
title: Frontend Integration
---

# How the Frontend Works

## The Critical Rule: Include Credentials

Browsers don't send cookies to different origins by default. You **must** explicitly include them:

```tsx
fetch('http://localhost:5042/api/auth/me', { 
  credentials: 'include'
})
```

**What this does:**
- Tells browser to include cookies in the request
- Backend receives the cookie and knows who you are
- Without it: backend sees an anonymous request

## App.tsx Explained

### On Page Load: Check Session

```tsx
useEffect(() => {
  fetch('/api/auth/me', { credentials: 'include' })
    .then(res => {
      if (res.status === 401) {
        setUser(null);  // Not logged in
        return;
      }
      return res.json();
    })
    .then(data => setUser(data))
    .finally(() => setLoading(false));
}, []);
```

**What happens:**
1. Browser sends `GET /api/auth/me` with cookie
2. If cookie exists and valid → Backend returns `{ name: "...", email: "..." }`
3. If no cookie or expired → Backend returns `401 Unauthorized`
4. Frontend updates UI based on response

### Starting Login

```tsx
const login = () => {
  const returnUrl = encodeURIComponent(window.location.href);
  window.location.href = `/api/auth/login?returnUrl=${returnUrl}`;
};
```

**What happens:**
1. Browser navigates to `/api/auth/login?returnUrl=http://localhost:5173/`
2. Backend redirects to Google
3. User logs in at Google
4. Google redirects to `/api/auth/google-callback`
5. Backend creates cookie, redirects to `returnUrl`
6. User lands back at frontend **with cookie**
7. Frontend calls `/api/auth/me` again, gets user info

**Why full page redirect?**
- OAuth requires redirects (not AJAX)
- Cookie gets set during the redirect flow
- Browser automatically includes cookie in subsequent requests

### Logging Out

```tsx
const logout = async () => {
  await fetch('/api/auth/logout', { credentials: 'include' });
  setUser(null);
};
```

**What happens:**
1. Browser sends `GET /api/auth/logout` with cookie
2. Backend deletes the cookie
3. Frontend clears user state
4. Next call to `/api/auth/me` returns 401

## Why CORS Matters

Your frontend (`http://localhost:5173`) and backend (`http://localhost:5042`) are different origins.

**Without CORS configuration:**
```
Browser: "I want to call localhost:5042 from localhost:5173"
Browser: "Security policy says NO"
Request blocked.
```

**With CORS configuration:**
```csharp
policy.WithOrigins("http://localhost:5173")
      .AllowCredentials();
```

```
Browser: "I want to call localhost:5042 from localhost:5173"
Backend: "I allow requests from localhost:5173 with credentials"
Browser: "OK, sending request with cookie"
```

## The Complete Flow

```
User visits http://localhost:5173
    ↓
Frontend calls /api/auth/me with credentials
    ↓
Backend checks cookie → No cookie found → Returns 401
    ↓
Frontend shows "Sign in" button
    ↓
User clicks "Sign in"
    ↓
Browser navigates to /api/auth/login?returnUrl=...
    ↓
Backend redirects to Google
    ↓
User logs in at Google
    ↓
Google redirects to /api/auth/google-callback?code=...
    ↓
Backend exchanges code for user info
    ↓
Backend creates cookie, redirects to returnUrl
    ↓
Browser lands back at http://localhost:5173 (with cookie!)
    ↓
Frontend calls /api/auth/me with credentials
    ↓
Backend reads cookie → User found → Returns { name, email }
    ↓
Frontend shows "Welcome, [name]"
```

## What's in the Requests?

### Request to /api/auth/me

```
GET /api/auth/me HTTP/1.1
Host: localhost:5042
Origin: http://localhost:5173
Cookie: auth_demo_cookie=CfDJ8KE...
```

Backend reads the cookie, decrypts it, extracts claims, returns user info.

### Response

```
HTTP/1.1 200 OK
Content-Type: application/json

{
  "name": "John Doe",
  "email": "john@example.com"
}
```

Frontend receives this and displays it.

## Testing

1. Open browser DevTools (F12)
2. Go to **Application** → **Cookies** → `http://localhost:5173`
3. After login, you'll see `auth_demo_cookie` (encrypted, can't read it)
4. Go to **Network** tab
5. Find request to `/api/auth/me`
6. Check **Request Headers** → Cookie is sent
7. Check **Response** → User info returned

## Next: Run It

Now run the app and see it work: [Run Locally](running-locally)
