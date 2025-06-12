using SponsorshipManagement.Application.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de notificaciones
    /// 🎯 CU-PA-05.02.1: Envío de notificaciones de vencimiento
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Envía una notificación de vencimiento de contrato
        /// </summary>
        /// <param name="notification">Datos de la notificación</param>
        /// <returns>True si la notificación se envió correctamente</returns>
        Task<bool> SendContractExpirationNotificationAsync(ContractExpirationNotificationDto notification);

        /// <summary>
        /// Envía múltiples notificaciones de vencimiento de contrato
        /// </summary>
        /// <param name="notifications">Lista de notificaciones a enviar</param>
        /// <returns>Diccionario con el resultado de cada notificación (ID de notificación -> éxito)</returns>
        Task<Dictionary<string, bool>> SendContractExpirationNotificationsAsync(IEnumerable<ContractExpirationNotificationDto> notifications);
        
        /// <summary>
        /// Obtiene los correos electrónicos de los contactos para un patrocinador
        /// </summary>
        /// <param name="sponsorId">ID del patrocinador</param>
        /// <returns>Lista de direcciones de correo electrónico</returns>
        Task<IEnumerable<string>> GetSponsorContactEmailsAsync(string sponsorId);
        
        /// <summary>
        /// Obtiene los correos electrónicos de los organizadores de un evento
        /// </summary>
        /// <param name="eventId">ID del evento</param>
        /// <returns>Lista de direcciones de correo electrónico</returns>
        Task<IEnumerable<string>> GetEventOrganizerEmailsAsync(string eventId);
    }
}