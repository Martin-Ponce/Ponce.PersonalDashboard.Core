using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication
{
    /// <summary>A job you applied to and where the process is at.</summary>
    public class JobApplication : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public string CompanyName { get; private set; } = default!;
        public DateOnly DateApplied { get; private set; }
        /// <summary>Null when unknown.</summary>
        public decimal? Salary { get; private set; }
        public string JobTitle { get; private set; } = default!;
        /// <summary>Null for remote jobs or when unknown.</summary>
        public string? City { get; private set; }
        public WorkMode WorkMode { get; private set; }
        public JobApplicationStatus Status { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private JobApplication() { } // Required by EF Core

        public JobApplication(Guid userId, string companyName, DateOnly dateApplied, decimal? salary, string jobTitle, string? city, WorkMode workMode, JobApplicationStatus status)
        {
            Id = Guid.NewGuid();
            UserId = userId.ValidateUserId();
            SetDetails(companyName, dateApplied, salary, jobTitle, city, workMode, status);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public void Update(string companyName, DateOnly dateApplied, decimal? salary, string jobTitle, string? city, WorkMode workMode, JobApplicationStatus status)
        {
            SetDetails(companyName, dateApplied, salary, jobTitle, city, workMode, status);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        private void SetDetails(string companyName, DateOnly dateApplied, decimal? salary, string jobTitle, string? city, WorkMode workMode, JobApplicationStatus status)
        {
            CompanyName = companyName.ValidateCompanyName();
            DateApplied = dateApplied.ValidateDateApplied();
            Salary = salary.ValidateSalary();
            JobTitle = jobTitle.ValidateJobTitle();
            WorkMode = workMode.ValidateWorkMode();
            City = WorkMode == WorkMode.Remote ? null : city.ValidateCity();
            Status = status.ValidateStatus();
        }
    }
}
