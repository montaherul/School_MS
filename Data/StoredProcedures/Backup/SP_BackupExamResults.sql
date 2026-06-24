CREATE OR ALTER PROCEDURE [dbo].[SP_BackupExamResults]
    @ExamId           INT,
    @BackupLabel      NVARCHAR(200) = NULL,
    @BackupId         INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF NOT EXISTS (SELECT 1 FROM sys.schemas WHERE name = 'backup')
        EXEC('CREATE SCHEMA [backup]');

    -- StudentExamResults backup â€” stores all columns including audit
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StudentExamResults_Backup' AND SCHEMA_NAME(schema_id) = 'backup')
    BEGIN
        CREATE TABLE [backup].[StudentExamResults_Backup] (
            [BackupId]        INT             IDENTITY(1,1) PRIMARY KEY,
            [OriginalId]      INT             NOT NULL,
            [ExamId]          INT             NOT NULL,
            [StudentId]       INT             NOT NULL,
            [TotalMarks]      DECIMAL(10,2)   NULL,
            [TotalFullMarks]  DECIMAL(10,2)   NULL,
            [Gpa]             DECIMAL(4,2)    NULL,
            [Grade]           NVARCHAR(10)    NULL,
            [Position]        INT             NULL,
            [ClassPosition]   INT             NULL,
            [GroupPosition]   INT             NULL,
            [IsPassed]        BIT             NULL,
            [FailedSubjectCount] INT          NULL,
            [PassedSubjectCount] INT          NULL,
            [Status]          INT             NULL,
            [CalculatedAt]    DATETIME2       NULL,
            [PublishedAt]     DATETIME2       NULL,
            [AcademicYearId]  INT             NULL,
            [ClassId]         INT             NULL,
            [SectionId]       INT             NULL,
            [StudentGroupId]  INT             NULL,
            [CreatedBy]       NVARCHAR(64)    NULL,
            [CreatedAt]       DATETIME2       NULL,
            [UpdatedBy]       NVARCHAR(64)    NULL,
            [UpdatedAt]       DATETIME2       NULL,
            [IsDeleted]       BIT             NULL,
            [Remarks]         NVARCHAR(MAX)   NULL,
            [BackupLabel]     NVARCHAR(200)   NULL,
            [BackupTimestamp] DATETIME2       NOT NULL DEFAULT SYSDATETIME(),
            [RestoreTimestamp] DATETIME2      NULL,
            [RestoredBy]      INT             NULL
        );
    END

    -- StudentSubjectResults backup
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'StudentSubjectResults_Backup' AND SCHEMA_NAME(schema_id) = 'backup')
    BEGIN
        CREATE TABLE [backup].[StudentSubjectResults_Backup] (
            [BackupId]        INT             IDENTITY(1,1) PRIMARY KEY,
            [OriginalId]      INT             NOT NULL,
            [ExamId]          INT             NOT NULL,
            [StudentId]       INT             NOT NULL,
            [SubjectId]       INT             NOT NULL,
            [MarksObtained]   DECIMAL(10,2)   NULL,
            [FullMarks]       DECIMAL(10,2)   NULL,
            [PassMarks]       DECIMAL(10,2)   NULL,
            [Grade]           NVARCHAR(10)    NULL,
            [GradePoint]      DECIMAL(4,2)    NULL,
            [IsPassed]        BIT             NULL,
            [IsOptionalSubject] BIT           NULL,
            [IsReligionSubject] BIT           NULL,
            [CalculatedAt]    DATETIME2       NULL,
            [AcademicYearId]  INT             NULL,
            [ClassId]         INT             NULL,
            [SectionId]       INT             NULL,
            [StudentGroupId]  INT             NULL,
            [CreatedBy]       NVARCHAR(64)    NULL,
            [CreatedAt]       DATETIME2       NULL,
            [UpdatedBy]       NVARCHAR(64)    NULL,
            [UpdatedAt]       DATETIME2       NULL,
            [IsDeleted]       BIT             NULL,
            [Remarks]         NVARCHAR(MAX)   NULL,
            [BackupLabel]     NVARCHAR(200)   NULL,
            [BackupTimestamp] DATETIME2       NOT NULL DEFAULT SYSDATETIME()
        );
    END

    -- ResultPublications backup
    IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ResultPublications_Backup' AND SCHEMA_NAME(schema_id) = 'backup')
    BEGIN
        CREATE TABLE [backup].[ResultPublications_Backup] (
            [BackupId]        INT             IDENTITY(1,1) PRIMARY KEY,
            [OriginalId]      INT             NOT NULL,
            [ExamId]          INT             NOT NULL,
            [Status]          INT             NULL,
            [PublishedAt]     DATETIME2       NULL,
            [ApprovedByUserId] INT            NULL,
            [IsLocked]        BIT             NULL,
            [LockedAt]        DATETIME2       NULL,
            [PublicationNotes] NVARCHAR(MAX)  NULL,
            [AcademicYearId]  INT             NULL,
            [CreatedBy]       NVARCHAR(64)    NULL,
            [CreatedAt]       DATETIME2       NULL,
            [UpdatedBy]       NVARCHAR(64)    NULL,
            [UpdatedAt]       DATETIME2       NULL,
            [IsDeleted]       BIT             NULL,
            [BackupLabel]     NVARCHAR(200)   NULL,
            [BackupTimestamp] DATETIME2       NOT NULL DEFAULT SYSDATETIME()
        );
    END

    -- Backup StudentExamResults
    INSERT INTO [backup].[StudentExamResults_Backup]
        ([OriginalId], [ExamId], [StudentId], [TotalMarks], [TotalFullMarks], [Gpa], [Grade],
         [Position], [ClassPosition], [GroupPosition], [IsPassed], [FailedSubjectCount], [PassedSubjectCount],
         [Status], [CalculatedAt], [PublishedAt], [AcademicYearId], [ClassId], [SectionId], [StudentGroupId],
         [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [Remarks], [BackupLabel])
    SELECT ser.[Id], ser.[ExamId], ser.[StudentId], ser.[TotalMarks], ser.[TotalFullMarks], ser.[Gpa], ser.[Grade],
           ser.[Position], ser.[ClassPosition], ser.[GroupPosition], ser.[IsPassed], ser.[FailedSubjectCount], ser.[PassedSubjectCount],
           ser.[Status], ser.[CalculatedAt], ser.[PublishedAt], ser.[AcademicYearId], ser.[ClassId], ser.[SectionId], ser.[StudentGroupId],
           ser.[CreatedBy], ser.[CreatedAt], ser.[UpdatedBy], ser.[UpdatedAt], ser.[IsDeleted], ser.[Remarks],
           ISNULL(@BackupLabel, 'Backup before ' + FORMAT(SYSDATETIME(), 'yyyy-MM-dd HH:mm:ss'))
    FROM [dbo].[StudentExamResults] ser
    WHERE ser.[ExamId] = @ExamId;

    -- Backup StudentSubjectResults
    INSERT INTO [backup].[StudentSubjectResults_Backup]
        ([OriginalId], [ExamId], [StudentId], [SubjectId], [MarksObtained], [FullMarks], [PassMarks],
         [Grade], [GradePoint], [IsPassed], [IsOptionalSubject], [IsReligionSubject],
         [CalculatedAt], [AcademicYearId], [ClassId], [SectionId], [StudentGroupId],
         [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [Remarks], [BackupLabel])
    SELECT ssr.[Id], ssr.[ExamId], ssr.[StudentId], ssr.[SubjectId], ssr.[MarksObtained], ssr.[FullMarks], ssr.[PassMarks],
           ssr.[Grade], ssr.[GradePoint], ssr.[IsPassed], ssr.[IsOptionalSubject], ssr.[IsReligionSubject],
           ssr.[CalculatedAt], ssr.[AcademicYearId], ssr.[ClassId], ssr.[SectionId], ssr.[StudentGroupId],
           ssr.[CreatedBy], ssr.[CreatedAt], ssr.[UpdatedBy], ssr.[UpdatedAt], ssr.[IsDeleted], ssr.[Remarks],
           ISNULL(@BackupLabel, 'Backup before ' + FORMAT(SYSDATETIME(), 'yyyy-MM-dd HH:mm:ss'))
    FROM [dbo].[StudentSubjectResults] ssr
    WHERE ssr.[ExamId] = @ExamId;

    -- Backup ResultPublications
    INSERT INTO [backup].[ResultPublications_Backup]
        ([OriginalId], [ExamId], [Status], [PublishedAt], [ApprovedByUserId], [IsLocked], [LockedAt],
         [PublicationNotes], [AcademicYearId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [BackupLabel])
    SELECT rp.[Id], rp.[ExamId], rp.[Status], rp.[PublishedAt], rp.[ApprovedByUserId], rp.[IsLocked], rp.[LockedAt],
           rp.[PublicationNotes], rp.[AcademicYearId], rp.[CreatedBy], rp.[CreatedAt], rp.[UpdatedBy], rp.[UpdatedAt], rp.[IsDeleted],
           ISNULL(@BackupLabel, 'Backup before ' + FORMAT(SYSDATETIME(), 'yyyy-MM-dd HH:mm:ss'))
    FROM [dbo].[ResultPublications] rp
    WHERE rp.[ExamId] = @ExamId;

    SET @BackupId = SCOPE_IDENTITY();

    SELECT
        'Backup Completed' AS BackupStatus,
        (SELECT COUNT(*) FROM [backup].[StudentExamResults_Backup] WHERE [ExamId] = @ExamId AND [BackupLabel] = ISNULL(@BackupLabel, 'Backup before ' + FORMAT(SYSDATETIME(), 'yyyy-MM-dd HH:mm:ss'))) AS StudentExamResultsBackedUp,
        (SELECT COUNT(*) FROM [backup].[StudentSubjectResults_Backup] WHERE [ExamId] = @ExamId AND [BackupLabel] = ISNULL(@BackupLabel, 'Backup before ' + FORMAT(SYSDATETIME(), 'yyyy-MM-dd HH:mm:ss'))) AS StudentSubjectResultsBackedUp,
        (SELECT COUNT(*) FROM [backup].[ResultPublications_Backup] WHERE [ExamId] = @ExamId AND [BackupLabel] = ISNULL(@BackupLabel, 'Backup before ' + FORMAT(SYSDATETIME(), 'yyyy-MM-dd HH:mm:ss'))) AS ResultPublicationsBackedUp;
END;
GO
