using System.Data;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IDbConnectionFactory
    {
        IDbConnection Create();
    }
}
