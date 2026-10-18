using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres
{
    /// <summary>PostgreSQL mapping for BalanceBaseline. One row per user, deleted with the user.</summary>
    internal class BalanceBaselinePostgresConfiguration : IEntityTypeConfiguration<BalanceBaseline>
    {
        public void Configure(EntityTypeBuilder<BalanceBaseline> builder)
        {
            builder.ToTable("BalanceBaselines")
                .HasKey(x => x.Id);

            builder.HasIndex(x => x.UserId)
                .IsUnique();

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

            builder.Property(x => x.InitialBalance)
                .HasColumnName("initial_balance")
                .HasColumnType(NUMERIC)
                .HasPrecision(TransactionLimits.AMOUNT_PRECISION, TransactionLimits.AMOUNT_SCALE)
                .IsRequired();

            builder.Property(x => x.AsOfMonth)
                .HasColumnName("as_of_month")
                .HasColumnType(SMALLINT)
                .IsRequired(false);

            builder.Property(x => x.AsOfYear)
                .HasColumnName("as_of_year")
                .HasColumnType(SMALLINT)
                .IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasColumnType(TIMESTAMPTZ)
                .IsRequired();
        }
    }
}
