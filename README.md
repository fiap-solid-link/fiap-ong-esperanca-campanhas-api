# Esperança Campanha API

API REST do contexto **Campanhas** da plataforma **Conexão Solidária** (ONG Esperança / FIAP).

## Stack

- .NET 10 / ASP.NET Core
- PostgreSQL 16 (write-side via EF Core)
- MongoDB 7 (read-side de transparência)
- RabbitMQ 3.13 (mensageria)
- Serilog (logging estruturado com correlation-id)
- MediatR + FluentValidation (CQRS / pipeline)
- JWT Bearer (autenticação)

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) + Docker Compose

---

## Execução via Docker Compose (recomendado)

Sobe todos os serviços de infraestrutura (Postgres, Mongo, RabbitMQ) e a API em uma única etapa:

```bash
docker compose up -d --build
```

A API fica disponível em **http://localhost:5010**.

| Serviço       | Porta local | Descrição                          |
|---------------|-------------|------------------------------------|
| campanha-api  | 5010        | API REST                           |
| postgres      | 5432        | Banco de dados principal           |
| mongo         | 27017       | Read-side de transparência         |
| rabbitmq      | 5672        | Broker de mensagens                |
| rabbitmq-mgmt | 15672       | Console de administração RabbitMQ  |

Para parar:

```bash
docker compose down
```

Para parar e remover volumes:

```bash
docker compose down -v
```

---

## Execução local

Requisito: Postgres em `localhost:5432`, MongoDB em `localhost:27017` e RabbitMQ em `localhost:5672` com credenciais `guest/guest`.

```bash
# Restaurar dependências e compilar
dotnet build Esperanca.Campanha.sln

# Rodar a API
dotnet run --project src/Esperanca.Campanha.WebApi
```

---

## Endpoints

### Swagger / OpenAPI

Disponível em **http://localhost:5010/swagger**.

Use o botão **Authorize** para inserir o token JWT no formato `Bearer <token>`.

### Health Check

```
GET /health
```

Retorna o status agregado de Postgres, MongoDB e RabbitMQ:

```json
{
  "status": "Healthy",
  "duration": "00:00:00.123",
  "checks": [
    { "name": "postgresql", "status": "Healthy", "tags": ["db", "ready"] },
    { "name": "mongodb",    "status": "Healthy", "tags": ["db", "ready"] },
    { "name": "rabbitmq",   "status": "Healthy", "tags": ["messaging", "ready"] }
  ]
}
```

### Campanhas — requer role `GestorONG`

| Método | Rota                              | Descrição                   |
|--------|-----------------------------------|-----------------------------|
| POST   | `/api/campanhas`                  | Criar campanha              |
| GET    | `/api/campanhas`                  | Listar campanhas (paginado) |
| GET    | `/api/campanhas/{id}`             | Obter campanha por ID       |
| PUT    | `/api/campanhas/{id}`             | Editar campanha             |
| PATCH  | `/api/campanhas/{id}/ativar`      | Ativar campanha             |
| PATCH  | `/api/campanhas/{id}/prorrogar`   | Prorrogar campanha          |
| PATCH  | `/api/campanhas/{id}/cancelar`    | Cancelar campanha           |

### Doações — requer role `Doador` ou `GestorONG`

| Método | Rota           | Descrição                          |
|--------|----------------|------------------------------------|
| POST   | `/api/doacoes` | Registrar intenção de doação (202) |

### Transparência — público, sem autenticação

| Método | Rota                                | Descrição                    |
|--------|-------------------------------------|------------------------------|
| GET    | `/api/transparencia/painel`         | Painel macro de arrecadação  |
| GET    | `/api/transparencia/campanhas`      | Lista pública de campanhas   |
| GET    | `/api/transparencia/campanhas/{id}` | Detalhe de campanha pública  |

---

## Testes

```bash
# Toda a solution
dotnet test

# Apenas testes unitários
dotnet test test/Esperanca.Campanha.UnitTests

# Filtrar por nome
dotnet test --filter "FullyQualifiedName~CriarCampanha"
```

---

## Migrations EF Core

```bash
dotnet ef migrations add <Nome> \
  -p src/Esperanca.Campanha.Infrastructure \
  -s src/Esperanca.Campanha.WebApi

dotnet ef database update \
  -p src/Esperanca.Campanha.Infrastructure \
  -s src/Esperanca.Campanha.WebApi
```

---

## Variáveis de ambiente

| Variável                          | Descrição                        |
|-----------------------------------|----------------------------------|
| `ConnectionStrings__CampanhaDb`   | Connection string do PostgreSQL  |
| `ConnectionStrings__DoacoesMongo` | Connection string do MongoDB     |
| `Jwt__SecretKey`                  | Chave HMAC-SHA256 para JWT       |
| `Jwt__Issuer`                     | Issuer do token JWT              |
| `Jwt__Audience`                   | Audience do token JWT            |
| `RabbitMq__Host`                  | Host do RabbitMQ                 |
| `RabbitMq__User`                  | Usuário do RabbitMQ              |
| `RabbitMq__Password`              | Senha do RabbitMQ                |

---

## Observabilidade

- **Correlation-ID**: toda request recebe (ou gera) um `X-Correlation-Id` propagado nos logs e nos headers AMQP das mensagens publicadas.
- **Request logging**: cada request HTTP é logada via `UseSerilogRequestLogging` com método, path, status code e duração.
- **Logs estruturados**: template `[HH:mm:ss LVL] {CorrelationId} {Message}`.
