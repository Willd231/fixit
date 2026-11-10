# FixIt

A help-desk ticketing app I'm building, modeled on the ticket workflow I use day to day as an MSP technician. Requesters submit tickets, the tickets get routed to teams and technicians, and every change is logged on the ticket.

**Status: work in progress.** The React frontend runs today against a built-in mock API. The ASP.NET Core backend has its domain model, services and endpoints written, but it isn't wired to the frontend yet (see [Roadmap](#roadmap)).

## What it does

**Frontend (React + TypeScript), runs now on mock data**
- Dashboard with counts of open, critical, unassigned and resolved-this-week tickets, plus the five most recent tickets
- Ticket list with search across title, description and ticket number, and status and priority badges
- Ticket creation form validated with React Hook Form + Zod
- Ticket detail page with description, activity log and comments, plus buttons to resolve, reopen and delete
- Teams, Technicians and Reports pages

**Backend (ASP.NET Core 9 Web API), written but not yet running end to end**
- Tickets numbered `FIX-<year>-000001`, moving through New → Open → In Progress → Pending → Resolved → Closed
- Priorities from Low to Critical, and categories: Hardware, Software, Network, Account Access, Security, Email, Other
- Role-based access with ASP.NET Identity + JWT, using four roles: Administrator, SupportManager, Technician, Requester. For example, only managers can assign tickets and only admins can delete them.
- An activity entry is logged automatically for every change: created, status, category, assignment, resolve, reopen, close, delete
- Comments with an internal-note flag. Tickets and comments are soft-deleted.
- Ticket filtering by status, priority, category, team, technician, requester and date range, with sorting and pagination
- Team endpoints: CRUD, members, and a team's ticket queue
- Seed data: 5 teams and demo users for each role
- Swagger UI, Serilog logging to console and file, and a `/health` endpoint

**Stack:** React 18, TypeScript, Vite, TanStack Query, React Hook Form, Zod · ASP.NET Core 9, EF Core (SQLite), ASP.NET Identity, JWT, AutoMapper, FluentValidation, Serilog

## Run it

```bash
npm install
npm run dev        # http://localhost:5173, uses the mock API
```

Backend setup is covered in [`backend/FixitBackend/README.md`](backend/FixitBackend/README.md). The JWT signing key comes from user secrets or an environment variable and is never committed.

## Roadmap

- [ ] Fix `Program.cs` (leftover sample code after `app.Run()`) and generate a real EF Core migration
- [ ] Add the `/api/users` and `/api/dashboard` controllers (the services already exist)
- [ ] Point the frontend at the real API (`VITE_USE_MOCK_API=false`, `/api` prefix) and replace the demo login with JWT sign-in
- [ ] Store and validate refresh tokens
- [ ] File attachments (the entity exists, but there's no endpoint yet)
- [ ] Tests (xUnit for the services) and a GitHub Actions build
- [ ] Deploy a live demo
