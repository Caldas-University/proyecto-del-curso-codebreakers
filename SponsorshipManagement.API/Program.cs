using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Services;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Infrastructure.Repositories;
using SponsorshipManagement.API.Services; // Añadir esta línea
using System.Reflection;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// 📋 REGISTRO DE SERVICIOS DE DOMINIO E INFRAESTRUCTURA

// Servicios existentes (Sponsors y Events)
builder.Services.AddScoped<ISponsorRepository, SponsorRepository>();
builder.Services.AddScoped<ISponsorService, SponsorService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IContractService, ContractService>(); 
builder.Services.AddScoped<IContractHistoryService, ContractHistoryService>();

// 🎯 SERVICIOS PARA CU-PA-02.01.3 - Validación de existencia del contrato
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractValidationService, ContractValidationService>();

// 🎯 SERVICIOS PARA CU-PA-02.01 - Sistema de compromisos
builder.Services.AddScoped<ICommitmentRepository, CommitmentRepository>();

// 🎯 NUEVO: CU-PA-02.01.4 - Gestión de estados
builder.Services.AddScoped<CommitmentStateService>();

builder.Services.AddScoped<ICommitmentService, CommitmentService>();

// 📋 CONFIGURACIÓN DE ASP.NET CORE
// Registrar repositorios de beneficios publicitarios
builder.Services.AddScoped<IAdvertisingBenefitRepository, AdvertisingBenefitRepository>();
builder.Services.AddScoped<IAdvertisingBenefitExecutionRepository, AdvertisingBenefitExecutionRepository>();

// Registrar servicios de beneficios publicitarios
builder.Services.AddScoped<IAdvertisingBenefitService, AdvertisingBenefitService>();
builder.Services.AddScoped<IAdvertisingBenefitExecutionService, AdvertisingBenefitExecutionService>();

// Registrar repositorios de métricas
builder.Services.AddScoped<IMetricRepository, MetricRepository>();
builder.Services.AddScoped<IMetricRecordRepository, MetricRecordRepository>();
builder.Services.AddScoped<IMetricComparisonRepository, MetricComparisonRepository>();

// Registrar servicios de métricas
builder.Services.AddScoped<IMetricService, MetricService>();
builder.Services.AddScoped<IMetricRecordService, MetricRecordService>();
builder.Services.AddScoped<IMetricComparisonService, MetricComparisonService>();

// Registrar repositorios de contratos
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractCommitmentRepository, ContractCommitmentRepository>();
builder.Services.AddScoped<IContractComplianceReportRepository, ContractComplianceReportRepository>();

// Registrar servicios de contratos
builder.Services.AddScoped<IContractService, ContractService>();
builder.Services.AddScoped<IContractComplianceService, ContractComplianceService>();

// 🎯 SERVICIOS PARA CU-PA-05 - Gestión de renovaciones y finalización de contratos
builder.Services.AddScoped<IContractRenewalService, ContractRenewalService>();

// 🎯 SERVICIOS PARA CU-PA-05.02.1 - Notificación automática de contratos a vencer
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IContractExpirationNotifier, ContractExpirationNotifier>();

// Registrar servicio en segundo plano para notificaciones automáticas
// Comentar esta línea si se prefiere usar el controlador con un job externo
builder.Services.AddHostedService<ContractExpirationBackgroundService>();

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
• CU-PA-02.01.4: Asignación de estado inicial 'pendiente'
• CU-PA-05.01.1: Consulta de contratos próximos a vencer
• CU-PA-05.01.2: Validación de cumplimiento e impacto publicitario
• CU-PA-05.01.3: Exposición de contratos próximos a vencer con filtros avanzados

📋 Endpoints de Renovación de Contratos:
• GET /api/ContractRenewal/expiring - Obtener contratos próximos a vencer
• GET /api/ContractRenewal/expiring-summary - Obtener resumen estadístico de contratos próximos a vencer
• GET /api/ContractRenewal/validate/{id} - Validar renovación de contrato
• POST /api/ContractRenewal/{id}/notify - Marcar contrato como notificado",
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
logger.LogInformation("   - CU-PA-02.01.4: Asignación de estado inicial 'pendiente'");
logger.LogInformation("📱 Swagger UI disponible en: /swagger");

app.Run();
