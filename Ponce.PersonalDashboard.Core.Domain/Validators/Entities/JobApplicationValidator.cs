using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;

namespace Ponce.PersonalDashboard.Core.Domain.Validators.Entities
{
    public static class JobApplicationValidator
    {
        public static string ValidateCompanyName(this string? companyName) =>
            companyName.ValidateTrimmedText(nameof(companyName), JobApplicationLimits.MAX_LENGTH_COMPANY_NAME);

        public static string ValidateJobTitle(this string? jobTitle) =>
            jobTitle.ValidateTrimmedText(nameof(jobTitle), JobApplicationLimits.MAX_LENGTH_JOB_TITLE);

        public static string? ValidateCity(this string? city) =>
            string.IsNullOrWhiteSpace(city) ? null : city.ValidateTrimmedText(nameof(city), JobApplicationLimits.MAX_LENGTH_CITY);

        public static DateOnly ValidateDateApplied(this DateOnly dateApplied)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            if (dateApplied < JobApplicationLimits.MIN_DATE_APPLIED || dateApplied > today)
                throw ValidationException.OutOfRange($"Argument '{nameof(dateApplied)}' must be between {JobApplicationLimits.MIN_DATE_APPLIED:yyyy-MM-dd} and today");
            return dateApplied;
        }

        public static decimal? ValidateSalary(this decimal? salary)
        {
            if (salary is not null && (salary <= 0 || salary > JobApplicationLimits.MAX_SALARY))
                throw ValidationException.OutOfRange($"Argument '{nameof(salary)}' must be greater than 0 and at most {JobApplicationLimits.MAX_SALARY}");
            return salary;
        }

        public static WorkMode ValidateWorkMode(this WorkMode workMode) =>
            Enum.IsDefined(workMode) ? workMode : throw ValidationException.NotSupported($"Work mode '{workMode}' is not supported");

        public static JobApplicationStatus ValidateStatus(this JobApplicationStatus status) =>
            Enum.IsDefined(status) ? status : throw ValidationException.NotSupported($"Job application status '{status}' is not supported");

        private static string ValidateTrimmedText(this string? value, string argumentName, int maxLength) =>
            value?.Trim().ValidateLength(
                maxLength,
                StringValidator.ExceptionWhenOutOfRangeArguments(argumentName, maxLength),
                JobApplicationLimits.MIN_LENGTH_TEXT)
            ?? throw StringValidator.ExceptionWhenNullArgument(argumentName);
    }
}
