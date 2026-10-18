using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Finances
{
    /// <summary>A single income or expense in a given month.</summary>
    public class Transaction : AggregateRoot
    {
        public short Month { get; private set; }
        public short Year { get; private set; }
        public Guid UserId { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionCategory CategoryId { get; private set; }
        public string Description { get; private set; } = default!;
        public decimal Amount { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private Transaction() { } // Required by EF Core

        public Transaction(short month, short year, Guid userId, TransactionType type, TransactionCategory categoryId, string description, decimal amount)
        {
            Id = Guid.NewGuid();
            Month = month.ValidateMonth();
            Year = year.ValidateYear();
            UserId = userId.ValidateUserId();
            SetDetails(type, categoryId, description, amount);
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
        }

        public void Update(TransactionType type, TransactionCategory categoryId, string description, decimal amount)
        {
            SetDetails(type, categoryId, description, amount);
            UpdatedAtUtc = DateTime.UtcNow;
        }

        public decimal SignedAmount() => Type == TransactionType.Expense ? -Amount : Amount;

        private void SetDetails(TransactionType type, TransactionCategory categoryId, string description, decimal amount)
        {
            Type = type.ValidateType();
            CategoryId = categoryId.ValidateCategoryOf(Type);
            Description = description.ValidateTransactionDescription();
            Amount = amount.ValidateAmount();
        }
    }
}
