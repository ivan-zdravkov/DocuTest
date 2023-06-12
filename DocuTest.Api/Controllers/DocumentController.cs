using DocuTest.Application.Interfaces;
using DocuTest.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace DocuTest.Api.Controllers
{
    [ApiController]
    [Route("document")]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService documentService;

        public DocumentController(IDocumentService documentService)
        {
            this.documentService = documentService;
        }

        [HttpGet]
        [Route("{documentId}")]
        public async Task<Document> Get(Guid documentId, CancellationToken ct) =>
            await this.documentService.Get(documentId, ct);

        [HttpGet]
        [Route("get-by-metadata/{key}/{value}")]
        public async Task<IEnumerable<Document>> GetByMetadata(string key, string value, CancellationToken ct) =>
            await this.documentService.GetByMetadata(key, value, ct);

        [HttpPost]
        public async Task<Guid> Create(Document document, CancellationToken ct) =>
            await this.documentService.Insert(document, ct);

        [HttpPut]
        public async Task Update(Document document, CancellationToken ct) =>
            await this.documentService.Update(document, ct);

        [HttpDelete]
        [Route("{documentId}")]
        public async Task Delete(Guid documentId, CancellationToken ct) =>
            await this.documentService.Delete(documentId, ct);
    }
}
