using Mapster;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Overview;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>Builds the overview page from finances, upcoming dates and job applications in one call.</summary>
    public class OverviewService(
        FinancesService financesService,
        IFinancesRepository financesRepository,
        IUpcomingDateRepository upcomingDateRepository,
        IJobApplicationRepository jobApplicationRepository,
        ICurrentUser currentUser)
    {
        private readonly FinancesService _financesService = financesService;
        private readonly IFinancesRepository _financesRepository = financesRepository;
        private readonly IUpcomingDateRepository _upcomingDateRepository = upcomingDateRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository = jobApplicationRepository;
        private readonly ICurrentUser _currentUser = currentUser;

        public async Task<OverviewResponse> GetOverview(DateOnly date, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;
            var (year, month) = ((short)date.Year, (short)date.Month);

            // One at a time, they share the same DbContext.
            var balance = await _financesService.GetBalance(ct);
            var summary = await _financesService.GetMonthlySummary(month, year, ct);
            var categories = await _financesRepository.GetMonthExpensesByCategory(userId, month, year, ct);
            var monthlyTotals = await GetMonthlyTotals(userId, date, ct);
            var upcomingDates = await _upcomingDateRepository.GetNextScheduled(userId, date, OverviewLimits.UPCOMING_DATES, ct);
            var statusCounts = await _jobApplicationRepository.CountByStatus(userId, ct);
            var newThisWeek = await _jobApplicationRepository.CountAppliedSince(
                userId, date.AddDays(1 - OverviewLimits.NEW_APPLICATIONS_DAYS), ct);
            var recentApplications = await _jobApplicationRepository.GetMostRecent(userId, OverviewLimits.RECENT_APPLICATIONS, ct);

            return new OverviewResponse
            {
                Date = date,
                Balance = new OverviewBalanceResponse { TotalBalance = balance.TotalBalance, MonthChange = summary.NetBalance },
                Month = new OverviewMonthResponse
                {
                    Year = year,
                    Month = month,
                    Spent = summary.TotalExpense,
                    Earned = summary.TotalIncome,
                    Budget = summary.HasBudget ? summary.Budget : null,
                    Categories = [.. categories.Select(x => new OverviewCategoryResponse { Category = x.Category, Amount = x.Amount })]
                },
                MonthlyTotals = monthlyTotals,
                UpcomingDates = upcomingDates.Adapt<List<GetUpcomingDatesResponse>>(),
                Applications = new OverviewApplicationsResponse
                {
                    StatusCounts = Enum.GetValues<JobApplicationStatus>()
                        .ToDictionary(status => status, status => statusCounts.GetValueOrDefault(status)),
                    NewThisWeek = newThisWeek,
                    Recent = recentApplications.Adapt<List<GetJobApplicationsResponse>>()
                }
            };
        }

        /// <summary>Last few months up to date's, oldest first. No summary means no transactions.</summary>
        private async Task<List<OverviewMonthlyTotalResponse>> GetMonthlyTotals(Guid userId, DateOnly date, CancellationToken ct)
        {
            var firstMonth = new DateOnly(date.Year, date.Month, 1).AddMonths(1 - OverviewLimits.MONTHLY_TOTALS);
            var summaries = await _financesRepository.GetSummaries(
                userId, (short)firstMonth.Year, (short)firstMonth.Month, (short)date.Year, (short)date.Month, ct);
            return [.. Enumerable.Range(0, OverviewLimits.MONTHLY_TOTALS).Select(offset =>
            {
                var day = firstMonth.AddMonths(offset);
                var summary = summaries.FirstOrDefault(x => x.Year == day.Year && x.Month == day.Month);
                return new OverviewMonthlyTotalResponse
                {
                    Year = (short)day.Year,
                    Month = (short)day.Month,
                    Spent = summary?.TotalExpense ?? 0,
                    Budget = summary is { Budget: > 0 } ? summary.Budget : null
                };
            })];
        }
    }
}
