using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Services
{
    /// <summary>
    /// Servicio para gestión de renovaciones y finalización de contratos
    /// 🎯 CU-PA-05: Gestionar renovaciones y finalización de contratos
    /// </summary>
    public class ContractRenewalService : IContractRenewalService
    {
        private readonly IContractRepository _contractRepository;
        private readonly ISponsorRepository _sponsorRepository;

        public ContractRenewalService(
            IContractRepository contractRepository,
            ISponsorRepository sponsorRepository)
        {
            _contractRepository = contractRepository;
            _sponsorRepository = sponsorRepository;
        }

        /// <summary>
        /// Obtiene contratos que vencen en los próximos N días
        /// 🎯 CU-PA-05.01.1: Consulta de contratos próximos a vencer
        /// </summary>
        public async Task<IEnumerable<ContractRenewalDto>> GetContractsExpiringInDaysAsync(
            int days = 30, 
            string[]? includeStatuses = null)
        {
            // Agregar esto al inicio del método
            Console.WriteLine($"🔍 Iniciando búsqueda de contratos próximos a vencer...");
            
            // Asegurarse de que today y expirationLimit estén definidos
            var today = DateTime.UtcNow.Date;
            var expirationLimit = today.AddDays(days);
            
            // Obtener todos los contratos
            var allContracts = await _contractRepository.GetAllAsync();
            
            // Filtrar por fecha de vencimiento y estado
            var filteredContracts = allContracts.Where(c => 
                c.EndDate.Date > today && 
                c.EndDate.Date <= expirationLimit && 
                (includeStatuses == null || includeStatuses.Length == 0 || includeStatuses.Contains(c.Status.ToString())));
            
            // Lista para almacenar los resultados
            var result = new List<ContractRenewalDto>();
            
            // Construir DTOs con información adicional
            foreach (var contract in filteredContracts)
            {
                // Calcular días hasta vencimiento
                var daysUntilExpiration = (int)(contract.EndDate.Date - today).TotalDays;
                
                // Obtener información del patrocinador
                string sponsorName = "Desconocido";
                try
                {
                    // Ajustar según la firma real del método en ISponsorRepository
                    // Posiblemente necesite usar GetByDocumentAsync o una conversión de tipo
                    var sponsor = await _sponsorRepository.GetByDocumentAsync(contract.SponsorId.ToString());
                    if (sponsor != null)
                    {
                        sponsorName = sponsor.Name;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠️ Error obteniendo información del patrocinador {contract.SponsorId}: {ex.Message}");
                }
                
                // Comprobar si el contrato ya tiene propiedades de notificación
                // Si no existen, usar valores predeterminados
                bool notificationSent = false;
                DateTime? lastNotificationDate = null;

                // Usar reflexión para verificar si las propiedades existen
                var notificationProperty = contract.GetType().GetProperty("NotificationSent");
                if (notificationProperty != null)
                {
                    var value = notificationProperty.GetValue(contract);
                    if (value != null)
                        notificationSent = (bool)value;
                }

                var dateProperty = contract.GetType().GetProperty("LastNotificationDate");
                if (dateProperty != null)
                {
                    lastNotificationDate = dateProperty.GetValue(contract) as DateTime?;
                }
                
                // Crear DTO con toda la información
                var renewalDto = new ContractRenewalDto
                {
                    Id = contract.Id.ToString(),
                    Title = contract.Title,
                    Description = contract.Description,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    DaysUntilExpiration = daysUntilExpiration,
                    Status = contract.Status.ToString(),
                    SponsorId = contract.SponsorId.ToString(),
                    SponsorName = sponsorName,
                    Value = contract.Value,
                    NotificationSent = notificationSent,
                    LastNotificationDate = lastNotificationDate
                };
                
                result.Add(renewalDto);
            }
            
            Console.WriteLine($"✅ CU-PA-05.01.1: Encontrados {result.Count} contratos próximos a vencer");
            
            // Ordenar por días hasta vencimiento (más cercanos primero)
            return result.OrderBy(c => c.DaysUntilExpiration);
        }

        /// <summary>
        /// Marca un contrato como notificado para renovación
        /// </summary>
        public async Task<bool> MarkContractAsNotifiedAsync(Guid contractId)
        {
            var contract = await _contractRepository.GetByIdAsync(contractId);
            if (contract == null)
                return false;
            
            // Verificar si las propiedades existen usando reflexión
            var notificationProperty = contract.GetType().GetProperty("NotificationSent");
            var dateProperty = contract.GetType().GetProperty("LastNotificationDate");
            
            // Si las propiedades existen, establecerlas
            if (notificationProperty != null && dateProperty != null)
            {
                notificationProperty.SetValue(contract, true);
                dateProperty.SetValue(contract, DateTime.UtcNow);
                
                try
                {
                    // UpdateAsync devuelve el contrato actualizado, no un bool
                    var updatedContract = await _contractRepository.UpdateAsync(contract);
                    return updatedContract != null; // Si devuelve el contrato, consideramos que fue exitoso
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error al actualizar el contrato: {ex.Message}");
                    return false;
                }
            }
            
            // Si las propiedades no existen, registrar el problema y devolver false
            Console.WriteLine("⚠️ Las propiedades NotificationSent y LastNotificationDate no existen en la entidad Contract");
            return false;
        }
    }
}