# Back

Backend services for Book-Manager built with ASP.NET Core and a modular multi-project solution.

## Projects

- Back.Api: HTTP API host, middleware, endpoint modules, auth, Swagger.
- Back.Domain: Domain logic and service abstractions.
- Back.Dal: DTOs and repository-facing data layer components.
- Back.Infrastructure: Shared infrastructure services and extensions.

Solution file: Back.sln

## Requirements

- .NET SDK 10.0
- MongoDB connection details (encrypted values expected by API config)
- Supabase URL and API key (encrypted values expected by API config)

## Configuration

The API loads configuration from:

1. appsettings.json
2. appsettings.{Environment}.json
3. Environment variables

A template exists at Back.Api/appsettings.template.json.

### First-time setup

1. Copy Back.Api/appsettings.template.json to Back.Api/appsettings.Development.json (or your target environment file).
2. Fill in values for:
   - AppSettings.EncryptionKey
   - MongoDBSettings.EncryptedConnectionString
   - MongoDBSettings.EncryptedDatabaseName
   - SupabaseSettings.EncryptedConnectionString
   - SupabaseSettings.EncryptedApiKey
   - JwtSettings.Key, Issuer, Audience, Expiry settings
3. Keep real secrets out of source control.

## Run locally

From this folder (Back):

```bash
dotnet restore Back.sln
dotnet build Back.sln
dotnet run --project Back.Api/Back.Api.csproj
```

Default development URLs are defined in Back.Api/Properties/launchSettings.json:

- https://localhost:7112
- http://localhost:5228

Swagger UI is available in Development at /swagger.

## Useful commands

```bash
# Build all backend projects
dotnet build Back.sln

# Run API in Development
ASPNETCORE_ENVIRONMENT=Development dotnet run --project Back.Api/Back.Api.csproj

# Publish API
dotnet publish Back.Api/Back.Api.csproj -c Release -o ./publish
```

## Notes

- appsettings.json, appsettings.Development.json, and .env files are ignored in this repository to protect secrets.
- The API uses custom middleware for correlation IDs, logging, exception handling, and security headers.
