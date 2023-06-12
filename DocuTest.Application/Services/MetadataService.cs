using DocuTest.Application.Interfaces;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Application.Services
{
    public class MetadataService : IMetadataService
    {
        private readonly IMetadataRepository metadataRepository;
        private readonly IDbConnectionFactory connectionFactory;

        public MetadataService(IMetadataRepository metadataRepository, IDbConnectionFactory connectionFactory)
        {
            this.metadataRepository = metadataRepository;
            this.connectionFactory = connectionFactory;
        }

        public async Task Insert(Metadata metadata, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.metadataRepository.Insert(transaction, metadata, ct);

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }

        public async Task Update(Metadata metadata, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.metadataRepository.Update(transaction, metadata, ct);
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }

        public async Task Delete(Metadata metadata, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.metadataRepository.Delete(transaction, metadata, ct);
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }

        public async Task Delete(Guid fileId, string key, CancellationToken ct)
        {
            using IDbConnection connection = this.connectionFactory.Create();

            connection.Open();

            IDbTransaction transaction = connection.BeginTransaction();

            try
            {
                await this.metadataRepository.Delete(transaction, fileId, key, ct);
            }
            catch (Exception)
            {
                transaction.Commit();
                throw;
            }
        }
    }
}
