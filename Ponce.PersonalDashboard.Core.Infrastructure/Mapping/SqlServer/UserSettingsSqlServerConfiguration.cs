using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for UserSettings. One row per user, deleted with the user.</summary>
    internal class UserSettingsSqlServerConfiguration : IEntityTypeConfiguration<UserSettings>
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
                .HasColumnType(UNIQUEIDENTIFIER)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("userId")
                .HasColumnType(UNIQUEIDENTIFIER)
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
                .HasColumnName("dateFormat")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_DATE_FORMAT)
                .IsRequired();

            builder.Property(x => x.Currency)
                .HasColumnName("currency")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.LENGTH_CURRENCY)
                .IsRequired();

            builder.Property(x => x.ShowDecimals)
                .HasColumnName("showDecimals")
                .HasColumnType(BIT)
                .IsRequired();

            builder.Property(x => x.AutoTimeZone)
                .HasColumnName("autoTimeZone")
                .HasColumnType(BIT)
                .IsRequired();

            builder.Property(x => x.TimeZone)
                .HasColumnName("timeZone")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_TIME_ZONE)
                .IsRequired();

            builder.Property(x => x.ProfilePicture)
                .HasColumnName("profilePicture")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_PROFILE_PICTURE)
                .IsRequired();

            builder.Property(x => x.ShowSummaryWidget)
                .HasColumnName("showSummaryWidget")
                .HasColumnType(BIT)
                .IsRequired();

            builder.Property(x => x.ShowSpendingWidget)
                .HasColumnName("showSpendingWidget")
                .HasColumnType(BIT)
                .IsRequired();

            builder.Property(x => x.ShowDatesWidget)
                .HasColumnName("showDatesWidget")
                .HasColumnType(BIT)
                .IsRequired();

            builder.Property(x => x.ShowApplicationsWidget)
                .HasColumnName("showApplicationsWidget")
                .HasColumnType(BIT)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updatedAtUtc")
                .HasColumnType(DATETIME2)
                .IsRequired();
        }
    }
}
