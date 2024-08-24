BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FriendRequests]') AND [c].[name] = N'AcceptedAt');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [FriendRequests] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [FriendRequests] ALTER COLUMN [AcceptedAt] datetimeoffset NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240824152900_FriendRequestAcceptAtNullable', N'8.0.7');
GO

COMMIT;
GO

