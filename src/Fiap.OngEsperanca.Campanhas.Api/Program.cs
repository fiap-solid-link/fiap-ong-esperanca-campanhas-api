using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurando a injeção do DbContext (PostgreSQL)
builder.Services.AddDbContext<CampanhasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CampanhasDb")));

// 2. Registrando os Repositórios
builder.Services.AddScoped<ICampanhaRepository, CampanhaRepository>();

// 3. Registrando o MediatR (procura automaticamente os Handlers)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 4. Registrando o FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// 5. SERVIÇOS DE API E CONTROLLERS (A mágica pro Swagger achar a rota!)
builder.Services.AddControllers();
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

// 6. MAPEANDO OS CONTROLLERS (O Comando que estava faltando!)
app.MapControllers();

// Rota de Health Check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "Campanhas API" }));

app.Run();