using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurando a injeção do DbContext (PostgreSQL)
builder.Services.AddDbContext<CampanhasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CampanhasDb")));

// Adiciona os serviços essenciais de API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configuração do Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Rota de Health Check exigida pela observabilidade da arquitetura
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "Campanhas API" }));

app.Run();