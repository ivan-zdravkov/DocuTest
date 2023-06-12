using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Generators;

public static class DataGenerator
{
    private static Random random = new Random();
    private static Sequential sequential = new Sequential();

    private static IList<(Guid Id, string Name)> documentTypes = new List<(Guid Id, string Name)>()
    {
        (new Guid("687E478F-B7BE-45F2-AD6A-6A716FC5465C"), "Mail"),
        (new Guid("264DC01A-9660-4E8F-823B-720C4F81F05C"), "CV"),
        (new Guid("4D4F70FC-F6BE-47A7-9962-EE354DB570F0"), "Invoice"),
        (new Guid("C1B18E83-10F0-487C-99D7-E23A20BB2455"), "Other")
    };

    private static IList<(Guid Id, string Name, string Email)> users = new List<(Guid Id, string Name, string Email)>()
    {
        (new Guid("A1B801AC-A916-4096-86A1-A879B369D41F"), "User", "someone@example.com"),
        (new Guid("39773585-d12d-47ab-8ad3-c0cd003815a9"), "Manager", "manager@example.com"),
        (new Guid("971aa795-c31e-4545-b4e3-d13a884580ab"), "Admin", "admin@example.com"),
        (new Guid("bcc088b2-fb80-4911-81d7-3b16d512c73d"), "CEO", "ceo@example.com")
    };

    public static async Task Generate(
        string connectionString,
        int totalDocuments = 50_000_000,
        int batchSize = 1000,
        int minFilesCount = 1,
        int maxFilesCount = 5,
        int minMetadataCount = 0,
        int maxMetadataCount = 5,
        int threadsPerCore = 2)
    {
        sequential.Batch = 0;

        int batchCount = totalDocuments / batchSize;
        int maxDegreeOfParallelism = Environment.ProcessorCount * threadsPerCore;

        Queue<SqlConnection> connectionPool = UniqueConnections(connectionString, maxDegreeOfParallelism);
        
        try
        {
            await InsertUsers(connectionPool.First());
            await InsertDocumentTypes(connectionPool.First());

            Parallel.For(sequential.Batch, batchCount, new ParallelOptions() { MaxDegreeOfParallelism = maxDegreeOfParallelism }, async _ =>
            {
                Guid userId = users[random.Next(0, users.Count)].Id;

                SqlConnection connection = AcquireConnection(connectionPool);

                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        for (int batchItem = 0; batchItem < batchSize; batchItem++)
                        {
                            Guid documentId = Guid.NewGuid();

                            await InsertDocument(transaction, documentId, userId, $"Document {sequential.Batch * batchSize + batchItem}");

                            foreach (Guid fileId in Enumerable.Range(minFilesCount, random.Next(minFilesCount, maxFilesCount)).Select(_ => Guid.NewGuid()))
                            {
                                await InsertFile(transaction, fileId);
                                await InsertDocumentFile(transaction, documentId, fileId);

                                for (int k = minMetadataCount; k < random.Next(minMetadataCount, maxMetadataCount); k++)
                                    await InsertMetadata(transaction, fileId, $"Metadata {k}");
                            }
                        }

                        await transaction.CommitAsync();

                        ReleaseConnection(connectionPool, connection);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();

                        ReleaseConnection(connectionPool, connection);

                        Console.WriteLine($"Error occurred during batch {sequential.Batch + 1}: {ex.Message}");
                    }
                }

                Console.WriteLine($"Completed batch {sequential.Batch + 1}/{batchCount}");
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unspecified error occured: {ex.Message}");

            throw;
        }
        finally
        {
            foreach(SqlConnection connection in connectionPool)
                connection.Close();
        }
    }

    private static Queue<SqlConnection> UniqueConnections(string connectionString, int maxDegreeOfParallelism)
    {
        Queue<SqlConnection> connections = new Queue<SqlConnection>(maxDegreeOfParallelism);

        for (int i = 0; i <= maxDegreeOfParallelism; i++)
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            connections.Enqueue(connection);
        }

