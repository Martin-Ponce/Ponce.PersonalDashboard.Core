using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances
{
    public class GetTransactionsResponse
    {
        public Guid Id { get; set; }
        public short Month { get; set; }
        public short Year { get; set; }
        public TransactionType Type { get; set; }
        public TransactionCategory CategoryId { get; set; }
        public required string Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
