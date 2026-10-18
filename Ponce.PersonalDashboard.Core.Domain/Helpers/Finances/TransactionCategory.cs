namespace Ponce.PersonalDashboard.Core.Domain.Helpers.Finances
{
    /// <summary>Transaction category. Each one belongs to one type, and they're numbered in the frontend's display order.</summary>
    public enum TransactionCategory
    {
        // Expenses
        Housing = 1,
        Groceries = 2,
        EatingOut = 3,
        Transport = 4,
        Subscriptions = 5,
        Shopping = 6,
        Gifts = 7,
        Travel = 8,
        Electronics = 9,
        // Income
        Freelance = 10,
        Sales = 11,
        Refunds = 12,
        GiftsReceived = 13
    }
}
