-- ============================================================================
-- EXAM MODULE RESET - SAFE CASCADE DELETION
-- Removes ALL exam-related data while preserving non-exam entities.
-- Wrapped in transaction with full rollback on failure.
-- ============================================================================
-- DEPENDENCY REPORT (generated from sys.foreign_keys):
--   ReEvaluationRequests.ExamId     → Exams.Id
--   ResultAuditLogs.ExamId          → Exams.Id
--   ResultLocks.ExamId              → Exams.Id
--   ResultPublications.ExamId       → Exams.Id
--   AdmitCards.ExamId               → Exams.Id
--   Marks.ExamId                    → Exams.Id
--   StudentSubjectResults.ExamId    → Exams.Id
--   StudentExamResults.ExamId       → Exams.Id
--   ExamSchedules.ExamId            → Exams.Id
--   SubjectMarkStructures.CompId    → ExamComponents.Id
--   ExamSubjects.ExamId             → Exams.Id
--   ExamSchedules.ClassId           → Classes.Id
--   ExamSchedules.SectionId         → Sections.Id
--   ExamSchedules.StudentGroupId    → StudentGroups.Id
--   ExamSchedules.SubjectId         → Subjects.Id
--   Marks.StudentId                 → Students.Id
--   Marks.SubjectId                 → Subjects.Id
--   Marks.EnteredByTeacherId        → Teachers.Id
--   StudentSubjectResults.StudentId → Students.Id
--   StudentSubjectResults.SubjectId → Subjects.Id
--   StudentExamResults.StudentId    → Students.Id
-- ============================================================================
-- DELETION ORDER (FK-safe):
--   1. ReEvaluationRequests  (FK→Exams)
--   2. ResultAuditLogs       (FK→Exams)
--   3. ResultLocks           (FK→Exams)
--   4. ResultPublications    (FK→Exams)
--   5. AdmitCards            (FK→Exams)
--   6. Marks                 (FK→Exams, Students, Subjects, Teachers)
--   7. StudentSubjectResults (FK→Exams, Students, Subjects)
--   8. StudentExamResults    (FK→Exams, Students)
--   9. FinalResults          (aggregate, no FK to Exams)
--  10. PromotionHistories    (no FK to Exams)
--  11. ExamSchedules         (FK→Exams, Classes, Sections, Groups, Subjects)
--  12. SubjectMarkStructures (FK→ExamComponents, Classes, Groups, Subjects)
--  13. ExamSubjects          (FK→Exams, Subjects, Teachers)
--  14. Exams                 (central hub)
-- ============================================================================

