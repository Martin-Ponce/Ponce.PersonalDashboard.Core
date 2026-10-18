using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for GoalCheckIn. One check-in per goal per day; the relationship lives in the DailyGoal mapping.</summary>
    internal class GoalCheckInSqlServerConfiguration : IEntityTypeConfiguration<GoalCheckIn>
    {
        public void Configure(EntityTypeBuilder<GoalCheckIn> builder)
        {
            builder.ToTable("GoalCheckIns")
                .HasKey(x => x.Id);

            builder.HasIndex(x => new { x.DailyGoalId, x.Date })
                .IsUnique();

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(UNIQUEIDENTIFIER)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.DailyGoalId)
                .HasColumnName("dailyGoalId")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("date")
                .HasColumnType(DATE)
                .IsRequired();

            builder.Property(x => x.CheckedAtUtc)
                .HasColumnName("checkedAtUtc")
                .HasColumnType(DATETIME2)
                .IsRequired();
        }
    }
}
