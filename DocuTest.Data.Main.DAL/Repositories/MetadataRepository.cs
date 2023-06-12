using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {
        public Task Delete(DbTransaction transaction, Guid fileId)
        {
            throw new NotImplementedException();
        }

        public Task Delete(DbTransaction transaction, IEnumerable<Guid> fileIds)
        {
            throw new NotImplementedException();
        }

        public Task Delete(DbTransaction transaction, Guid fileId, string key)
        {
            throw new NotImplementedException();
        }

        public Task Delete(DbTransaction transaction, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Metadata>> Get(SqlConnection connection, IEnumerable<Guid> fileIds)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Guid>> GetFileIds(SqlConnection connection, string key, string value)
        {
            throw new NotImplementedException();
        }

        public Task Insert(DbTransaction transaction, Metadata metadata)
        {
            throw new NotImplementedException();
        }

        public Task Insert(DbTransaction transaction, IEnumerable<Metadata> metadata)
        {
            throw new NotImplementedException();
        }

        public Task Update(DbTransaction transaction, Metadata metadata)
        {
            throw new NotImplementedException();
        }
    }
}
