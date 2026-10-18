using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for BalanceBaseline. One row per user, deleted with the user.</summary>
    internal class BalanceBaselineSqlServerConfiguration : IEntityTypeConfiguration<BalanceBaseline>
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
                .HasColumnType(UNIQUEIDENTIFIER)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("userId")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.InitialBalance)
                .HasColumnName("initialBalance")
                .HasColumnType(DECIMAL)
                .HasPrecision(TransactionLimits.AMOUNT_PRECISION, TransactionLimits.AMOUNT_SCALE)
                .IsRequired();

            builder.Property(x => x.AsOfMonth)
                .HasColumnName("asOfMonth")
                .HasColumnType(SMALLINT)
                .IsRequired(false);

            builder.Property(x => x.AsOfYear)
                .HasColumnName("asOfYear")
                .HasColumnType(SMALLINT)
                .IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updatedAtUtc")
                .HasColumnType(DATETIME2)
                .IsRequired();
        }
    }
}
