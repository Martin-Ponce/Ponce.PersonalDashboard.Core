using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Finances;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>Transactions, budgets and balance. Each month's summary gets recalculated on every transaction write.</summary>
    public class FinancesService(IFinancesRepository financesRepository, ICurrentUser currentUser, ILogger<FinancesService> logger)
    {
        private readonly IFinancesRepository _financesRepository = financesRepository;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<FinancesService> _logger = logger;

        public IQueryable<GetTransactionsResponse> GetTransactions(short month, short year) =>
            _financesRepository.QueryTransactions(_currentUser.UserId, month, year).ProjectToType<GetTransactionsResponse>();

        public Task<decimal> GetMonthTotal(short month, short year, CancellationToken ct = default) =>
            _financesRepository.GetMonthTotal(_currentUser.UserId, month, year, ct);

        public Task<decimal> GetYearTotal(short year, CancellationToken ct = default) =>
            _financesRepository.GetYearTotal(_currentUser.UserId, year, ct);

        public async Task<GetTransactionResponse> GetTransaction(Guid transactionId, CancellationToken ct = default)
        {
            var transaction = await GetTransactionOrThrow(transactionId, ct);
            return transaction.Adapt<GetTransactionResponse>();
        }

        public async Task<SaveTransactionResponse> SaveTransaction(SaveTransactionRequest request, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving transaction for {Month}/{Year}", request.Month, request.Year);
            var transaction = new Transaction(
                request.Month,
                request.Year,
                _currentUser.UserId,
                request.Type,
                request.CategoryId,
                request.Description,
                request.Amount);
            await _financesRepository.SaveTransaction(transaction, ct);
            await _financesRepository.SaveChanges(ct);
            await RefreshSummary(transaction.Month, transaction.Year, ct);
            _logger.LogInformation("Transaction {TransactionId} saved successfully", transaction.Id);
            return await ToSaveTransactionResponse(transaction, ct);
        }

        public async Task<SaveTransactionResponse> UpdateTransaction(Guid transactionId, UpdateTransactionRequest request, CancellationToken ct = default)
        {
            var transaction = await GetTransactionOrThrow(transactionId, ct);
            transaction.Update(request.Type, request.CategoryId, request.Description, request.Amount);
            await _financesRepository.SaveChanges(ct);
            await RefreshSummary(transaction.Month, transaction.Year, ct);
            _logger.LogInformation("Transaction {TransactionId} updated successfully", transaction.Id);
            return await ToSaveTransactionResponse(transaction, ct);
        }

        public async Task DeleteTransaction(Guid transactionId, CancellationToken ct = default)
        {
            var transaction = await GetTransactionOrThrow(transactionId, ct);
            _financesRepository.DeleteTransaction(transaction);
            await _financesRepository.SaveChanges(ct);
            await RefreshSummary(transaction.Month, transaction.Year, ct);
            _logger.LogInformation("Transaction {TransactionId} deleted successfully", transactionId);
        }

        /// <summary>Month totals and budget. If there's no stored summary it's calculated from the transactions.</summary>
        public async Task<MonthlySummaryResponse> GetMonthlySummary(short month, short year, CancellationToken ct = default)
        {
            month.ValidateMonth();
            year.ValidateYear();
            var summary = await _financesRepository.GetSummary(_currentUser.UserId, month, year, ct);
            MonthlySummaryResponse response;
            if (summary is not null)
            {
                response = summary.Adapt<MonthlySummaryResponse>();
            }
            else
            {
                var (income, expense) = await _financesRepository.GetMonthTotals(_currentUser.UserId, month, year, ct);
                response = new MonthlySummaryResponse { Month = month, Year = year, TotalIncome = income, TotalExpense = expense };
            }
            response.TotalBalance = await GetTotalBalanceAt(year, month, ct);
            return response;
        }

        public async Task<MonthlySummaryResponse> SetMonthlyBudget(short month, short year, SetMonthlyBudgetRequest request, CancellationToken ct = default)
        {
            var summary = await RefreshSummary(month, year, ct, save: false);
            summary.SetBudget(request.Budget);
            await _financesRepository.SaveChanges(ct);
            _logger.LogInformation("Budget of {Month}/{Year} set", month, year);
            var response = summary.Adapt<MonthlySummaryResponse>();
            response.TotalBalance = await GetTotalBalanceAt(year, month, ct);
            return response;
        }

        /// <summary>Opening balance plus every month up to the current one (future months don't count yet).</summary>
        public async Task<BalanceResponse> GetBalance(CancellationToken ct = default)
        {
            var baseline = await _financesRepository.GetBalanceBaseline(_currentUser.UserId, ct);
            return await ToBalanceResponse(baseline, ct);
        }

        public async Task<BalanceResponse> SetInitialBalance(SetInitialBalanceRequest request, CancellationToken ct = default)
        {
            var baseline = await _financesRepository.GetBalanceBaseline(_currentUser.UserId, ct);
            if (baseline is null)
            {
                baseline = new BalanceBaseline(_currentUser.UserId, request.InitialBalance, request.AsOfMonth, request.AsOfYear);
                await _financesRepository.SaveBalanceBaseline(baseline, ct);
            }
            else
            {
                baseline.Update(request.InitialBalance, request.AsOfMonth, request.AsOfYear);
            }
            await _financesRepository.SaveChanges(ct);
            _logger.LogInformation("Opening balance of user {UserId} set", _currentUser.UserId);
            return await ToBalanceResponse(baseline, ct);
        }

        private async Task<BalanceResponse> ToBalanceResponse(BalanceBaseline? baseline, CancellationToken ct)
        {
            var today = DateUtil.TodayUtc();
            var net = await _financesRepository.GetSummariesNet(
                _currentUser.UserId, baseline?.AsOfYear, baseline?.AsOfMonth, (short)today.Year, (short)today.Month, ct);
            var initialBalance = baseline?.InitialBalance ?? 0;
            return new BalanceResponse
            {
                InitialBalance = initialBalance,
                AsOfMonth = baseline?.AsOfMonth,
                AsOfYear = baseline?.AsOfYear,
                NetSinceStart = net,
                TotalBalance = initialBalance + net
            };
        }

        /// <summary>Balance at the end of the month, null if it's before the as-of month.</summary>
        private async Task<decimal?> GetTotalBalanceAt(short year, short month, CancellationToken ct)
        {
            var baseline = await _financesRepository.GetBalanceBaseline(_currentUser.UserId, ct);
            if (baseline?.AsOfYear is not null && baseline.AsOfMonth is not null &&
                year * 12 + month < baseline.AsOfYear.Value * 12 + baseline.AsOfMonth.Value)
                return null;
            var net = await _financesRepository.GetSummariesNet(_currentUser.UserId, baseline?.AsOfYear, baseline?.AsOfMonth, year, month, ct);
            return (baseline?.InitialBalance ?? 0) + net;
        }

        /// <summary>Recalculates the month's totals from its transactions, creating the summary if needed.</summary>
        private async Task<Summary> RefreshSummary(short month, short year, CancellationToken ct, bool save = true)
        {
            var (income, expense) = await _financesRepository.GetMonthTotals(_currentUser.UserId, month, year, ct);
            var summary = await _financesRepository.GetSummary(_currentUser.UserId, month, year, ct);
            if (summary is null)
            {
                summary = new Summary(month, year, _currentUser.UserId, income, expense, budget: 0);
                await _financesRepository.SaveSummary(summary, ct);
            }
            else
            {
                summary.UpdateTotals(income, expense);
            }
            if (save)
                await _financesRepository.SaveChanges(ct);
            return summary;
        }

        private async Task<SaveTransactionResponse> ToSaveTransactionResponse(Transaction transaction, CancellationToken ct) => new()
        {
            Transaction = transaction.Adapt<GetTransactionResponse>(),
            MonthTotal = await _financesRepository.GetMonthTotal(_currentUser.UserId, transaction.Month, transaction.Year, ct)
        };

        private async Task<Transaction> GetTransactionOrThrow(Guid transactionId, CancellationToken ct) =>
            await _financesRepository.GetTransactionById(transactionId, _currentUser.UserId, ct) ??
                throw new NotFoundException(
                    CustomExceptionCodes.TRANSACTION_NOT_FOUND,
                    $"Transaction with id {transactionId} doesn't exist",
                    new Dictionary<string, object> { { "TransactionId", transactionId } });
    }
}
