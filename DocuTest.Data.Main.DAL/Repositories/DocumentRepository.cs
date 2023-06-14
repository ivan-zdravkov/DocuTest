using Dapper;
using DocuTest.Data.Main.DAL.Interfaces;
using DocuTest.Shared.Interfaces;
using DocuTest.Shared.Models;
using System.Data;

namespace DocuTest.Data.Main.DAL.Repositories
{
    public class DocumentRepository : IDocumentRepository
    {
        public async Task Delete(IDbTransaction transaction, Guid documentId, IDataStrategy<Document> strategy, CancellationToken ct)
        {
            Document document = await this.Get(transaction.Connection!, documentId, strategy, ct);

            bool canDelete = strategy.Allows(document);

            if (!canDelete)
                throw new ArgumentException($"Document with id {documentId} cannot be deleted due to the applied IDataStrategy.");

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"DELETE FROM [dbo].[Document] WHERE [Id] = @documentId",
                transaction: transaction,
                parameters: new { documentId },
                cancellationToken: ct)
            );
        }

        public async Task<Document> Get(IDbConnection connection, Guid documentId, IDataStrategy<Document> strategy, CancellationToken ct) =>
            await connection.QueryFirstOrDefaultAsync<Document>(new CommandDefinition(
                commandText: $"SELECT TOP 1 * FROM [dbo].[Document] WHERE [Id] = @documentId AND {strategy.Expression()}",
                parameters: new { documentId },
                cancellationToken: ct)
            );

        public async Task<IEnumerable<Document>> Get(IDbConnection connection, IEnumerable<Guid> documentIds, IDataStrategy<Document> strategy, CancellationToken ct) =>
            await connection.QueryAsync<Document>(new CommandDefinition(
                commandText: $"SELECT * FROM [dbo].[Document] WHERE [Id] IN @documentIds AND {strategy.Expression()}",
                parameters: new { documentIds },
                cancellationToken: ct)
            );

        public async Task<Guid> Insert(IDbTransaction transaction, Document document, IDataStrategy<Document> strategy, CancellationToken ct)
        {
            document.Id = Guid.NewGuid();

            bool canInsert = strategy.Allows(document);

            if (!canInsert)
                throw new ArgumentException($"Document with id {document.Id} cannot be inserted due to the applied IDataStrategy.");

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: @$"INSERT INTO [dbo].[Document] ([Id], [Name], [DocumentTypeId], [UserId]) VALUES (@Id, @Name, @DocumentTypeId, @UserId)",
                transaction: transaction,
                parameters: document,
                cancellationToken: ct)
            );

            return document.Id;
        }

        public async Task Update(IDbTransaction transaction, Document document, IDataStrategy<Document> strategy, CancellationToken ct)
        {
            bool canUpdate = strategy.Allows(document);

            if (!canUpdate)
                throw new ArgumentException($"Document with id {document.Id} cannot be updated due to the applied IDataStrategy.");

            await transaction.Connection.ExecuteAsync(new CommandDefinition(
                commandText: $"UPDATE [dbo].[Document] SET [Name] = @Name, [DocumentTypeId] = @DocumentTypeId, [UserId] = @UserId WHERE [Id] = @Id",
                transaction: transaction,
                parameters: document,
                cancellationToken: ct)
            );
        }
    }
}
