using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {
        public Task Delete(IDbTransaction transaction, Guid fileId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IDbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IDbTransaction transaction, Guid fileId, string key, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Delete(IDbTransaction transaction, Metadata metadata, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Metadata>> Get(IDbConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetFileIds(IDbConnection connection, string key, string value, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Insert(IDbTransaction transaction, Metadata metadata, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Insert(IDbTransaction transaction, IEnumerable<Metadata> metadata, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task Update(IDbTransaction transaction, Metadata metadata, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
