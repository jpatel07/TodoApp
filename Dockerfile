# syntax=docker/dockerfile:1

# ---------- Stage 1: Build the Angular client ----------
FROM node:22 AS client-build
WORKDIR /client

# Install dependencies first for better layer caching.
COPY src/Todo.Client/package.json src/Todo.Client/package-lock.json ./
RUN npm ci

# Copy the rest of the client and build the production bundle.
COPY src/Todo.Client/ ./
RUN npm run build -- --configuration production

# ---------- Stage 2: Build & publish the .NET API ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api-build
WORKDIR /src

# Copy the solution projects and restore.
COPY src/Todo.Api/Todo.Api.csproj src/Todo.Api/
COPY src/Todo.Core/Todo.Core.csproj src/Todo.Core/
COPY src/Todo.Infrastructure/Todo.Infrastructure.csproj src/Todo.Infrastructure/
RUN dotnet restore src/Todo.Api/Todo.Api.csproj

# Copy the remaining source and publish.
COPY src/Todo.Api/ src/Todo.Api/
COPY src/Todo.Core/ src/Todo.Core/
COPY src/Todo.Infrastructure/ src/Todo.Infrastructure/
RUN dotnet publish src/Todo.Api/Todo.Api.csproj -c Release -o /app/publish

# ---------- Stage 3: Runtime image (API + Client) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy the published API.
COPY --from=api-build /app/publish ./

# Copy the built Angular client into wwwroot so the API can serve it.
COPY --from=client-build /client/dist/Todo.Client/browser ./wwwroot

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Todo.Api.dll"]
