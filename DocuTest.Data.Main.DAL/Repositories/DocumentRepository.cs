using Dapper;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Models;
using System.Data;
using System.Transactions;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public async Task Delete(IDbTransaction transaction, Guid documentId, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[Document] WHERE [Id] = @documentId",
                transaction: transaction,
                parameters: new { documentId },
                cancellationToken: ct)
            );

        public async Task<Document> Get(IDbConnection connection, Guid documentId, CancellationToken ct) =>
            await connection.QueryFirstOrDefaultAsync<Document>(new CommandDefinition(
                commandText: $"SELECT TOP 1 * FROM [dbo].[Document] WHERE [Id] = @documentId",
                parameters: new { documentId },
                cancellationToken: ct)
            );

        public async Task<IEnumerable<Document>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, CancellationToken ct) =>
            await connection.QueryAsync<Document>(new CommandDefinition(
                commandText: $"SELECT * FROM [dbo].[Document] WHERE [Id] IN @documentIds",
                parameters: new { documentIds },
                cancellationToken: ct)
            );

        public async Task<Guid> Insert(IDbTransaction transaction, Document document, CancellationToken ct)
        {
            document.Id = Guid.NewGuid();

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: @$"INSERT INTO [dbo].[Document] ([Id], [Name], [DocumentTypeId], [UserId])  VALUES (@Id, @Name, @DocumentTypeId, @UserId)",
                transaction: transaction,
                parameters: document,
                cancellationToken: ct)
            );

            return document.Id;
        }

        public async Task Update(IDbTransaction transaction, Document document, CancellationToken ct) =>
            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"UPDATE [dbo].[Document] SET [Name] = @Name, [DocumentTypeId] = @DocumentTypeId, [UserId] = @UserId WHERE [Id] = @Id",
                transaction: transaction,
                parameters: document,
                cancellationToken: ct)
            );

    }
}
