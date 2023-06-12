using DocuTest.Application.Interfaces;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository documentRepository;
        private readonly IFileRepository fileRepository;
        private readonly IMetadataRepository metadataRepository;
        private readonly IDbConnectionFactory connectionFactory;

        public DocumentService(IDocumentRepository documentRepository, IFileRepository fileRepository, IMetadataRepository metadataRepository, IDbConnectionFactory connectionFactory)
        {
            this.documentRepository = documentRepository;
            this.fileRepository = fileRepository;
            this.metadataRepository = metadataRepository;
            this.connectionFactory = connectionFactory;
        }

        public async Task<Document> Get(Guid documentId, CancellationToken ct)
        {
            IEnumerable<Document> documents = await this.Get(new[] { documentId }, ct);

            if (!documents.Any())
                throw new ArgumentException($"Document with id {documentId} not found.");

            return documents.First();
        }

        public async Task<IEnumerable<Document>> Get(IEnumerable<Guid> documentIds, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IEnumerable<Document> documents = await this.documentRepository.Get(connection, documentIds, ct);
            IEnumerable<Shared.Models.File> files = await this.fileRepository.Get(connection, documentIds, ct);

            IEnumerable<Guid> fileIds = files.Select(f => f.Id);

            IEnumerable<Metadata> metadata = await this.metadataRepository.Get(connection, fileIds, ct);

            foreach (Shared.Models.File file in files)
                file.Metadata = metadata.Where(m => m.FileId == file.Id);

            foreach (Document document in documents)
                document.Files = files.Where(f => f.DocumentId == document.Id);

            return documents;
        }

        public async Task<IEnumerable<Document>> GetByMetadata(string key, string value, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IEnumerable<Guid> fileIds = await this.metadataRepository.GetFileIds(connection, key, value, ct);
            IEnumerable<Guid> documentIds = await this.fileRepository.GetDocumentIds(connection, fileIds, ct);

            return await this.Get(documentIds, ct);
        }

        public async Task<Guid> Insert(Document document, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                Guid documentId = await this.documentRepository.Insert(transaction, document, ct);

                foreach (Shared.Models.File file in document.Files)
                {
                    Guid fileId = await this.fileRepository.Insert(transaction, documentId, file, ct);

                    await this.metadataRepository.Insert(transaction, file.Metadata, ct);
                }

                transaction.Commit();

                return documentId;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task Update(Document document, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            Document existingDocument = await this.Get(document.Id, ct);

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.documentRepository.Update(transaction, document, ct);

                await InsertFiles(document, existingDocument, transaction, ct);
                await UpdateFiles(document, existingDocument, transaction, ct);
                await DeleteFiles(document, existingDocument, transaction, ct);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task Delete(Guid documentId, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IEnumerable<Guid> fileIds = await this.fileRepository.GetFileIds(connection, documentId, ct);

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.metadataRepository.Delete(transaction, fileIds, ct);
                await this.fileRepository.Delete(transaction, fileIds, ct);
                await this.documentRepository.Delete(transaction, documentId, ct);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        private async Task InsertFiles(Document document, Document existingDocument, IDbTransaction transaction, CancellationToken ct)
        {
            IEnumerable<Shared.Models.File> filesToInsert = document.Files.Where(newFile => !existingDocument.Files.Any(existing => existing.Id == newFile.Id));

            foreach (Shared.Models.File file in filesToInsert)
            {
                Guid fileId = await this.fileRepository.Insert(transaction, document.Id, file, ct);

                await this.metadataRepository.Insert(transaction, file.Metadata, ct);
            }
        }

        private async Task UpdateFiles(Document document, Document existingDocument, IDbTransaction transaction, CancellationToken ct)
        {
            IEnumerable<Shared.Models.File> filesToUpdate = document.Files.Where(newFile => existingDocument.Files.Any(existing => existing.Id == newFile.Id));

            foreach (Shared.Models.File file in filesToUpdate)
            {
                await this.fileRepository.Update(transaction, file, ct);

                //An arguably more efficient way to do this could be to compare the metadata and only update the records that have actually changed.
                //I decided agains this, since we currently don't have audit fields and we also have a composite primary key,
                //so we have no actual loss of data, but we would introduce quite a bit of complexity.
                await this.metadataRepository.Delete(transaction, file.Id, ct);
                await this.metadataRepository.Insert(transaction, file.Metadata, ct);
            }
        }

        private async Task DeleteFiles(Document document, Document existingDocument, IDbTransaction transaction, CancellationToken ct)
        {
            IEnumerable<Guid> fileIdsToDelete = existingDocument.Files.Select(f => f.Id).Except(document.Files.Select(f => f.Id));

            foreach (Guid fileId in fileIdsToDelete)
            {
                await this.metadataRepository.Delete(transaction, fileId, ct);
                await this.fileRepository.Delete(transaction, fileId, ct);
            }
        }
    }
}
