using Microsoft.EntityFrameworkCore;
using Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Strategies
{
    internal class SqlServerConfigurationsStrategy : IDbConfigurationsStrategy
    {
        public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ExampleItemSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new UserSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new UserSettingsSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new TransactionSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new SummarySqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceBaselineSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new JobApplicationSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new UpcomingDateSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new DailyGoalSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new WeeklyGoalSqlServerConfiguration());
            modelBuilder.ApplyConfiguration(new GoalCheckInSqlServerConfiguration());
            return modelBuilder;
        }
    }
}
