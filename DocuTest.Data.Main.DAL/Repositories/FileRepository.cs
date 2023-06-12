using DocuTest.Data.Main.DAL.Interfaces;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class FileRepository : IFileRepository
    {
        public Task Delete(DbTransaction transaction, Guid fileId)
        {
            throw new NotImplementedException();
        }

        public Task Delete(DbTransaction transaction, IEnumerable<Guid> fileIds)
        {
            throw new NotImplementedException();
        }

        public Task<Shared.Models.File> Get(SqlConnection connection, Guid documentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Shared.Models.File>> Get(SqlConnection connection, IEnumerable<Guid> documentIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetDocumentIds(SqlConnection connection, IEnumerable<Guid> fileIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetFileIds(SqlConnection connection, Guid documentId)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Insert(DbTransaction transaction, Guid documentId, Shared.Models.File file)
        {
            throw new NotImplementedException();
        }

        public Task Update(DbTransaction transaction, Shared.Models.File file)
        {
            throw new NotImplementedException();
        }
    }
}
