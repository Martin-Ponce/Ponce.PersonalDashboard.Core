using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Configurations
{
    /// <summary>Registers AppDbContext with whichever provider is configured.</summary>
    public static class DbContextServiceExtensions
    {
        public static IServiceCollection AddAppDbContext(this IServiceCollection services, DatabaseConnectionConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddDbContext<AppDbContext>(options =>
            {
                switch (configuration.Provider)
                {
                    case DatabaseProviders.SQL_SERVER:
                        options.UseSqlServer(configuration.ConnectionString);
                        break;
                    case DatabaseProviders.POSTGRES:
                        options.UseNpgsql(configuration.ConnectionString);
                        break;
                    case DatabaseProviders.SQLITE:
                        options.UseSqlite(configuration.ConnectionString);
                        break;
                    default:
                        throw ConfigurationException.InvalidConfiguration(
                            ExceptionSources.APPSETTINGS,
                            $"Database provider '{configuration.Provider}' is not supported. Use one of: {DatabaseProviders.SQL_SERVER}, {DatabaseProviders.POSTGRES}, {DatabaseProviders.SQLITE}."
                        );
                }
            });
            return services;
        }
    }
}
