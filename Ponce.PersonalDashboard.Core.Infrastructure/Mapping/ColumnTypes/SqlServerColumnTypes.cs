namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes
{
    /// <summary>SQL Server store type names used by the mappings. Add new ones here instead of letting EF infer them.</summary>
    internal static class SqlServerColumnTypes
    {
        public const string NVARCHAR = "nvarchar";
        public const string VARCHAR = "varchar";
        public const string BIT = "bit";
        public const string DATETIME2 = "datetime2";
        public const string UNIQUEIDENTIFIER = "uniqueidentifier";
        public const string DECIMAL = "decimal";
        public const string SMALLINT = "smallint";
        public const string INT = "int";
        public const string DATE = "date";
        public const string TIME = "time";
    }
}
