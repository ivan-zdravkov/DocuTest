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
        public async Task Insert(Metadata metadata, CancellationToken ct) =>
            await this.metadataService.Insert(metadata, ct);

        [HttpPut]
        public async Task Update(Metadata metadata, CancellationToken ct) =>
            await this.metadataService.Update(metadata, ct);

        [HttpDelete]
        public async Task Delete(Metadata metadata, CancellationToken ct) =>
            await this.metadataService.Delete(metadata, ct);

        [HttpDelete]
        [Route("{fileId}/{key}")]
        public async Task Delete(Guid fileId, string key, CancellationToken ct) =>
            await this.metadataService.Delete(fileId, key, ct);
    }
}
