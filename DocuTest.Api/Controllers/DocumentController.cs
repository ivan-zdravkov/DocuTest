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
        public async Task<Document> Get(Guid documentId) =>
            await this.documentService.Get(documentId);

        [HttpGet]
        [Route("get-by-metadata/{key}/{value}")]
        public async Task<IEnumerable<Document>> GetByMetadata(string key, string value) =>
            await this.documentService.GetByMetadata(key, value);

        [HttpPost]
        public async Task<Guid> Create(Document document) =>
            await this.documentService.Insert(document);

        [HttpPut]
        public async Task Update(Document document) =>
            await this.documentService.Update(document);

        [HttpDelete]
        [Route("{documentId}")]
        public async Task Delete(Guid documentId) =>
            await this.documentService.Delete(documentId);
    }
}
