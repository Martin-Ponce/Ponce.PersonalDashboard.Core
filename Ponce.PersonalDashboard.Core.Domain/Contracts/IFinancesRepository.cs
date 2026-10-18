using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    /// <summary>Data access for transactions, monthly summaries and the balance baseline.</summary>
    public interface IFinancesRepository : IRepository
    {
        IQueryable<Transaction> QueryTransactions(Guid userId, short month, short year);
        Task<Transaction?> GetTransactionById(Guid transactionId, Guid userId, CancellationToken ct = default);
        Task SaveTransaction(Transaction transaction, CancellationToken ct = default);
        void DeleteTransaction(Transaction transaction);
        Task<decimal> GetMonthTotal(Guid userId, short month, short year, CancellationToken ct = default);
        Task<decimal> GetYearTotal(Guid userId, short year, CancellationToken ct = default);
        /// <summary>Month's income and expenses, both as positive numbers.</summary>
        Task<(decimal Income, decimal Expense)> GetMonthTotals(Guid userId, short month, short year, CancellationToken ct = default);
        /// <summary>Month's expenses per category, highest first. Empty categories are skipped.</summary>
        Task<List<(TransactionCategory Category, decimal Amount)>> GetMonthExpensesByCategory(Guid userId, short month, short year, CancellationToken ct = default);

        Task<Summary?> GetSummary(Guid userId, short month, short year, CancellationToken ct = default);
        Task SaveSummary(Summary summary, CancellationToken ct = default);
        /// <summary>Summaries in the range, both ends included.</summary>
        Task<List<Summary>> GetSummaries(Guid userId, short fromYear, short fromMonth, short toYear, short toMonth, CancellationToken ct = default);
        /// <summary>Income minus expenses over the range (inclusive). No lower bound if "from" is null.</summary>
        Task<decimal> GetSummariesNet(Guid userId, short? fromYear, short? fromMonth, short toYear, short toMonth, CancellationToken ct = default);

        Task<BalanceBaseline?> GetBalanceBaseline(Guid userId, CancellationToken ct = default);
        Task SaveBalanceBaseline(BalanceBaseline baseline, CancellationToken ct = default);
    }
}
