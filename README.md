# TodoApp

Clone the repo and navigate into it.

    git clone <repo-url>
    cd TodoApp

## Option 1 - Docker

    docker-compose up -d

App runs at http://localhost:8080.

## Option 2 - Manual

Requires .NET 10 SDK and Node.js with Angular CLI installed (`npm install -g @angular/cli`).

Start SQL Server, then in separate terminals run the API and the client.

    docker-compose up -d sqlserver

    dotnet test --project .\src\Todo.Api\Todo.Api.csproj #to run integration tests
    dotnet run --project .\src\Todo.Api\Todo.Api.csproj

    cd src\Todo.Client
    ng serve

Client runs at http://localhost:54915.
