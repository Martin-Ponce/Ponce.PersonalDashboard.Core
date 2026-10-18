namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances
{
    public class BalanceResponse
    {
        /// <summary>0 when no opening balance was set.</summary>
        public decimal InitialBalance { get; set; }
        public short? AsOfMonth { get; set; }
        public short? AsOfYear { get; set; }
        /// <summary>Net of every month from the as-of month up to the current one.</summary>
        public decimal NetSinceStart { get; set; }
        public decimal TotalBalance { get; set; }
    }
}
