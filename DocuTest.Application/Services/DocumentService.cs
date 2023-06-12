using DocuTest.Application.Interfaces;
using DocuTest.Shared.Models;

namespace DocuTest.Application.Services
{
    public class DocumentService : IDocumentService
    {
        public async Task<Document> Get(Guid documentId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Document>> GetByMetadata(string key, string value)
        {
            throw new NotImplementedException();
        }

        public async Task<Guid> Insert(Document document)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Document document)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(Guid documentId)
        {
            throw new NotImplementedException();
        }
    }
}
