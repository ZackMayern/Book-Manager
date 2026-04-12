FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Back.Api/Back.Api.csproj", "Back.Api/"]
COPY ["Back.Dal/Back.Dal.csproj", "Back.Dal/"]
COPY ["Back.Domain/Back.Domain.csproj", "Back.Domain/"]
COPY ["Back.Infrastructure/Back.Infrastructure.csproj", "Back.Infrastructure/"]
RUN dotnet restore "Back.Api/Back.Api.csproj"

COPY . .
WORKDIR "/src/Back.Api"
RUN dotnet build "Back.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Back.Api.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Back.Api.dll"]
