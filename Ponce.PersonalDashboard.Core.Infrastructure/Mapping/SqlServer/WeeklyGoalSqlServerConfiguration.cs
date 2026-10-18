using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for WeeklyGoal. The relationship lives in the DailyGoal mapping.</summary>
    internal class WeeklyGoalSqlServerConfiguration : IEntityTypeConfiguration<WeeklyGoal>
    {
        public void Configure(EntityTypeBuilder<WeeklyGoal> builder)
        {
            builder.ToTable("WeeklyGoals")
                .HasKey(x => x.Id);

            builder.HasIndex(x => x.DailyGoalId)
                .IsUnique();

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(UNIQUEIDENTIFIER)
                .ValueGeneratedNever() // Id is set in code; without this EF treats a new weekly goal as already existing
                .IsRequired();

            builder.Property(x => x.DailyGoalId)
                .HasColumnName("dailyGoalId")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(GoalLimits.MAX_LENGTH_TITLE)
                .IsRequired(false);

            builder.Property(x => x.TargetCount)
                .HasColumnName("targetCount")
                .HasColumnType(INT)
                .IsRequired();
        }
    }
}
