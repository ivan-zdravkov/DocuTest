using DocuTest.Application.Interfaces;

namespace DocuTest.Application.Services
{
    public class FileService : IFileService
    {
        public async Task<Guid> Insert(Guid documentId, Shared.Models.File file)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Shared.Models.File file)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(Guid fileId)
        {
            throw new NotImplementedException();
        }
    }
}
