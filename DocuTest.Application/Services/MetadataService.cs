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

        public async Task Insert(Metadata metadata, CancellationToken ct)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Insert(transaction, metadata, ct);

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Update(Metadata metadata, CancellationToken ct)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Update(transaction, metadata, ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Metadata metadata, CancellationToken ct)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Delete(transaction, metadata, ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task Delete(Guid fileId, string key, CancellationToken ct)
        {
            using SqlConnection connection = this.connectionFactory.Create();

            await connection.OpenAsync();

            DbTransaction transaction = await connection.BeginTransactionAsync();

            try
            {
                await this.metadataRepository.Delete(transaction, fileId, key, ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
