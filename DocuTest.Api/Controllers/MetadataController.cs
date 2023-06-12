using DocuTest.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocuTest.Api.Controllers
{
    [ApiController]
    [Route("metadata")]
    public class MetadataController : ControllerBase
    {
        private readonly IMetadataService metadataService;

        public MetadataController(IMetadataService metadataService)
        {
            this.metadataService = metadataService;
        }

        [HttpPost]
        [Route("{fileId}/{key}")]
        public async Task<Guid> Insert(Guid fileId, string key, string value) =>
            await this.metadataService.Insert(fileId, key, value);

        [HttpPut]
        [Route("{fileId}/{key}")]
        public async Task Update(Guid fileId, string key, string value) =>
            await this.metadataService.Update(fileId, key, value);

        [HttpDelete]
        [Route("{fileId}/{key}")]
        public async Task Delete(Guid fileId, string key) =>
            await this.metadataService.Delete(fileId, key);
    }
}