        return connections;
    }

    private static SqlConnection AcquireConnection(Queue<SqlConnection> connections)
    {
        do
        {
            lock (connections)
            {
                if (connections.Any())
                    return connections.Dequeue();
            }
        }
        while (true);
    }

    private static void ReleaseConnection(Queue<SqlConnection> connections, SqlConnection connection)
    {
        lock (connections)
        {
            connections.Enqueue(connection);
        }

        lock (sequential)
        {
            sequential.Batch++;
        }
    }

    private static async Task InsertFile(SqlTransaction transaction, Guid fileId)
    {
        using (SqlCommand command = transaction.Connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO [dbo].[File] ([Id], [Name], [Extension], [Content])
                VALUES (@Id, @Name, @Extension, @Content)";

            command.Parameters.AddWithValue("@Id", fileId);
            command.Parameters.AddWithValue("@Name", fileId);
            command.Parameters.AddWithValue("@Extension", "txt");
            command.Parameters.AddWithValue("@Content", 1024);

            await command.ExecuteNonQueryAsync();
        }
    }

    private static async Task InsertUsers(SqlConnection connection)
    {
        foreach ((Guid Id, string Name, string Email) user in users)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[User] WHERE [Id] = @Id)
                INSERT INTO [dbo].[User] ([Id], [Name], [Email])
                VALUES (@Id, @Name, @Email)";

                command.Parameters.AddWithValue("@Id", user.Id);
                command.Parameters.AddWithValue("@Name", user.Name);
                command.Parameters.AddWithValue("@Email", user.Email);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task InsertDocumentTypes(SqlConnection connection)
    {
        foreach ((Guid Id, string Name) type in documentTypes)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                IF NOT EXISTS (SELECT 1 FROM [dbo].[DocumentType] WHERE [Id] = @Id)
                INSERT INTO [dbo].[DocumentType] ([Id], [Name])
                VALUES (@Id, @Name)";

                command.Parameters.AddWithValue("@Id", type.Id);
                command.Parameters.AddWithValue("@Name", type.Name);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task InsertDocument(SqlTransaction transaction, Guid documentId, Guid userId, string name)
    {
        Guid documentTypeId = documentTypes[random.Next(0, documentTypes.Count)].Id;

        using (SqlCommand command = transaction.Connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO [dbo].[Document] ([Id], [Name], [DocumentTypeId], [UserId])
                VALUES (@Id, @Name, @DocumentTypeId, @UserId)";

            command.Parameters.AddWithValue("@Id", documentId);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@DocumentTypeId", documentTypeId);
            command.Parameters.AddWithValue("@UserId", userId);

            await command.ExecuteNonQueryAsync();
        }
    }

    private static async Task InsertDocumentFile(SqlTransaction transaction, Guid documentId, Guid fileId)
    {
        using (SqlCommand command = transaction.Connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO [dbo].[DocumentFile] ([DocumentId], [FileId])
                VALUES (@DocumentId, @FileId)";

            command.Parameters.AddWithValue("@DocumentId", documentId);
            command.Parameters.AddWithValue("@FileId", fileId);

            await command.ExecuteNonQueryAsync();
        }
    }

    private static async Task InsertMetadata(SqlTransaction transaction, Guid fileId, string key)
    {
        using (SqlCommand command = transaction.Connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = @"
                INSERT INTO [dbo].[Metadata] ([FileId], [Key], [Value])
                VALUES (@FileId, @Key, @Value)";

            command.Parameters.AddWithValue("@FileId", fileId);
            command.Parameters.AddWithValue("@Key", key);
            command.Parameters.AddWithValue("@Value", Guid.NewGuid());

            await command.ExecuteNonQueryAsync();
        }
    }
}

internal class Sequential
{
    internal int Batch;
}