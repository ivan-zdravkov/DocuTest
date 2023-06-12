namespace DocuTest.Application.Interfaces
{
    public interface IFileService
    {
        Task<Guid> Insert(Guid documentId, Shared.Models.File file, CancellationToken ct);
        Task Update(Shared.Models.File file, CancellationToken ct);
        Task Delete(Guid fileId, CancellationToken ct);
    }
}
