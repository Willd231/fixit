# FixIt Issue Tracking System 2025

A professional-grade help-desk and issue-tracking system built with modern technologies. This repository contains both a React + TypeScript frontend and a comprehensive ASP.NET Core backend API with JWT authentication, Entity Framework Core, and SQL Server.

## Features

### Frontend (React + TypeScript + Vite)
- **Modern UI** with responsive design using React Router v6 and TanStack Query
- **Ticket Management**: Create, view, update, and resolve support tickets
- **Advanced Search & Filtering**: Search by ticket number, title, status, priority, category, team, technician
- **Dashboard**: Real-time metrics and recent ticket activity
- **Comments & Activity History**: Track all ticket changes and discussions
- **Team & User Management**: View technicians and support teams
- **Authentication Ready**: Integration with JWT-based backend authentication
- **Form Validation**: React Hook Form with Zod schema validation
- **Type-Safe**: Full TypeScript in strict mode

### Backend (ASP.NET Core + Entity Framework Core)
- **RESTful API** with comprehensive endpoint documentation via Swagger/OpenAPI
- **JWT Authentication** with role-based access control (Administrator, SupportManager, Technician, Requester)
- **Ticket Lifecycle Management**: Full CRUD operations with status tracking, assignment, resolution
- **Activity Tracking**: Automatic logging of all ticket changes
- **Comments & Attachments**: Support for ticket discussions and file uploads
- **Advanced Filtering**: Complex queries with search, status, priority, category, team, date range filters
- **Pagination & Sorting**: Efficient data retrieval with configurable page sizes
- **Dashboard & Reporting**: Statistics, trends, and technician workload analysis
- **Soft Deletion**: Preserve audit trail with logical deletion
- **Entity Framework Core** with SQL Server and migrations
- **Comprehensive Logging**: Serilog integration with file and console output
- **Docker Support**: Dockerfile and docker-compose for containerization

## Technology Stack

### Frontend
- **React 18.3.1** - UI framework
- **TypeScript 5.5.2** - Type safety
- **Vite 5.3.2** - Build tool
- **React Router 6.20.2** - Client-side routing
- **TanStack Query 5.8.0** - Server state management
- **React Hook Form 7.46.2** - Form management
- **Zod 3.23.2** - Schema validation
- **Lucide React 0.263.0** - Icons
- **CSS Variables** - Theming

### Backend
- **.NET 9.0** - Runtime
- **ASP.NET Core** - Web framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Database
- **JWT** - Authentication
- **AutoMapper 12.0.1** - Object mapping
- **FluentValidation 11.9.2** - Input validation
- **Serilog 8.0.1** - Logging
- **Swagger/OpenAPI** - API documentation

## Prerequisites

### Frontend
- **Node.js 18+** or **npm 9+**
- **Git** for version control

