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
        /// Obtiene contratos que vencen según criterios avanzados de filtrado
        /// 🎯 CU-PA-05.01.3: Exposición de contratos próximos a vencer con filtros avanzados
        /// </summary>
        /// <param name="filter">Criterios de filtrado</param>
        /// <returns>Lista de contratos próximos a vencer que cumplen los criterios</returns>
        Task<IEnumerable<ContractRenewalDto>> GetContractsExpiringAsync(ExpiringContractsFilterDto filter);

        /// <summary>
        /// Obtiene un resumen estadístico de los contratos próximos a vencer
        /// 🎯 CU-PA-05.01.3: Exposición de estadísticas de contratos próximos a vencer
        /// </summary>
        /// <param name="days">Número de días para considerar</param>
        /// <returns>Resumen estadístico de contratos próximos a vencer</returns>
        Task<ExpiringContractsSummaryDto> GetExpiringContractsSummaryAsync(int days = 30);

        /// <summary>
        /// Valida cumplimiento e impacto publicitario para un contrato
        /// 🎯 CU-PA-05.01.2: Validación de cumplimiento e impacto publicitario
        /// </summary>
        /// <param name="contractId">ID del contrato a validar</param>
        /// <returns>Información combinada de cumplimiento e impacto</returns>
        Task<ContractRenewalValidationDto> ValidateContractForRenewalAsync(Guid contractId);

        /// <summary>
        /// Marca un contrato como notificado para renovación
        /// </summary>
        /// <param name="contractId">ID del contrato</param>
        /// <returns>True si se marcó correctamente</returns>
        Task<bool> MarkContractAsNotifiedAsync(Guid contractId);

        /// <summary>
        /// Obtiene un contrato por su ID
        /// 🎯 CU-PA-05.02.1: Consulta de contrato por ID
        /// </summary>
        /// <param name="id">ID del contrato</param>
        /// <returns>Contrato o null si no existe</returns>
        Task<ContractRenewalDto?> GetContractByIdAsync(Guid id);

        /// <summary>
        /// Registra una decisión de renovación o finalización de contrato
        /// 🎯 CU-PA-05.02.3: Backend para registrar decisiones de renovación o finalización
        /// </summary>
        /// <param name="decision">Datos de la decisión</param>
        /// <returns>Respuesta con el resultado del registro</returns>
        Task<ContractDecisionResponseDto> RegisterContractDecisionAsync(ContractDecisionDto decision);
    }
}