namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances
{
    public class SetMonthlyBudgetRequest
    {
        /// <summary>0 removes the budget.</summary>
        public decimal Budget { get; set; }
    }
}
