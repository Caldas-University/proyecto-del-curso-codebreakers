using SponsorshipManagement.Application.Dtos;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;


namespace SponsorshipManagement.Application.Services
{
    public class CommitmentService : ICommitmentService
    {
        private readonly ICommitmentRepository _commitmentRepository;
        private readonly IContractValidationService _contractValidationService;
        private readonly CommitmentStateService _stateService;
        private readonly IAuditService? _auditService;  // DEBE ESTAR AQUÍ

        public CommitmentService(
            ICommitmentRepository commitmentRepository,
            IContractValidationService contractValidationService,
            CommitmentStateService stateService,
            IAuditService? auditService = null)  // DEBE RECIBIR IAuditService
        {
            _commitmentRepository = commitmentRepository;
            _contractValidationService = contractValidationService;
            _stateService = stateService;
            _auditService = auditService;  // DEBE ASIGNARSE
        }

        // ...existing methods...

        public async Task<CommitmentDto?> GetCommitmentByIdAsync(Guid id)
        {
            try
            {
                var commitment = await _commitmentRepository.GetByIdAsync(id);
                if (commitment == null) 
                    return null;

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
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error obteniendo compromiso {id}: {ex.Message}");
                return null;
            }
        }

        public async Task<IEnumerable<CommitmentDto>> GetCommitmentsByContractIdAsync(Guid contractId)
        {
            try
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
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error obteniendo compromisos del contrato {contractId}: {ex.Message}");
                return Enumerable.Empty<CommitmentDto>();
            }
        }

