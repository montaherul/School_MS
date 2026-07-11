BEGIN TRANSACTION;
GO

ALTER TABLE [ClassSubjects] DROP CONSTRAINT [FK_ClassSubjects_StudentGroups_StudentGroupId];
GO

DROP INDEX [IX_Sections_SchoolClassId_StudentGroupId_Name] ON [Sections];
GO

DROP INDEX [IX_ClassSubjects_SchoolClassId_SubjectId_GroupName] ON [ClassSubjects];
GO

DROP INDEX [IX_ClassSubjects_StudentGroupId] ON [ClassSubjects];
GO

                INSERT INTO [dbo].[ClassSubjectGroups] ([ClassSubjectId], [StudentGroupId], [CreatedBy], [CreatedAt], [IsDeleted])
                SELECT [Id], [StudentGroupId], 'migration', SYSUTCDATETIME(), 0
                FROM [dbo].[ClassSubjects]
                WHERE [StudentGroupId] IS NOT NULL
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'IsGroupSubject');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [IsGroupSubject];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'StudentGroupId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [StudentGroupId];
GO

CREATE TABLE [ClassSubjectGroups] (
    [Id] int NOT NULL IDENTITY,
    [ClassSubjectId] int NOT NULL,
    [StudentGroupId] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ClassSubjectGroups] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassSubjectGroups_ClassSubjects_ClassSubjectId] FOREIGN KEY ([ClassSubjectId]) REFERENCES [ClassSubjects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ClassSubjectGroups_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Sections_SchoolClassId_Name] ON [Sections] ([SchoolClassId], [Name]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_ClassSubjects_SchoolClassId_SubjectId] ON [ClassSubjects] ([SchoolClassId], [SubjectId]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_ClassSubjectGroups_ClassSubjectId_StudentGroupId] ON [ClassSubjectGroups] ([ClassSubjectId], [StudentGroupId]) WHERE [IsDeleted] = 0;
GO

CREATE INDEX [IX_ClassSubjectGroups_StudentGroupId] ON [ClassSubjectGroups] ([StudentGroupId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260710092709_AddClassSubjectGroupJunction', N'8.0.0');
GO

COMMIT;
GO

