namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances
{
    /// <summary>The saved transaction plus its month's updated net total.</summary>
    public class SaveTransactionResponse
    {
        public required GetTransactionResponse Transaction { get; set; }
        public decimal MonthTotal { get; set; }
    }
}
