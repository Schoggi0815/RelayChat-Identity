BEGIN TRANSACTION;
GO

ALTER TABLE [Users] ADD [RefreshToken] nvarchar(255) NULL;
GO

ALTER TABLE [Users] ADD [RefreshTokenExpiryTime] datetimeoffset NOT NULL DEFAULT '0001-01-01T00:00:00.0000000+00:00';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20240810091828_AddBackRefreshToken', N'8.0.7');
GO

COMMIT;
GO

