-- ============================================================
-- Missing Index Report — Production Performance Optimization
-- Generated for: SchoolManagementSystem
-- ============================================================
-- Focus: GPA, Merit, Report Card, Analytics queries
-- ============================================================

-- ═══ MARK ENTRY ═══
-- Used by: sp_SaveMarks, sp_GetMarksEntryList, sp_BulkImportMarks
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarkEntries_ExamId_SubjectId_StudentId')
    CREATE NONCLUSTERED INDEX [IX_MarkEntries_ExamId_SubjectId_StudentId]
        ON [dbo].[MarkEntries] ([ExamId], [SubjectId], [StudentId])
        INCLUDE ([TotalMarks], [Grade], [GradePoint], [IsLocked]);
GO

-- Used by: sp_GetTeacherMarksEntrySheet
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarkEntries_TeacherId_IsLocked')
    CREATE NONCLUSTERED INDEX [IX_MarkEntries_TeacherId_IsLocked]
        ON [dbo].[MarkEntries] ([TeacherId], [IsLocked])
        INCLUDE ([ExamId], [SubjectId], [StudentId], [TotalMarks]);
GO

-- Used by: Import validation
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MarkEntries_ExamSubjectStudent')
    CREATE UNIQUE NONCLUSTERED INDEX [IX_MarkEntries_ExamSubjectStudent]
        ON [dbo].[MarkEntries] ([ExamId], [SubjectId], [StudentId])
        WHERE ([ExamId] IS NOT NULL AND [SubjectId] IS NOT NULL AND [StudentId] IS NOT NULL);
GO

-- ═══ STUDENT EXAM RESULTS ═══
-- Used by: sp_GetResultList, sp_GetResultSummary, sp_CalculateMerit
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StudentExamResults_ExamId_StudentId')
    CREATE NONCLUSTERED INDEX [IX_StudentExamResults_ExamId_StudentId]
        ON [dbo].[StudentExamResults] ([ExamId], [StudentId])
        INCLUDE ([TotalMarks], [Grade], [GradePoint], [ClassPosition], [GroupPosition], [IsPublished], [PublishedAt]);
GO

-- Used by: sp_CalculateMerit (PARTITION BY ClassId, ExamGroupId ORDER BY GradePoint DESC)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StudentExamResults_MeritCalc')
    CREATE NONCLUSTERED INDEX [IX_StudentExamResults_MeritCalc]
        ON [dbo].[StudentExamResults] ([ExamId], [ClassId], [ExamGroupId], [GradePoint] DESC)
        INCLUDE ([StudentId], [TotalMarks], [GroupPosition]);
GO

-- Used by: sp_GetReportCard, sp_BulkGenerateReportCards
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StudentExamResults_ReportCard')
    CREATE NONCLUSTERED INDEX [IX_StudentExamResults_ReportCard]
        ON [dbo].[StudentExamResults] ([ExamId], [ClassId], [StudentId])
        INCLUDE ([Grade], [GradePoint], [TotalMarks], [ClassPosition], [GroupPosition], [IsPublished]);
GO

-- Used by: Dashboard / Analytics
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StudentExamResults_Dashboard')
    CREATE NONCLUSTERED INDEX [IX_StudentExamResults_Dashboard]
        ON [dbo].[StudentExamResults] ([ExamId], [IsPublished])
        INCLUDE ([StudentId], [ClassId], [ExamGroupId], [GradePoint], [TotalMarks], [Grade]);
GO

-- ═══ STUDENT SUBJECT RESULTS ═══
-- Used by: sp_GetResultList (subject details), sp_GetReportCard
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StudentSubjectResults_ExamResult_Subject')
    CREATE NONCLUSTERED INDEX [IX_StudentSubjectResults_ExamResult_Subject]
        ON [dbo].[StudentSubjectResults] ([StudentExamResultId], [SubjectId])
        INCLUDE ([TotalMarks], [Grade], [GradePoint], [ComponentMarksJson]);
GO

-- ═══ EXAM SUBJECTS ═══
-- Used by: sp_GetExamComponents, sp_GetSubjectMarkStructure
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ExamSubjects_ExamId_ClassId')
    CREATE NONCLUSTERED INDEX [IX_ExamSubjects_ExamId_ClassId]
        ON [dbo].[ExamSubjects] ([ExamId], [ClassId])
        INCLUDE ([SubjectId], [StudentGroupId], [FullMarks], [PassMarks]);
GO

-- ═══ EXAMS ═══
-- Used by: Dashboard KPI queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Exams_Status_AcademicYearId')
    CREATE NONCLUSTERED INDEX [IX_Exams_Status_AcademicYearId]
        ON [dbo].[Exams] ([Status], [AcademicYearId])
        INCLUDE ([ExamName], [ExamType], [StartDate], [EndDate]);
GO

-- ═══ ATTENDANCE ═══
-- Used by: Report card attendance summary, Analytics
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Attendance_Student_Date')
    CREATE NONCLUSTERED INDEX [IX_Attendance_Student_Date]
        ON [dbo].[Attendances] ([StudentId], [AttendanceDate] DESC)
        INCLUDE ([Status]);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Attendance_Class_Date')
    CREATE NONCLUSTERED INDEX [IX_Attendance_Class_Date]
        ON [dbo].[Attendances] ([ClassId], [SectionId], [AttendanceDate] DESC)
        INCLUDE ([StudentId], [Status]);
GO

-- Used by: sp_GetAttendanceAnalytics
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Attendance_Analytics')
    CREATE NONCLUSTERED INDEX [IX_Attendance_Analytics]
        ON [dbo].[Attendances] ([AttendanceDate], [Status])
        INCLUDE ([StudentId], [ClassId], [SectionId]);
GO

-- ═══ RESULT PUBLICATIONS ═══
-- Lookup by exam
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ResultPublications_ExamId')
    CREATE NONCLUSTERED INDEX [IX_ResultPublications_ExamId]
        ON [dbo].[ResultPublications] ([ExamId])
        INCLUDE ([PublishedAt], [PublishedBy], [Status]);
GO

-- ═══ SP_GetMissingIndexes ═══
PRINT 'Missing index report generated. Run the above CREATE INDEX statements in sequence.';
GO
