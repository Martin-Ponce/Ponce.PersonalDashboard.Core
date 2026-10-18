using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ponce.PersonalDashboard.Core.API.Dtos.Finances;
using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class GetTransactions
    {
        public const string ENDPOINT_NAME = "Get transactions by month and year";

        public static RouteGroupBuilder MapGetTransactions(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("transactions",
                async (
                    HttpContext context,
                    FinancesService financesService,
                    [AsParameters] PaginationRequest request,
                    [AsParameters] TransactionFiltersRequest filters,
                    [FromQuery] short month,
                    [FromQuery] short year,
                    CancellationToken ct) =>
                {
                    var transactions = financesService.GetTransactions(month, year);
                    var monthCount = await transactions.CountAsync(ct);
                    var matching = ApplyFilters(transactions, request.Filter, filters);
                    var page = await Operation(matching, request);
                    context.AddPaginationHeader(page);
                    var total = await financesService.GetMonthTotal(month, year, ct);
                    var filteredTotal = await GetNetTotal(matching, ct);
                    return Results.Ok(new GetTransactionsPageResponse(page, total, monthCount, filteredTotal));
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GetTransactionsPageResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets the transactions of the specified month and year (optionally filtered by description, types, categories and amount range), plus the month's net total (income minus expenses), its transaction count and the net total of the matching transactions.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }

        // Ties broken by creation date so pages stay stable.
        private static IReadOnlyDictionary<string, IOrderedQueryable<GetTransactionsResponse>> GetTransactionsSorts(IQueryable<GetTransactionsResponse> query) => new Dictionary<string, IOrderedQueryable<GetTransactionsResponse>>()
        {
            { "description", query.OrderBy(a => a.Description).ThenBy(a => a.CreatedAtUtc) },
            { "description_desc", query.OrderByDescending(a => a.Description).ThenByDescending(a => a.CreatedAtUtc) },
            { "createdat", query.OrderBy(a => a.CreatedAtUtc) },
            { "createdat_desc", query.OrderByDescending(a => a.CreatedAtUtc) },
            { "updatedat", query.OrderBy(a => a.UpdatedAtUtc).ThenBy(a => a.CreatedAtUtc) },
            { "updatedat_desc", query.OrderByDescending(a => a.UpdatedAtUtc).ThenByDescending(a => a.CreatedAtUtc) },
            { "amount", query.OrderBy(a => a.Amount).ThenBy(a => a.CreatedAtUtc) },
            { "amount_desc", query.OrderByDescending(a => a.Amount).ThenByDescending(a => a.CreatedAtUtc) },
            { "category", query.OrderBy(a => a.CategoryId).ThenBy(a => a.CreatedAtUtc) },
            { "category_desc", query.OrderByDescending(a => a.CategoryId).ThenByDescending(a => a.CreatedAtUtc) },
        };

        private static IQueryable<GetTransactionsResponse> ApplyFilters(IQueryable<GetTransactionsResponse> query, string? descriptionFilter, TransactionFiltersRequest filters)
        {
            if (!string.IsNullOrEmpty(descriptionFilter))
            {
                var filter = descriptionFilter.ToLower();
                query = query.Where(a => a.Description.ToLower().Contains(filter));
            }
            if (filters.Types is { Length: > 0 } types)
                query = query.Where(a => types.Contains(a.Type));
            if (filters.Categories is { Length: > 0 } categories)
                query = query.Where(a => categories.Contains(a.CategoryId));
            if (filters.MinAmount is decimal minAmount)
                query = query.Where(a => a.Amount >= minAmount);
            if (filters.MaxAmount is decimal maxAmount)
                query = query.Where(a => a.Amount <= maxAmount);
            return query;
        }

        private static async Task<PagedList<GetTransactionsResponse>> Operation(IQueryable<GetTransactionsResponse> query, PaginationRequest paginationRequest)
        {
            var sort = string.IsNullOrEmpty(paginationRequest.Sort) ? "createdat" : paginationRequest.Sort;
            if (!GetTransactionsSorts(query).TryGetValue(sort, out var sortedQuery))
            {
                throw ValidationException.NotSupported("An unsupported sort parameter was received");
            }
            return await PagedList<GetTransactionsResponse>.ToPagedList(sortedQuery, paginationRequest);
        }

        // Summed in memory since SQLite can't aggregate decimals.
        private static async Task<decimal> GetNetTotal(IQueryable<GetTransactionsResponse> query, CancellationToken ct)
        {
            var amounts = await query.Select(a => new { a.Type, a.Amount }).ToListAsync(ct);
            return amounts.Sum(a => a.Type == TransactionType.Expense ? -a.Amount : a.Amount);
        }
    }
}
