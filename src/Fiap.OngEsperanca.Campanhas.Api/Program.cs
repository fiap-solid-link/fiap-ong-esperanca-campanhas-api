using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Services;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Mensageria;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Mensageria.Consumers;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Schedules;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicione a Fábrica de Conexões do RabbitMQ
builder.Services.AddSingleton(new RabbitMQ.Client.ConnectionFactory { HostName = "localhost" });
builder.Services.AddHostedService<CampanhaVencimentoScheduler>();

// O seu consumer que já estava aí:
builder.Services.AddHostedService<DoacaoProcessadaConsumer>();

// 1. Configurando a injeção do DbContext (PostgreSQL)
builder.Services.AddDbContext<CampanhasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CampanhasDb")));

// 2. Registrando os Repositórios e Mensageria
builder.Services.AddScoped<ICampanhaRepository, CampanhaRepository>();
builder.Services.AddScoped<IMessageBusService, RabbitMqService>();

// 3. Registrando o MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

// 4. Registrando o FluentValidation
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

// --- INÍCIO DA CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!))
        };
    });

// 5. SERVIÇOS DE API E CONTROLLERS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// 6. MAPEANDO OS CONTROLLERS
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "Campanhas API" }));

app.Run();