using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres
{
    /// <summary>PostgreSQL mapping for GoalCheckIn. One check-in per goal per day; the relationship lives in the DailyGoal mapping.</summary>
    internal class GoalCheckInPostgresConfiguration : IEntityTypeConfiguration<GoalCheckIn>
    {
        public void Configure(EntityTypeBuilder<GoalCheckIn> builder)
        {
            builder.ToTable("GoalCheckIns")
                .HasKey(x => x.Id);

            builder.HasIndex(x => new { x.DailyGoalId, x.Date })
                .IsUnique();

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(UUID)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.DailyGoalId)
                .HasColumnName("daily_goal_id")
                .HasColumnType(UUID)
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("date")
                .HasColumnType(DATE)
                .IsRequired();

            builder.Property(x => x.CheckedAtUtc)
                .HasColumnName("checked_at_utc")
                .HasColumnType(TIMESTAMPTZ)
                .IsRequired();
        }
    }
}
