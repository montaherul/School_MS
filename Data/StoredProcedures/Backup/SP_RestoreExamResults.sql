CREATE OR ALTER PROCEDURE [dbo].[SP_RestoreExamResults]
    @ExamId           INT,
    @BackupId         INT           = NULL,
    @RestoredBy       NVARCHAR(64)  = N'System_Restore',
    @RestoreLabel     NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @TargetLabel NVARCHAR(200);
        DECLARE @Now DATETIME2 = SYSDATETIME();

        IF @BackupId IS NOT NULL
        BEGIN
            SELECT @TargetLabel = [BackupLabel]
            FROM [backup].[StudentExamResults_Backup]
            WHERE [BackupId] = @BackupId;
            IF @TargetLabel IS NULL BEGIN RAISERROR('BackupId not found', 16, 1); ROLLBACK; RETURN; END
        END
        ELSE IF @RestoreLabel IS NOT NULL
            SET @TargetLabel = @RestoreLabel;
        ELSE
        BEGIN
            SELECT TOP 1 @TargetLabel = [BackupLabel]
            FROM [backup].[StudentExamResults_Backup]
            WHERE [ExamId] = @ExamId ORDER BY [BackupTimestamp] DESC;
            IF @TargetLabel IS NULL BEGIN RAISERROR('No backup found for ExamId %d', 16, 1, @ExamId); ROLLBACK; RETURN; END
        END

        IF NOT EXISTS (SELECT 1 FROM [backup].[StudentExamResults_Backup] WHERE [BackupLabel] = @TargetLabel AND [ExamId] = @ExamId)
        BEGIN RAISERROR('Backup data not found', 16, 1); ROLLBACK; RETURN; END

        -- Delete current results
        DELETE FROM [dbo].[StudentSubjectResults] WHERE [ExamId] = @ExamId;
        DELETE FROM [dbo].[StudentExamResults] WHERE [ExamId] = @ExamId;
        DELETE FROM [dbo].[ResultPublications] WHERE [ExamId] = @ExamId;

        -- Restore StudentExamResults
        SET IDENTITY_INSERT [dbo].[StudentExamResults] ON;
        INSERT INTO [dbo].[StudentExamResults]
            ([Id], [ExamId], [StudentId], [TotalMarks], [TotalFullMarks], [Gpa], [Grade],
             [Position], [ClassPosition], [GroupPosition], [IsPassed], [FailedSubjectCount], [PassedSubjectCount],
             [Status], [CalculatedAt], [PublishedAt], [AcademicYearId], [ClassId], [SectionId], [StudentGroupId],
             [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [Remarks])
        SELECT [OriginalId], [ExamId], [StudentId], [TotalMarks], [TotalFullMarks], [Gpa], [Grade],
               [Position], [ClassPosition], [GroupPosition], [IsPassed], [FailedSubjectCount], [PassedSubjectCount],
               [Status], [CalculatedAt], [PublishedAt], [AcademicYearId], [ClassId], [SectionId], [StudentGroupId],
               [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [Remarks]
        FROM [backup].[StudentExamResults_Backup]
        WHERE [BackupLabel] = @TargetLabel AND [ExamId] = @ExamId;
        SET IDENTITY_INSERT [dbo].[StudentExamResults] OFF;

        -- Restore StudentSubjectResults
        SET IDENTITY_INSERT [dbo].[StudentSubjectResults] ON;
        INSERT INTO [dbo].[StudentSubjectResults]
            ([Id], [ExamId], [StudentId], [SubjectId], [MarksObtained], [FullMarks], [PassMarks],
             [Grade], [GradePoint], [IsPassed], [IsOptionalSubject], [IsReligionSubject],
             [CalculatedAt], [AcademicYearId], [ClassId], [SectionId], [StudentGroupId],
             [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [Remarks])
        SELECT [OriginalId], [ExamId], [StudentId], [SubjectId], [MarksObtained], [FullMarks], [PassMarks],
               [Grade], [GradePoint], [IsPassed], [IsOptionalSubject], [IsReligionSubject],
               [CalculatedAt], [AcademicYearId], [ClassId], [SectionId], [StudentGroupId],
               [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted], [Remarks]
        FROM [backup].[StudentSubjectResults_Backup]
        WHERE [ExamId] = @ExamId AND [BackupLabel] = @TargetLabel;
        SET IDENTITY_INSERT [dbo].[StudentSubjectResults] OFF;

        -- Restore ResultPublications
        SET IDENTITY_INSERT [dbo].[ResultPublications] ON;
        INSERT INTO [dbo].[ResultPublications]
            ([Id], [ExamId], [Status], [PublishedAt], [ApprovedByUserId], [IsLocked], [LockedAt],
             [PublicationNotes], [AcademicYearId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted])
        SELECT [OriginalId], [ExamId], [Status], [PublishedAt], [ApprovedByUserId], [IsLocked], [LockedAt],
               [PublicationNotes], [AcademicYearId], [CreatedBy], [CreatedAt], [UpdatedBy], [UpdatedAt], [IsDeleted]
        FROM [backup].[ResultPublications_Backup]
        WHERE [ExamId] = @ExamId AND [BackupLabel] = @TargetLabel;
        SET IDENTITY_INSERT [dbo].[ResultPublications] OFF;

        -- Mark restore timestamp
        UPDATE [backup].[StudentExamResults_Backup]
        SET [RestoreTimestamp] = @Now, [RestoredBy] = TRY_CAST(@RestoredBy AS INT)
        WHERE [BackupLabel] = @TargetLabel AND [ExamId] = @ExamId;

        COMMIT TRANSACTION;

        SELECT
            'Restore Completed' AS RestoreStatus,
            @TargetLabel AS RestoredFromBackup,
            (SELECT COUNT(*) FROM [dbo].[StudentExamResults] WHERE [ExamId] = @ExamId) AS StudentExamResultsRestored,
            (SELECT COUNT(*) FROM [dbo].[StudentSubjectResults] WHERE [ExamId] = @ExamId) AS StudentSubjectResultsRestored,
            (SELECT COUNT(*) FROM [dbo].[ResultPublications] WHERE [ExamId] = @ExamId) AS ResultPublicationsRestored;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK;
        THROW;
    END CATCH;
END;
GO
