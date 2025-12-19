---
title: Google Cloud Setup
---

# Registering Your App with Google

For OAuth to work, Google needs to know about your app. You'll get two values:
- **Client ID** - Public identifier for your app
- **Client Secret** - Private key that proves your backend is your backend

## Create OAuth Credentials

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create a new project (or select existing)
3. Go to **APIs & Services** → **OAuth consent screen**
   - Choose **External**
   - App name: "Auth Demo"
   - Add your email
   - Save
4. Go to **Credentials** → **Create Credentials** → **OAuth client ID**
5. Application type: **Web application**

### Configure URLs

**Authorized JavaScript origins** (where your frontend runs):
```
http://localhost:5173
```

**Authorized redirect URIs** (where Google sends users after login):
```
http://localhost:5042/api/auth/google-callback
```

This must **exactly match** the `CallbackPath` in your backend code.

6. Click **Create**
7. Copy the **Client ID** and **Client Secret**

## What These Values Do

### Client ID
- Identifies your app to Google
- Public (embedded in frontend redirects)
- Google shows this on the consent screen

### Client Secret
- Proves your backend is authentic
- **Private** - never commit to Git, never expose to frontend
- Used when your backend exchanges the authorization code for user info

## Why These URLs Matter

### JavaScript Origins
When your frontend redirects to Google, Google checks:
- "Is `http://localhost:5173` allowed?"
- If not allowed → Error

### Redirect URIs
After user logs in, Google redirects to:
- `http://localhost:5042/api/auth/google-callback?code=...`
- Google checks: "Is this URI registered?"
- If not registered → Error: "redirect_uri_mismatch"

## The OAuth Contract

By registering your app, you're telling Google:
- "My app is called 'Auth Demo'"
- "Users access it at localhost:5173"
- "Send authentication responses to localhost:5042/api/auth/google-callback"
- "Here's my Client ID and Secret to prove it's really me"

Google will only work with URLs you explicitly register.

## Next: Backend Setup

Now that Google knows about your app, configure your backend: [Backend Setup](backend-dotnet)
