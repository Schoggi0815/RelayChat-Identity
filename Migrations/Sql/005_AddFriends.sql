BEGIN TRANSACTION;
GO

DROP TABLE [Tests];
GO

CREATE TABLE [FriendRequests] (
    [SenderId] uniqueidentifier NOT NULL,
    [ReceiverId] uniqueidentifier NOT NULL,
    [CreatedAt] datetimeoffset NOT NULL DEFAULT (getdate()),
    [Accepted] bit NOT NULL,
    [AcceptedAt] datetimeoffset NOT NULL,
    CONSTRAINT [PK_FriendRequests] PRIMARY KEY ([SenderId], [ReceiverId]),
    CONSTRAINT [FK_FriendRequests_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_FriendRequests_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_FriendRequests_ReceiverId] ON [FriendRequests] ([ReceiverId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240822135137_AddFriends', N'8.0.7');
GO

COMMIT;
GO

