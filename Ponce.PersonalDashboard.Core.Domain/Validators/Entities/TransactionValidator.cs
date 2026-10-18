using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Finances;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;

namespace Ponce.PersonalDashboard.Core.Domain.Validators.Entities
{
    public static class TransactionValidator
    {
        /// <summary>Trims, checks the length and capitalizes the first letter.</summary>
        public static string ValidateTransactionDescription(this string? description)
        {
            var trimmed = description?.Trim();
            trimmed.ValidateLength(
                TransactionLimits.MAX_LENGTH_DESCRIPTION,
                StringValidator.ExceptionWhenOutOfRangeArguments(nameof(description), TransactionLimits.MAX_LENGTH_DESCRIPTION),
                TransactionLimits.MIN_LENGTH_DESCRIPTION);
            return char.ToUpperInvariant(trimmed![0]) + trimmed[1..];
        }

        public static decimal ValidateBudget(this decimal budget)
        {
            if (budget < 0 || budget > TransactionLimits.MAX_AMOUNT)
                throw ValidationException.OutOfRange($"Argument '{nameof(budget)}' must be between 0 and {TransactionLimits.MAX_AMOUNT}");
            return budget;
        }

        public static decimal ValidateInitialBalance(this decimal initialBalance)
        {
            if (initialBalance < -TransactionLimits.MAX_AMOUNT || initialBalance > TransactionLimits.MAX_AMOUNT)
                throw ValidationException.OutOfRange($"Argument '{nameof(initialBalance)}' must be between -{TransactionLimits.MAX_AMOUNT} and {TransactionLimits.MAX_AMOUNT}");
            return initialBalance;
        }

        /// <summary>Month and year go together (or both null) and can't be after the current month.</summary>
        public static (short? Month, short? Year) ValidateBalanceAsOf(this (short? Month, short? Year) asOf)
        {
            if (asOf.Month is null && asOf.Year is null)
                return asOf;
            if (asOf.Month is null || asOf.Year is null)
                throw ValidationException.NullOrEmpty("Arguments 'asOfMonth' and 'asOfYear' must be sent together");
            var month = asOf.Month.Value.ValidateMonth();
            var year = asOf.Year.Value.ValidateYear();
            var today = DateTime.UtcNow;
            if (year * 12 + month > today.Year * 12 + today.Month)
                throw ValidationException.OutOfRange("The opening balance can't be dated after the current month");
            return (month, year);
        }

        public static decimal ValidateAmount(this decimal amount)
        {
            if (amount <= 0 || amount > TransactionLimits.MAX_AMOUNT)
                throw ValidationException.OutOfRange($"Argument '{nameof(amount)}' must be greater than 0 and at most {TransactionLimits.MAX_AMOUNT}");
            return amount;
        }

        public static short ValidateMonth(this short month)
        {
            if (month < 1 || month > 12)
                throw ValidationException.OutOfRange($"Argument '{nameof(month)}' must be between 1 and 12");
            return month;
        }

        public static short ValidateYear(this short year)
        {
            if (year < TransactionLimits.MIN_YEAR || year > TransactionLimits.MAX_YEAR)
                throw ValidationException.OutOfRange($"Argument '{nameof(year)}' must be between {TransactionLimits.MIN_YEAR} and {TransactionLimits.MAX_YEAR}");
            return year;
        }

        public static TransactionType ValidateType(this TransactionType type) =>
            Enum.IsDefined(type) ? type : throw ValidationException.NotSupported($"Transaction type '{type}' is not supported");

        public static TransactionCategory ValidateCategory(this TransactionCategory category) =>
            Enum.IsDefined(category) ? category : throw ValidationException.NotSupported($"Transaction category '{category}' is not supported");

        public static TransactionCategory ValidateCategoryOf(this TransactionCategory category, TransactionType type) =>
            category.ValidateCategory().TypeOf() == type
                ? category
                : throw ValidationException.NotSupported($"Transaction category '{category}' is not a '{type}' category");
    }
}
