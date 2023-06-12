using DocuTest.Shared.Models;

namespace DocuTest.Application.Interfaces
{
    public interface IMetadataService
    {
        Task Insert(Metadata metadata, CancellationToken ct);
        Task Update(Metadata metadata, CancellationToken ct);
        Task Delete(Metadata metadata, CancellationToken ct);
        Task Delete(Guid fileId, string key, CancellationToken ct);
    }
}
