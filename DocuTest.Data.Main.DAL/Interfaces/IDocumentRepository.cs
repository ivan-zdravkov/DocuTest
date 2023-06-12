using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> Get(SqlConnection connection, Guid documentId);
        Task<IEnumerable<Document>> Get(SqlConnection connection, IEnumerable<Guid> documentIds);
        Task<Guid> Insert(DbTransaction transaction, Document document);
        Task Update(DbTransaction transaction, Document document);
        Task Delete(DbTransaction transaction, Guid documentId);
    }
}
