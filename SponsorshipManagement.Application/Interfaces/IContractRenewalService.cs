using SponsorshipManagement.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SponsorshipManagement.Application.Interfaces
{
    /// <summary>
    /// Interfaz para el servicio de renovación de contratos
    /// 🎯 CU-PA-05: Gestionar renovaciones y finalización de contratos
    /// </summary>
    public interface IContractRenewalService
    {
        /// <summary>
        /// Obtiene contratos que vencen en los próximos N días
        /// 🎯 CU-PA-05.01.1: Consulta de contratos próximos a vencer
        /// </summary>
        /// <param name="days">Número de días para filtrar (por defecto 30)</param>
        /// <param name="includeStatuses">Estados de contrato a incluir</param>
        /// <returns>Lista de contratos próximos a vencer</returns>
        Task<IEnumerable<ContractRenewalDto>> GetContractsExpiringInDaysAsync(
            int days = 30, 
            string[]? includeStatuses = null);

        /// <summary>
        /// Marca un contrato como notificado para renovación
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>True si se marcó correctamente</returns>
        Task<bool> MarkContractAsNotifiedAsync(Guid contractId);
    }
}