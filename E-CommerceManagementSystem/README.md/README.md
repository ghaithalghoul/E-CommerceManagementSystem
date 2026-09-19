![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-ASP.NET%20Core-239120?logo=csharp)
![EF Core](https://img.shields.io/badge/Entity%20Framework%20Core-9.0-6DB33F)
![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?logo=microsoftsqlserver)
![JWT](https://img.shields.io/badge/Auth-JWT%20%2B%20Refresh%20Tokens-000000?logo=jsonwebtokens)

# E-Commerce Management System

A RESTful backend API for an e-commerce platform, built with **ASP.NET Core 9** and **Entity Framework Core**. The project implements the core domain of an online store — authentication, product catalog, shopping cart, orders, reviews, wishlist, and an admin management layer — using a layered architecture and JWT-based security.

This is a **portfolio project** built to demonstrate backend development skills at a Junior .NET Backend Developer level.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Features](#features)
- [Architecture](#architecture)
- [Authentication Flow](#authentication-flow)
- [Database](#database)
- [Security & Configuration](#security--configuration)
- [CORS](#cors)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Overview](#api-overview)
- [Backend Practices Demonstrated](#backend-practices-demonstrated)

---

## Tech Stack

| Category | Technology |
|---|---|
| Language | C# |
| Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Authentication | JWT (Access Tokens + Refresh Tokens) |
| API Style | REST |
| API Docs | OpenAPI + Scalar |
| Validation | Data Annotations |
| Cross-Origin Support | CORS |
| Architecture | Layered (Controllers → Services → Data) |

---

## Features

**Authentication & Authorization**
- User registration and login
- Secure password hashing
- JWT access tokens
- Refresh tokens with rotation and revocation
- Logout with token invalidation
- Role-based authorization (Admin / Customer)

**Product Catalog**
- Product CRUD (Admin)
- Search, filtering, and pagination
- Category CRUD with unique category names

**Shopping Experience**
- Shopping cart with item management
- Wishlist management
- Order creation from cart contents
- Stock management on order placement and cancellation
- Order cancellation with status tracking
- Product reviews (purchase-verified, one review per product per user)

**Admin**
- User management and role assignment
- Review moderation
- Order oversight
- Admin dashboard with aggregate statistics
- Audit logging of key account and order actions

**Cross-Cutting Concerns**
- Global exception handling middleware
- Request validation via Data Annotations
- CORS configuration for frontend integration

---

## Architecture

The project follows a layered architecture that separates concerns across distinct layers:

```
Controllers   → Handle HTTP requests/responses, routing, and status codes
Services      → Contain business logic
Interfaces    → Define service contracts for dependency injection
DTOs          → Shape data moving in and out of the API
Models        → Represent database entities
Data          → EF Core DbContext and persistence configuration
Middlewares   → Cross-cutting concerns (e.g., global exception handling)
```

Dependency Injection is used throughout to wire services and their interfaces, keeping controllers thin and business logic testable and isolated from the HTTP layer.

---

## Authentication Flow

The API uses JWT-based authentication with refresh token rotation:

1. The user registers or logs in with their credentials.
2. The server validates the credentials against the stored (hashed) password.
3. On success, the server issues a short-lived **access token** (JWT).
4. The server also issues a **refresh token**, used to obtain new access tokens without re-authenticating.
5. The access token is sent in the `Authorization: Bearer <token>` header to reach protected endpoints.
6. When the access token expires, the client calls the refresh endpoint with the refresh token to receive a new access/refresh token pair.
7. Refresh tokens are never stored in plain text — only their **hash** is persisted in the database.
8. A refresh token can be revoked; revocation is tracked via a `RevokedAt` timestamp, and revoked or expired tokens are rejected.
9. Logging out revokes the associated refresh token, immediately invalidating it for future use.

---

## Database

The project uses **SQL Server** with **Entity Framework Core** as the ORM. The schema is managed entirely through **EF Core Migrations**, which are included in the repository under the `Migrations/` folder.

Core entities include Users, Products, Categories, Carts, Cart Items, Orders, Order Items, Reviews, Wishlists, Refresh Tokens, and Audit Logs, with relationships modeled through EF Core navigation properties.

---

## Security & Configuration

No secrets, connection strings, or keys are stored in this repository. All sensitive configuration is expected to be supplied locally via **.NET User Secrets** (development) or **environment variables** (production/deployment).

The application expects the following configuration values:

| Key | Description |
|---|---|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `AppSettings:Token` | Secret key used to sign JWT access tokens |
| `AppSettings:Issuer` | JWT issuer |
| `AppSettings:Audience` | JWT audience |

None of these have real values checked into `appsettings.json`. See [Getting Started](#getting-started) for how to configure them locally.

---

## CORS

CORS is configured to allow a local React development frontend to call the API during development. No production origins or secrets are hard-coded — the allowed origin is defined in configuration and can be adjusted per environment.

---

## Project Structure

```
E-CommerceManagementSystem/
├── Controllers/
├── Services/
├── Interfaces/
├── DTO/
├── Models/
├── Data/
├── Middlewares/
├── Migrations/
├── Program.cs
└── appsettings.json
```

---

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance or container)

### 1. Clone the repository
```bash
git clone https://github.com/ghaithalghoul/E-CommerceManagementSystem.git
cd E-CommerceManagementSystem
```

### 2. Restore dependencies
```bash
dotnet restore
```

### 3. Configure User Secrets
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "AppSettings:Token" "YOUR_JWT_SECRET"
dotnet user-secrets set "AppSettings:Issuer" "YOUR_ISSUER"
dotnet user-secrets set "AppSettings:Audience" "YOUR_AUDIENCE"
```

### 4. Configure SQL Server
Ensure a SQL Server instance is running and reachable using the connection string provided above.

### 5. Apply EF Core migrations
```bash
dotnet ef database update
```

### 6. Build and run
```bash
dotnet build
dotnet run
```

### 7. Explore the API
Once running in development mode, the OpenAPI specification and interactive Scalar documentation are available from the application's root URL, allowing you to browse and test every endpoint directly in the browser.

---

## API Overview

> Endpoint groups and their purpose. Refer to the Scalar/OpenAPI documentation for exact routes, request/response schemas, and status codes.

**Authentication**
- Register a new user
- Log in and receive an access/refresh token pair
- Refresh an access token
- Log out and revoke the refresh token

**Products**
- List, search, filter, and paginate products
- Get a single product
- Create, update, and delete products (Admin)
- Get paginated reviews for a product
- Add, update, and delete a product review

**Categories**
- List categories
- Get a single category
- Create, update, and delete categories (Admin)

**Cart**
- Get the current user's cart
- Add an item to the cart
- Update a cart item's quantity
- Remove a cart item
- Clear the cart

**Orders**
- Create an order from the current cart
- List the current user's orders
- Get a single order
- Cancel an order
- Update order status (Admin)

**Wishlist**
- View the wishlist
- Add a product to the wishlist
- Remove a product from the wishlist

**Admin**
- Manage users and roles
- Moderate reviews
- View all orders
- View dashboard statistics

---

## Backend Practices Demonstrated

- Dependency Injection across all service layers
- Separation of concerns (Controllers / Services / Data)
- DTOs for request/response shaping, decoupled from EF Core entities
- Async/await throughout data access and business logic
- Entity Framework Core with relational modeling and migrations
- Global exception handling middleware
- Request validation via Data Annotations
- JWT authentication with refresh token rotation and revocation
- Role-based authorization
- Database transactions for multi-step operations (e.g., order creation/cancellation)
- Pagination and filtering on list endpoints
- Secure password hashing
- Audit logging of sensitive account and order actions

---

## License

This project is available for review as part of a developer portfolio. No license has been specified.
