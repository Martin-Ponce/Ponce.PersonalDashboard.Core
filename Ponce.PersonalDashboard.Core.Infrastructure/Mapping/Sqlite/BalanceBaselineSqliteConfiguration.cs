using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for BalanceBaseline. One row per user, deleted with the user.</summary>
    internal class BalanceBaselineSqliteConfiguration : IEntityTypeConfiguration<BalanceBaseline>
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
                .HasColumnType(TEXT)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.InitialBalance)
                .HasColumnName("initial_balance")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.AsOfMonth)
                .HasColumnName("as_of_month")
                .HasColumnType(INTEGER)
                .IsRequired(false);

            builder.Property(x => x.AsOfYear)
                .HasColumnName("as_of_year")
                .HasColumnType(INTEGER)
                .IsRequired(false);

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasColumnType(TEXT)
                .IsRequired();
        }
    }
}
