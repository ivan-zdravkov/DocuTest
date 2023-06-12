namespace DocuTest.Application.Interfaces
{
    public interface IFileService
    {
        public Task<Guid> Insert(Guid documentId, Shared.Models.File file);
        public Task Update(Shared.Models.File file);
        public Task Delete(Guid fileId);
    }
}
