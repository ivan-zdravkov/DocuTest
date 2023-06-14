using DocuTest.Shared.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IDocumentRepository
    {
        Task<Document> Get(IDbConnection connection, Guid documentId, IDataStrategy<Document> strategy, CancellationToken ct);
        Task<IEnumerable<Document>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, IDataStrategy<Document> strategy, CancellationToken ct);
        Task<Guid> Insert(IDbTransaction transaction, Document document, IDataStrategy<Document> strategy, CancellationToken ct);
        Task Update(IDbTransaction transaction, Document document, IDataStrategy<Document> strategy, CancellationToken ct);
        Task Delete(IDbTransaction transaction, Guid documentId, IDataStrategy<Document> strategy, CancellationToken ct);
    }
}
