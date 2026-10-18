using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for DailyGoal, plus its weekly goal and check-ins (both deleted with it).</summary>
    internal class DailyGoalSqliteConfiguration : IEntityTypeConfiguration<DailyGoal>
    {
        public void Configure(EntityTypeBuilder<DailyGoal> builder)
        {
            builder.ToTable("DailyGoals")
                .HasKey(x => x.Id);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(TEXT)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType(TEXT)
                .HasMaxLength(GoalLimits.MAX_LENGTH_TITLE)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Unit)
                .HasColumnName("unit")
                .HasColumnType(INTEGER)
                .IsRequired();

            // Stored as a JSON array of DayOfWeek values.
            builder.PrimitiveCollection(x => x.PlannedDays)
                .HasColumnName("planned_days")
                .HasColumnType(TEXT)
                .HasMaxLength(GoalLimits.MAX_LENGTH_PLANNED_DAYS)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.HasOne(x => x.WeeklyGoal)
                .WithOne()
                .HasForeignKey<WeeklyGoal>(x => x.DailyGoalId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CheckIns)
                .WithOne()
                .HasForeignKey(x => x.DailyGoalId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
