using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    public class FinancesRepository(AppDbContext context, ILogger<FinancesRepository> logger)
        : Repository<AppDbContext>(context), IFinancesRepository
    {
        private readonly ILogger<FinancesRepository> _logger = logger;

        public IQueryable<Transaction> QueryTransactions(Guid userId, short month, short year)
        {
            _logger.LogDebug("Querying transactions for {Month}/{Year}", month, year);
            return _context.Transactions.Where(x => x.UserId == userId && x.Month == month && x.Year == year);
        }

        public async Task<Transaction?> GetTransactionById(Guid transactionId, Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching transaction with id {TransactionId}", transactionId);
            return await _context.Transactions.FirstOrDefaultAsync(x => x.Id == transactionId && x.UserId == userId, ct);
        }

        public async Task SaveTransaction(Transaction transaction, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving transaction with id {TransactionId}", transaction.Id);
            await _context.Transactions.AddAsync(transaction, ct);
        }

        public void DeleteTransaction(Transaction transaction)
        {
            _logger.LogDebug("Deleting transaction with id {TransactionId}", transaction.Id);
            _context.Transactions.Remove(transaction);
        }

        public Task<decimal> GetMonthTotal(Guid userId, short month, short year, CancellationToken ct = default) =>
            GetNetTotal(_context.Transactions.Where(x => x.UserId == userId && x.Month == month && x.Year == year), ct);

        public Task<decimal> GetYearTotal(Guid userId, short year, CancellationToken ct = default) =>
            GetNetTotal(_context.Transactions.Where(x => x.UserId == userId && x.Year == year), ct);

        public async Task<(decimal Income, decimal Expense)> GetMonthTotals(Guid userId, short month, short year, CancellationToken ct = default)
        {
            var amounts = await LoadAmounts(_context.Transactions.Where(x => x.UserId == userId && x.Month == month && x.Year == year), ct);
            return (
                amounts.Where(x => x.Type == TransactionType.Income).Sum(x => x.Amount),
                amounts.Where(x => x.Type == TransactionType.Expense).Sum(x => x.Amount));
        }

        public async Task<List<(TransactionCategory Category, decimal Amount)>> GetMonthExpensesByCategory(Guid userId, short month, short year, CancellationToken ct = default)
        {
            _logger.LogDebug("Summing expenses per category of {Month}/{Year}", month, year);
            // Summed in memory since SQLite can't aggregate decimals.
            var expenses = await _context.Transactions
                .Where(x => x.UserId == userId && x.Month == month && x.Year == year && x.Type == TransactionType.Expense)
                .Select(x => new { x.CategoryId, x.Amount })
                .ToListAsync(ct);
            return [.. expenses
                .GroupBy(x => x.CategoryId)
                .Select(g => (g.Key, g.Sum(x => x.Amount)))
                .OrderByDescending(x => x.Item2)];
        }

        public async Task<Summary?> GetSummary(Guid userId, short month, short year, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching summary of {Month}/{Year}", month, year);
            return await _context.Summaries.FirstOrDefaultAsync(x => x.UserId == userId && x.Month == month && x.Year == year, ct);
        }

        public async Task SaveSummary(Summary summary, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving summary of {Month}/{Year}", summary.Month, summary.Year);
            await _context.Summaries.AddAsync(summary, ct);
        }

        public async Task<List<Summary>> GetSummaries(Guid userId, short fromYear, short fromMonth, short toYear, short toMonth, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching summaries of user {UserId} from {FromMonth}/{FromYear} to {ToMonth}/{ToYear}", userId, fromMonth, fromYear, toMonth, toYear);
            // year * 12 + month so the range can span years.
            var from = fromYear * 12 + fromMonth;
            var to = toYear * 12 + toMonth;
            return await _context.Summaries
                .Where(x => x.UserId == userId && x.Year * 12 + x.Month >= from && x.Year * 12 + x.Month <= to)
                .ToListAsync(ct);
        }

        public async Task<decimal> GetSummariesNet(Guid userId, short? fromYear, short? fromMonth, short toYear, short toMonth, CancellationToken ct = default)
        {
            _logger.LogDebug("Summing summaries of user {UserId} up to {Month}/{Year}", userId, toMonth, toYear);
            // year * 12 + month so the range can span years.
            var to = toYear * 12 + toMonth;
            var query = _context.Summaries.Where(x => x.UserId == userId && x.Year * 12 + x.Month <= to);
            if (fromYear is not null && fromMonth is not null)
            {
                var from = fromYear.Value * 12 + fromMonth.Value;
                query = query.Where(x => x.Year * 12 + x.Month >= from);
            }
            // Summed in memory since SQLite can't aggregate decimals.
            var totals = await query.Select(x => new { x.TotalIncome, x.TotalExpense }).ToListAsync(ct);
            return totals.Sum(x => x.TotalIncome - x.TotalExpense);
        }

        public async Task<BalanceBaseline?> GetBalanceBaseline(Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching balance baseline of user {UserId}", userId);
            return await _context.BalanceBaselines.FirstOrDefaultAsync(x => x.UserId == userId, ct);
        }

        public async Task SaveBalanceBaseline(BalanceBaseline baseline, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving balance baseline of user {UserId}", baseline.UserId);
            await _context.BalanceBaselines.AddAsync(baseline, ct);
        }

        private static async Task<decimal> GetNetTotal(IQueryable<Transaction> query, CancellationToken ct)
        {
            var amounts = await LoadAmounts(query, ct);
            return amounts.Sum(x => x.Type == TransactionType.Expense ? -x.Amount : x.Amount);
        }

        // Sums happen in memory since SQLite can't aggregate decimals.
        private static async Task<List<(TransactionType Type, decimal Amount)>> LoadAmounts(IQueryable<Transaction> query, CancellationToken ct)
        {
            var amounts = await query.Select(x => new { x.Type, x.Amount }).ToListAsync(ct);
            return [.. amounts.Select(x => (x.Type, x.Amount))];
        }
    }
}
