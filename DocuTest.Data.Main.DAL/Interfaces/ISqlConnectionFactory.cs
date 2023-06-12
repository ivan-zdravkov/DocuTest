using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface ISqlConnectionFactory
    {
        SqlConnection Create();
    }
}
