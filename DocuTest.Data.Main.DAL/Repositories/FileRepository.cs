using DocuTest.Data.Main.DAL.Interfaces;
using System.Data;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class FileRepository : IFileRepository
    {
        public Task Delete(IDbTransaction transaction, Guid fileId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IDbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Shared.Models.File>> Get(IDbConnection connection, Guid documentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Shared.Models.File>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetDocumentIds(IDbConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetFileIds(IDbConnection connection, Guid documentId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<Guid> Insert(IDbTransaction transaction, Guid documentId, Shared.Models.File file, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Update(IDbTransaction transaction, Shared.Models.File file, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
