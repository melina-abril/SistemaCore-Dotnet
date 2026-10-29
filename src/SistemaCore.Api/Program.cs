using System.Text.Json.Serialization;
using SistemaCore.Api.Modules.Auditoria;
using SistemaCore.Api.Modules.Autenticacion;
using SistemaCore.Api.Modules.Inicio;
using SistemaCore.Api.Shared.Errors;
using SistemaCore.Api.Shared.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpCurrentUser>();
builder.Services.AddJwtSecurity(builder.Configuration);

builder.Services.AddAuditoriaModule();
builder.Services.AddAutenticacionModule();
builder.Services.AddInicioModule();

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseMiddleware<TokenRenewalMiddleware>();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
