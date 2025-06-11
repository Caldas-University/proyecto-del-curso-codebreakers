using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Domain.Entities;

namespace SponsorshipManagement.Application.Services
{
    public interface IContractValidationService
    {
        Task<ContractValidationResult> ValidateContractForCommitmentAsync(Guid contractId);
        Task<Contract?> GetContractAsync(Guid contractId);
    }

    public class ContractValidationService : IContractValidationService
    {
        private readonly IContractRepository _contractRepository;

        public ContractValidationService(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        public async Task<ContractValidationResult> ValidateContractForCommitmentAsync(Guid contractId)
        {
            try
            {
                // 1. Verificar que el contrato existe
                var contract = await _contractRepository.GetByIdAsync(contractId);
                
                if (contract == null)
                {
                    return new ContractValidationResult
                    {
                        IsValid = false,
                        ErrorCode = "CONTRACT_NOT_FOUND",
                        ErrorMessage = "El contrato especificado no existe"
                    };
                }

                // 2. Verificar que el contrato puede tener compromisos
                if (!contract.CanHaveCommitments())
                {
                    return new ContractValidationResult
                    {
                        IsValid = false,
                        ErrorCode = "CONTRACT_INVALID_STATUS",
                        ErrorMessage = $"El contrato está en estado '{contract.Status}' y no puede tener compromisos. Debe estar 'Firmado' o 'Activo'."
                    };
                }

                // 3. Verificar que no esté expirado
                if (contract.Status == ContractStatus.Expired)
                {
                    return new ContractValidationResult
                    {
                        IsValid = false,
                        ErrorCode = "CONTRACT_EXPIRED",
                        ErrorMessage = "El contrato ha expirado y no puede tener nuevos compromisos"
                    };
                }

                // 4. Validación exitosa
                return new ContractValidationResult
                {
                    IsValid = true,
                    Contract = contract,
                    ErrorCode = null,
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new ContractValidationResult
                {
                    IsValid = false,
                    ErrorCode = "VALIDATION_ERROR",
                    ErrorMessage = $"Error al validar el contrato: {ex.Message}"
                };
            }
        }

        public async Task<Contract?> GetContractAsync(Guid contractId)
        {
            return await _contractRepository.GetByIdAsync(contractId);
        }
    }

    public class ContractValidationResult
    {
        public bool IsValid { get; set; }
        public Contract? Contract { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
