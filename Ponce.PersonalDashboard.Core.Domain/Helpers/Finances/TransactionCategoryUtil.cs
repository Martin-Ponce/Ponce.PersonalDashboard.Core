namespace Ponce.PersonalDashboard.Core.Domain.Helpers.Finances
{
    public static class TransactionCategoryUtil
    {
        public static TransactionType TypeOf(this TransactionCategory category) => category switch
        {
            TransactionCategory.Freelance or
            TransactionCategory.Sales or
            TransactionCategory.Refunds or
            TransactionCategory.GiftsReceived => TransactionType.Income,
            _ => TransactionType.Expense
        };
    }
}
