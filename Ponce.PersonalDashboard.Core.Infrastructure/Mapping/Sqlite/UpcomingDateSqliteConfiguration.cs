using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.UpcomingDate;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for UpcomingDate.</summary>
    internal class UpcomingDateSqliteConfiguration : IEntityTypeConfiguration<UpcomingDate>
    {
        public void Configure(EntityTypeBuilder<UpcomingDate> builder)
        {
            builder.ToTable("UpcomingDates")
                .HasKey(x => x.Id);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.Date, x.Time });

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Date)
                .HasColumnName("date")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Time)
                .HasColumnName("time")
                .HasColumnType(TEXT)
                .IsRequired(false);

            builder.Property(x => x.Title)
                .HasColumnName("title")
                .HasColumnType(TEXT)
                .HasMaxLength(UpcomingDateLimits.MAX_LENGTH_TITLE)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType(TEXT)
                .HasMaxLength(UpcomingDateLimits.MAX_LENGTH_DESCRIPTION)
                .IsRequired(false);

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType(INTEGER)
                .IsRequired();
        }
    }
}
