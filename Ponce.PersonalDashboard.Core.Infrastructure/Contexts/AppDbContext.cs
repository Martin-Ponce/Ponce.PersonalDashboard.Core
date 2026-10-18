using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Ponce.PersonalDashboard.Core.Domain.Entities.Example;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Entities.UpcomingDate;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using Ponce.PersonalDashboard.Core.Infrastructure.Configurations;
using Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Factories;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Contexts
{
    /// <summary>The app's EF Core context. Mappings are picked per database provider at startup.</summary>
    public class AppDbContext(DbContextOptions<AppDbContext> options, DatabaseConnectionConfiguration configuration) : DbContext(options)
    {
        public DbSet<ExampleItem> ExampleItems => Set<ExampleItem>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserSettings> UserSettings => Set<UserSettings>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Summary> Summaries => Set<Summary>();
        public DbSet<BalanceBaseline> BalanceBaselines => Set<BalanceBaseline>();
        public DbSet<JobApplication> JobApplications => Set<JobApplication>();
        public DbSet<UpcomingDate> UpcomingDates => Set<UpcomingDate>();
        public DbSet<DailyGoal> DailyGoals => Set<DailyGoal>();
        public DbSet<WeeklyGoal> WeeklyGoals => Set<WeeklyGoal>();
        public DbSet<GoalCheckIn> GoalCheckIns => Set<GoalCheckIn>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            DbConfigurationsStrategyFactory.GetStrategy(configuration.Provider).ApplyConfigurations(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>Reads every DateTime back as UTC, since SQL Server and SQLite don't store the kind.</summary>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveConversion<UtcDateTimeConverter>();
            base.ConfigureConventions(configurationBuilder);
        }

        private sealed class UtcDateTimeConverter() : ValueConverter<DateTime, DateTime>(
            value => value.Kind == DateTimeKind.Utc ? value : value.ToUniversalTime(),
            value => DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }
}
