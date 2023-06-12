using DocuTest.Shared.Models;

namespace DocuTest.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> Get(Guid documentId, CancellationToken ct);
        Task<IEnumerable<Document>> Get(IEnumerable<Guid> documentIds, CancellationToken ct);
        Task<IEnumerable<Document>> GetByMetadata(string key, string value, CancellationToken ct);
        Task<Guid> Insert(Document document, CancellationToken ct);
        Task Update(Document document, CancellationToken ct);
        Task Delete(Guid documentId, CancellationToken ct);
    }
}
