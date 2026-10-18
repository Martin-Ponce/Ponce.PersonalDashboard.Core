using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances
{
    public class SaveTransactionRequest
    {
        public short Month { get; set; }
        public short Year { get; set; }
        public TransactionType Type { get; set; }
        public TransactionCategory CategoryId { get; set; }
        public required string Description { get; set; }
        public decimal Amount { get; set; }
    }
}
