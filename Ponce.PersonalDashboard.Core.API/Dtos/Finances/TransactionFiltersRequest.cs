using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;

namespace Ponce.PersonalDashboard.Core.API.Dtos.Finances
{
    /// <summary>Extra filters for the transactions list. Empty means any, repeat a param for several values.</summary>
    public class TransactionFiltersRequest
    {
        [FromQuery]
        public TransactionType[]? Types { get; set; }
        [FromQuery]
        public TransactionCategory[]? Categories { get; set; }
        [FromQuery]
        public decimal? MinAmount { get; set; }
        [FromQuery]
        public decimal? MaxAmount { get; set; }
    }
}
