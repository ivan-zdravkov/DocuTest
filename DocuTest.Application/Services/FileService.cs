using DocuTest.Application.Interfaces;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;
        private readonly IMetadataRepository metadataRepository;
        private readonly IDbConnectionFactory connectionFactory;

        public FileService(IFileRepository fileRepository, IMetadataRepository metadataRepository, IDbConnectionFactory connectionFactory)
        {
            this.fileRepository = fileRepository;
            this.metadataRepository = metadataRepository;
            this.connectionFactory = connectionFactory;
        }

        public async Task<Guid> Insert(Shared.Models.File file, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                Guid fileId = await this.fileRepository.Insert(transaction, file, ct);

                foreach (Metadata metadata in file.Metadata)
                    metadata.FileId = fileId;

                await this.metadataRepository.Insert(transaction, file.Metadata, ct);

                transaction.Commit();

                return fileId;
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }

        public async Task Update(Shared.Models.File file, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.fileRepository.Update(transaction, file, ct);

                //An arguably more efficient way to do this could be to compare the metadata and only update the records that have actually changed.
                //I decided agains this, since we currently don't have audit fields and we also have a composite primary key,
                //so we have no actual loss of data, but we would introduce quite a bit of complexity.
                await this.metadataRepository.Delete(transaction, file.Id, ct);
                await this.metadataRepository.Insert(transaction, file.Metadata, ct);
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }

        public async Task Delete(Guid fileId, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.metadataRepository.Delete(transaction, fileId, ct);
                await this.fileRepository.Delete(transaction, fileId, ct);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }
    }
}
