using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IMetadataRepository
    {
        Task<IEnumerable<Metadata>> Get(SqlConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct);
        Task<IEnumerable<Guid>> GetFileIds(SqlConnection connection, string key, string value, CancellationToken ct);
        Task Insert(DbTransaction transaction, Metadata metadata, CancellationToken ct);
        Task Insert(DbTransaction transaction, IEnumerable<Metadata> metadata, CancellationToken ct);
        Task Update(DbTransaction transaction, Metadata metadata, CancellationToken ct);
        Task Delete(DbTransaction transaction, Guid fileId, CancellationToken ct);
        Task Delete(DbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct);
        Task Delete(DbTransaction transaction, Guid fileId, string key, CancellationToken ct);
        Task Delete(DbTransaction transaction, Metadata metadata, CancellationToken ct);
    }
}
