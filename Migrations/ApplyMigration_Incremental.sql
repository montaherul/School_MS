BEGIN TRANSACTION;
GO

ALTER TABLE [ExamSchedules] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [ExamSchedules] ADD [SectionId] int NULL;
GO

ALTER TABLE [ExamSchedules] ADD [StudentGroupId] int NULL;
GO

DROP INDEX [IX_ClassSubjects_SchoolClassId_SubjectId_GroupName] ON [ClassSubjects];
DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'GroupName');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var0 + '];');
UPDATE [ClassSubjects] SET [GroupName] = N'' WHERE [GroupName] IS NULL;
ALTER TABLE [ClassSubjects] ALTER COLUMN [GroupName] nvarchar(50) NOT NULL;
ALTER TABLE [ClassSubjects] ADD DEFAULT N'' FOR [GroupName];
CREATE UNIQUE INDEX [IX_ClassSubjects_SchoolClassId_SubjectId_GroupName] ON [ClassSubjects] ([SchoolClassId], [SubjectId], [GroupName]) WHERE [IsDeleted] = 0;
GO

UPDATE [StudentGroups] SET [Name] = N'BusinessStudies'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 11;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 12;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 13;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 14;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 15;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'BusinessStudies'
WHERE [Id] = 20;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'BusinessStudies'
WHERE [Id] = 21;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'BusinessStudies'
WHERE [Id] = 22;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 34;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_ExamSchedules_ClassId] ON [ExamSchedules] ([ClassId]);
GO

CREATE INDEX [IX_ExamSchedules_SectionId] ON [ExamSchedules] ([SectionId]);
GO

CREATE INDEX [IX_ExamSchedules_StudentGroupId] ON [ExamSchedules] ([StudentGroupId]);
GO

ALTER TABLE [ExamSchedules] ADD CONSTRAINT [FK_ExamSchedules_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [ExamSchedules] ADD CONSTRAINT [FK_ExamSchedules_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [ExamSchedules] ADD CONSTRAINT [FK_ExamSchedules_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260611160144_AddExamScheduleClassGroupSection', N'8.0.0');
GO

COMMIT;
GO

