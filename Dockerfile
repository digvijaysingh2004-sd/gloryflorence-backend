# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_HTTP_PORTS=8080

# SDK build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["GloryFlorence.API/GloryFlorence.API.csproj", "GloryFlorence.API/"]
COPY ["GloryFlorence.Application/GloryFlorence.Application.csproj", "GloryFlorence.Application/"]
COPY ["GloryFlorence.Domain/GloryFlorence.Domain.csproj", "GloryFlorence.Domain/"]
COPY ["GloryFlorence.Infrastructure/GloryFlorence.Infrastructure.csproj", "GloryFlorence.Infrastructure/"]

RUN dotnet restore "GloryFlorence.API/GloryFlorence.API.csproj"

# Copy all source code
COPY . .
WORKDIR "/src/GloryFlorence.API"
RUN dotnet build "GloryFlorence.API.csproj" -c Release -o /app/build

# Publish application
FROM build AS publish
RUN dotnet publish "GloryFlorence.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GloryFlorence.API.dll"]
