using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using Fiap.OngEsperanca.Doacoes.Worker;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Registra o banco de dados com a mesma string de conexão do Docker
builder.Services.AddDbContext<CampanhasDbContext>(options =>
    options.UseNpgsql("Host=localhost;Database=campanhas_db;Username=postgres;Password=postgres"));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();