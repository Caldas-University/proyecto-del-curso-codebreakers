using Microsoft.Extensions.DependencyInjection;
using SponsorshipManagement.Application.Interfaces;
using SponsorshipManagement.Application.Services;
using SponsorshipManagement.Domain.Interfaces;
using SponsorshipManagement.Infrastructure.Repositories;
using System;
using System.Threading.Tasks;

namespace SponsorshipManagement.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("🎯 CU-PA-05.02.2: Generación de mensajes con opciones de renovación o finalización");
            Console.WriteLine("====================================================================");
            
            // Configurar servicios
            var serviceProvider = ConfigureServices();
            
            try
            {
                // Obtener servicios
                var notificationService = serviceProvider.GetService<INotificationService>();
                var contractRepository = serviceProvider.GetService<IContractRepository>();
                
                // Listar contratos disponibles
                var contracts = await contractRepository.GetAllAsync();
                Console.WriteLine("Contratos disponibles:");
                Console.WriteLine("---------------------");
                foreach (var contract in contracts)
                {
                    Console.WriteLine($"ID: {contract.Id}");
                    Console.WriteLine($"Título: {contract.Title}");
                    Console.WriteLine($"Vence: {contract.EndDate:dd/MM/yyyy}");
                    Console.WriteLine("---------------------");
                }
                
                // Solicitar ID de contrato
                Console.WriteLine("\nIngrese el ID del contrato para generar mensaje:");
                string contractId = Console.ReadLine();
                
                // Verificar que el contrato existe
                var selectedContract = await contractRepository.GetByIdAsync(Guid.Parse(contractId));
                if (selectedContract == null)
                {
                    Console.WriteLine("❌ Contrato no encontrado");
                    return;
                }
                
                // Solicitar rol del destinatario
                Console.WriteLine("\nSeleccione el rol del destinatario:");
                Console.WriteLine("1. Patrocinador");
                Console.WriteLine("2. Organizador");
                string roleOption = Console.ReadLine();
                string role = roleOption == "1" ? "Patrocinador" : "Organizador";
                
                // Solicitar email del destinatario
                Console.WriteLine("\nIngrese el email del destinatario:");
                string email = Console.ReadLine();
                
                // Generar y enviar mensaje
                Console.WriteLine("\nGenerando mensaje estructurado...");
                var message = await notificationService.GenerateActionMessageAsync(contractId, role, email);
                await notificationService.SendActionMessageAsync(message);
                
                Console.WriteLine("\n✅ Mensaje generado y enviado correctamente");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
            
            Console.WriteLine("\nPresione cualquier tecla para salir...");
            Console.ReadKey();
        }
        
        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            // Registrar servicios
            services.AddScoped<IContractRepository, ContractRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<ISponsorRepository, SponsorRepository>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IContractRenewalService, ContractRenewalService>();
            
            return services.BuildServiceProvider();
        }
    }
}