using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.PostgresColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.Postgres
{
    /// <summary>PostgreSQL mapping for JobApplication.</summary>
    internal class JobApplicationPostgresConfiguration : IEntityTypeConfiguration<JobApplication>
    {
        public void Configure(EntityTypeBuilder<JobApplication> builder)
        {
            builder.ToTable("JobApplications")
                .HasKey(x => x.Id);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.UserId, x.DateApplied });

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .HasColumnType(UUID)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("user_id")
                .HasColumnType(UUID)
                .IsRequired();

            builder.Property(x => x.CompanyName)
                .HasColumnName("company_name")
                .HasColumnType(VARCHAR)
                .HasMaxLength(JobApplicationLimits.MAX_LENGTH_COMPANY_NAME)
                .IsRequired();

            builder.Property(x => x.DateApplied)
                .HasColumnName("date_applied")
                .HasColumnType(DATE)
                .IsRequired();

            builder.Property(x => x.Salary)
                .HasColumnName("salary")
                .HasColumnType(NUMERIC)
                .HasPrecision(JobApplicationLimits.SALARY_PRECISION, JobApplicationLimits.SALARY_SCALE)
                .IsRequired(false);

            builder.Property(x => x.JobTitle)
                .HasColumnName("job_title")
                .HasColumnType(VARCHAR)
                .HasMaxLength(JobApplicationLimits.MAX_LENGTH_JOB_TITLE)
                .IsRequired();

            builder.Property(x => x.City)
                .HasColumnName("city")
                .HasColumnType(VARCHAR)
                .HasMaxLength(JobApplicationLimits.MAX_LENGTH_CITY)
                .IsRequired(false);

            builder.Property(x => x.WorkMode)
                .HasColumnName("work_mode")
                .HasColumnType(INTEGER)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType(INTEGER)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updated_at_utc")
                .HasColumnType(TIMESTAMPTZ)
                .IsRequired();
        }
    }
}
