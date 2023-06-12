using DocuTest.Application.Interfaces;
using DocuTest.Shared.Models;
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
        public async Task Insert(Metadata metadata) =>
            await this.metadataService.Insert(metadata);

        [HttpPut]
        public async Task Update(Metadata metadata) =>
            await this.metadataService.Update(metadata);

        [HttpDelete]
        public async Task Delete(Metadata metadata) =>
            await this.metadataService.Delete(metadata);

        [HttpDelete]
        [Route("{fileId}/{key}")]
        public async Task Delete(Guid fileId, string key) =>
            await this.metadataService.Delete(fileId, key);
    }
}
