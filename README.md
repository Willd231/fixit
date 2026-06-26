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



## Author

Created as a professional GitHub portfolio project demonstrating enterprise-grade web application development with modern technologies.
