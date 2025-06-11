using SponsorshipManagement.Application.Interfaces;  // Para IAuditService
using SponsorshipManagement.Application.Services;    // Para AuditService
using SponsorshipManagement.Domain.Interfaces;       // Para IAuditRepository
using SponsorshipManagement.Infrastructure.Repositories; // Para AuditRepository
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// 📋 REGISTRO DE SERVICIOS DE DOMINIO E INFRAESTRUCTURA

// Servicios existentes (Sponsors y Events)
builder.Services.AddScoped<ISponsorRepository, SponsorRepository>();
builder.Services.AddScoped<ISponsorService, SponsorService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

// 🎯 SERVICIOS PARA CU-PA-02.01.3 - Validación de existencia del contrato
builder.Services.AddScoped<IContractRepository, ContractRepository>();
builder.Services.AddScoped<IContractValidationService, ContractValidationService>();

// 🎯 SERVICIOS PARA CU-PA-02.01 - Sistema de compromisos
builder.Services.AddScoped<ICommitmentRepository, CommitmentRepository>();

// 🎯 NUEVO: CU-PA-02.01.4 - Gestión de estados
builder.Services.AddScoped<CommitmentStateService>();

// CU-PA-02.01.5: Servicios de auditoría
builder.Services.AddScoped<IAuditRepository, AuditRepository>();
builder.Services.AddScoped<IAuditService, AuditService>();

// SERVICIOS DE NEGOCIO (CommitmentService debe recibir IAuditService)
builder.Services.AddScoped<ICommitmentService, CommitmentService>();

// CU-PA-02.04.1: Servicio para consultar compromisos
builder.Services.AddScoped<ICommitmentQueryService, CommitmentQueryService>();

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


app.Run();
