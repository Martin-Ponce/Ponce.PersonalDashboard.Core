using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres
{
    /// <summary>PostgreSQL mapping for DailyGoal, plus its weekly goal and check-ins (both deleted with it).</summary>
    internal class DailyGoalPostgresConfiguration : IEntityTypeConfiguration<DailyGoal>
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
                .HasColumnType(UUID)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(UUID)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType(VARCHAR)
                .HasMaxLength(GoalLimits.MAX_LENGTH_TITLE)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasColumnType(NUMERIC)
                .HasPrecision(GoalLimits.AMOUNT_PRECISION, GoalLimits.AMOUNT_SCALE)
                .IsRequired();

            builder.Property(x => x.Unit)
                .HasColumnName("unit")
                .HasColumnType(INTEGER)
                .IsRequired();

            // Stored as a native integer[] of DayOfWeek values.
            builder.PrimitiveCollection(x => x.PlannedDays)
                .HasColumnName("planned_days")
                .HasColumnType(INTEGER_ARRAY)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasColumnType(TIMESTAMPTZ)
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
