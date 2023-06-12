using DocuTest.Application.Interfaces;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data.Common;
using System.Data.SqlClient;

namespace DocuTest.Application.Services
{
    public class MetadataService : IMetadataService
    {
        private readonly IMetadataRepository metadataRepository;
        private readonly ISqlConnectionFactory connectionFactory;

        public MetadataService(IMetadataRepository metadataRepository, ISqlConnectionFactory connectionFactory)
        {
            this.metadataRepository = metadataRepository;
            this.connectionFactory = connectionFactory;
        }

        public async Task Insert(Metadata metadata)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Insert(transaction, metadata);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(Metadata metadata)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Update(transaction, metadata);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Metadata metadata)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Delete(transaction, metadata);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Guid fileId, string key)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Delete(transaction, fileId, key);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
