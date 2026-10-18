using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for WeeklyGoal. The relationship lives in the DailyGoal mapping.</summary>
    internal class WeeklyGoalSqliteConfiguration : IEntityTypeConfiguration<WeeklyGoal>
    {
        public void Configure(EntityTypeBuilder<WeeklyGoal> builder)
        {
            builder.ToTable("WeeklyGoals")
                .HasKey(x => x.Id);

            builder.HasIndex(x => x.DailyGoalId)
                .IsUnique();

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(TEXT)
                .ValueGeneratedNever() // Id is set in code; without this EF treats a new weekly goal as already existing
                .IsRequired();

            builder.Property(x => x.DailyGoalId)
                .HasColumnName("daily_goal_id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType(TEXT)
                .HasMaxLength(GoalLimits.MAX_LENGTH_TITLE)
                .IsRequired(false);

            builder.Property(x => x.TargetCount)
                .HasColumnName("target_count")
                .HasColumnType(INTEGER)
                .IsRequired();
        }
    }
}
