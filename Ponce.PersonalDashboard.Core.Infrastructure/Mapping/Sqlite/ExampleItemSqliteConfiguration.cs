using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.Example;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqliteColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Sqlite
{
    /// <summary>SQLite mapping for ExampleItem.</summary>
    internal class ExampleItemSqliteConfiguration : IEntityTypeConfiguration<ExampleItem>
    {
        public void Configure(EntityTypeBuilder<ExampleItem> builder)
        {
            builder.ToTable("ExampleItems")
                .HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(TEXT)
                .IsRequired();

            builder.Property(x => x.Name)
                .HasColumnName("name")
                .HasColumnType(TEXT)
                .HasMaxLength(ExampleItemLimits.MAX_LENGTH_NAME)
                .IsRequired();

            builder.Property(x => x.Description)
                .HasColumnName("description")
                .HasColumnType(TEXT)
                .HasMaxLength(ExampleItemLimits.MAX_LENGTH_DESCRIPTION)
                .IsRequired(false);
        }
    }
}
