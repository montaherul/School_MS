SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Now DATETIME2 = SYSUTCDATETIME();
    DECLARE @SYSTEM NVARCHAR(64) = 'system';
    DECLARE @AcademicYearId INT = 1;

    PRINT '--- Fixing StudentGroupId for Class 9-10 students ---';

    UPDATE Students
    SET StudentGroupId = CASE SectionId
        WHEN 18 THEN 1 WHEN 19 THEN 1
        WHEN 21 THEN 2 WHEN 22 THEN 2
        WHEN 24 THEN 3 WHEN 25 THEN 3
        WHEN 27 THEN 1 WHEN 28 THEN 1
        WHEN 30 THEN 2 WHEN 31 THEN 2
        WHEN 33 THEN 3 WHEN 34 THEN 3
        ELSE StudentGroupId
    END
    WHERE ClassId IN (9,10) AND StudentGroupId IS NULL;

    PRINT 'Updated: ' + CAST(@@ROWCOUNT AS NVARCHAR) + ' students';

    -- Get Class 9-10 Half Yearly exam IDs
    DECLARE @ExamIds TABLE (ExamId INT, ClassId INT, StudentGroupId INT);
    INSERT INTO @ExamIds
    SELECT Id, ClassId, StudentGroupId
    FROM Exams
    WHERE ClassId IN (9,10) AND StudentGroupId IS NOT NULL
      AND Term = 2 AND IsDeleted = 0;

    -- Add marks
    DECLARE @NextMarkId INT;
    SELECT @NextMarkId = ISNULL(MAX(Id), 0) + 1 FROM Marks;

    SET IDENTITY_INSERT Marks ON;

    INSERT INTO Marks (
        Id, ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
        WrittenMarks, MCQMarks, CQMarks, PracticalMarks,
        MarksObtained, Grade, GradePoint,
        EnteredByTeacherId, Status, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY ES.ExamId, S.Id, ES.SubjectId) + @NextMarkId - 1,
        ES.ExamId, S.Id, ES.SubjectId, @AcademicYearId,
        ES.ClassId, S.SectionId, ES.StudentGroupId,
        CASE WHEN ES.StudentGroupId = 1 AND ES.SubjectId IN (16,17,18,19) THEN ROUND(10 + (ABS(CHECKSUM(NEWID())) % 36), 0) ELSE ROUND(15 + (ABS(CHECKSUM(NEWID())) % 53), 0) END,
        CASE WHEN ES.StudentGroupId = 1 AND ES.SubjectId IN (16,17,18,19) THEN NULL ELSE ROUND(5 + (ABS(CHECKSUM(NEWID())) % 24), 0) END,
        CASE WHEN ES.StudentGroupId = 1 AND ES.SubjectId IN (16,17,18,19) THEN ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0) ELSE NULL END,
        CASE WHEN ES.StudentGroupId = 1 AND ES.SubjectId IN (16,17,18,19) THEN ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0) ELSE NULL END,
        CASE WHEN ES.StudentGroupId = 1 AND ES.SubjectId IN (16,17,18,19)
            THEN ROUND(10 + (ABS(CHECKSUM(NEWID())) % 36), 0) + ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0) + ROUND(5 + (ABS(CHECKSUM(NEWID())) % 21), 0)
            ELSE ROUND(15 + (ABS(CHECKSUM(NEWID())) % 53), 0) + ROUND(5 + (ABS(CHECKSUM(NEWID())) % 24), 0)
        END,
        NULL, NULL, 1, 5, 0, @Now, @SYSTEM, NULL, NULL, 0
    FROM ExamSubjects ES
    INNER JOIN Students S ON S.ClassId = ES.ClassId AND S.StudentGroupId = ES.StudentGroupId
    WHERE ES.ExamId IN (SELECT ExamId FROM @ExamIds) AND ES.IsDeleted = 0
      AND S.IsDeleted = 0 AND S.Status = 1
      AND NOT EXISTS (SELECT 1 FROM Marks M WHERE M.ExamId = ES.ExamId AND M.StudentId = S.Id AND M.SubjectId = ES.SubjectId);

    SET IDENTITY_INSERT Marks OFF;

    UPDATE Marks SET
        Grade = CASE WHEN MarksObtained >= 80 THEN N'A+' WHEN MarksObtained >= 70 THEN N'A' WHEN MarksObtained >= 60 THEN N'A-' WHEN MarksObtained >= 50 THEN N'B' WHEN MarksObtained >= 40 THEN N'C' WHEN MarksObtained >= 33 THEN N'D' ELSE N'F' END,
        GradePoint = CASE WHEN MarksObtained >= 80 THEN 5.00 WHEN MarksObtained >= 70 THEN 4.00 WHEN MarksObtained >= 60 THEN 3.50 WHEN MarksObtained >= 50 THEN 3.00 WHEN MarksObtained >= 40 THEN 2.00 WHEN MarksObtained >= 33 THEN 1.00 ELSE 0.00 END
    WHERE ExamId IN (SELECT ExamId FROM @ExamIds) AND Grade IS NULL;

    PRINT 'Marks: ' + CAST(@@ROWCOUNT AS NVARCHAR);

    -- StudentSubjectResults
    DECLARE @NextSSRId INT;
    SELECT @NextSSRId = ISNULL(MAX(Id), 0) + 1 FROM StudentSubjectResults;

    SET IDENTITY_INSERT StudentSubjectResults ON;
    INSERT INTO StudentSubjectResults (
        Id, ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
        IsOptionalSubject, IsReligionSubject, MarksObtained, FullMarks, PassMarks,
        Grade, GradePoint, IsPassed, CalculatedAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY ES.ExamId, S.Id, ES.SubjectId) + @NextSSRId - 1,
        ES.ExamId, S.Id, ES.SubjectId, @AcademicYearId,
        ES.ClassId, S.SectionId, ES.StudentGroupId,
        0, 0, M.MarksObtained, ES.FullMarks, ES.PassMarks,
        M.Grade, M.GradePoint,
        CASE WHEN M.MarksObtained >= 33 THEN 1 ELSE 0 END,
        @Now, @Now, @SYSTEM, NULL, NULL, 0
    FROM ExamSubjects ES
    INNER JOIN Students S ON S.ClassId = ES.ClassId AND S.StudentGroupId = ES.StudentGroupId
    INNER JOIN Marks M ON M.ExamId = ES.ExamId AND M.StudentId = S.Id AND M.SubjectId = ES.SubjectId
    WHERE ES.ExamId IN (SELECT ExamId FROM @ExamIds) AND ES.IsDeleted = 0 AND S.IsDeleted = 0
      AND NOT EXISTS (SELECT 1 FROM StudentSubjectResults SSR WHERE SSR.ExamId = ES.ExamId AND SSR.StudentId = S.Id AND SSR.SubjectId = ES.SubjectId);
    SET IDENTITY_INSERT StudentSubjectResults OFF;
    PRINT 'StudentSubjectResults: ' + CAST(@@ROWCOUNT AS NVARCHAR);

    -- StudentExamResults
    DECLARE @NextSERId INT;
    SELECT @NextSERId = ISNULL(MAX(Id), 0) + 1 FROM StudentExamResults;

    SET IDENTITY_INSERT StudentExamResults ON;
    INSERT INTO StudentExamResults (
        Id, ExamId, StudentId, AcademicYearId, ClassId, SectionId, StudentGroupId,
        TotalMarks, TotalFullMarks, Gpa, Grade, Position, ClassPosition, GroupPosition,
        IsPassed, FailedSubjectCount, PassedSubjectCount,
        Status, PublishedAt, CalculatedAt, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
    )
    SELECT
        ROW_NUMBER() OVER (ORDER BY A.ExamId, A.StudentId) + @NextSERId - 1,
        A.ExamId, A.StudentId, @AcademicYearId, A.ClassId, A.SectionId, A.StudentGroupId,
        A.TotalMarks, A.TotalFullMarks, A.Gpa,
        CASE WHEN A.Gpa >= 5.00 THEN N'A+' WHEN A.Gpa >= 4.00 THEN N'A' WHEN A.Gpa >= 3.50 THEN N'A-' WHEN A.Gpa >= 3.00 THEN N'B' WHEN A.Gpa >= 2.00 THEN N'C' WHEN A.Gpa >= 1.00 THEN N'D' ELSE N'F' END,
        0, 0, NULL,
        CASE WHEN A.FailedCount = 0 THEN 1 ELSE 0 END, A.FailedCount, A.PassedCount,
        5, @Now, @Now, @Now, @SYSTEM, NULL, NULL, 0
    FROM (
        SELECT SSR.ExamId, SSR.StudentId, SSR.ClassId, SSR.SectionId, SSR.StudentGroupId,
            SUM(SSR.MarksObtained) AS TotalMarks, SUM(SSR.FullMarks) AS TotalFullMarks,
            ROUND(AVG(SSR.GradePoint), 2) AS Gpa,
            SUM(CASE WHEN SSR.IsPassed = 0 THEN 1 ELSE 0 END) AS FailedCount,
            SUM(CASE WHEN SSR.IsPassed = 1 THEN 1 ELSE 0 END) AS PassedCount
        FROM StudentSubjectResults SSR
        WHERE SSR.ExamId IN (SELECT ExamId FROM @ExamIds)
        GROUP BY SSR.ExamId, SSR.StudentId, SSR.ClassId, SSR.SectionId, SSR.StudentGroupId
    ) A
    WHERE NOT EXISTS (SELECT 1 FROM StudentExamResults SER WHERE SER.ExamId = A.ExamId AND SER.StudentId = A.StudentId);
    SET IDENTITY_INSERT StudentExamResults OFF;
    PRINT 'StudentExamResults: ' + CAST(@@ROWCOUNT AS NVARCHAR);

    -- Merit positions
    WITH RankedResults AS (
        SELECT Id,
            DENSE_RANK() OVER (PARTITION BY ExamId, ClassId ORDER BY Gpa DESC, TotalMarks DESC) AS NewPosition,
            DENSE_RANK() OVER (PARTITION BY ExamId, ClassId ORDER BY Gpa DESC, TotalMarks DESC) AS NewClassPosition,
            CASE WHEN StudentGroupId IS NOT NULL
                THEN DENSE_RANK() OVER (PARTITION BY ExamId, ClassId, StudentGroupId ORDER BY Gpa DESC, TotalMarks DESC)
                ELSE NULL END AS NewGroupPosition
        FROM StudentExamResults
        WHERE ExamId IN (SELECT ExamId FROM @ExamIds) AND Position = 0
    )
    UPDATE SER SET SER.Position = R.NewPosition, SER.ClassPosition = R.NewClassPosition, SER.GroupPosition = R.NewGroupPosition
    FROM StudentExamResults SER INNER JOIN RankedResults R ON R.Id = SER.Id;
    PRINT 'Merit positions: ' + CAST(@@ROWCOUNT AS NVARCHAR);

    -- PublishedAt
    UPDATE SER SET SER.PublishedAt = @Now
    FROM StudentExamResults SER
    WHERE SER.ExamId IN (SELECT ExamId FROM @ExamIds) AND SER.PublishedAt IS NULL;

    -- AdmitCards
    DECLARE @NextACId INT;
    SELECT @NextACId = ISNULL(MAX(Id), 0) + 1 FROM AdmitCards;

    SET IDENTITY_INSERT AdmitCards ON;
    INSERT INTO AdmitCards (Id, ExamId, StudentId, CardNo, RollNumber, IsIssued, IsGenerated, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT
        ROW_NUMBER() OVER (ORDER BY E.Id, S.Id) + @NextACId - 1,
        E.Id, S.Id, N'ADC-' + CAST(E.Id AS NVARCHAR) + '-' + RIGHT('0000' + CAST(S.RollNumber AS NVARCHAR), 4),
        S.RollNumber, 1, 1, @Now, @SYSTEM, NULL, NULL, 0
    FROM Exams E
    INNER JOIN Students S ON S.ClassId = E.ClassId AND S.StudentGroupId = E.StudentGroupId
    WHERE E.Id IN (SELECT ExamId FROM @ExamIds) AND S.IsDeleted = 0 AND S.Status = 1
      AND NOT EXISTS (SELECT 1 FROM AdmitCards AC WHERE AC.ExamId = E.Id AND AC.StudentId = S.Id);
    SET IDENTITY_INSERT AdmitCards OFF;
    PRINT 'AdmitCards: ' + CAST(@@ROWCOUNT AS NVARCHAR);

    -- Final summary
    SELECT
        E.Id AS ExamId, E.Name,
        COUNT(DISTINCT ES.SubjectId) AS Subjects,
        COUNT(DISTINCT M.StudentId) AS StudentsWithMarks,
        COUNT(DISTINCT SER.Id) AS StudentResults,
        COUNT(DISTINCT AC.Id) AS AdmitCards
    FROM Exams E
    LEFT JOIN ExamSubjects ES ON ES.ExamId = E.Id AND ES.IsDeleted = 0
    LEFT JOIN Marks M ON M.ExamId = E.Id
    LEFT JOIN StudentExamResults SER ON SER.ExamId = E.Id
    LEFT JOIN AdmitCards AC ON AC.ExamId = E.Id
    WHERE E.Id IN (SELECT ExamId FROM @ExamIds)
    GROUP BY E.Id, E.Name ORDER BY E.Id;

    COMMIT TRANSACTION;
    PRINT 'Fix complete!';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    PRINT 'ERROR: ' + ERROR_MESSAGE();
    PRINT 'Line: ' + CAST(ERROR_LINE() AS NVARCHAR);
    THROW;
END CATCH;