BEGIN TRANSACTION;
BEGIN TRY

    DECLARE @counts TABLE (TableName NVARCHAR(100), RowsDeleted INT);

    -- ========================================================================
    -- SAFE DELETE: Disable FK checks temporarily (only if needed)
    -- We don't need to disable FKs because the order is safe.
    -- ========================================================================

    -- 1. ReEvaluationRequests
    DELETE FROM ReEvaluationRequests;
    INSERT INTO @counts SELECT 'ReEvaluationRequests', @@ROWCOUNT;

    -- 2. ResultAuditLogs
    DELETE FROM ResultAuditLogs;
    INSERT INTO @counts SELECT 'ResultAuditLogs', @@ROWCOUNT;

    -- 3. ResultLocks
    DELETE FROM ResultLocks;
    INSERT INTO @counts SELECT 'ResultLocks', @@ROWCOUNT;

    -- 4. ResultPublications
    DELETE FROM ResultPublications;
    INSERT INTO @counts SELECT 'ResultPublications', @@ROWCOUNT;

    -- 5. AdmitCards
    DELETE FROM AdmitCards;
    INSERT INTO @counts SELECT 'AdmitCards', @@ROWCOUNT;

    -- 6. Marks (FK to Exams, Students, Subjects, Teachers — all preserved)
    DELETE FROM Marks;
    INSERT INTO @counts SELECT 'Marks', @@ROWCOUNT;

    -- 7. StudentSubjectResults
    DELETE FROM StudentSubjectResults;
    INSERT INTO @counts SELECT 'StudentSubjectResults', @@ROWCOUNT;

    -- 8. StudentExamResults
    DELETE FROM StudentExamResults;
    INSERT INTO @counts SELECT 'StudentExamResults', @@ROWCOUNT;

    -- 9. FinalResults (no FK to Exams — deletes all; development reset)
    DELETE FROM FinalResults;
    INSERT INTO @counts SELECT 'FinalResults', @@ROWCOUNT;

    -- 10. PromotionHistories (no FK to Exams — deletes all; development reset)
    DELETE FROM PromotionHistories;
    INSERT INTO @counts SELECT 'PromotionHistories', @@ROWCOUNT;

    -- 11. ExamSchedules (FK to Exams, Classes, Sections, StudentGroups, Subjects)
    DELETE FROM ExamSchedules;
    INSERT INTO @counts SELECT 'ExamSchedules', @@ROWCOUNT;

    -- 12. SubjectMarkStructures (FK to ExamComponents, Classes, StudentGroups, Subjects)
    DELETE FROM SubjectMarkStructures;
    INSERT INTO @counts SELECT 'SubjectMarkStructures', @@ROWCOUNT;

    -- 13. ExamSubjects (FK to Exams, Subjects, Teachers)
    DELETE FROM ExamSubjects;
    INSERT INTO @counts SELECT 'ExamSubjects', @@ROWCOUNT;

    -- 14. Exams (central hub — all dependents already removed)
    DELETE FROM Exams;
    INSERT INTO @counts SELECT 'Exams', @@ROWCOUNT;

    -- ========================================================================
    -- OUTPUT: Records deleted per table
    -- ========================================================================
    PRINT '=== RECORDS DELETED ===';
    SELECT TableName, RowsDeleted FROM @counts WHERE RowsDeleted > 0 ORDER BY TableName;

    -- ========================================================================
    -- POST-RESET VALIDATION: All counts must be 0
    -- ========================================================================
    PRINT '=== POST-RESET VALIDATION ===';
    SELECT 'AdmitCards' AS TableName, COUNT(*) AS Remaining FROM AdmitCards
    UNION ALL SELECT 'ExamSchedules', COUNT(*) FROM ExamSchedules
    UNION ALL SELECT 'ExamSubjects', COUNT(*) FROM ExamSubjects
    UNION ALL SELECT 'Exams', COUNT(*) FROM Exams
    UNION ALL SELECT 'FinalResults', COUNT(*) FROM FinalResults
    UNION ALL SELECT 'Marks', COUNT(*) FROM Marks
    UNION ALL SELECT 'PromotionHistories', COUNT(*) FROM PromotionHistories
    UNION ALL SELECT 'ReEvaluationRequests', COUNT(*) FROM ReEvaluationRequests
    UNION ALL SELECT 'ResultAuditLogs', COUNT(*) FROM ResultAuditLogs
    UNION ALL SELECT 'ResultLocks', COUNT(*) FROM ResultLocks
    UNION ALL SELECT 'ResultPublications', COUNT(*) FROM ResultPublications
    UNION ALL SELECT 'StudentExamResults', COUNT(*) FROM StudentExamResults
    UNION ALL SELECT 'StudentSubjectResults', COUNT(*) FROM StudentSubjectResults
    UNION ALL SELECT 'SubjectMarkStructures', COUNT(*) FROM SubjectMarkStructures;

    DECLARE @totalRemaining INT = 0;
    SELECT @totalRemaining = SUM(Remaining) FROM (
        SELECT COUNT(*) AS Remaining FROM AdmitCards
        UNION ALL SELECT COUNT(*) FROM ExamSchedules
        UNION ALL SELECT COUNT(*) FROM ExamSubjects
        UNION ALL SELECT COUNT(*) FROM Exams
        UNION ALL SELECT COUNT(*) FROM FinalResults
        UNION ALL SELECT COUNT(*) FROM Marks
        UNION ALL SELECT COUNT(*) FROM PromotionHistories
        UNION ALL SELECT COUNT(*) FROM ReEvaluationRequests
        UNION ALL SELECT COUNT(*) FROM ResultAuditLogs
        UNION ALL SELECT COUNT(*) FROM ResultLocks
        UNION ALL SELECT COUNT(*) FROM ResultPublications
        UNION ALL SELECT COUNT(*) FROM StudentExamResults
        UNION ALL SELECT COUNT(*) FROM StudentSubjectResults
        UNION ALL SELECT COUNT(*) FROM SubjectMarkStructures
    ) t;

    IF @totalRemaining = 0
    BEGIN
        PRINT 'Exam module reset completed successfully.';
    END
    ELSE
    BEGIN
        PRINT 'WARNING: ' + CAST(@totalRemaining AS NVARCHAR) + ' records remain. Review validation output above.';
    END

    -- ========================================================================
    -- ORPHAN CHECK: Verify no records reference deleted Exams
    -- ========================================================================
    PRINT '=== ORPHAN CHECK ===';
    SELECT 'AdmitCards' AS TableName, COUNT(*) AS Orphans FROM AdmitCards ac LEFT JOIN Exams e ON ac.ExamId = e.Id WHERE e.Id IS NULL AND ac.ExamId IS NOT NULL
    UNION ALL SELECT 'ExamSchedules', COUNT(*) FROM ExamSchedules es LEFT JOIN Exams e ON es.ExamId = e.Id WHERE e.Id IS NULL AND es.ExamId IS NOT NULL
    UNION ALL SELECT 'ExamSubjects', COUNT(*) FROM ExamSubjects es LEFT JOIN Exams e ON es.ExamId = e.Id WHERE e.Id IS NULL AND es.ExamId IS NOT NULL
    UNION ALL SELECT 'Marks', COUNT(*) FROM Marks m LEFT JOIN Exams e ON m.ExamId = e.Id WHERE e.Id IS NULL AND m.ExamId IS NOT NULL
    UNION ALL SELECT 'ReEvaluationRequests', COUNT(*) FROM ReEvaluationRequests r LEFT JOIN Exams e ON r.ExamId = e.Id WHERE e.Id IS NULL AND r.ExamId IS NOT NULL
    UNION ALL SELECT 'ResultAuditLogs', COUNT(*) FROM ResultAuditLogs r LEFT JOIN Exams e ON r.ExamId = e.Id WHERE e.Id IS NULL AND r.ExamId IS NOT NULL
    UNION ALL SELECT 'ResultLocks', COUNT(*) FROM ResultLocks r LEFT JOIN Exams e ON r.ExamId = e.Id WHERE e.Id IS NULL AND r.ExamId IS NOT NULL
    UNION ALL SELECT 'ResultPublications', COUNT(*) FROM ResultPublications r LEFT JOIN Exams e ON r.ExamId = e.Id WHERE e.Id IS NULL AND r.ExamId IS NOT NULL
    UNION ALL SELECT 'StudentExamResults', COUNT(*) FROM StudentExamResults s LEFT JOIN Exams e ON s.ExamId = e.Id WHERE e.Id IS NULL AND s.ExamId IS NOT NULL
    UNION ALL SELECT 'StudentSubjectResults', COUNT(*) FROM StudentSubjectResults s LEFT JOIN Exams e ON s.ExamId = e.Id WHERE e.Id IS NULL AND s.ExamId IS NOT NULL;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'ERROR: Transaction rolled back.';
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
GO
