using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for DailyGoal, plus its weekly goal and check-ins (both deleted with it).</summary>
    internal class DailyGoalSqlServerConfiguration : IEntityTypeConfiguration<DailyGoal>
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
                .HasColumnType(UNIQUEIDENTIFIER)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("userId")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(GoalLimits.MAX_LENGTH_TITLE)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasColumnType(DECIMAL)
                .HasPrecision(GoalLimits.AMOUNT_PRECISION, GoalLimits.AMOUNT_SCALE)
                .IsRequired();

            builder.Property(x => x.Unit)
                .HasColumnName("unit")
                .HasColumnType(INT)
                .IsRequired();

            // Stored as a JSON array of DayOfWeek values.
            builder.PrimitiveCollection(x => x.PlannedDays)
                .HasColumnName("plannedDays")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(GoalLimits.MAX_LENGTH_PLANNED_DAYS)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("createdAtUtc")
                .HasColumnType(DATETIME2)
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
