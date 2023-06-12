using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IMetadataRepository
    {
        Task<IEnumerable<Metadata>> Get(SqlConnection connection, IEnumerable<Guid> fileIds);
        Task<IEnumerable<Guid>> GetFileIds(SqlConnection connection, string key, string value);
        Task Insert(DbTransaction transaction, Metadata metadata);
        Task Insert(DbTransaction transaction, IEnumerable<Metadata> metadata);
        Task Update(DbTransaction transaction, Metadata metadata);
        Task Delete(DbTransaction transaction, Guid fileId);
        Task Delete(DbTransaction transaction, IEnumerable<Guid> fileIds);
        Task Delete(DbTransaction transaction, Guid fileId, string key);
        Task Delete(DbTransaction transaction, Metadata metadata);
    }
}
