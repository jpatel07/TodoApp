# TodoApp

## Clone the repo

```bash
git clone <repo-url>
cd TodoApp
```

---

## Option 1 — Run everything with Docker

Starts SQL Server and the API together.

```bash
docker-compose up -d
```

App will be available at `http://localhost:8080`.

---

## Option 2 — Run manually

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
