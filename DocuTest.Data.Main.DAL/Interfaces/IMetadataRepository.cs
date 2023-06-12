using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IMetadataRepository
    {
        Task<IEnumerable<Metadata>> Get(IDbConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct);
        Task<IEnumerable<Guid>> GetFileIds(IDbConnection connection, string key, string value, CancellationToken ct);
        Task Insert(IDbTransaction transaction, Metadata metadata, CancellationToken ct);
        Task Insert(IDbTransaction transaction, IEnumerable<Metadata> metadata, CancellationToken ct);
        Task Update(IDbTransaction transaction, Metadata metadata, CancellationToken ct);
        Task Delete(IDbTransaction transaction, Guid fileId, CancellationToken ct);
        Task Delete(IDbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct);
        Task Delete(IDbTransaction transaction, Guid fileId, string key, CancellationToken ct);
        Task Delete(IDbTransaction transaction, Metadata metadata, CancellationToken ct);
    }
}
