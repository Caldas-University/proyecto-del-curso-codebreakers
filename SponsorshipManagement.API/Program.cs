using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Services;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ISponsorRepository, SponsorRepository>();
builder.Services.AddScoped<ISponsorService, SponsorService>();
builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IEventService, EventService>();

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

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

app.Run();
