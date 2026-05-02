using Fiap.OngEsperanca.Campanhas.Api.Domain.Repositories;
using Fiap.OngEsperanca.Campanhas.Api.Domain.Services;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Mensageria;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational;
using Fiap.OngEsperanca.Campanhas.Api.Infrastructure.Persistence.Relational.Repositories;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// 1. Configurando a injeção do DbContext (PostgreSQL)
builder.Services.AddDbContext<CampanhasDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CampanhasDb")));

// 2. Registrando os Repositórios
builder.Services.AddScoped<ICampanhaRepository, CampanhaRepository>();
// Sai o Fake, entra o RabbitMQ Real!
builder.Services.AddScoped<IMessageBusService, RabbitMqService>();

// 3. Registrando o MediatR (procura automaticamente os Handlers)
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
// --- FIM DA CONFIGURAÇÃO DE AUTENTICAÇÃO JWT ---

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
app.UseAuthentication(); // 1º Pergunta "Quem é você?"
app.UseAuthorization();  // 2º Pergunta "Você tem permissão de GestorONG?"

// 6. MAPEANDO OS CONTROLLERS (O Comando que estava faltando!)
app.MapControllers();

// Rota de Health Check
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Service = "Campanhas API" }));

app.Run();