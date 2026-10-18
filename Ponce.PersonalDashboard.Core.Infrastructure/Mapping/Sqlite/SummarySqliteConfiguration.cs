using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for Summary.</summary>
    internal class SummarySqliteConfiguration : IEntityTypeConfiguration<Summary>
    {
        public void Configure(EntityTypeBuilder<Summary> builder)
        {
            builder.ToTable("Summaries")
                .HasKey(x => x.Id);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.Year, x.Month })
                .IsUnique();

            builder.Ignore(x => x.NetBalance);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Month)
                .HasColumnName("month")
                .HasColumnType(INTEGER)
                .IsRequired();

            builder.Property(x => x.Year)
                .HasColumnName("year")
                .HasColumnType(INTEGER)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.TotalIncome)
                .HasColumnName("total_income")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.TotalExpense)
                .HasColumnName("total_expense")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Budget)
                .HasColumnName("budget")
                .HasColumnType(TEXT)
                .IsRequired();
        }
    }
}
