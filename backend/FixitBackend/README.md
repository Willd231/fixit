# FixIt Issue Tracking System - Backend API

A professional-grade ASP.NET Core Web API backend for a help-desk and issue-tracking system. Built with clean architecture principles, Entity Framework Core, SQL Server, JWT authentication, and comprehensive logging.

## Features

- **Ticket Management**: Create, read, update, assign, resolve, and close support tickets
- **Advanced Filtering**: Search by ticket number, title, description, requester, status, priority, category, team, technician, and date ranges
- **Activity Tracking**: Automatic logging of all ticket changes and activity history
- **Comments & Collaboration**: Add public and internal comments to tickets
- **File Attachments**: Upload and manage ticket attachments
- **Role-Based Access Control**: Administrator, SupportManager, Technician, and Requester roles
- **Team Management**: Organize teams and assign members
- **User Management**: Manage technicians and requesters
- **Dashboard & Reporting**: Statistics, trends, workload analysis
- **JWT Authentication**: Secure token-based authentication with refresh tokens
- **Soft Deletion**: Preserve audit trail with soft delete implementation
- **Structured Logging**: Serilog integration for comprehensive application logging

## Technology Stack

- **.NET 9.0** - Latest .NET runtime
- **ASP.NET Core** - Web API framework
- **Entity Framework Core 9.0** - ORM and database context
- **SQL Server** - Relational database
- **ASP.NET Core Identity** - User authentication and authorization
- **JWT (JSON Web Tokens)** - Stateless authentication
- **AutoMapper** - Object-to-object mapping
- **FluentValidation** - Request validation
- **Serilog** - Structured logging
- **Swagger/OpenAPI** - API documentation and testing

## Prerequisites

