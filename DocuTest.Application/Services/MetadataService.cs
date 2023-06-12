using DocuTest.Application.Interfaces;

namespace DocuTest.Application.Services
{
    public class MetadataService : IMetadataService
    {
        public async Task<Guid> Insert(Guid fileId, string key, string value)
        {
            throw new NotImplementedException();
        }

        public async Task Update(Guid fileId, string key, string value)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(Guid fileId, string key)
        {
            throw new NotImplementedException();
        }
    }
}
