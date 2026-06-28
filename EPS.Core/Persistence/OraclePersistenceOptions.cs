using System;
using NHibernate.Dialect;

namespace app.Persistence
{
    public sealed class OraclePersistenceOptions
    {
        public OraclePersistenceOptions(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("Oracle connection string is required.", nameof(connectionString));
            }

            ConnectionString = connectionString;
        }

        public string ConnectionString { get; }

        public bool ShowSql { get; set; }

        public bool FormatSql { get; set; } = true;

        public string QuerySubstitutions { get; set; } = "true 1, false 0, yes 'Y', no 'N'";

        public bool UseNPrefixedTypesForUnicode { get; set; }

        public Type DialectType { get; set; } = typeof(Oracle12cDialect);
    }
}
