---
title: Frontend integration
---

The React app calls the backend with cookies and relies on the backend-authenticated session.

## API usage
`src/App.tsx` implements the flow:

```tsx
const res = await fetch('/api/auth/me', { credentials: 'include' });
// 401 → not signed in; 200 → { name, email }

const login = () => {
  const returnUrl = encodeURIComponent(window.location.href);
  window.location.href = `/api/auth/login?returnUrl=${returnUrl}`;
};

await fetch('/api/auth/logout', { credentials: 'include' });
```

Important: always send `credentials: 'include'` so cookies flow between browser and backend.

## Dev proxy
`vite.config.ts` proxies `/api` to the .NET app:

```ts
server: {
  proxy: {
    '/api': {
      target: 'http://localhost:5042',
      changeOrigin: true,
    },
  },
},
```

- Keep the backend running on the target port (default from the project is 5042).
- If the backend port changes, update the proxy target and restart `npm run dev`.

## UI messaging
The UI shows three states: checking session, signed-in (name/email + sign out), and signed-out (sign in button). Adjust copy in `src/App.tsx` if you change routes.