### Backend
- **.NET 9.0 SDK** ([download](https://dotnet.microsoft.com/download))
- **SQL Server 2019+** (LocalDB, Express, or Docker)
- **Visual Studio 2022** or **Visual Studio Code** with C# extensions

## Quick Start

### Frontend Setup

```bash
cd frontend
npm install
npm run dev
```

Frontend will run at: `http://localhost:5175` (or `5174` if port is in use)

### Backend Setup

```bash
cd backend/FixitBackend

# Restore packages
dotnet restore

# Update database connection string in appsettings.Development.json
# (Already configured for LocalDB by default)

# Create and apply migrations
dotnet ef database update

# Run the API
dotnet run
```

Backend will run at: `https://localhost:7001` (HTTPS) or `http://localhost:5000` (HTTP)

Access Swagger API documentation at: `https://localhost:7001/swagger`

### Docker Setup (Optional)

```bash
cd backend

# Build and run with Docker Compose
docker-compose up -d

# Backend API: http://localhost:5000
# SQL Server: localhost:1433
```

## Development Accounts

After initial setup, test with these credentials:

| Email | Password | Role |
|-------|----------|------|
| admin@fixit.local | Admin123!@# | Administrator |
| manager@fixit.local | Manager123!@# | SupportManager |
| tech1@fixit.local | Tech123!@# | Technician |
| user@fixit.local | User123!@# | Requester |

## Project Structure

```
fixit/
├── frontend/                          # React + TypeScript application
│   ├── src/
│   │   ├── api/                      # API integration layer
│   │   ├── components/               # Reusable React components
│   │   ├── context/                  # React Context (Auth)
│   │   ├── layouts/                  # Page layouts
│   │   ├── pages/                    # Page components
│   │   ├── types/                    # TypeScript interfaces
│   │   ├── App.tsx                   # Root app component
│   │   └── main.tsx                  # Entry point
│   ├── index.html                    # HTML template
│   ├── tsconfig.json                 # TypeScript config
│   ├── vite.config.ts                # Vite configuration
│   └── package.json                  # Dependencies
│
├── backend/
│   └── FixitBackend/                 # ASP.NET Core Web API
│       ├── Domain/
│       │   ├── Entities/             # Domain models
│       │   └── Enums/                # Enumeration types
│       ├── Application/
│       │   ├── DTOs/                 # Data transfer objects
│       │   ├── Interfaces/           # Service contracts
│       │   ├── Validators/           # FluentValidation rules
│       │   └── Mappings/             # AutoMapper profiles
│       ├── Infrastructure/
│       │   ├── Data/                 # EF Core DbContext
│       │   └── Services/             # Service implementations
│       ├── Controllers/              # API endpoints
│       ├── Program.cs                # Application entry point
│       ├── appsettings.json          # Configuration
│       ├── Dockerfile                # Docker image definition
│       ├── docker-compose.yml        # Docker orchestration
│       └── README.md                 # Detailed backend docs
│
├── README.md                         # This file
└── .gitignore                        # Git ignore rules
```

## API Endpoints Overview

### Authentication
```
POST   /api/auth/login              # Authenticate and get JWT token
POST   /api/auth/register           # Create new user account
GET    /api/auth/me                 # Get current user info
POST   /api/auth/refresh            # Refresh access token
```

### Tickets
```
GET    /api/tickets                 # List with filtering, search, pagination
POST   /api/tickets                 # Create new ticket
GET    /api/tickets/{id}            # Get ticket details
PUT    /api/tickets/{id}            # Update ticket
PATCH  /api/tickets/{id}/status     # Update status
PATCH  /api/tickets/{id}/assignment # Assign to team/technician
POST   /api/tickets/{id}/resolve    # Resolve ticket
POST   /api/tickets/{id}/reopen     # Reopen ticket
POST   /api/tickets/{id}/close      # Close ticket
DELETE /api/tickets/{id}            # Delete ticket (soft delete)
GET    /api/tickets/{id}/comments   # Get ticket comments
POST   /api/tickets/{id}/comments   # Add comment
GET    /api/tickets/{id}/activities # Get activity history
```

### Teams
```
GET    /api/teams                   # List all teams
GET    /api/teams/{id}              # Get team details
POST   /api/teams                   # Create team
PUT    /api/teams/{id}              # Update team
DELETE /api/teams/{id}              # Delete team
GET    /api/teams/{id}/members      # Get team members
GET    /api/teams/{id}/tickets      # Get team's tickets
```

## Frontend Configuration

By default, the frontend connects to:
- Development: `http://localhost:5000/api` (mock API if backend is down)
- Can be configured via `VITE_API_BASE_URL` environment variable

### Environment Variables (.env)
```
VITE_API_BASE_URL=http://localhost:5000/api
VITE_USE_MOCK_API=false  # Set to true to use mock data
```

## Database Migrations

### Create Migration
```bash
cd backend/FixitBackend
dotnet ef migrations add <MigrationName>
```

### Apply Migrations
```bash
dotnet ef database update
```

### Revert Migration
```bash
dotnet ef database update <PreviousMigration>
```

## Running Tests

### Frontend Unit Tests
```bash
cd frontend
npm run test
```

### Backend Unit Tests
```bash
cd backend/FixitBackend
dotnet test
```

## Building for Production

### Frontend
```bash
cd frontend
npm run build
npm run preview  # Test production build locally
```

Output: `frontend/dist/` - ready to deploy to static hosting

### Backend
```bash
cd backend/FixitBackend
dotnet publish -c Release -o publish
```

Output: `publish/` folder contains deployable artifacts

### Docker Production Build
```bash
cd backend
docker build -t fixit-api:latest .
docker run -p 5000:5000 fixit-api:latest
```

## Common Issues & Solutions

### Issue: Frontend can't connect to backend
**Solution**: 
- Ensure backend is running on `http://localhost:5000`
- Check `VITE_API_BASE_URL` environment variable
- Set `VITE_USE_MOCK_API=true` to use mock data for testing

### Issue: Database connection error
**Solution**:
- Verify SQL Server is installed and running
- Check connection string in `appsettings.Development.json`
- Run `dotnet ef database update` to apply migrations

### Issue: JWT token validation fails
**Solution**:
- Ensure same JWT secret key in frontend and backend configuration
- Check token hasn't expired
- Verify Authorization header format: `Bearer <token>`

## Performance Optimization

- Frontend uses TanStack Query for intelligent caching
- Backend uses EF Core compiled queries and indexes
- Pagination implemented for large datasets
- Async/await for non-blocking operations
- Soft deletion for data preservation

## Security Considerations

- ✅ JWT authentication with role-based access control
- ✅ HTTPS/TLS recommended for production
- ✅ Password hashing with ASP.NET Core Identity
- ✅ CORS configured for specific origins
- ✅ Input validation on both frontend and backend
- ✅ SQL injection prevention via parameterized queries
- ✅ XSS protection via React's default escaping

## Deployment

### Frontend (Netlify/Vercel)
```bash
npm run build
# Deploy dist/ folder
```

### Backend (Azure App Service/AWS/DigitalOcean)
```bash
dotnet publish -c Release
# Deploy published folder with SQL Server
```

### Docker Deployment
```bash
docker-compose -f docker-compose.yml up -d
```

## Monitoring & Logging

- **Frontend**: Browser console and Network tab for debugging
- **Backend**: Serilog logs in `logs/fixit-YYYY-MM-DD.txt`
- **API Errors**: Returned in consistent format with status codes
- **Swagger**: Live API documentation at `/swagger`

## Future Enhancements

- [ ] Real-time notifications with SignalR
- [ ] File attachment management
- [ ] Advanced reporting and analytics
- [ ] Email notifications
- [ ] Two-factor authentication (2FA)
- [ ] Audit logging
- [ ] Custom workflows
- [ ] SLA management
- [ ] Knowledge base integration
- [ ] Mobile app

## Contributing

1. Fork the repository
2. Create feature branch: `git checkout -b feature/your-feature`
3. Commit changes: `git commit -am 'Add feature'`
4. Push to branch: `git push origin feature/your-feature`
5. Submit pull request

## Documentation

- **Frontend Details**: See [frontend/README.md](frontend/README.md)
- **Backend Details**: See [backend/FixitBackend/README.md](backend/FixitBackend/README.md)
- **API Documentation**: Available at `/swagger` when backend is running

## License

This project is part of a GitHub portfolio demonstrating professional software development practices.

## Support

For issues or questions:
1. Check existing issues on GitHub
2. Review the API documentation at `/swagger`
3. Check application logs
4. Create a new issue with detailed information

## Architecture Highlights

### Clean Architecture
- Clear separation of concerns (Domain, Application, Infrastructure)
- Dependency injection throughout
- Interface-based design for testability

### Database Design
- Normalized schema with proper relationships
- Indexes for query performance
- Soft deletion for audit trails
- UTC timestamps throughout

### API Design
- RESTful conventions
- Consistent error responses
- Comprehensive pagination
- Advanced filtering capabilities
- Swagger/OpenAPI documentation

### Frontend Design
- Component-based architecture
- Custom hooks for logic reuse
- Context API for state management
- Type-safe with TypeScript
- Responsive design with CSS Grid/Flexbox

## Author

Created as a professional GitHub portfolio project demonstrating enterprise-grade web application development with modern technologies.
