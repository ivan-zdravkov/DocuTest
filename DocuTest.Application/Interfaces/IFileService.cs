namespace DocuTest.Application.Interfaces
{
    public interface IFileService
    {
        Task<Guid> Insert(Shared.Models.File file, CancellationToken ct);
        Task Update(Shared.Models.File file, CancellationToken ct);
        Task Delete(Guid fileId, CancellationToken ct);
    }
}
