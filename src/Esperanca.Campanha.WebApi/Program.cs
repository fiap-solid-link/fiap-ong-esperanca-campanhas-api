using Esperanca.Campanha.WebApi;
using Esperanca.Campanha.WebApi._Shared.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, config) =>
    config.ReadFrom.Configuration(context.Configuration));

CampanhaWebApiModule.ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ValidationExceptionMiddleware>();
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Esperanca Campanha API v1"));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

namespace Esperanca.Campanha.WebApi
{
    public partial class Program;
}