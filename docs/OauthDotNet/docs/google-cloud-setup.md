---
title: Google Cloud setup
---

Set up the OAuth client that the .NET backend uses.

1. Go to **Google Cloud Console → APIs & Services → Credentials**.
2. Create an **OAuth client ID** (type: Web application).
3. Add **Authorized JavaScript origins**: `http://localhost:5173` and `https://localhost:5173` for local dev.
4. Add **Authorized redirect URIs**: `http://localhost:5042/api/auth/google-callback` and `https://localhost:5042/api/auth/google-callback` (match your backend base URL and the `Authentication:Google:CallbackPath` value).
5. Copy the **Client ID** and **Client secret**; you will place them in `appsettings.Development.json` or user-secrets (recommended).
6. If you deploy, add the production origin and callback URLs to the same lists.

Notes
- Keep the consent screen published and the scopes limited to basic profile/email.
- If you change the callback path in code, you must update the redirect URIs here too.
