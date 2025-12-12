---
sidebar_position: 1
---

---
title: Overview
slug: /
---

Welcome to the OAuth with Google + .NET guide for this project. The stack here is:

- ASP.NET Core minimal API using cookie auth + Google OAuth (`/api/auth/login`, `/api/auth/logout`, `/api/auth/me`).
- Vite + React frontend that calls the API and relies on cookies (`credentials: 'include'`).
- Local dev targets `http://localhost:5173` (frontend) -> `http://localhost:5042` (backend) via Vite proxy.

Use this guide to set up Google credentials, configure the backend securely, connect the frontend, and run everything locally or in production.

**Quick links**
- [Google Cloud setup](google-cloud-setup)
- [Backend (.NET) setup](backend-dotnet)
- [Frontend integration](frontend-vite)
- [Run locally](running-locally)
- [Deploy & harden](deployment)
- [Troubleshooting](troubleshooting)
