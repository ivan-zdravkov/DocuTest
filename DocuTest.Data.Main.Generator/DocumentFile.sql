CREATE TABLE [dbo].[DocumentFile]
(
	[DocumentId] UNIQUEIDENTIFIER NOT NULL REFERENCES [dbo].[Document], 
    [FileId] UNIQUEIDENTIFIER NOT NULL REFERENCES [dbo].[File], 
    CONSTRAINT [PK_DocumentFile] PRIMARY KEY ([DocumentId], [FileId])
)
