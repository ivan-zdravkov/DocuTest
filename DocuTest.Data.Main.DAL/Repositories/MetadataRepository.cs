using Dapper;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class MetadataRepository : IMetadataRepository
    {
        public async Task Delete(IDbTransaction transaction, Guid fileId, CancellationToken ct) =>
            await this.Delete(transaction, new[] { fileId }, ct);

        public async Task Delete(IDbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[Metadata] WHERE [FileId] IN @fileIds",
                transaction: transaction,
                parameters: new { fileIds },
                cancellationToken: ct)
            );

        public async Task Delete(IDbTransaction transaction, Guid fileId, string key, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[Metadata] WHERE [FileId] = @fileId AND [Key] = @key",
                transaction: transaction,
                parameters: new { fileId, key },
                cancellationToken: ct)
            );

        public async Task Delete(IDbTransaction transaction, Metadata metadata, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[Metadata] WHERE [FileId] = @FileId AND [Key] = @Key AND [Value] = @Value",
                transaction: transaction,
                parameters: metadata,
                cancellationToken: ct)
            );

        public async Task<IEnumerable<Metadata>> Get(IDbConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct) =>
            await connection.QueryAsync<Metadata>(new CommandDefinition(
                commandText: $"SELECT * FROM [dbo].[Metadata] WHERE [FileId] IN @fileIds",
                parameters: new { fileIds },
                cancellationToken: ct)
            );

        public async Task<IEnumerable<Guid>> GetFileIds(IDbConnection connection, string key, string value, CancellationToken ct) =>
            await connection.QueryAsync<Guid>(new CommandDefinition(
                commandText: $"SELECT DISTINCT [FileId] FROM [dbo].[Metadata] WHERE [Key] = @key AND [Value] = @value",
                parameters: new { key, value },
                cancellationToken: ct)
            );

        public async Task Insert(IDbTransaction transaction, Metadata metadata, CancellationToken ct) =>
            await this.Insert(transaction, new[] { metadata }, ct);

        public async Task Insert(IDbTransaction transaction, IEnumerable<Metadata> metadata, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"INSERT INTO [dbo].[Metadata] ([FileId], [Key], [Value]) VALUES (@FileId, @Key, @Value)",
                transaction: transaction,
                parameters: metadata,
                cancellationToken: ct)
            );

        public async Task Update(IDbTransaction transaction, Metadata metadata, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"UPDATE [dbo].[Metadata] SET [Value] = @Value WHERE [FileId] = @FileId AND [Key] = @Key",
                transaction: transaction,
                parameters: metadata,
                cancellationToken: ct)
            );
    }
}
