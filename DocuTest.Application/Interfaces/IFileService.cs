namespace DocuTest.Application.Interfaces
{
    public interface IFileService
    {
        Task<Guid> Insert(Guid documentId, Shared.Models.File file);
        Task Update(Shared.Models.File file);
        Task Delete(Guid fileId);
    }
}
