using Microsoft.EntityFrameworkCore;
using Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Strategies
{
    internal class SqliteConfigurationsStrategy : IDbConfigurationsStrategy
    {
        public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ExampleItemSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new UserSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingsSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new SummarySqliteConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceBaselineSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new JobApplicationSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new UpcomingDateSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new DailyGoalSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new WeeklyGoalSqliteConfiguration());
            modelBuilder.ApplyConfiguration(new GoalCheckInSqliteConfiguration());
            return modelBuilder;
        }
    }
}
