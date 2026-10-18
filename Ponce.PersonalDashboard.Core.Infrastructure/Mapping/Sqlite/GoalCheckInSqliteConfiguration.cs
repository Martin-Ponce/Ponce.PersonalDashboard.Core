using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for GoalCheckIn. One check-in per goal per day; the relationship lives in the DailyGoal mapping.</summary>
    internal class GoalCheckInSqliteConfiguration : IEntityTypeConfiguration<GoalCheckIn>
    {
        public void Configure(EntityTypeBuilder<GoalCheckIn> builder)
        {
            builder.ToTable("GoalCheckIns")
                .HasKey(x => x.Id);

            builder.HasIndex(x => new { x.DailyGoalId, x.Date })
                .IsUnique();

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(TEXT)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.DailyGoalId)
                .HasColumnName("daily_goal_id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("date")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.CheckedAtUtc)
                .HasColumnName("checked_at_utc")
                .HasColumnType(TEXT)
                .IsRequired();
        }
    }
}
