using DocuTest.Shared.Models;

namespace DocuTest.Application.Interfaces
{
    public interface IDocumentService
    {
        public Task<Document> Get(Guid documentId);
        public Task<IEnumerable<Document>> GetByMetadata(string key, string value);
        public Task<Guid> Insert(Document document);
        public Task Update(Document document);
        public Task Delete(Guid documentId);
    }
}
