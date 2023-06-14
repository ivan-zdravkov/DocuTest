using DocuTest.Shared.Interfaces;

namespace DocuTest.Shared.Strategies
{
    public abstract class SqlStrategy<T> : IDataStrategy<T>
    {
        private const string TRUE = "1";
        private const string FALSE = "0";

        public string ColumnName { get; private set; }
        public (string Value, bool Take)[] Records { get; private set; }

        public SqlStrategy(string column, params (string Value, bool Take)[] records)
        {
            this.ColumnName = column;
            this.Records = records;
        }

        public string Expression() => @$"CASE [{this.ColumnName}] {ToSql(this.Records)} END = 1";

        public abstract bool Allows(T value);

        private string ToSql(bool value) => value ? TRUE : FALSE;

        private string ToSql(IEnumerable<(string Value, bool Take)> records) =>
            string.Join(" ", records.Select(record => $"WHEN '{record.Value}' THEN {ToSql(record.Take)}"));
    }
}
