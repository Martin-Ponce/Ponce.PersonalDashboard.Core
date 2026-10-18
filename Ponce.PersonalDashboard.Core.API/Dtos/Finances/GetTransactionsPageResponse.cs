using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;

namespace Ponce.PersonalDashboard.Core.API.Dtos.Finances
{
    /// <summary>A page of a month's transactions plus the month's totals (computed over every page, not just this one).</summary>
    public record GetTransactionsPageResponse(PagedList<GetTransactionsResponse> Transactions, decimal Total, int MonthCount, decimal FilteredTotal);
}
