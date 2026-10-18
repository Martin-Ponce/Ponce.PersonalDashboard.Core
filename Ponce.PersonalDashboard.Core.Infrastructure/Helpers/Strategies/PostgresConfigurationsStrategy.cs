using Microsoft.EntityFrameworkCore;
using Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Strategies
{
    internal class PostgresConfigurationsStrategy : IDbConfigurationsStrategy
    {
        public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ExampleItemPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new UserPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingsPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new SummaryPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceBaselinePostgresConfiguration());
            modelBuilder.ApplyConfiguration(new JobApplicationPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new UpcomingDatePostgresConfiguration());
            modelBuilder.ApplyConfiguration(new DailyGoalPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new WeeklyGoalPostgresConfiguration());
            modelBuilder.ApplyConfiguration(new GoalCheckInPostgresConfiguration());
            return modelBuilder;
        }
    }
}
