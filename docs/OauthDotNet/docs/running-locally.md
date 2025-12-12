---
title: Run locally
---

1) Backend
- `cd backend`
- `dotnet restore`
- Set Google secrets (user-secrets or `appsettings.Development.json`).
- `dotnet run`
- Confirm it listens on `https://localhost:5042` (or check console output).

2) Frontend
- `cd frontend`
- `npm install`
- `npm run dev`
- Open `http://localhost:5173`.

3) Test the flow
- Load the app; it calls `/api/auth/me` with cookies.
- Click **Sign in with Google**; approve; you are redirected back with an auth cookie.
- Click **Sign out** to clear the cookie.

Tips
- If cookies do not stick, ensure the backend and frontend run on the same top-level domain in dev (`localhost`) and that the browser is not blocking third-party cookies.
- Keep the backend running over HTTPS during dev; browsers may block cookies over plain HTTP when `SameSite=Lax`.
