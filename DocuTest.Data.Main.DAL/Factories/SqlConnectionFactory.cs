using DocuTest.Data.Main.DAL.Interfaces;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Factories
{
    public class SqlConnectionFactory : ISqlConnectionFactory
    {
        private readonly string connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public SqlConnection Create() => new SqlConnection(this.connectionString); 
    }
}
