using DocuTest.Application.Interfaces;
using DocuTest.Data.Main.DAL.Interfaces;
using System.Data.Common;
using System.Data.SqlClient;
using System.Transactions;

namespace DocuTest.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IFileRepository fileRepository;
        private readonly IMetadataRepository metadataRepository;
        private readonly ISqlConnectionFactory connectionFactory;

        public FileService(IFileRepository fileRepository, IMetadataRepository metadataRepository, ISqlConnectionFactory connectionFactory)
        {
            this.fileRepository = fileRepository;
            this.metadataRepository = metadataRepository;
            this.connectionFactory = connectionFactory;
        }

        public async Task<Guid> Insert(Guid documentId, Shared.Models.File file)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                Guid fileId = await this.fileRepository.Insert(transaction, documentId, file);

                await this.metadataRepository.Insert(transaction, file.Metadata);

                await transaction.CommitAsync();

                return fileId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(Shared.Models.File file)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.fileRepository.Update(transaction, file);

                //An arguably more efficient way to do this could be to compare the metadata and only update the records that have actually changed.
                //I decided agains this, since we currently don't have audit fields and we also have a composite primary key,
                //so we have no actual loss of data, but we would introduce quite a bit of complexity.
                await this.metadataRepository.Delete(transaction, file.Id);
                await this.metadataRepository.Insert(transaction, file.Metadata);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Guid fileId)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Delete(transaction, fileId);
                await this.fileRepository.Delete(transaction, fileId);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
