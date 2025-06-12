using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el notificador de vencimiento de contratos
    /// 🎯 CU-PA-05.02.1: Detección y notificación de contratos a vencer
    /// </summary>
    public interface IContractExpirationNotifier
    {
        /// <summary>
        /// Detecta contratos próximos a vencer y envía notificaciones
        /// </summary>
        /// <returns>Número de notificaciones enviadas</returns>
        Task<int> ProcessContractsAndSendNotificationsAsync();
        
        /// <summary>
        /// Detecta contratos próximos a vencer pero sin enviar notificaciones
        /// </summary>
        /// <returns>Lista de contratos que requieren notificación</returns>
        Task<IEnumerable<ContractRenewalDto>> GetContractsRequiringNotificationAsync();
        
        /// <summary>
        /// Obtiene o establece la configuración de notificaciones
        /// </summary>
        ContractNotificationSettingsDto Settings { get; set; }
    }
}