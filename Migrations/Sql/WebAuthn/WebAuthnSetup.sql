CREATE TABLE [CredentialRecords]
(
    [Id]                        uniqueidentifier NOT NULL,
    [RpId]                      nvarchar(256)    NOT NULL,
    [UserHandle]                varbinary(128)   NOT NULL,
    [CredentialId]              varbinary(1024)  NOT NULL,
    [Type]                      int              NOT NULL,
    [Kty]                       int              NOT NULL,
    [Alg]                       int              NOT NULL,
    [Ec2Crv]                    int              NULL,
    [Ec2X]                      varbinary(256)   NULL,
    [Ec2Y]                      varbinary(256)   NULL,
    [RsaModulusN]               varbinary(1024)  NULL,
    [RsaExponentE]              varbinary(32)    NULL,
    [OkpCrv]                    int              NULL,
    [OkpX]                      varbinary(32)    NULL,
    [SignCount]                 bigint           NOT NULL,
    [Transports]                nvarchar(max)    NOT NULL,
    [UvInitialized]             bit              NOT NULL,
    [BackupEligible]            bit              NOT NULL,
    [BackupState]               bit              NOT NULL,
    [AttestationObject]         varbinary(max)   NULL,
    [AttestationClientDataJson] varbinary(max)   NULL,
    [Description]               nvarchar(200)    NULL,
    [CreatedAtUnixTime]         bigint           NOT NULL,
    [UpdatedAtUnixTime]         bigint           NOT NULL,
    CONSTRAINT [PK_CredentialRecords] PRIMARY KEY ([Id])
    );
ALTER TABLE [CredentialRecords]
    ADD CONSTRAINT [Transports should be formatted as JSON] CHECK (ISJSON(Transports) = 1);
CREATE UNIQUE INDEX [IX_CredentialRecords_UserHandle_CredentialId_RpId] ON [CredentialRecords] ([UserHandle], [CredentialId], [RpId]);
CREATE UNIQUE INDEX [IX_CredentialRecords_CredentialId_RpId] ON [CredentialRecords] ([CredentialId], [RpId]);