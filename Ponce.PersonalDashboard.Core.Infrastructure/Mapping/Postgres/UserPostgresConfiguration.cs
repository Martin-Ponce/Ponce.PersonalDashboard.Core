using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres
{
    /// <summary>PostgreSQL mapping for User. The id comes from the identity provider's sub claim, never generated here.</summary>
    internal class UserPostgresConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users")
                .HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(UUID)
                .ValueGeneratedNever()
                .IsRequired();

            builder.Property(x => x.UserName)
                .HasColumnName("user_name")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_USER_NAME)
                .IsRequired();

            builder.Property(x => x.DisplayName)
                .HasColumnName("display_name")
                .HasColumnType(VARCHAR)
                .HasMaxLength(UserLimits.MAX_LENGTH_DISPLAY_NAME)
                .IsRequired(false);

            builder.Property(x => x.CreatedAtUtc)
                .HasColumnName("created_at_utc")
                .HasColumnType(TIMESTAMPTZ)
                .IsRequired();
        }
    }
}
