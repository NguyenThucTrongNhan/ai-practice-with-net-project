using InventoryService.Application.Interfaces;
using InventoryService.Infrastructure.Persistence;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using InventoryService.Infrastructure.UnitOfWork;

namespace InventoryService.Infrastructure
{
    public static class InfrastructureServiceRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, bool useInMemory = false)
        {
            if (useInMemory)
            {
                services.AddDbContext<InventoryDbContext>(opt => opt.UseInMemoryDatabase("InventoryDb"));
            }
            else
            {
                var conn = configuration.GetConnectionString("DefaultConnection");
                services.AddDbContext<InventoryDbContext>(opt => opt.UseNpgsql(conn));
            }

            // Repositories and UoW - internal types
            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Example service used in event handlers
            services.AddScoped<IEmailService, ConsoleEmailService>();

            return services;
        }
    }
}
