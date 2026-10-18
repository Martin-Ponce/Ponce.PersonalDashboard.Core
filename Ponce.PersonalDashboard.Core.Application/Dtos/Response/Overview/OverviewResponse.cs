using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Overview
{
    /// <summary>Everything the overview page shows for Date's month and week.</summary>
    public class OverviewResponse
    {
        /// <summary>The client's today.</summary>
        public DateOnly Date { get; set; }
        public required OverviewBalanceResponse Balance { get; set; }
        public required OverviewMonthResponse Month { get; set; }
        /// <summary>Oldest first, current month last.</summary>
        public required List<OverviewMonthlyTotalResponse> MonthlyTotals { get; set; }
        /// <summary>Next scheduled ones from Date on, soonest first.</summary>
        public required List<GetUpcomingDatesResponse> UpcomingDates { get; set; }
        public required OverviewApplicationsResponse Applications { get; set; }
    }

    public class OverviewBalanceResponse
    {
        public decimal TotalBalance { get; set; }
        public decimal MonthChange { get; set; }
    }

    public class OverviewMonthResponse
    {
        public short Year { get; set; }
        /// <summary>1-12</summary>
        public short Month { get; set; }
        public decimal Spent { get; set; }
        public decimal Earned { get; set; }
        /// <summary>Null when there's no budget.</summary>
        public decimal? Budget { get; set; }
        /// <summary>Highest first.</summary>
        public required List<OverviewCategoryResponse> Categories { get; set; }
    }

    public class OverviewCategoryResponse
    {
        public TransactionCategory Category { get; set; }
        public decimal Amount { get; set; }
    }

    public class OverviewMonthlyTotalResponse
    {
        public short Year { get; set; }
        /// <summary>1-12</summary>
        public short Month { get; set; }
        public decimal Spent { get; set; }
        /// <summary>Null when there's no budget.</summary>
        public decimal? Budget { get; set; }
    }

    public class OverviewApplicationsResponse
    {
        /// <summary>All time, every status is there (0 when none).</summary>
        public required Dictionary<JobApplicationStatus, int> StatusCounts { get; set; }
        /// <summary>Sent on Date or in the 6 days before.</summary>
        public int NewThisWeek { get; set; }
        /// <summary>Newest first.</summary>
        public required List<GetJobApplicationsResponse> Recent { get; set; }
    }
}
