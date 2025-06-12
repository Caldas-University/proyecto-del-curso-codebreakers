using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using SponsorshipManagement.Application.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SponsorshipManagement.API.Services
{
    /// <summary>
    /// Servicio en segundo plano para procesamiento automático de notificaciones de contratos
    /// 🎯 CU-PA-05.02.1: Notificación automática de contratos a vencer
    /// </summary>
    public class ContractExpirationBackgroundService : BackgroundService
    {
        private readonly ILogger<ContractExpirationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Una vez al día

        public ContractExpirationBackgroundService(
            ILogger<ContractExpirationBackgroundService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("🚀 CU-PA-05.02.1: Servicio de notificación de contratos iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("⏰ Iniciando verificación programada de contratos a vencer...");
                    
                    // Crear un scope para obtener servicios scoped
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var notifier = scope.ServiceProvider.GetRequiredService<IContractExpirationNotifier>();
                        
                        var startTime = DateTime.UtcNow;
                        int notificationsSent = await notifier.ProcessContractsAndSendNotificationsAsync();
                        var duration = DateTime.UtcNow - startTime;
                        
                        _logger.LogInformation($"✅ Procesamiento completado en {duration.TotalSeconds:F1}s: {notificationsSent} notificaciones enviadas");
                    }

                    // Esperar hasta la próxima ejecución programada
                    await Task.Delay(_checkInterval, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Error al procesar notificaciones de contratos");
                    
                    // Esperar un tiempo antes de reintentar en caso de error
                    await Task.Delay(TimeSpan.FromMinutes(30), stoppingToken);
                }
            }
            
            _logger.LogInformation("⏹️ Servicio de notificación de contratos detenido");
        }
    }
}