using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Interfaces
{
    public interface IFileRepository
    {
        Task<Shared.Models.File> Get(SqlConnection connection, Guid documentId);
        Task<IEnumerable<Shared.Models.File>> Get(SqlConnection connection, IEnumerable<Guid> documentIds);
        Task<IEnumerable<Guid>> GetDocumentIds(SqlConnection connection, IEnumerable<Guid> fileIds);
        Task<IEnumerable<Guid>> GetFileIds(SqlConnection connection, Guid documentId);
        Task<Guid> Insert(DbTransaction transaction, Guid documentId, Shared.Models.File file);
        Task Update(DbTransaction transaction, Shared.Models.File file);
        Task Delete(DbTransaction transaction, Guid fileId);
        Task Delete(DbTransaction transaction, IEnumerable<Guid> fileIds);
    }
}
