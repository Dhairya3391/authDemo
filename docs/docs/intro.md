---
sidebar_position: 1
title: Overview
slug: /overview
---

# Google OAuth Authentication

Learn how OAuth 2.0 authentication works by building a real application.

## What Is OAuth?

Instead of creating your own login system (usernames, passwords, password resets, etc.), you let Google handle it. Your app never sees the user's password.

**The OAuth Flow:**

```
1. User clicks "Sign in with Google"
   ↓
2. Your app redirects to Google
   ↓
3. User logs in at Google (not your app!)
   ↓
4. Google redirects back with a proof of identity
   ↓
5. Your backend verifies this proof with Google
   ↓
6. Your backend creates a session cookie
   ↓
7. User is logged in
```

## Why Use OAuth?

- **Security**: Google handles passwords, 2FA, account security
- **Convenience**: Users already have Google accounts
- **Less Code**: You don't build login forms, password resets, etc.
- **Trust**: Users trust Google's login page

## What This Project Does

**Backend (ASP.NET Core):**
- Handles the OAuth handshake with Google
- Creates and validates session cookies
- Provides user info from the session

**Frontend (React):**
- Redirects to backend to start login
- Sends cookies with each request
- Displays user information

## The Three Endpoints

Your backend has three endpoints:

1. **`GET /api/auth/login`** - Starts the OAuth flow
2. **`GET /api/auth/me`** - Returns current user (or 401 if not logged in)
3. **`GET /api/auth/logout`** - Ends the session

## What You'll Learn

- How OAuth 2.0 authorization code flow works
- What happens during the Google redirect
- How sessions are stored in cookies
- Why `credentials: 'include'` is necessary
- How CORS allows cross-origin cookies

**Follow in order:**
1. [Google Cloud Setup](google-cloud-setup)
2. [Backend Setup](backend-dotnet)
3. [Frontend Integration](frontend-vite)
4. [Run Locally](running-locally)
