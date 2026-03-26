# ============================================
# Stage 1: Frontend já compilado (build local)
# ============================================
FROM scratch AS frontend-build

COPY frontend/build /build

# ============================================
# Stage 2: Build do Backend .NET
# ============================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend-build

WORKDIR /src

# Copiar csproj e restaurar dependências
COPY src/OkrTracker.Domain/OkrTracker.Domain.csproj src/OkrTracker.Domain/
COPY src/OkrTracker.Application/OkrTracker.Application.csproj src/OkrTracker.Application/
COPY src/OkrTracker.Infrastructure/OkrTracker.Infrastructure.csproj src/OkrTracker.Infrastructure/
COPY src/OkrTracker.Api/OkrTracker.Api.csproj src/OkrTracker.Api/
RUN dotnet restore src/OkrTracker.Api/OkrTracker.Api.csproj

# Copiar todo o código e publicar
COPY src/ src/
RUN dotnet publish src/OkrTracker.Api/OkrTracker.Api.csproj -c Release -o /app/publish --no-restore

# ============================================
# Stage 3: Runtime
# ============================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

WORKDIR /app

# Copiar backend publicado
COPY --from=backend-build /app/publish .

# Copiar frontend build para wwwroot
COPY --from=frontend-build /build ./wwwroot

# Criar diretório para o banco de dados (montado via volume)
RUN mkdir -p /data

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 80

ENTRYPOINT ["dotnet", "OkrTracker.Api.dll"]
