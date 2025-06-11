using SponsorshipManagement.Domain.Entities;
using SponsorshipManagement.Domain.Interfaces;
using System.Text.Json;

namespace SponsorshipManagement.Infrastructure.Repositories
{
    /// <summary>
    /// Implementación del repositorio de auditoría basado en archivos JSON
    /// CU-PA-02.01.5: Registro de auditoría al crear compromiso
    /// </summary>
    public class AuditRepository : IAuditRepository
    {
        private readonly string _filePath = GetDataFilePath();
        private readonly JsonSerializerOptions _jsonOptions;

        public AuditRepository()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            
            EnsureFileExists();
        }

        public async Task<bool> LogAsync(AuditLog auditLog)
        {
            try
            {
                // Console.WriteLine($"🔍 AuditRepository.LogAsync iniciado");
                // Console.WriteLine($"🔍 Archivo: {_filePath}");
                // Console.WriteLine($"🔍 Archivo existe: {File.Exists(_filePath)}");
                
                var logs = await GetAllLogsFromFileAsync();
                // Console.WriteLine($"🔍 Logs existentes leídos: {logs.Count}");
                
                logs.Add(auditLog);
                // Console.WriteLine($"🔍 Log agregado. Total ahora: {logs.Count}");
                
                await SaveLogsToFileAsync(logs);
                // Console.WriteLine($"✅ Logs guardados en archivo");
                
                // VERIFICAR que se guardó
                var verification = await GetAllLogsFromFileAsync();
                // Console.WriteLine($"🔍 Verificación - Logs en archivo después de guardar: {verification.Count}");
                
                return true;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error en AuditRepository.LogAsync: {ex.Message}");
                // Console.WriteLine($"❌ StackTrace: {ex.StackTrace}");
                return false;
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId)
        {
            try
            {
                var logs = await GetAllLogsFromFileAsync();
                return logs.Where(l => l.EntityType == entityType && l.EntityId == entityId)
                          .OrderByDescending(l => l.Timestamp);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error obteniendo auditoría por entidad: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId)
        {
            try
            {
                var logs = await GetAllLogsFromFileAsync();
                return logs.Where(l => l.UserId == userId)
                          .OrderByDescending(l => l.Timestamp);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error obteniendo auditoría por usuario: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                var logs = await GetAllLogsFromFileAsync();
                return logs.Where(l => l.Timestamp >= startDate && l.Timestamp <= endDate)
                          .OrderByDescending(l => l.Timestamp);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error obteniendo auditoría por fecha: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetByActionAsync(string action)
        {
            try
            {
                var logs = await GetAllLogsFromFileAsync();
                return logs.Where(l => l.Action == action)
                          .OrderByDescending(l => l.Timestamp);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error obteniendo auditoría por acción: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<IEnumerable<AuditLog>> GetAllAsync()
        {
            try
            {
                var logs = await GetAllLogsFromFileAsync();
                return logs.OrderByDescending(l => l.Timestamp);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error obteniendo todos los logs de auditoría: {ex.Message}");
                return Enumerable.Empty<AuditLog>();
            }
        }

        public async Task<(IEnumerable<AuditLog> Logs, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var allLogs = await GetAllLogsFromFileAsync();
                var totalCount = allLogs.Count;
                
                var pagedLogs = allLogs.OrderByDescending(l => l.Timestamp)
                                     .Skip((page - 1) * pageSize)
                                     .Take(pageSize);
                
                return (pagedLogs, totalCount);
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error obteniendo auditoría paginada: {ex.Message}");
                return (Enumerable.Empty<AuditLog>(), 0);
            }
        }

        public async Task<int> CleanupOldLogsAsync(DateTime cutoffDate)
        {
            try
            {
                var logs = await GetAllLogsFromFileAsync();
                var initialCount = logs.Count;
                
                logs.RemoveAll(l => l.Timestamp < cutoffDate);
                
                await SaveLogsToFileAsync(logs);
                
                var removedCount = initialCount - logs.Count;
                // Console.WriteLine($"Limpieza de auditoría: {removedCount} registros eliminados");
                
                return removedCount;
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error limpiando logs antiguos: {ex.Message}");
                return 0;
            }
        }

        #region Private Methods

        private void EnsureFileExists()
        {
            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(_filePath))
                {
                    File.WriteAllText(_filePath, "[]");
                }
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error creando archivo de auditoría: {ex.Message}");
            }
        }

        private async Task<List<AuditLog>> GetAllLogsFromFileAsync()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    return new List<AuditLog>();
                }

                var json = await File.ReadAllTextAsync(_filePath);
                
                if (string.IsNullOrWhiteSpace(json))
                {
                    return new List<AuditLog>();
                }

                return JsonSerializer.Deserialize<List<AuditLog>>(json, _jsonOptions) ?? new List<AuditLog>();
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"Error leyendo archivo de auditoría: {ex.Message}");
                return new List<AuditLog>();
            }
        }

        private async Task SaveLogsToFileAsync(List<AuditLog> logs)
        {
            try
            {
                // Console.WriteLine($"🔍 SaveLogsToFileAsync - Guardando {logs.Count} logs en {_filePath}");
                
                var json = JsonSerializer.Serialize(logs, _jsonOptions);
                // Console.WriteLine($"🔍 JSON serializado - Longitud: {json.Length}");
                // Console.WriteLine($"🔍 JSON preview: {json.Substring(0, Math.Min(200, json.Length))}...");
                
                await File.WriteAllTextAsync(_filePath, json);
                // Console.WriteLine($"✅ Archivo escrito exitosamente");
                
                // Verificar que se escribió
                var fileContent = await File.ReadAllTextAsync(_filePath);
                // Console.WriteLine($"🔍 Contenido del archivo después de escribir - Longitud: {fileContent.Length}");
            }
            catch (Exception ex)
            {
                // Console.WriteLine($"❌ Error en SaveLogsToFileAsync: {ex.Message}");
                throw;
            }
        }

        private static string GetDataFilePath()
        {
            // Obtener la carpeta del proyecto Infrastructure
            var infrastructureFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "..", 
                "SponsorshipManagement.Infrastructure", 
                "Data"
            );
            
            // Si no existe, usar la ruta actual
            if (!Directory.Exists(infrastructureFolder))
            {
                infrastructureFolder = Path.Combine("Data");
            }
            
            // Crear directorio si no existe
            if (!Directory.Exists(infrastructureFolder))
            {
                Directory.CreateDirectory(infrastructureFolder);
            }
            
            return Path.Combine(infrastructureFolder, "audit-logs.json");
        }

        #endregion
    }
}
