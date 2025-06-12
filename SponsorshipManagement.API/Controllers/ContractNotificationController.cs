using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.API.Controllers
{
    /// <summary>
    /// Controlador para gestión de notificaciones de vencimiento de contratos
    /// 🎯 CU-PA-05.02.1: Detección y notificación de contratos a vencer
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ContractNotificationController : ControllerBase
    {
        private readonly IContractExpirationNotifier _notifier;
        
        public ContractNotificationController(IContractExpirationNotifier notifier)
        {
            _notifier = notifier;
        }
        
        /// <summary>
        /// Ejecuta manualmente el proceso de detección y notificación de contratos a vencer
        /// </summary>
        /// <returns>Número de notificaciones enviadas</returns>
        [HttpPost("process")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<int>> ProcessNotifications()
        {
            try
            {
                Console.WriteLine("🔄 Iniciando procesamiento manual de notificaciones");
                int notificationsSent = await _notifier.ProcessContractsAndSendNotificationsAsync();
                Console.WriteLine($"✅ Procesamiento completado: {notificationsSent} notificaciones enviadas");
                
                return Ok(new { 
                    NotificationsSent = notificationsSent,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error en procesamiento: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obtiene contratos que requieren notificación sin enviarlas
        /// </summary>
        /// <returns>Lista de contratos que requieren notificación</returns>
        [HttpGet("pending")]
        [ProducesResponseType(typeof(IEnumerable<ContractRenewalDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ContractRenewalDto>>> GetPendingNotifications()
        {
            try
            {
                var contracts = await _notifier.GetContractsRequiringNotificationAsync();
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error obteniendo contratos pendientes: {ex.Message}");
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Obtiene la configuración actual de notificaciones
        /// </summary>
        /// <returns>Configuración de notificaciones</returns>
        [HttpGet("settings")]
        [ProducesResponseType(typeof(ContractNotificationSettingsDto), StatusCodes.Status200OK)]
        public ActionResult<ContractNotificationSettingsDto> GetSettings()
        {
            return Ok(_notifier.Settings);
        }
        
        /// <summary>
        /// Actualiza la configuración de notificaciones
        /// </summary>
        /// <param name="settings">Nueva configuración</param>
        /// <returns>Configuración actualizada</returns>
        [HttpPut("settings")]
        [ProducesResponseType(typeof(ContractNotificationSettingsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<ContractNotificationSettingsDto> UpdateSettings(ContractNotificationSettingsDto settings)
        {
            if (settings == null)
                return BadRequest("La configuración no puede ser nula");
                
            _notifier.Settings = settings;
            return Ok(_notifier.Settings);
        }
    }
}