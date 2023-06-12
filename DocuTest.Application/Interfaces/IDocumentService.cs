using DocuTest.Shared.Models;

namespace DocuTest.Application.Interfaces
{
    public interface IDocumentService
    {
        Task<Document> Get(Guid documentId);
        Task<IEnumerable<Document>> Get(IEnumerable<Guid> documentIds);
        Task<IEnumerable<Document>> GetByMetadata(string key, string value);
        Task<Guid> Insert(Document document);
        Task Update(Document document);
        Task Delete(Guid documentId);
    }
}
