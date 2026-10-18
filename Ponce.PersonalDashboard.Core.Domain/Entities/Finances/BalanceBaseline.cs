using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Finances
{
    /// <summary>A user's opening balance (one per user). The total balance is this plus the monthly summaries, starting from AsOfMonth/AsOfYear when set.</summary>
    public class BalanceBaseline : Entity
    {
        public Guid UserId { get; private set; }
        /// <summary>Can be negative.</summary>
        public decimal InitialBalance { get; private set; }
        public short? AsOfMonth { get; private set; }
        public short? AsOfYear { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private BalanceBaseline() { } // Required by EF Core

        public BalanceBaseline(Guid userId, decimal initialBalance, short? asOfMonth, short? asOfYear)
        {
            UserId = userId.ValidateUserId();
            Update(initialBalance, asOfMonth, asOfYear);
        }

        public void Update(decimal initialBalance, short? asOfMonth, short? asOfYear)
        {
            InitialBalance = initialBalance.ValidateInitialBalance();
            (AsOfMonth, AsOfYear) = (asOfMonth, asOfYear).ValidateBalanceAsOf();
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
