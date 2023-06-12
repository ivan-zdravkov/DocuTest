using System.Data;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IFileRepository
    {
        Task<IEnumerable<Shared.Models.File>> Get(IDbConnection connection, Guid documentId, CancellationToken ct);
        Task<IEnumerable<Shared.Models.File>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct);
        Task<IEnumerable<Guid>> GetDocumentIds(IDbConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct);
        Task<IEnumerable<Guid>> GetFileIds(IDbConnection connection, Guid documentId, CancellationToken ct);
        Task<Guid> Insert(IDbTransaction transaction, Guid documentId, Shared.Models.File file, CancellationToken ct);
        Task Update(IDbTransaction transaction, Shared.Models.File file, CancellationToken ct);
        Task Delete(IDbTransaction transaction, Guid fileId, CancellationToken ct);
        Task Delete(IDbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct);
    }
}
