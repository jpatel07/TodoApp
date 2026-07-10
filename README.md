# TodoApp

## Clone the repo

```bash
git clone <repo-url>
cd TodoApp
```

---

## Option 1 � Run everything with Docker

Starts SQL Server and the API together.

```bash
docker-compose up -d
```

App will be available at `http://localhost:8080`.

---

## Option 2 � Run manually

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org) + Angular CLI (`npm install -g @angular/cli`)
- Docker (for SQL Server)

### 1. Start SQL Server

```bash
docker-compose up -d sqlserver
```

### 2. Run the tests

```bash
dotnet test --project .\src\Todo.Api\Todo.Api.csproj
```

### 3. Run the API

```bash
dotnet run --project .\src\Todo.Api\Todo.Api.csproj
```

### 4. Run the Angular client

In a separate terminal:

```bash
cd src\Todo.Client
ng serve
```

Client will be available at `http://localhost:54915/`.


## Assumptions

- This is a simple to-do app without user management or authentication.

## Design Choices

### Project Structure

The solution follows a layered architecture:

- **Todo.Api** → Controllers
- **Todo.Core** → Entities & Interfaces
- **Todo.Infrastructure** → All ORM-specific implementation
- **Todo.Client** -> Angular FrontEnd Code

Entity configuration is separated out from the `DbContext` so that, as the number of entities grows, the `DbContext` itself stays clean and easy to read.

### Testing Strategy

These days, Docker makes it easy to spin up a SQL Server instance for testing, so all tests run against Docker in the local environment — the same approach the EF Core team itself uses. It's fast, efficient, and removes the need to mock the database.

Unit testing on the controllers was skipped since this is a fairly simple app. However, as a strong believer in TDD, an **Integration Test** project is included that uses ASP.NET Core's `WebApplicationFactory` to test each endpoint against the same Dockerized SQL Server, using a dedicated test database.

### Data Model Decisions

Some time was spent thinking through the data model:

1. **`CreatedAt` is `DateTimeOffset`** — in the long run, `DateTimeOffset` is more valuable, especially once users in different time zones are using the app.
2. **`Title` has a 255 nvarchar limit, `Description` has a 4000 nvarchar limit** — the 255 limit mirrors what Microsoft uses for Task titles. The 4000 nvarchar limit (equivalent to 8000 varchar) is based on the fact that SQL Server begins paginating data past 8000 characters, which can cause indexing issues as a table grows. This is a lesson learned from prior experience.
3. **No Accepted Schema for Todo** -I initially checked schema.org to see whether an established standard existed for representing a TODO or Task object. Since I couldn’t find anything suitable, I proceeded with the structure defined in the requirements.

### Frontend

Angular was chosen for the frontend, as it's the framework used in the past. Note: this code was written under time pressure, as the priority was getting a working solution done.

### Bonus / Optional Enhancements

- Only **Docker** was implemented from the optional enhancements list, to make the app easy to build and run.
- Basic validation was added for max length, as well as via SQL configuration — since SQL Server has no built-in empty-string check, the `LEN(TRIM([Title]))` trick is used instead.
- Date validation was added on the UI as an example. Note: there's a known defect where the API currently allows past dates.

## Trade-offs / Notes

- Normally, a middleware would be added to return consistent API responses — this wasn't implemented here due to time constraints.
- Logging and observability were also skipped. Ideally, OpenTelemetry would be added to capture telemetry data.
- In hindsight, **.NET Aspire** would have been a better starting point, since it provides much of this (service defaults, telemetry, orchestration) out of the box.
