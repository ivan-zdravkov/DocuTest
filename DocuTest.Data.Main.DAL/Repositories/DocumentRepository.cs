using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public Task Delete(IDbTransaction transaction, Guid documentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Document> Get(IDbConnection connection, Guid documentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Document>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Insert(IDbTransaction transaction, Document document, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Update(IDbTransaction transaction, Document document, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
