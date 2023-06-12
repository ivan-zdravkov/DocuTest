using DocuTest.Application.Interfaces;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Application.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository documentRepository;
        private readonly IFileRepository fileRepository;
        private readonly IMetadataRepository metadataRepository;
        private readonly ISqlConnectionFactory connectionFactory;

        public DocumentService(IDocumentRepository documentRepository, IFileRepository fileRepository, IMetadataRepository metadataRepository, ISqlConnectionFactory connectionFactory)
        {
            this.documentRepository = documentRepository;
            this.fileRepository = fileRepository;
            this.metadataRepository = metadataRepository;
            this.connectionFactory = connectionFactory;
        }

        public async Task<Document> Get(Guid documentId)
        {
            IEnumerable<Document> documents = await this.Get(new[] { documentId });

            if (!documents.Any())
                throw new Exception($"Document with id {documentId} not found.");

            return documents.First();
        }

        public async Task<IEnumerable<Document>> Get(IEnumerable<Guid> documentIds)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            IEnumerable<Document> documents = await this.documentRepository.Get(connection, documentIds);
            IEnumerable<Shared.Models.File> files = await this.fileRepository.Get(connection, documentIds);

            IEnumerable<Guid> fileIds = files.Select(f => f.Id);

            IEnumerable<Metadata> metadata = await this.metadataRepository.Get(connection, fileIds);

            foreach (Shared.Models.File file in files)
                file.Metadata = metadata.Where(m => m.FileId == file.Id);

            foreach (Document document in documents)
                document.Files = files.Where(f => f.DocumentId == document.Id);

            return documents;
        }

        public async Task<IEnumerable<Document>> GetByMetadata(string key, string value)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            IEnumerable<Guid> fileIds = await this.metadataRepository.GetFileIds(connection, key, value);
            IEnumerable<Guid> documentIds = await this.fileRepository.GetDocumentIds(connection, fileIds);

            return await this.Get(documentIds);
        }

        public async Task<Guid> Insert(Document document)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                Guid documentId = await this.documentRepository.Insert(transaction, document);

                foreach (Shared.Models.File file in document.Files)
                {
                    Guid fileId = await this.fileRepository.Insert(transaction, documentId, file);

                    await this.metadataRepository.Insert(transaction, file.Metadata);
                }

                await transaction.CommitAsync();

                return documentId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(Document document)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            Document existingDocument = await this.Get(document.Id);

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.documentRepository.Update(transaction, document);

                await InsertFiles(document, existingDocument, transaction);
                await UpdateFiles(document, existingDocument, transaction);
                await DeleteFiles(document, existingDocument, transaction);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Guid documentId)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            IEnumerable<Guid> fileIds = await this.fileRepository.GetFileIds(connection, documentId);

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Delete(transaction, fileIds);
                await this.fileRepository.Delete(transaction, fileIds);
                await this.documentRepository.Delete(transaction, documentId);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task InsertFiles(Document document, Document existingDocument, DbTransaction transaction)
        {
            IEnumerable<Shared.Models.File> filesToInsert = document.Files.Where(newFile => !existingDocument.Files.Any(existing => existing.Id == newFile.Id));

            foreach (Shared.Models.File file in filesToInsert)
            {
                Guid fileId = await this.fileRepository.Insert(transaction, document.Id, file);

                await this.metadataRepository.Insert(transaction, file.Metadata);
            }
        }

        private async Task UpdateFiles(Document document, Document existingDocument, DbTransaction transaction)
        {
            IEnumerable<Shared.Models.File> filesToUpdate = document.Files.Where(newFile => existingDocument.Files.Any(existing => existing.Id == newFile.Id));

            foreach (Shared.Models.File file in filesToUpdate)
            {
                await this.fileRepository.Update(transaction, file);

                //An arguably more efficient way to do this could be to compare the metadata and only update the records that have actually changed.
                //I decided agains this, since we currently don't have audit fields and we also have a composite primary key,
                //so we have no actual loss of data, but we would introduce quite a bit of complexity.
                await this.metadataRepository.Delete(transaction, file.Id);
                await this.metadataRepository.Insert(transaction, file.Metadata);
            }
        }

        private async Task DeleteFiles(Document document, Document existingDocument, DbTransaction transaction)
        {
            IEnumerable<Guid> fileIdsToDelete = existingDocument.Files.Select(f => f.Id).Except(document.Files.Select(f => f.Id));

            foreach (Guid fileId in fileIdsToDelete)
            {
                await this.metadataRepository.Delete(transaction, fileId);
                await this.fileRepository.Delete(transaction, fileId);
            }
        }
    }
}
