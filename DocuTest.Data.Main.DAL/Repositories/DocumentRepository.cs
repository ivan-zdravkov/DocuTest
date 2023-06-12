using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public Task Delete(DbTransaction transaction, Guid documentId)
        {
            throw new NotImplementedException();
        }

        public Task<Document> Get(SqlConnection connection, Guid documentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Document>> Get(SqlConnection connection, IEnumerable<Guid> documentIds)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Insert(DbTransaction transaction, Document document)
        {
            throw new NotImplementedException();
        }

        public Task Update(DbTransaction transaction, Document document)
        {
            throw new NotImplementedException();
        }
    }
}
