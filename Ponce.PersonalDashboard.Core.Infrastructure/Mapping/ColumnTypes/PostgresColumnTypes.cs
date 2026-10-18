namespace Ponce.PersonalDashboard.Core.Infrastructure.Mapping.ColumnTypes
{
    /// <summary>PostgreSQL store type names used by the mappings. Add new ones here instead of letting EF infer them.</summary>
    internal static class PostgresColumnTypes
    {
        public const string VARCHAR = "varchar";
        public const string BOOLEAN = "boolean";
        public const string TIMESTAMPTZ = "timestamptz";
        public const string UUID = "uuid";
        public const string NUMERIC = "numeric";
        public const string SMALLINT = "smallint";
        public const string INTEGER = "integer";
        public const string INTEGER_ARRAY = "integer[]";
        public const string DATE = "date";
        public const string TIME = "time";
    }
}
