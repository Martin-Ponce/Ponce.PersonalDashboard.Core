using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;
using static Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes.SqlServerColumnTypes;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.SqlServer
{
    /// <summary>SQL Server mapping for JobApplication.</summary>
    internal class JobApplicationSqlServerConfiguration : IEntityTypeConfiguration<JobApplication>
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
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.UserId)
                .HasColumnName("userId")
                .HasColumnType(UNIQUEIDENTIFIER)
                .IsRequired();

            builder.Property(x => x.CompanyName)
                .HasColumnName("companyName")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(JobApplicationLimits.MAX_LENGTH_COMPANY_NAME)
                .IsRequired();

            builder.Property(x => x.DateApplied)
                .HasColumnName("dateApplied")
                .HasColumnType(DATE)
                .IsRequired();

            builder.Property(x => x.Salary)
                .HasColumnName("salary")
                .HasColumnType(DECIMAL)
                .HasPrecision(JobApplicationLimits.SALARY_PRECISION, JobApplicationLimits.SALARY_SCALE)
                .IsRequired(false);

            builder.Property(x => x.JobTitle)
                .HasColumnName("jobTitle")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(JobApplicationLimits.MAX_LENGTH_JOB_TITLE)
                .IsRequired();

            builder.Property(x => x.City)
                .HasColumnName("city")
                .HasColumnType(NVARCHAR)
                .HasMaxLength(JobApplicationLimits.MAX_LENGTH_CITY)
                .IsRequired(false);

            builder.Property(x => x.WorkMode)
                .HasColumnName("workMode")
                .HasColumnType(INT)
                .IsRequired();

            builder.Property(x => x.Status)
                .HasColumnName("status")
                .HasColumnType(INT)
                .IsRequired();

            builder.Property(x => x.UpdatedAtUtc)
                .HasColumnName("updatedAtUtc")
                .HasColumnType(DATETIME2)
                .IsRequired();
        }
    }
}
