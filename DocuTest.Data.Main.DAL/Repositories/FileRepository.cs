using Dapper;
using DocuTest.Data.Main.DAL.Interfaces;
using System.Data;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class FileRepository : IFileRepository
    {
        public async Task Delete(IDbTransaction transaction, Guid fileId, CancellationToken ct) =>
            await this.Delete(transaction, new[] { fileId }, ct);

        public async Task Delete(IDbTransaction transaction, IEnumerable<Guid> fileIds, CancellationToken ct)
        {
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[DocumentFile] WHERE [FileId] IN @fileIds",
                transaction: transaction,
                parameters: new { fileIds },
                cancellationToken: ct)
            );

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[File] WHERE [Id] IN @fileIds",
                transaction: transaction,
                parameters: new { fileIds },
                cancellationToken: ct)
            );
        }

        public async Task<IEnumerable<Shared.Models.File>> Get(IDbConnection connection, Guid documentId, CancellationToken ct) =>
            await this.Get(connection, new[] { documentId }, ct);

        public async Task<IEnumerable<Shared.Models.File>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct) =>
            await connection.QueryAsync<Shared.Models.File>(new CommandDefinition(
                commandText: @$"
                    SELECT f.*, df.[DocumentId] As [DocumentId]
                    FROM [dbo].[File] f
                    JOIN [dbo].[DocumentFile] df ON df.[FileId] = f.[Id]
                    WHERE df.[DocumentId] IN @documentIds",
                parameters: new { documentIds },
                cancellationToken: ct)
            );

        public async Task<IEnumerable<Guid>> GetDocumentIds(IDbConnection connection, IEnumerable<Guid> fileIds, CancellationToken ct) =>
            await connection.QueryAsync<Guid>(new CommandDefinition(
                commandText: $"SELECT DISTINCT [DocumentId] FROM [dbo].[DocumentFile] WHERE [FileId] IN @fileIds",
                parameters: new { fileIds },
                cancellationToken: ct)
            );

        public async Task<IEnumerable<Guid>> GetFileIds(IDbConnection connection, Guid documentId, CancellationToken ct) =>
            await connection.QueryAsync<Guid>(new CommandDefinition(
                commandText: $"SELECT DISTINCT [FileId] FROM [dbo].[DocumentFile] WHERE [DocumentId] = @documentId",
                parameters: new { documentId },
                cancellationToken: ct)
            );

        public async Task<Guid> Insert(IDbTransaction transaction, Shared.Models.File file, CancellationToken ct)
        {
            file.Id = Guid.NewGuid();

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: @$"INSERT INTO [dbo].[File] ([Id], [Name], [Extension], [Content]) VALUES (@Id, @Name, @Extension, @Content)",
                transaction: transaction,
                parameters: file,
                cancellationToken: ct)
            );

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"INSERT INTO [dbo].[DocumentFile] ([DocumentId], [FileId]) VALUES (@documentId, @fileId)",
                transaction: transaction,
                parameters: new { documentId = file.DocumentId, fileId = file.Id },
                cancellationToken: ct)
            );

            return file.Id;
        }

        public async Task Update(IDbTransaction transaction, Shared.Models.File file, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"UPDATE [dbo].[File] SET [Name] = @Name, [Extension] = @Extension, [Content] = @Content WHERE [Id] = @Id",
                transaction: transaction,
                parameters: new { file },
                cancellationToken: ct)
            );
    }
}
