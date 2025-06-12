using System.Collections.Generic;

namespace SponsorshipManagement.Application.Dtos
{
    /// <summary>
    /// Configuración para las notificaciones automáticas de vencimiento de contratos
    /// 🎯 CU-PA-05.02.1: Configuración para notificaciones automáticas
    /// </summary>
    public class ContractNotificationSettingsDto
    {
        /// <summary>
        /// Días antes del vencimiento para enviar notificaciones
        /// </summary>
        public List<int> DaysBeforeExpiration { get; set; } = new List<int> { 30, 15, 7, 3, 1 };

        /// <summary>
        /// Estados de contrato para los que se enviarán notificaciones
        /// </summary>
        public List<string> ContractStatuses { get; set; } = new List<string> { "Active", "Signed" };

        /// <summary>
        /// Valor mínimo del contrato para enviar notificaciones
        /// </summary>
        public decimal? MinimumContractValue { get; set; } = null;

        /// <summary>
        /// Indica si se deben enviar notificaciones a organizadores
        /// </summary>
        public bool NotifyOrganizers { get; set; } = true;

        /// <summary>
        /// Indica si se deben enviar notificaciones a patrocinadores
        /// </summary>
        public bool NotifySponsors { get; set; } = true;

        /// <summary>
        /// Plantilla para el asunto del correo electrónico de notificación
        /// </summary>
        public string EmailSubjectTemplate { get; set; } = "Contrato {ContractTitle} vence en {DaysRemaining} días";

        /// <summary>
        /// Plantilla para el cuerpo del correo electrónico de notificación
        /// </summary>
        public string EmailBodyTemplate { get; set; } = 
            "Estimado {RecipientName},\n\n" +
            "Le informamos que el contrato '{ContractTitle}' con valor de {ContractValue:C} vencerá en {DaysRemaining} días " +
            "({ExpirationDate:dd/MM/yyyy}).\n\n" +
            "Por favor, contacte a la otra parte para discutir la renovación o finalización del contrato.\n\n" +
            "Saludos cordiales,\n" +
            "Sistema de Gestión de Patrocinios";
    }
}