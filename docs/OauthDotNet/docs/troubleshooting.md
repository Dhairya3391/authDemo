---
title: Troubleshooting
---

- **401 from `/api/auth/me` after login**: check that the callback path matches Google Console, the cookie is set for the domain, and the browser is not blocking third-party cookies.
- **CORS errors**: ensure the frontend origin is added to `WithOrigins(...)` and that `AllowCredentials()` is present.
- **Redirect mismatch**: verify the exact callback URL and protocol in Google Console.
- **Stuck on login loop**: confirm `SameSite=Lax` works for your flow; for production over HTTPS you can keep `Lax` with HTTPS. Avoid `None` unless you must support cross-site iframes.
- **Wrong port**: update Vite proxy target if the backend port is different from 5042.
- **Need logs**: set `Logging:LogLevel:Microsoft.AspNetCore` to `Information` while debugging auth issues.