        public async Task<IEnumerable<CommitmentDto>> GetAllCommitmentsAsync()
        {
            try
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
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error obteniendo compromisos: {ex.Message}");
                return Enumerable.Empty<CommitmentDto>();
            }
        }

        /// <summary>
        /// 🎯 CU-PA-02.01.4: Obtiene estadísticas de compromisos por contrato
        /// </summary>
        public async Task<CommitmentStatisticsDto> GetCommitmentStatisticsAsync(Guid contractId)
        {
            try
            {
                // Console.WriteLine($"📊 Obteniendo estadísticas para contrato {contractId}");

                var commitments = await _commitmentRepository.GetByContractIdAsync(contractId);
                var commitmentsList = commitments.ToList();

                // Obtener información del contrato si está disponible
                string? contractTitle = null;
                string? contractStatus = null;
                
                if (_contractValidationService != null)
                {
                    try
                    {
                        var validationResult = await _contractValidationService.ValidateContractForCommitmentAsync(contractId);
                        if (validationResult.IsValid && validationResult.Contract != null)
                        {
                            contractTitle = validationResult.Contract.Title;
                            contractStatus = validationResult.Contract.Status.ToString();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Console.WriteLine($"⚠️ No se pudo obtener información del contrato: {ex.Message}");
                    }
                }

                // Calcular estadísticas
                var stats = new CommitmentStatisticsDto
                {
                    ContractId = contractId.ToString(),
                    ContractTitle = contractTitle,
                    ContractStatus = contractStatus,
                    TotalCommitments = commitmentsList.Count,
                    PendingCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Pending),
                    InProgressCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.InProgress),
                    CompletedCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Completed),
                    CancelledCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Cancelled),
                    OverdueCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.Overdue),
                    OnHoldCommitments = commitmentsList.Count(c => c.Status == CommitmentStatus.OnHold),
                    GeneratedAt = DateTime.UtcNow
                };

                // Calcular porcentajes
                if (stats.TotalCommitments > 0)
                {
                    stats.CompletionRate = Math.Round((double)stats.CompletedCommitments / stats.TotalCommitments * 100, 2);
                    stats.PendingRate = Math.Round((double)stats.PendingCommitments / stats.TotalCommitments * 100, 2);
                    stats.OverdueRate = Math.Round((double)stats.OverdueCommitments / stats.TotalCommitments * 100, 2);
                }

                // Console.WriteLine($"✅ Estadísticas generadas: {stats.TotalCommitments} compromisos, {stats.CompletionRate}% completados");
                return stats;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error al obtener estadísticas para contrato {contractId}: {ex.Message}");
                return new CommitmentStatisticsDto 
                { 
                    ContractId = contractId.ToString(),
                    GeneratedAt = DateTime.UtcNow
                };
            }
        }

        // ...existing methods como CreateCommitmentAsync, UpdateCommitmentAsync, etc...

        public async Task<CommitmentDto?> CreateCommitmentAsync(CreateCommitmentRequest createCommitmentRequest)
        {
            try
            {
                // Console.WriteLine($"📥 Recibiendo request para crear compromiso:");
                // Console.WriteLine($"   ContractId: {createCommitmentRequest.ContractId}");
                // Console.WriteLine($"   Description: {createCommitmentRequest.Description}");
                // Console.WriteLine($"   DueDate: {createCommitmentRequest.DueDate}");
                // Console.WriteLine($"   Responsible: {createCommitmentRequest.Responsible}");

                // VALIDACIÓN Y CREACIÓN DEL COMPROMISO
                // PASO 1-3: Validaciones existentes
                await ValidateRequestDataAsync(createCommitmentRequest);
                
                if (!Guid.TryParse(createCommitmentRequest.ContractId, out var contractId))
                {
                    throw new ArgumentException("El formato del ID del contrato es inválido. Debe ser un GUID válido.");
                }

                await ValidateContractExistenceAsync(contractId);
                await ValidateBusinessRulesAsync(createCommitmentRequest);

                // ✅ PASO 4: Crear el compromiso 
                // 🎯 CU-PA-02.01.4: El constructor asigna automáticamente estado "Pending"
                var commitment = new Commitment(
                    contractId,
                    createCommitmentRequest.Description.Trim(),
                    createCommitmentRequest.Obligations.Trim(),
                    createCommitmentRequest.DueDate,
                    createCommitmentRequest.Responsible.Trim()
                );

                // Console.WriteLine($"🎯 CU-PA-02.01.4: Compromiso creado con estado inicial: {commitment.Status}");

                // PASO 5: Almacenar en repositorio
                var success = await _commitmentRepository.AddAsync(commitment);
                if (!success)
                {
                    throw new InvalidOperationException("Error interno: No se pudo almacenar el compromiso");
                }

                // CU-PA-02.01.5: Registrar auditoría de creación
                if (_auditService != null)
                {
                    // Console.WriteLine($"🔍 Llamando a AuditService.LogCommitmentCreationAsync...");
                    var auditResult = await _auditService.LogCommitmentCreationAsync(commitment);
                    // Console.WriteLine($"📝 Resultado de auditoría: {auditResult}");
                }
                else
                {
                    // Console.WriteLine("❌ AuditService NO disponible - No se registrará auditoría");
                }

                // STATE SERVICE
                if (_stateService != null)
                {
                    // Console.WriteLine($"🎯 CU-PA-02.01.4: Inicializando estado para compromiso {commitment.Id}");
                    await _stateService.InitializeCommitmentStateAsync(commitment.Id);
                    // Console.WriteLine($"✅ CU-PA-02.01.4: Compromiso {commitment.Id} confirmado en estado {commitment.Status}");
                }

                // Console.WriteLine($"✅ Compromiso {commitment.Id} creado exitosamente en estado {commitment.Status}");
                // Console.WriteLine($"✅ Compromiso creado exitosamente: {commitment.Id}");

                // ✅ RETORNAR CommitmentDto EN LUGAR DE Guid
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
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error creando compromiso: {ex.Message}");
                throw;
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
                // Console.WriteLine($"❌ Error al eliminar compromiso {id}: {ex.Message}");
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
                // Console.WriteLine($"❌ Error al obtener compromisos vencidos: {ex.Message}");
                return Enumerable.Empty<CommitmentDto>();
            }
        }

        #region Private Methods

        private static async Task ValidateRequestDataAsync(CreateCommitmentRequest request)
        {
            var errors = new List<string>();

            if (request == null)
            {
                throw new ArgumentException("Los datos del compromiso son obligatorios");
            }

            if (string.IsNullOrWhiteSpace(request.ContractId))
                errors.Add("ContractId es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Description))
                errors.Add("Description es obligatoria");

            if (string.IsNullOrWhiteSpace(request.Obligations))
                errors.Add("Obligations es obligatorio");

            if (string.IsNullOrWhiteSpace(request.Responsible))
                errors.Add("Responsible es obligatorio");

            if (request.DueDate != default(DateTime))
            {
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

            if (!string.IsNullOrWhiteSpace(request.Description) && request.Description.Length > 500)
                errors.Add("Description no puede exceder 500 caracteres");

            if (!string.IsNullOrWhiteSpace(request.Obligations) && request.Obligations.Length > 1000)
                errors.Add("Obligations no puede exceder 1000 caracteres");

            if (!string.IsNullOrWhiteSpace(request.Responsible) && request.Responsible.Length > 200)
                errors.Add("Responsible no puede exceder 200 caracteres");

            if (errors.Any())
            {
                var errorMessage = $"Faltan datos obligatorios o son inválidos: {string.Join(", ", errors)}";
                // Console.WriteLine($"❌ Validación fallida: {errorMessage}");
                throw new ArgumentException(errorMessage);
            }

            // Console.WriteLine("✅ Validación de datos exitosa");
            await Task.CompletedTask;
        }

        private async Task ValidateContractExistenceAsync(Guid contractId)
        {
            try
            {
                // Console.WriteLine($"🔍 CU-PA-02.01.3: Validando existencia del contrato {contractId}");

                if (_contractValidationService != null)
                {
                    var validationResult = await _contractValidationService.ValidateContractForCommitmentAsync(contractId);
                    
                    if (!validationResult.IsValid)
                    {
                        // Console.WriteLine($"❌ A1 - Contrato inválido: {validationResult.ErrorMessage}");
                        throw new InvalidOperationException(
                            validationResult.ErrorMessage ?? "El contrato no es válido para crear compromisos"
                        );
                    }

                    // Console.WriteLine($"✅ CU-PA-02.01.3: Contrato {contractId} validado exitosamente");
                }
                else
                {
                    // Console.WriteLine("⚠️ Usando validación básica (fallback)");
                    var contractExists = await _commitmentRepository.ContractExistsAsync(contractId);
                    
                    if (!contractExists)
                    {
                        // Console.WriteLine($"❌ A1 - Contrato no encontrado: {contractId}");
                        throw new InvalidOperationException(
                            "El contrato especificado no existe o no está disponible para compromisos"
                        );
                    }

                    // Console.WriteLine($"✅ Contrato {contractId} validado (validación básica)");
                }
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error inesperado en CU-PA-02.01.3: {ex.Message}");
                throw new InvalidOperationException(
                    $"Error al validar la existencia del contrato: {ex.Message}", ex
                );
            }
        }

        private async Task ValidateBusinessRulesAsync(CreateCommitmentRequest request)
        {
            if (request.DueDate.DayOfWeek == DayOfWeek.Saturday || 
                request.DueDate.DayOfWeek == DayOfWeek.Sunday)
            {
                var daysDifference = (request.DueDate - DateTime.UtcNow).Days;
                if (daysDifference <= 7)
                {
                    // Console.WriteLine($"⚠️ Advertencia: Compromiso con vencimiento en fin de semana ({request.DueDate:yyyy-MM-dd})");
                }
            }

            if (Guid.TryParse(request.ContractId, out var contractId))
            {
                var existingCommitments = await _commitmentRepository.GetByContractIdAsync(contractId);
                var sameDateCommitments = existingCommitments.Count(c => c.DueDate.Date == request.DueDate.Date);
                
                if (sameDateCommitments >= 5)
                {
                    // Console.WriteLine($"⚠️ Advertencia: El contrato {contractId} ya tiene {sameDateCommitments} compromisos para la fecha {request.DueDate:yyyy-MM-dd}");
                }
            }

            await Task.CompletedTask;
        }

        private static async Task RegisterAuditLogAsync(string action, Commitment commitment)
        {
            try
            {
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

                // Console.WriteLine($"📝 AUDIT LOG: {action} - Commitment {commitment.Id} for Contract {commitment.ContractId} at {auditEntry.Timestamp}");
                
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error en auditoría: {ex.Message}");
            }
        }

        #endregion
    }
}
