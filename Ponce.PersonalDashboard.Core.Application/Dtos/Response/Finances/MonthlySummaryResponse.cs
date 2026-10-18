namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances
{
    public class MonthlySummaryResponse
    {
        public short Month { get; set; }
        public short Year { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetBalance => TotalIncome - TotalExpense;
        /// <summary>0 means no budget.</summary>
        public decimal Budget { get; set; }
        public bool HasBudget => Budget > 0;
        /// <summary>Negative when over budget, null when there's no budget.</summary>
        public decimal? RemainingBudget => HasBudget ? Budget - TotalExpense : null;
        public bool IsOverBudget => HasBudget && TotalExpense > Budget;
        /// <summary>Balance at the end of this month, null if it's before the opening balance's as-of month.</summary>
        public decimal? TotalBalance { get; set; }
    }
}
