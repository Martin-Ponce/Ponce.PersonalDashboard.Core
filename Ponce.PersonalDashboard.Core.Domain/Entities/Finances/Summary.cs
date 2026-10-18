using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Finances
{
    /// <summary>A user's income and expense totals for one month.</summary>
    public class Summary : AggregateRoot
    {
        public short Month { get; private set; }
        public short Year { get; private set; }
        public Guid UserId { get; private set; }
        public decimal TotalIncome { get; private set; }
        public decimal TotalExpense { get; private set; }
        /// <summary>0 means no budget.</summary>
        public decimal Budget { get; private set; }
        public decimal NetBalance => TotalIncome - TotalExpense;

        private Summary() { } // Required by EF Core

        public Summary(short month, short year, Guid userId, decimal totalIncome, decimal totalExpense, decimal budget)
        {
            Id = Guid.NewGuid();
            Month = month.ValidateMonth();
            Year = year.ValidateYear();
            UserId = userId.ValidateUserId();
            UpdateTotals(totalIncome, totalExpense);
            SetBudget(budget);
        }

        public void SetBudget(decimal budget) => Budget = budget.ValidateBudget();

        public void UpdateTotals(decimal totalIncome, decimal totalExpense)
        {
            TotalIncome = totalIncome;
            TotalExpense = totalExpense;
        }
    }
}
