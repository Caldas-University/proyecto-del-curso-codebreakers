using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;

namespace SponsorshipManagement.Application.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentRepository _commitmentRepository;
        private readonly IContractValidationService? _contractValidationService;

        // Constructor con validación de contratos (DI completa)
        public CommitmentService(
            ICommitmentRepository commitmentRepository,
            IContractValidationService contractValidationService)
        {
            _commitmentRepository = commitmentRepository;
            _contractValidationService = contractValidationService;
        }

        // Constructor sin validación de contratos (fallback)
        public CommitmentService(ICommitmentRepository commitmentRepository)
        {
            _commitmentRepository = commitmentRepository;
            _contractValidationService = null;
        }

        public async Task<CommitmentDto?> GetCommitmentByIdAsync(Guid id)
        {
            var commitment = await _commitmentRepository.GetByIdAsync(id);
            if (commitment == null) return null;

            return new CommitmentDto
            {
                Id = commitment.Id.ToString(),
                ContractId = commitment.ContractId.ToString(),
                Description = commitment.Description,
                Obligations = commitment.Obligations,
                DueDate = commitment.DueDate,
                Responsible = commitment.Responsible,
                Status = commitment.Status.ToString(),
                CreatedAt = commitment.CreatedAt,
                UpdatedAt = commitment.UpdatedAt
            };
        }

        public async Task<IEnumerable<CommitmentDto>> GetCommitmentsByContractIdAsync(Guid contractId)
        {
            var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
            return commitments.Select(c => new CommitmentDto
            {
                Id = c.Id.ToString(),
                ContractId = c.ContractId.ToString(),
                Description = c.Description,
                Obligations = c.Obligations,
                DueDate = c.DueDate,
                Responsible = c.Responsible,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<IEnumerable<CommitmentDto>> GetAllCommitmentsAsync()
        {
            var commitments = await _commitmentRepository.GetAllAsync();
            return commitments.Select(c => new CommitmentDto
            {
                Id = c.Id.ToString(),
                ContractId = c.ContractId.ToString(),
                Description = c.Description,
                Obligations = c.Obligations,
                DueDate = c.DueDate,
                Responsible = c.Responsible,
                Status = c.Status.ToString(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        /// <summary>
        /// 🎯 CU-PA-02.01 - Registrar compromisos contractuales
        /// Implementa validación completa incluyendo CU-PA-02.01.3
        /// </summary>
        /// <param name="request">Datos del compromiso a crear</param>
        /// <returns>DTO del compromiso creado</returns>
        /// <exception cref="ArgumentException">A2: Faltan datos obligatorios</exception>
        /// <exception cref="InvalidOperationException">A1: El contrato no existe o no es válido</exception>
        public async Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentRequest request)
        {
            try
            {
                // 🔍 PASO 1: Validación de datos obligatorios (CU-PA-02.01 - Flujo Alterno A2)
                await ValidateRequestDataAsync(request);

                // 🔍 PASO 2: Parsear y validar formato de ContractId
                if (!Guid.TryParse(request.ContractId, out var contractId))
                {
                    throw new ArgumentException("El formato del ID del contrato es inválido. Debe ser un GUID válido.");
                }

                // 🎯 PASO 3: CU-PA-02.01.3 - Validación de existencia del contrato
                await ValidateContractExistenceAsync(contractId);

                // 🔍 PASO 4: Validaciones de negocio adicionales
                await ValidateBusinessRulesAsync(request);

                // ✅ PASO 5: Crear el compromiso con estado inicial "pendiente"
                var commitment = new Commitment(
                    contractId,
                    request.Description.Trim(),
                    request.Obligations.Trim(),
                    request.DueDate,
                    request.Responsible.Trim()
                );

                // 📝 PASO 6: Registrar auditoría al crear compromiso (CU-PA-02.01.5)
                await RegisterAuditLogAsync("CREATE_COMMITMENT", commitment);

                // 💾 PASO 7: Almacenar en repositorio
                var success = await _commitmentRepository.AddAsync(commitment);
                if (!success)
                {
                    throw new InvalidOperationException("Error interno: No se pudo almacenar el compromiso en el repositorio");
                }

                // 📤 PASO 8: Retornar DTO del compromiso creado
                return new CommitmentDto
                {
                    Id = commitment.Id.ToString(),
                    ContractId = commitment.ContractId.ToString(),
                    Description = commitment.Description,
                    Obligations = commitment.Obligations,
                    DueDate = commitment.DueDate,
                    Responsible = commitment.Responsible,
                    Status = commitment.Status.ToString(),
                    CreatedAt = commitment.CreatedAt,
                    UpdatedAt = commitment.UpdatedAt
                };
            }
            catch (ArgumentException)
            {
                // Re-throw validation errors (A2)
                throw;
            }
            catch (InvalidOperationException)
            {
                // Re-throw business logic errors (A1)
                throw;
            }
            catch (Exception ex)
            {
                // Wrap unexpected errors
                throw new InvalidOperationException($"Error inesperado al crear el compromiso: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Valida que todos los datos obligatorios estén presentes y sean válidos
        /// </summary>
        private static async Task ValidateRequestDataAsync(CreateCommitmentRequest request)
        {
            var errors = new List<string>();

            // Validar que el request no sea null
            if (request == null)
            {
                throw new ArgumentException("Los datos del compromiso son obligatorios");
            }

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(request.ContractId))
                errors.Add("ContractId es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Description))
                errors.Add("Description es obligatoria");

            if (string.IsNullOrWhiteSpace(request.Obligations))
                errors.Add("Obligations es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Responsible))
                errors.Add("Responsible es obligatorio");

            // 🔧 VALIDACIÓN DE FECHA MEJORADA
            if (request.DueDate != default(DateTime))
            {
                // Validar que la fecha sea futura (con margen de 1 día para testing)
                var currentDate = DateTime.UtcNow.Date;
                var dueDate = request.DueDate.Date;
                
                if (dueDate < currentDate)
                {
                    errors.Add($"DueDate debe ser una fecha futura. Fecha actual: {currentDate:yyyy-MM-dd}, Fecha proporcionada: {dueDate:yyyy-MM-dd}");
                }
            }
            else
            {
                errors.Add("DueDate es obligatorio");
            }

            // Validar longitudes máximas
            if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > 500)
                errors.Add("Description no puede exceder 500 caracteres");

            if (!string.IsNullOrWhiteSpace(request.Obligations) && request.Obligations.Length > 1000)
                errors.Add("Obligations no puede exceder 1000 caracteres");

            if (!string.IsNullOrWhiteSpace(request.Responsible) && request.Responsible.Length > 200)
                errors.Add("Responsible no puede exceder 200 caracteres");

            // Si hay errores, lanzar excepción con todos los problemas
            if (errors.Any())
            {
                var errorMessage = $"Faltan datos obligatorios o son inválidos: {string.Join(", ", errors)}";
                Console.WriteLine($"❌ Validación fallida: {errorMessage}");
                throw new ArgumentException(errorMessage);
            }

            Console.WriteLine("✅ Validación de datos exitosa");
            await Task.CompletedTask;
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.3: Validación de existencia del contrato
        /// </summary>
        private async Task ValidateContractExistenceAsync(Guid contractId)
        {
            try
            {
                // Si tenemos el servicio de validación completo, usarlo
                if (_contractValidationService != null)
                {
                    var validationResult = await _contractValidationService.ValidateContractForCommitmentAsync(contractId);
                    
                    if (!validationResult.IsValid)
                    {
                        // A1: El contrato no existe o no es válido
                        throw new InvalidOperationException(
                            validationResult.ErrorMessage ?? "El contrato no es válido para crear compromisos"
                        );
                    }

                    // Log adicional para auditoría
                    Console.WriteLine($"✅ Contrato {contractId} validado exitosamente para compromisos");
                }
                else
                {
                    // Fallback: usar validación básica del repositorio
                    var contractExists = await _commitmentRepository.ContractExistsAsync(contractId);
                    
                    if (!contractExists)
                    {
                        // A1: El contrato no existe
                        throw new InvalidOperationException(
                            "El contrato especificado no existe o no está disponible para compromisos"
                        );
                    }

                    Console.WriteLine($"✅ Contrato {contractId} validado (validación básica)");
                }
            }
            catch (InvalidOperationException)
            {
                // Re-throw business errors
                throw;
            }
            catch (Exception ex)
            {
                // Wrap validation errors
                throw new InvalidOperationException(
                    $"Error al validar la existencia del contrato: {ex.Message}", ex
                );
            }
        }

        /// <summary>
        /// Validaciones de reglas de negocio adicionales
        /// </summary>
        private async Task ValidateBusinessRulesAsync(CreateCommitmentRequest request)
        {
            // Validar que no sea un fin de semana para compromisos urgentes
            if (request.DueDate.DayOfWeek == DayOfWeek.Saturday || 
                request.DueDate.DayOfWeek == DayOfWeek.Sunday)
            {
                var daysDifference = (request.DueDate - DateTime.UtcNow).Days;
                if (daysDifference <= 7)
                {
                    Console.WriteLine($"⚠️ Advertencia: Compromiso con vencimiento en fin de semana ({request.DueDate:yyyy-MM-dd})");
                }
            }

            // Validar que no haya muchos compromisos para el mismo contrato en la misma fecha
            if (Guid.TryParse(request.ContractId, out var contractId))
            {
                var existingCommitments = await _commitmentRepository.GetByContractIdAsync(contractId);
                var sameDateCommitments = existingCommitments.Count(c => c.DueDate.Date == request.DueDate.Date);
                
                if (sameDateCommitments >= 5)
                {
                    Console.WriteLine($"⚠️ Advertencia: El contrato {contractId} ya tiene {sameDateCommitments} compromisos para la fecha {request.DueDate:yyyy-MM-dd}");
                }
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Registra auditoría para el compromiso (CU-PA-02.01.5)
        /// </summary>
        private static async Task RegisterAuditLogAsync(string action, Commitment commitment)
        {
            try
            {
                // En una implementación real, esto iría a un sistema de auditoría
                var auditEntry = new
                {
                    Action = action,
                    CommitmentId = commitment.Id,
                    ContractId = commitment.ContractId,
                    Timestamp = DateTime.UtcNow,
                    Details = new
                    {
                        commitment.Description,
                        commitment.DueDate,
                        commitment.Responsible,
                        commitment.Status
                    }
                };

                // Por ahora, solo log en consola
                Console.WriteLine($"📝 AUDIT LOG: {action} - Commitment {commitment.Id} for Contract {commitment.ContractId} at {auditEntry.Timestamp}");
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // No fallar por errores de auditoría, solo logear
                Console.WriteLine($"❌ Error en auditoría: {ex.Message}");
            }
        }

        public async Task<CommitmentDto?> UpdateCommitmentAsync(Guid id, CreateCommitmentRequest request)
        {
            try
            {
                var existingCommitment = await _commitmentRepository.GetByIdAsync(id);
                if (existingCommitment == null) 
                    return null;

                // Validar datos de entrada
                await ValidateRequestDataAsync(request);

                // Actualizar el compromiso
                existingCommitment.UpdateCommitment(
                    request.Description.Trim(),
                    request.Obligations.Trim(),
                    request.DueDate,
                    request.Responsible.Trim()
                );

                // Registrar auditoría
                await RegisterAuditLogAsync("UPDATE_COMMITMENT", existingCommitment);

                var success = await _commitmentRepository.UpdateAsync(existingCommitment);
                if (!success) return null;

                return new CommitmentDto
                {
                    Id = existingCommitment.Id.ToString(),
                    ContractId = existingCommitment.ContractId.ToString(),
                    Description = existingCommitment.Description,
                    Obligations = existingCommitment.Obligations,
                    DueDate = existingCommitment.DueDate,
                    Responsible = existingCommitment.Responsible,
                    Status = existingCommitment.Status.ToString(),
                    CreatedAt = existingCommitment.CreatedAt,
                    UpdatedAt = existingCommitment.UpdatedAt
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al actualizar el compromiso: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteCommitmentAsync(Guid id)
        {
            try
            {
                var commitment = await _commitmentRepository.GetByIdAsync(id);
                if (commitment != null)
                {
                    await RegisterAuditLogAsync("DELETE_COMMITMENT", commitment);
                }

                return await _commitmentRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar compromiso {id}: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<CommitmentDto>> GetOverdueCommitmentsAsync()
        {
            try
            {
                var commitments = await _commitmentRepository.GetOverdueCommitmentsAsync();
                return commitments.Select(c => new CommitmentDto
                {
                    Id = c.Id.ToString(),
                    ContractId = c.ContractId.ToString(),
                    Description = c.Description,
                    Obligations = c.Obligations,
                    DueDate = c.DueDate,
                    Responsible = c.Responsible,
                    Status = c.Status.ToString(),
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener compromisos vencidos: {ex.Message}");
                return Enumerable.Empty<CommitmentDto>();
            }
        }

        /// <summary>
        /// Método adicional para obtener estadísticas de compromisos por contrato
        /// </summary>
        public async Task<CommitmentStatisticsDto> GetCommitmentStatisticsAsync(Guid contractId)
        {
            try
            {
                var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
                var commitmentsList = commitments.ToList();

                return new CommitmentStatisticsDto
                {
                    ContractId = contractId.ToString(),
                    TotalCommitments = commitmentsList.Count,
                    PendingCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Pending),
                    InProgressCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.InProgress),
                    CompletedCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Completed),
                    CancelledCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Cancelled),
                    OverdueCommitments = commitmentsList.Count(c => 
                        c.DueDate < DateTime.UtcNow && c.Status == CommitmentStatus.Pending),
                    CompletionRate = commitmentsList.Count > 0 
                        ? (double)commitmentsList.Count(c => c.Status == CommitmentStatus.Completed) / commitmentsList.Count * 100 
                        : 0
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener estadísticas para contrato {contractId}: {ex.Message}");
                return new CommitmentStatisticsDto { ContractId = contractId.ToString() };
            }
        }
    }

    /// <summary>
    /// DTO para estadísticas de compromisos
    /// </summary>
    public class CommitmentStatisticsDto
    {
        public string ContractId { get; set; } = string.Empty;
        public int TotalCommitments { get; set; }
        public int PendingCommitments { get; set; }
        public int InProgressCommitments { get; set; }
        public int CompletedCommitments { get; set; }
        public int CancelledCommitments { get; set; }
        public int OverdueCommitments { get; set; }
        public double CompletionRate { get; set; }
    }
}