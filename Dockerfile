# ═══════════════════════════════════════════════════
# ESTÁGIO 1 — BUILD (compila a aplicação)
# ═══════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto primeiro (otimização de cache)
COPY ["src/Cafescore.Domain/Cafescore.Domain.csproj", "src/Cafescore.Domain/"]
COPY ["src/Cafescore.Application/Cafescore.Application.csproj", "src/Cafescore.Application/"]
COPY ["src/Cafescore.Infrastructure/Cafescore.Infrastructure.csproj", "src/Cafescore.Infrastructure/"]
COPY ["src/Cafescore.API/Cafescore.API.csproj", "src/Cafescore.API/"]

# Restaura as dependências
RUN dotnet restore "src/Cafescore.API/Cafescore.API.csproj"

# Copia todo o código fonte
COPY . .

# Publica a aplicação em modo Release
RUN dotnet publish "src/Cafescore.API/Cafescore.API.csproj" -c Release -o /app/publish

# ═══════════════════════════════════════════════════
# ESTÁGIO 2 — RUNTIME (só executa, não compila)
# ═══════════════════════════════════════════════════
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copia apenas o resultado compilado do estágio anterior
COPY --from=build /app/publish .

# Fly.io usa a porta 8080 por padrão
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Cafescore.API.dll"]