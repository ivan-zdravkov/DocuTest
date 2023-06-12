using DocuTest.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DocuTest.Api.Controllers
{
    [ApiController]
    [Route("file")]
    public class FileController : ControllerBase
    {
        private readonly IFileService fileService;

        public FileController(IFileService fileService)
        {
            this.fileService = fileService;
        }

        [HttpPost]
        [Route("{documentId}")]
        public async Task<Guid> Insert(Guid documentId, Shared.Models.File file, CancellationToken ct) =>
            await this.fileService.Insert(documentId, file, ct);

        [HttpPut]
        public async Task Update(Shared.Models.File file, CancellationToken ct) =>
            await this.fileService.Update(file, ct);

        [HttpDelete]
        [Route("{fileId}")]
        public async Task Delete(Guid fileId, CancellationToken ct) =>
            await this.fileService.Delete(fileId, ct);
    }
}
