using Microsoft.EntityFrameworkCore;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Strategies
{
    /// <summary>Applies all the entity mappings for one database provider.</summary>
    internal interface IDbConfigurationsStrategy
    {
        ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder);
    }
}
