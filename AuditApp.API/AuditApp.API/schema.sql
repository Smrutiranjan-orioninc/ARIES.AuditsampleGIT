IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Auditors] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NOT NULL,
    [Role] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Auditors] PRIMARY KEY ([Id])
);

CREATE TABLE [Clients] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Industry] nvarchar(max) NOT NULL,
    [ContactEmail] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Clients] PRIMARY KEY ([Id])
);

CREATE TABLE [Audits] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [ClientId] int NOT NULL,
    [AuditorId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [Status] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Audits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Audits_Auditors_AuditorId] FOREIGN KEY ([AuditorId]) REFERENCES [Auditors] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Audits_Clients_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [Clients] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [ChecklistItems] (
    [Id] int NOT NULL IDENTITY,
    [AuditId] int NOT NULL,
    [TaskName] nvarchar(max) NOT NULL,
    [IsCompleted] bit NOT NULL,
    CONSTRAINT [PK_ChecklistItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ChecklistItems_Audits_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Findings] (
    [Id] int NOT NULL IDENTITY,
    [AuditId] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Severity] nvarchar(max) NOT NULL,
    [Recommendation] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Findings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Findings_Audits_AuditId] FOREIGN KEY ([AuditId]) REFERENCES [Audits] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Audits_AuditorId] ON [Audits] ([AuditorId]);

CREATE INDEX [IX_Audits_ClientId] ON [Audits] ([ClientId]);

CREATE INDEX [IX_ChecklistItems_AuditId] ON [ChecklistItems] ([AuditId]);

CREATE INDEX [IX_Findings_AuditId] ON [Findings] ([AuditId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260128143142_InitialCreate', N'10.0.2');

COMMIT;
GO

