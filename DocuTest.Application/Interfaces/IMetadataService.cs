using DocuTest.Shared.Models;

namespace DocuTest.Application.Interfaces
{
    public interface IMetadataService
    {
        Task Insert(Metadata metadata);
        Task Update(Metadata metadata);
        Task Delete(Metadata metadata);
        Task Delete(Guid fileId, string key);
    }
}
