using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres
{
    /// <summary>PostgreSQL mapping for UserSettings. One row per user, deleted with the user.</summary>
    internal class UserSettingsPostgresConfiguration : IEntityTypeConfiguration<UserSettings>
    {
        public void Configure(EntityTypeBuilder<UserSettings> builder)
        {
            builder.ToTable("UserSettings")
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

            builder.Property(x => x.Theme)
                .HasColumnName("theme")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_THEME)
                .IsRequired();

            builder.Property(x => x.Language)
                .HasColumnName("language")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_LANGUAGE)
                .IsRequired();

            builder.Property(x => x.DateFormat)
                .HasColumnName("date_format")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_DATE_FORMAT)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasColumnName("currency")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.LENGTH_CURRENCY)
                .IsRequired();

            builder.Property(x => x.ShowDecimals)
                .HasColumnName("show_decimals")
                .HasColumnType(BOOLEAN)
                .IsRequired();

            builder.Property(x => x.AutoTimeZone)
                .HasColumnName("auto_time_zone")
                .HasColumnType(BOOLEAN)
                .IsRequired();

            builder.Property(x => x.TimeZone)
                .HasColumnName("time_zone")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_TIME_ZONE)
                .IsRequired();

            builder.Property(x => x.ProfilePicture)
                .HasColumnName("profile_picture")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_PROFILE_PICTURE)
                .IsRequired();

            builder.Property(x => x.ShowSummaryWidget)
                .HasColumnName("show_summary_widget")
                .HasColumnType(BOOLEAN)
                .IsRequired();

            builder.Property(x => x.ShowSpendingWidget)
                .HasColumnName("show_spending_widget")
                .HasColumnType(BOOLEAN)
                .IsRequired();

            builder.Property(x => x.ShowDatesWidget)
                .HasColumnName("show_dates_widget")
                .HasColumnType(BOOLEAN)
                .IsRequired();

            builder.Property(x => x.ShowApplicationsWidget)
                .HasColumnName("show_applications_widget")
                .HasColumnType(BOOLEAN)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasColumnType(TIMESTAMPTZ)
                .IsRequired();
        }
    }
}
