namespace Ponce.PersonalDashboard.Core.Domain
{
    public static class DomainConstants
    {
        /// <summary>Source labels for config errors.</summary>
        public static class ExceptionSources
        {
            public const string APPSETTINGS = "Configuration file";
            public const string DATABASE = "Database";
            public const string CONNECTION = "Connection";
        }

        /// <summary>Error codes sent to API clients. Keep them unique within each group.</summary>
        public static class CustomExceptionCodes
        {
            #region Not Found
            public const int EXAMPLEITEM_NOT_FOUND = 1;
            public const int TRANSACTION_NOT_FOUND = 2;
            public const int JOBAPPLICATION_NOT_FOUND = 3;
            public const int UPCOMINGDATE_NOT_FOUND = 4;
            public const int DAILYGOAL_NOT_FOUND = 5;
            public const int USER_NOT_FOUND = 6;
            #endregion

            #region Invalid Operation
            #endregion
        }

        public static class Values
        {
            public const int MIN_LENGTH_VALUE = 3;

            public static class ExampleItemLimits
            {
                public const int MIN_LENGTH_NAME = MIN_LENGTH_VALUE;
                public const int MAX_LENGTH_NAME = 128;
                public const int MAX_LENGTH_DESCRIPTION = 512;
            }

            public static class TransactionLimits
            {
                public const int MIN_LENGTH_DESCRIPTION = 1;
                public const int MAX_LENGTH_DESCRIPTION = 256;
                public const int AMOUNT_PRECISION = 18;
                public const int AMOUNT_SCALE = 2;
                public const decimal MAX_AMOUNT = 9_999_999_999_999_999.99m; // Biggest value that fits decimal(18, 2)
                public const short MIN_YEAR = 2000;
                public const short MAX_YEAR = 2100;
            }

            public static class JobApplicationLimits
            {
                public const int MIN_LENGTH_TEXT = 1;
                public const int MAX_LENGTH_COMPANY_NAME = 128;
                public const int MAX_LENGTH_JOB_TITLE = 128;
                public const int MAX_LENGTH_CITY = 128;
                public const int SALARY_PRECISION = 18;
                public const int SALARY_SCALE = 2;
                public const decimal MAX_SALARY = 9_999_999_999_999_999.99m; // Biggest value that fits decimal(18, 2)
                public static readonly DateOnly MIN_DATE_APPLIED = new(2000, 1, 1);
            }

            public static class UpcomingDateLimits
            {
                public const int MIN_LENGTH_TITLE = 1;
                public const int MAX_LENGTH_TITLE = 128;
                public const int MAX_LENGTH_DESCRIPTION = 512;
                public static readonly DateOnly MIN_DATE = new(2000, 1, 1);
                public static readonly DateOnly MAX_DATE = new(2100, 12, 31);
            }

            public static class GoalLimits
            {
                public const int MIN_LENGTH_TITLE = 1;
                public const int MAX_LENGTH_TITLE = 128;
                public const int AMOUNT_PRECISION = 6;
                public const int AMOUNT_SCALE = 2;
                public const decimal MAX_AMOUNT = 9_999.99m; // Biggest value that fits decimal(6, 2)
                public const int MAX_LENGTH_PLANNED_DAYS = 32; // JSON array of up to 7 days, e.g. "[1,2,3,4,5,6,0]"
                public const int MIN_WEEKLY_TARGET = 1;
                public const int MAX_WEEKLY_TARGET = 7; // One check-in per day max
                public const DayOfWeek WEEK_START = DayOfWeek.Monday; // Weekly progress resets on this day
            }

            public static class UserLimits
            {
                public const int MIN_LENGTH_USER_NAME = 1;
                public const int MAX_LENGTH_USER_NAME = 256;
                public const int MAX_LENGTH_DISPLAY_NAME = 128;
                // Settings column sizes
                public const int MAX_LENGTH_THEME = 32;
                public const int MAX_LENGTH_LANGUAGE = 8;
                public const int MAX_LENGTH_DATE_FORMAT = 16;
                public const int LENGTH_CURRENCY = 3; // ISO 4217
                public const int MAX_LENGTH_TIME_ZONE = 64;
                public const int MAX_LENGTH_PROFILE_PICTURE = 16;
                /// <summary>IANA zone id, e.g. "UTC", "Europe/Madrid", "Etc/GMT+5".</summary>
                public const string TIME_ZONE_PATTERN = @"^[A-Za-z][A-Za-z0-9_+-]*(/[A-Za-z0-9_+-]+)*$";
            }
            /// <summary>How many items of each module the overview shows.</summary>
            public static class OverviewLimits
            {
                public const int MONTHLY_TOTALS = 6; // Months in the spending chart, current one last
                public const int UPCOMING_DATES = 5;
                public const int RECENT_APPLICATIONS = 5;
                public const int NEW_APPLICATIONS_DAYS = 7; // "New this week" = today plus the 6 days before
            }
        }
    }
}
