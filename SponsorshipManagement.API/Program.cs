using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Services;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 📋 REGISTRO DE SERVICIOS DE DOMINIO E INFRAESTRUCTURA

// Servicios existentes (Sponsors y Events)
builder.Services.AddScoped<ISponsorRepository, SponsorRepository>();
builder.Services.AddScoped<ISponsorService, SponsorService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

// 🎯 NUEVOS SERVICIOS PARA CU-PA-02.01.3 - Validación de existencia del contrato
// Registrar repositorio de contratos (dependencia de CU-PA-01)
builder.Services.AddScoped<IContractRepository, ContractRepository>();

// Registrar servicio de validación de contratos
builder.Services.AddScoped<IContractValidationService, ContractValidationService>();

// 🎯 SERVICIOS PARA CU-PA-02.01 - Sistema de compromisos
// Registrar repositorio de compromisos (recibe IContractRepository por DI)
builder.Services.AddScoped<ICommitmentRepository, CommitmentRepository>();

// Registrar servicio de compromisos (recibe ICommitmentRepository y IContractValidationService por DI)
builder.Services.AddScoped<ICommitmentService, CommitmentService>();

// 📋 CONFIGURACIÓN DE ASP.NET CORE
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 📚 CONFIGURACIÓN DE SWAGGER CON DOCUMENTACIÓN XML
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sponsorship Management API",
        Version = "v1",
        Description = @"API para la gestión de patrocinios y compromisos contractuales

🎯 Casos de Uso Implementados:
• CU-PA-02.01: Registrar compromisos contractuales
• CU-PA-02.01.1: Crear modelo y repositorio de compromisos  
• CU-PA-02.01.2: Endpoint para registrar compromisos
• CU-PA-02.01.3: Validación de existencia del contrato

📋 Endpoints Principales:
• POST /api/Commitment - Registrar nuevo compromiso
• GET /api/Commitment - Obtener todos los compromisos
• GET /api/Commitment/{id} - Obtener compromiso por ID
• GET /api/Commitment/contract/{contractId} - Compromisos por contrato",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CodeBreakers Team",
            Email = "team@codebreakers.com"
        }
    });

    // 📖 Incluir comentarios XML para documentación automática
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // 🏷️ Configurar tags para organizar endpoints
    c.TagActionsBy(api => new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] });
    c.DocInclusionPredicate((name, api) => true);
});

// 🔧 CONFIGURACIÓN DE LOGGING
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}

// 🌐 CONFIGURACIÓN DE CORS PARA DESARROLLO
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// 🔧 CONFIGURACIÓN DEL PIPELINE DE REQUEST

// Configurar Swagger en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sponsorship Management API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Sponsorship Management API - CodeBreakers";
        c.DisplayRequestDuration();
        c.EnableDeepLinking();
        c.EnableFilter();
    });

    // Redirect root to Swagger in development
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

// Habilitar CORS en desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAll");
}

app.UseAuthorization();

// 🎯 MAPEAR CONTROLADORES
app.MapControllers();

// 📝 LOGGING DE INICIO DE APLICACIÓN
var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("🚀 Sponsorship Management API iniciada");
logger.LogInformation("📋 Casos de uso implementados:");
logger.LogInformation("   - CU-PA-02.01: Registrar compromisos contractuales");
logger.LogInformation("   - CU-PA-02.01.1: Crear modelo y repositorio de compromisos");
logger.LogInformation("   - CU-PA-02.01.2: Endpoint para registrar compromisos");  
logger.LogInformation("   - CU-PA-02.01.3: Validación de existencia del contrato");

if (app.Environment.IsDevelopment())
{
    logger.LogInformation("🔗 Swagger UI disponible en: /swagger");
}

app.Run();