using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> Get(SqlConnection connection, Guid documentId, CancellationToken ct);
        Task<IEnumerable<Document>> Get(SqlConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct);
        Task<Guid> Insert(DbTransaction transaction, Document document, CancellationToken ct);
        Task Update(DbTransaction transaction, Document document, CancellationToken ct);
        Task Delete(DbTransaction transaction, Guid documentId, CancellationToken ct);
    }
}
