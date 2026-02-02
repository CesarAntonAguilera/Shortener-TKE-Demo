# Shortener TKE Demo

## URL Shortener – Technical Challenge (TKE)

This repository contains a simple **URL Shortener** application developed as part of a technical coding challenge.

The solution consists of:

* **Backend**: ASP.NET Core Web API (.NET 8)
* **Frontend**: Angular 20

The goal of the application is to generate a short URL from a long URL and redirect users to the original address when accessing the shortened link.

---

## Architecture Overview

The project is split into **Backend** and **Frontend**, following a clean and simple architecture suitable for a real-world application while keeping the scope limited as requested.

### Backend

* ASP.NET Core Web API (.NET 8)
* Layered architecture:

  * **Controllers**: HTTP endpoints and request handling
  * **Services**: Business logic (URL validation, code generation, idempotency)
  * **Repositories**: In-memory data storage
  * **DTOs**: Immutable data contracts
* In-memory storage (no database)
* Global exception handling middleware
* Swagger / OpenAPI documentation
* CORS enabled to allow communication with the Angular frontend
* Basic logging using `ILogger`

### Frontend

* Angular 20 (standalone components)
* Single-page UI for URL shortening
* Environment configuration for API connection
* Full-screen loader while API requests are in progress
* User-friendly error messages

---

## Backend Details

### Technology

* ASP.NET Core Web API
* .NET 8

### Key Design Decisions

#### CORS

Enabled to allow requests from:

```
http://localhost:4200
```

#### DTOs as `sealed record`

DTOs are defined as `sealed record` types because they are immutable contracts used only to transport data, not domain models.
Marking them as `sealed` makes the intention explicit: they are not designed for inheritance.

#### Repository Pattern (In-Memory)

The repository stores URL mappings using `ConcurrentDictionary`.
It is registered as a **Singleton** to ensure that in-memory data persists between requests.

#### Service Layer (Scoped)

The service contains business logic such as:

* URL validation
* Short code generation
* Collision handling
* Idempotency (same URL ? same code)

It is registered as **Scoped** because it does not store global state.

#### Idempotent URL Creation

Submitting the same long URL multiple times always returns the same short code.

#### Global Exception Handling

A custom exception middleware catches application exceptions and converts them into proper HTTP responses using **ProblemDetails**:

* `400 Bad Request` for invalid URLs
* `500 Internal Server Error` for unexpected errors

#### Redirection Handling

Accessing:

```
GET /{shortCode}
```

returns an HTTP **302 Redirect** to the original URL.

Swagger may show a *"Failed to fetch"* error for redirects due to CORS restrictions, but the redirection works correctly when accessed from a browser or the frontend application.

---

## Frontend Details

### Technology

* Angular 20
* Standalone components
* Template-driven forms

### Features

* Input field for long URL
* Button to generate short URL
* Display of generated short URL with copy functionality
* Full-screen loader during API calls
* Friendly validation and error messages based on backend responses

### Environment Configuration

The frontend uses an environment file to define the backend API URL:

```ts
apiBaseUrl: https://localhost:7053
```

---

## API Endpoints

### Create Short URL

```
POST /api/shorten
```

**Request Body**

```json
{
  "longUrl": "https://example.com/some/very/long/url"
}
```

**Response**

```json
{
  "longUrl": "https://example.com/some/very/long/url",
  "shortCode": "Ab3Kx9",
  "shortUrl": "https://localhost:7053/Ab3Kx9"
}
```

---

### Redirect to Original URL

```
GET /{shortCode}
```

* Returns **302 Found**
* Redirects to the original long URL

---

## Tests

An **integration test project** is included to validate the main application behavior using `WebApplicationFactory`.

### Covered Scenarios

* Creating a short URL successfully
* Idempotency (same URL returns the same short code)
* Redirection returns HTTP 302 with correct `Location` header
* Invalid URL returns HTTP 400 with `ProblemDetails`

Run tests using:

```bash
dotnet test
```

---

## Running the Application (Windows)

### Prerequisites

* .NET 8 SDK
* Node.js (LTS)
* Angular CLI
* Visual Studio 2026 (optional but recommended)

---

### Backend

```bash
cd Backend
dotnet restore
dotnet run
```

Swagger will be available at:

```
https://localhost:7053/swagger
```

> **Note:** On first run, you may need to accept the development HTTPS certificate in your browser.

> **Note:** Redirection responses do not work correctly in Swagger due to CORS restrictions.

---

### Frontend

```bash
cd Frontend
npm install
ng serve
```

The application will be available at:

```
http://localhost:4200
```

---

## Assumptions

* Short URLs point directly to the backend (`https://localhost:7053/{code}`)
* All data is stored in memory and will be lost when the backend restarts
* Only `http` and `https` URLs are accepted
* The same long URL always returns the same short code

---

## Future Improvements / Not Finished Due to Time Constraints

* Persist data using a real database (SQL Server / Redis)
* Add URL expiration (TTL) and cleanup jobs
* Add click tracking and metrics
* Rate limiting and abuse prevention
* Custom short code aliases
* More comprehensive test coverage
* Improve frontend UI and UX
* Deployment to Azure
* Separate environments (Development / UAT / Production)
* Centralized request validation using FluentValidation
* Dockerization and CI/CD pipeline

---

## Demo Notes

During the interview, the application can be demonstrated by:

1. Creating a short URL from the Angular frontend
2. Opening the generated short URL in a new browser tab
3. Showing automatic redirection to the original URL
4. Demonstrating error handling for invalid URLs
5. Reviewing the code structure and tests
