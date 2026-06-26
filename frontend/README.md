# FixIt Frontend

This is the React + TypeScript front end for FixIt Issue Tracking System 2025.

Features:
- React + Vite + TypeScript
- Routing with React Router
- Data fetching with TanStack Query
- Forms with React Hook Form + Zod
- Mock API support when `VITE_USE_MOCK_API=true`

Environment variables:

```
VITE_API_BASE_URL=https://localhost:7001/api
VITE_USE_MOCK_API=true
```

To run:

1. cd frontend
2. npm install
3. npm run dev

Notes:
- The app will use the mock API by default. Set `VITE_USE_MOCK_API=false` to point to your ASP.NET Core backend.
- Expected backend endpoints: `/api/tickets`, `/api/tickets/{id}`, `/api/teams`, `/api/users`, `/api/dashboard/statistics`.
