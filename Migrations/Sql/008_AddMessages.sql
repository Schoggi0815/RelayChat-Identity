BEGIN TRANSACTION;
GO

CREATE TABLE [DirectMessages] (
    [Id] uniqueidentifier NOT NULL,
    [SenderId] uniqueidentifier NOT NULL,
    [ReceiverId] uniqueidentifier NOT NULL,
    [SentAt] datetimeoffset NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [Read] bit NOT NULL,
    CONSTRAINT [PK_DirectMessages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DirectMessages_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_DirectMessages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id])
);
GO

CREATE INDEX [IX_DirectMessages_ReceiverId] ON [DirectMessages] ([ReceiverId]);
GO

CREATE INDEX [IX_DirectMessages_SenderId] ON [DirectMessages] ([SenderId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240825130209_AddMessages', N'8.0.7');
GO

COMMIT;
GO

