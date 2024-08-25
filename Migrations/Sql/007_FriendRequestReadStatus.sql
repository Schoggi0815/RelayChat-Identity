BEGIN TRANSACTION;
GO

ALTER TABLE [FriendRequests] ADD [Read] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240825094553_FriendRequestReadStatus', N'8.0.7');
GO

COMMIT;
GO

