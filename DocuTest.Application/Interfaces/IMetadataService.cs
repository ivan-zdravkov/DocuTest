namespace DocuTest.Application.Interfaces
{
    public interface IMetadataService
    {
        public Task<Guid> Insert(Guid fileId, string key, string value);
        public Task Update(Guid fileId, string key, string value);
        public Task Delete(Guid fileId, string key);
    }
}
