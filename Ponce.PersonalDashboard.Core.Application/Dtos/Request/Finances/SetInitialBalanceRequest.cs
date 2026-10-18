namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances
{
    public class SetInitialBalanceRequest
    {
        /// <summary>Can be negative.</summary>
        public required decimal InitialBalance { get; set; }
        /// <summary>Optional, sent with AsOfYear. Only this month and later ones count, leave both out to count every month.</summary>
        public short? AsOfMonth { get; set; }
        public short? AsOfYear { get; set; }
    }
}
