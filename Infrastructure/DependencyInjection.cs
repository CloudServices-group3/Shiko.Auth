
// Using: Microsoft.EntityFrameworkCore.InMemory for the in-memory database.

using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure
        (   
            this IServiceCollection services,
            IConfiguration configuration,
            IHostEnvironment environment
        )
    {
        // Use in-memory database in Development environment.
        if (environment.IsDevelopment()) 
        {
            services.AddDbContext<DataContext>(options =>
                options.UseInMemoryDatabase("DevelopmentDb")
            );
        }
        else
        {
            // Use SQL Server in Production environment.
            var connectionString = configuration.GetConnectionString("ProductionDatabaseUrl")
            ?? throw new ArgumentException("Production database Url not provided.");

            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(connectionString)
            );
        }

        return services;
    }
}
