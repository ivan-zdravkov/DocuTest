using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> Get(IDbConnection connection, Guid documentId, CancellationToken ct);
        Task<IEnumerable<Document>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct);
        Task<Guid> Insert(IDbTransaction transaction, Document document, CancellationToken ct);
        Task Update(IDbTransaction transaction, Document document, CancellationToken ct);
        Task Delete(IDbTransaction transaction, Guid documentId, CancellationToken ct);
    }
}