- **.NET 9.0 SDK** or later ([download](https://dotnet.microsoft.com/download))
- **SQL Server** 2019 or later (LocalDB, Express, or Full)
  - Or **Docker** for SQL Server containerization
- **Visual Studio** 2022 or **Visual Studio Code** with C# extensions
- **Git** for version control

## Local Setup

### 1. Clone the Repository

```bash
git clone <repository-url>
cd FixIt/backend/FixitBackend
```

### 2. SQL Server Setup

#### Option A: Using LocalDB (Windows)

LocalDB is installed with Visual Studio. Create the database:

```bash
sqlcmd -S (localdb)\mssqllocaldb
CREATE DATABASE FixItDb_Dev;
GO
EXIT
```

#### Option B: Using SQL Server Express

Install [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads) and create the database.

#### Option C: Using Docker

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest
```

Then create the database using SQL Management Studio or sqlcmd.

### 3. Update Connection String

Edit `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=<your-server>;Database=FixItDb_Dev;User Id=<user>;Password=<password>;MultipleActiveResultSets=true;"
}
```

Examples:
- **LocalDB**: `Server=(localdb)\mssqllocaldb;Database=FixItDb_Dev;Trusted_Connection=true;`
- **SQL Server Express**: `Server=localhost\SQLEXPRESS;Database=FixItDb_Dev;Trusted_Connection=true;`
- **Docker**: `Server=localhost,1433;Database=FixItDb_Dev;User Id=sa;Password=YourPassword123!;`

### 4. JWT Secret Configuration

Update `appsettings.json` with a strong JWT secret (minimum 32 characters):

```json
"JwtSettings": {
  "SecretKey": "your-very-long-secret-key-with-at-least-32-characters-changed-in-production",
  "Issuer": "FixIt",
  "Audience": "FixItClient",
  "AccessTokenExpiryMinutes": 60
}
```

For production, use **User Secrets** or **environment variables**:

```bash
# Set via environment variables
set JwtSettings__SecretKey=your-production-secret-key

# Or using .NET User Secrets (development only)
dotnet user-secrets init
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"
```

### 5. Restore Dependencies

```bash
dotnet restore
```

### 6. Create Initial Migration

```bash
dotnet ef migrations add InitialCreate -o Infrastructure/Migrations
```

### 7. Apply Database Migrations

```bash
dotnet ef database update
```

This will:
- Create the database schema
- Create all tables, indexes, and relationships
- Seed development data (admin, manager, technician, requester accounts + sample tickets)

### 8. Run the Application

```bash
dotnet run
```

The API will start at: `https://localhost:7001` or `http://localhost:5000` (depending on your configuration)

### 9. Access Swagger Documentation

Navigate to: `https://localhost:7001/swagger`

## Development Accounts

The database is seeded with test accounts:

| Email | Password | Role | Purpose |
|-------|----------|------|---------|
| admin@fixit.local | Admin123!@# | Administrator | Full system access |
| manager@fixit.local | Manager123!@# | SupportManager | Team and ticket management |
| tech1@fixit.local | Tech123!@# | Technician | Assign and resolve tickets |
| user@fixit.local | User123!@# | Requester | Create and view own tickets |

## API Endpoints

### Authentication

```http
POST /api/auth/login
POST /api/auth/register
GET  /api/auth/me
POST /api/auth/refresh
```

### Tickets

```http
GET    /api/tickets                           # List with filtering
GET    /api/tickets/{id}                      # Get details
GET    /api/tickets/number/{ticketNumber}    # Get by ticket number
POST   /api/tickets                           # Create
PUT    /api/tickets/{id}                      # Update
PATCH  /api/tickets/{id}/status              # Update status
PATCH  /api/tickets/{id}/assignment          # Assign to team/technician
POST   /api/tickets/{id}/resolve             # Resolve ticket
POST   /api/tickets/{id}/reopen              # Reopen ticket
POST   /api/tickets/{id}/close               # Close ticket
DELETE /api/tickets/{id}                      # Delete (soft)
```

### Ticket Comments

```http
GET    /api/tickets/{ticketId}/comments
POST   /api/tickets/{ticketId}/comments
DELETE /api/tickets/{ticketId}/comments/{commentId}
```

### Ticket Activity

```http
GET /api/tickets/{ticketId}/activities
```

### Teams

```http
GET    /api/teams
GET    /api/teams/{id}
POST   /api/teams
PUT    /api/teams/{id}
DELETE /api/teams/{id}
GET    /api/teams/{id}/members
GET    /api/teams/{id}/tickets
```

### Example Requests

**Login:**
```bash
curl -X POST "https://localhost:7001/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@fixit.local","password":"Admin123!@#"}'
```

**Get Tickets:**
```bash
curl -X GET "https://localhost:7001/api/tickets?status=Open&priority=High&page=1&pageSize=20" \
  -H "Authorization: Bearer <access-token>"
```

**Create Ticket:**
```bash
curl -X POST "https://localhost:7001/api/tickets" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <access-token>" \
  -d '{
    "title":"Cannot access email",
    "description":"User cannot connect to corporate email",
    "requesterName":"John Smith",
    "requesterEmail":"john.smith@example.com",
    "category":"Email",
    "priority":"High"
  }'
```

## Database Migrations

### Create a New Migration

```bash
dotnet ef migrations add <MigrationName>
```

### Remove Last Migration

```bash
dotnet ef migrations remove
```

### Update Database

```bash
dotnet ef database update
```

### Revert to Previous Migration

```bash
dotnet ef database update <MigrationName>
```

### View Migration History

```bash
dotnet ef migrations list
```

## Frontend Integration

Configure your React frontend to point to this API:

**.env** (Frontend)
```
VITE_API_BASE_URL=https://localhost:7001/api
```

The API uses **camelCase** serialization. Ensure your frontend TypeScript interfaces match:

```typescript
interface Ticket {
  id: number;
  ticketNumber: string;
  title: string;
  description: string;
  status: string;  // "New", "Open", "InProgress", "Pending", "Resolved", "Closed"
  priority: string; // "Low", "Medium", "High", "Critical"
  category: string;
  requesterName: string;
  requesterEmail: string;
  assignedTeamId?: number;
  assignedTeamName?: string;
  assignedUserId?: string;
  assignedUserName?: string;
  createdAt: string;
  updatedAt: string;
  resolvedAt?: string;
}
```

## Docker Setup

### Build Docker Image

```bash
docker build -t fixit-api:latest -f Dockerfile .
```

### Run with Docker Compose

```bash
docker-compose up -d
```

## Project Structure

```
FixitBackend/
├── Domain/
│   ├── Entities/              # Domain models (Ticket, Team, User, etc.)
│   ├── Enums/                 # Status, Priority, Category, ActivityType
│   └── Interfaces/            # Domain contracts
├── Application/
│   ├── DTOs/                  # Data transfer objects
│   ├── Interfaces/            # Service contracts
│   ├── Validators/            # FluentValidation rules
│   └── Mappings/              # AutoMapper profiles
├── Infrastructure/
│   ├── Data/
│   │   ├── FixItDbContext.cs  # EF Core DbContext
│   │   ├── SeedData.cs        # Database seeding
│   │   └── Migrations/        # Database migrations
│   └── Services/              # Service implementations
├── Controllers/               # API endpoints
├── Middleware/                # Custom middleware
├── Properties/                # Launch settings
├── Program.cs                 # Application entry point
├── FixitBackend.csproj        # Project file
├── appsettings.json           # Configuration
├── appsettings.Development.json # Development config
└── README.md                  # This file
```

## Logging

Logs are written to:
- **Console** - Development output
- **File** - `logs/fixit-YYYY-MM-DD.txt` - Rolling daily files

Configure logging in `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft": "Warning",
    "Microsoft.EntityFrameworkCore": "Information"
  }
}
```

## Testing

Run unit and integration tests:

```bash
dotnet test
```

Run with coverage:

```bash
dotnet test /p:CollectCoverage=true
```

## Common Issues

### Issue: "Database 'FixItDb_Dev' does not exist"

**Solution**: Verify connection string and create database manually, or ensure migrations have been applied:
```bash
dotnet ef database update
```

### Issue: "Column name or number of supplied values does not match table definition"

**Solution**: Migration is out of sync. Ensure all migrations are applied:
```bash
dotnet ef database update
```

### Issue: "The REMOTE_USER header was not present on the request"

**Solution**: This is a Kestrel security header. Ignore if running locally. In production, ensure proper proxy headers.

### Issue: JWT token validation fails

**Solution**: Ensure:
- Secret key in `appsettings.json` matches the key used to generate tokens
- Token hasn't expired
- Issuer and Audience match configuration
- Token is in the `Authorization: Bearer <token>` header

## Production Deployment

### Security Checklist

- [ ] Change JWT SecretKey in production configuration
- [ ] Use strong database passwords
- [ ] Enable HTTPS/TLS
- [ ] Configure CORS for trusted frontend URLs only
- [ ] Set up firewall rules
- [ ] Enable SQL Server encryption
- [ ] Use environment variables for secrets
- [ ] Enable request logging and monitoring
- [ ] Implement rate limiting
- [ ] Regular security updates

### Database Backup

```bash
# Backup database
BACKUP DATABASE FixItDb TO DISK = 'C:\Backups\FixItDb.bak'

# Restore database
RESTORE DATABASE FixItDb FROM DISK = 'C:\Backups\FixItDb.bak'
```

## Performance Optimization

- Implement database query caching for dashboard statistics
- Add indexes for frequently searched fields
- Use async/await throughout for non-blocking operations
- Implement pagination for large result sets
- Consider adding request caching headers

## Future Enhancements

- [ ] Advanced reporting and analytics
- [ ] Email notifications for ticket updates
- [ ] SMS alerts for critical tickets
- [ ] Integration with external ticketing systems
- [ ] AI-powered ticket categorization
- [ ] Customer portal
- [ ] Mobile app
- [ ] Two-factor authentication (2FA)
- [ ] Audit logging
- [ ] Custom ticket workflows
- [ ] SLA management
- [ ] Knowledge base integration

## Support

For issues or questions:
1. Check the [Issues](../../issues) section
2. Review the API documentation at `/swagger`
3. Check application logs in `logs/` folder

## License

This project is part of a GitHub portfolio. See LICENSE file for details.

## Author

Created as a professional portfolio project demonstrating enterprise-grade ASP.NET Core development practices.
