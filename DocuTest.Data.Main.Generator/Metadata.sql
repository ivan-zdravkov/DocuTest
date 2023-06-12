CREATE TABLE [dbo].[Metadata]
(
    [FileId] UNIQUEIDENTIFIER NOT NULL REFERENCES [dbo].[File], 
    [Key] NVARCHAR(255) NOT NULL, 
    [Value] NVARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_Metadata] PRIMARY KEY ([FileId], [Key])
)
