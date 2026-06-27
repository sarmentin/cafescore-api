# ☕ Caféscore API

Backend da aplicação Caféscore — plataforma para avaliação do café servido em clínicas médicas.

## 🛠️ Tecnologias

- .NET 8
- Entity Framework Core 8
- SQL Server
- JWT Bearer Authentication
- FluentValidation
- BCrypt.Net
- xUnit + Moq (testes)
- Swagger

## 🏗️ Arquitetura

Clean Architecture com 4 camadas:

- **Domain** — entidades e interfaces (contratos)
- **Application** — regras de negócio, DTOs, validators, services
- **Infrastructure** — EF Core, repositórios, seed do banco
- **API** — controllers, autenticação, configuração

## ⚙️ Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server)
- [dotnet-ef](https://learn.microsoft.com/ef/core/cli/dotnet)

```bash
dotnet tool install --global dotnet-ef
```

## 🚀 Como rodar localmente

### 1. Clone o repositório

```bash
git clone https://github.com/sarmentin/cafescore-api.git
cd cafescore-api
```

### 2. Configure as variáveis de ambiente

Crie o arquivo `src/Cafescore.API/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=CafescoreDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_MINIMO_32_CARACTERES"
  }
}
```

### 3. Aplique as migrations

```bash
dotnet ef database update --project src/Cafescore.Infrastructure --startup-project src/Cafescore.API
```

### 4. Rode a aplicação

```bash
dotnet run --project src/Cafescore.API
```

A API estará disponível em `https://localhost:7183` e o Swagger em `https://localhost:7183/swagger`.

## 🧪 Testes

```bash
dotnet test tests/Cafescore.Application.Tests
```

## 📋 Endpoints

| Método | Rota | Autenticado | Descrição |
|---|---|---|---|
| POST | `/api/auth/registrar` | Não | Cria novo usuário |
| POST | `/api/auth/login` | Não | Autentica e retorna token JWT |
| GET | `/api/clinicas` | Não | Lista todas as clínicas |
| GET | `/api/clinicas/{id}` | Não | Detalhe de uma clínica |
| GET | `/api/clinicas/{id}/avaliacoes` | Não | Lista avaliações de uma clínica |
| POST | `/api/avaliacoes` | Sim | Cria uma avaliação |
| PUT | `/api/avaliacoes/{id}` | Sim (dono) | Edita a própria avaliação |
| DELETE | `/api/avaliacoes/{id}` | Sim (dono) | Exclui a própria avaliação |

## 📁 Estrutura do Projeto

```
cafescore/
├── src/
│   ├── Cafescore.Domain/
│   ├── Cafescore.Application/
│   ├── Cafescore.Infrastructure/
│   └── Cafescore.API/
└── tests/
    └── Cafescore.Application.Tests/
```