---
title: Deploy & harden
---

Key changes for production:

- **HTTPS only**: serve the backend over HTTPS and set `CookieSecurePolicy = Always` for the auth cookie.
- **CORS**: add your production frontend origin to `WithOrigins(...)`.
- **Google allowed URLs**: add the production origin and callback URL in Google Console.
- **Environment variables**: supply `Authentication__Google__ClientId`, `Authentication__Google__ClientSecret`, and `Authentication__Google__CallbackPath` via env vars or your secret store.
- **Return URLs**: keep `/api/auth/login?returnUrl=` pointing to your deployed frontend; validate the target if you expose it publicly.
- **Proxy headers**: when behind nginx/ingress, forward `X-Forwarded-Proto` so the app knows it is on HTTPS.

Sample systemd service (excerpt):

```ini
[Service]
Environment=ASPNETCORE_URLS=https://0.0.0.0:5042
Environment=Authentication__Google__ClientId=...
Environment=Authentication__Google__ClientSecret=...
Environment=Authentication__Google__CallbackPath=/api/auth/google-callback
```
