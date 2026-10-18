using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for Transaction.</summary>
    internal class TransactionSqlServerConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions")
                .HasKey(x => x.Id);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.Year, x.Month });

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.Month)
                .HasColumnName("month")
                .HasColumnType(SMALLINT)
                .IsRequired();

            builder.Property(x => x.Year)
                .HasColumnName("year")
                .HasColumnType(SMALLINT)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("userId")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.Type)
                .HasColumnName("type")
                .HasColumnType(INT)
                .IsRequired();

            builder.Property(x => x.CategoryId)
                .HasColumnName("categoryId")
                .HasColumnType(INT)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(TransactionLimits.MAX_LENGTH_DESCRIPTION)
                .IsRequired();

            builder.Property(x => x.Amount)
                .HasColumnName("amount")
                .HasColumnType(DECIMAL)
                .HasPrecision(TransactionLimits.AMOUNT_PRECISION, TransactionLimits.AMOUNT_SCALE)
                .IsRequired();

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("createdAtUtc")
                .HasColumnType(DATETIME2)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updatedAtUtc")
                .HasColumnType(DATETIME2)
                .IsRequired();
        }
    }
}
