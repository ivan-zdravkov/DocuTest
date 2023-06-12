using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IFileRepository
    {
        Task<Shared.Models.File> Get(SqlConnection connection, Guid documentId, CancellationToken ct);
        Task<IEnumerable<Shared.Models.File>> Get(SqlConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct);
        Task<IEnumerable<Guid>> GetDocumentIds(SqlConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct);
        Task<IEnumerable<Guid>> GetFileIds(SqlConnection connection, Guid documentId, CancellationToken ct);
        Task<Guid> Insert(DbTransaction transaction, Guid documentId, Shared.Models.File file, CancellationToken ct);
        Task Update(DbTransaction transaction, Shared.Models.File file, CancellationToken ct);
        Task Delete(DbTransaction transaction, Guid fileId, CancellationToken ct);
        Task Delete(DbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct);
    }
}
