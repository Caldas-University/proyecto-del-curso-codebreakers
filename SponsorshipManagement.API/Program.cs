using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Services;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Infrastructure.Repositories;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Registrar servicios
builder.Services.AddScoped<ISponsorRepository, SponsorRepository>();
builder.Services.AddScoped<ISponsorService, SponsorService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<ICommitmentRepository, CommitmentRepository>();
builder.Services.AddScoped<ICommitmentService, CommitmentService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurar Swagger con documentación XML
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sponsorship Management API",
        Version = "v1",
        Description = "API para la gestión de patrocinios y compromisos contractuales",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CodeBreakers Team",
            Email = "team@codebreakers.com"
        }
    });

    // Incluir comentarios XML para documentación
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sponsorship Management API v1");
        c.RoutePrefix = "swagger";
        c.DocumentTitle = "Sponsorship Management API";
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();