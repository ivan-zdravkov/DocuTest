using DocuTest.Data.Main.DAL.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Factories
{
    public class SqlConnectionFactory : IDbConnectionFactory
    {
        private readonly string connectionString;

        public SqlConnectionFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IDbConnection Create() => new SqlConnection(this.connectionString); 
    }
}
