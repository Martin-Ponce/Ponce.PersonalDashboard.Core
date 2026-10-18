# Runtime base
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Ponce.PersonalDashboard.Core.API/Ponce.PersonalDashboard.Core.API.csproj", "Ponce.PersonalDashboard.Core.API/"]
COPY ["Ponce.PersonalDashboard.Core.Application/Ponce.PersonalDashboard.Core.Application.csproj", "Ponce.PersonalDashboard.Core.Application/"]
COPY ["Ponce.PersonalDashboard.Core.Domain/Ponce.PersonalDashboard.Core.Domain.csproj", "Ponce.PersonalDashboard.Core.Domain/"]
COPY ["Ponce.PersonalDashboard.Core.Infrastructure/Ponce.PersonalDashboard.Core.Infrastructure.csproj", "Ponce.PersonalDashboard.Core.Infrastructure/"]
RUN dotnet restore "./Ponce.PersonalDashboard.Core.API/Ponce.PersonalDashboard.Core.API.csproj"
COPY . .
WORKDIR "/src/Ponce.PersonalDashboard.Core.API"
RUN dotnet build "./Ponce.PersonalDashboard.Core.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Ponce.PersonalDashboard.Core.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# The non-root app user needs somewhere to write its log files
USER root
RUN mkdir -p /app/Logs && chown $APP_UID /app/Logs
USER $APP_UID
# Plain http on 8080, so put a TLS proxy in front of it outside dev
ENTRYPOINT ["dotnet", "Ponce.PersonalDashboard.Core.API.dll"]