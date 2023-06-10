using System.Data.SqlClient;

namespace DocuTest.Data.Main.DAL.Generators;

public static class DataGenerator
{
    private static Random random = new Random();
    private static IList<(Guid Id, string Name)> documentTypes = new List<(Guid Id, string Name)>()
    {
        (new Guid("687E478F-B7BE-45F2-AD6A-6A716FC5465C"), "Mail"),
        (new Guid("264DC01A-9660-4E8F-823B-720C4F81F05C"), "CV"),
        (new Guid("4D4F70FC-F6BE-47A7-9962-EE354DB570F0"), "Invoice"),
        (new Guid("C1B18E83-10F0-487C-99D7-E23A20BB2455"), "Other")
    };

    public static async Task Generate(
        string connectionString,
        int totalDocuments = 50_000_000,
        int batchSize = 1000,
        int minimumFilesCount = 1,
        int maximumFilesCount = 5,
        int minumumMetadataCount = 0,
        int maximumMetadataCount = 5)
    {
        int batchCount = totalDocuments / batchSize;

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            bool shouldGenerate = await ShouldGenerate(connection);

            if (shouldGenerate)
            {
                Guid userId = new Guid("A1B801AC-A916-4096-86A1-A879B369D41F");

                await InsertUser(connection, userId);
                await InsertDocumentTypes(connection);

                for (int batch = 0; batch < batchCount; batch++)
                {
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            for (int batchItem = 0; batchItem < batchSize; batchItem++)
                            {
                                Guid documentId = Guid.NewGuid();

                                await InsertDocument(transaction, documentId, userId, $"Document {batch * batchSize + batchItem}");

                                foreach (Guid fileId in Enumerable.Range(minimumFilesCount, random.Next(minimumFilesCount, maximumFilesCount)).Select(_ => Guid.NewGuid()))
                                {
                                    await InsertFile(transaction, fileId);
                                    await InsertDocumentFile(transaction, documentId, fileId);

                                    for (int k = minumumMetadataCount; k < random.Next(minumumMetadataCount, maximumMetadataCount); k++)
                                        await InsertMetadata(transaction, fileId, $"Metadata {k}");
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            Console.WriteLine($"Error occurred during batch {batch}: {ex.Message}");
                        }
                    }

                    Console.WriteLine($"Completed batch {batch + 1}/{batchCount}");
                }
            }
            else
            {
                Console.WriteLine($"Some data already exists. Generator aborted.");
            }
        }
    }

    private static Task InsertFile(SqlTransaction transaction, Guid fileId)
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

            return command.ExecuteNonQueryAsync();
        }
    }

    private static Task InsertUser(SqlConnection connection, Guid userId)
    {
        using (SqlCommand command = connection.CreateCommand())
        {
            command.CommandText = @"
                INSERT INTO [dbo].[User] ([Id], [Name], [Email])
                VALUES (@Id, @Name, @Email)";

            command.Parameters.AddWithValue("@Id", userId);
            command.Parameters.AddWithValue("@Name", "User");
            command.Parameters.AddWithValue("@Email", "someone@example.com");

            return command.ExecuteNonQueryAsync();
        }
    }

    private static async Task InsertDocumentTypes(SqlConnection connection)
    {
        foreach ((Guid Id, string Name) type in documentTypes)
        {
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                INSERT INTO [dbo].[DocumentType] ([Id], [Name])
                VALUES (@Id, @Name)";

                command.Parameters.AddWithValue("@Id", type.Id);
                command.Parameters.AddWithValue("@Name", type.Name);

                await command.ExecuteNonQueryAsync();
            }
        }
    }

    private static async Task<bool> ShouldGenerate(SqlConnection connection)
    {
        using (SqlCommand command = connection.CreateCommand())
        {
            command.CommandText = "SELECT COUNT(*) FROM [dbo].[DocumentType]";

            object? records = await command.ExecuteScalarAsync();

            return records != null && (int)records == 0;
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
                INSERT INTO [dbo].[Metadata] ([Id], [FileId], [Key], [Value])
                VALUES (@Id, @FileId, @Key, @Value)";

            command.Parameters.AddWithValue("@Id", Guid.NewGuid());
            command.Parameters.AddWithValue("@FileId", fileId);
            command.Parameters.AddWithValue("@Key", key);
            command.Parameters.AddWithValue("@Value", Guid.NewGuid());

            await command.ExecuteNonQueryAsync();
        }
    }
}