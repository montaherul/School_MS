CREATE OR ALTER PROCEDURE [dbo].[SP_System_VerifyStoredProcedures]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalSqlFiles INT;
    DECLARE @TotalProcedures INT;

    SELECT @TotalProcedures = COUNT(*)
    FROM sys.procedures
    WHERE is_ms_shipped = 0;

    -- Compare against expected list
    SELECT
        sp.name AS StoredProcedureName,
        SCHEMA_NAME(sp.schema_id) AS SchemaName,
        sp.create_date AS CreatedDate,
        sp.modify_date AS ModifiedDate,
        CASE WHEN sp.type_desc = 'PROCEDURE' THEN 'Present' ELSE 'Unknown' END AS DeploymentStatus
    INTO #ActualProcs
    FROM sys.procedures sp
    WHERE sp.is_ms_shipped = 0;

    DECLARE @ExpectedProcs TABLE (ProcName NVARCHAR(255));
    INSERT INTO @ExpectedProcs VALUES
        -- Exam
        ('sp_GetExamList'), ('sp_GetExamDashboard'), ('sp_GetExamScheduleList'), ('sp_GetExamComponents'),
        ('sp_GetExamMarkStructure'), ('sp_GetMarksEntryList'), ('sp_GetSubjectMarkStructure'), ('sp_SaveSubjectMarkStructure'),
        ('sp_GetGroupReport'), ('SP_Exam_GetAllResults'), ('SP_Exam_DashboardSummary'),
        -- Marks
        ('sp_GetTeacherMarksEntrySheet'), ('sp_GetTeacherResultSummary'), ('sp_GetTeacherExportSheet'),
        ('sp_SaveMarks'), ('sp_BulkImportMarks'), ('sp_LockMarksEntry'), ('sp_UnlockMarksEntry'),
        ('SP_MarkEntry_GetGrid'),
        -- Results (fixed)
        ('sp_GetMarkEntrySheet_Fixed'), ('sp_GetExamsForAdmin_Fixed'), ('sp_CalculateExamRanking_Fixed'),
        -- Result
        ('sp_GetResultList'), ('sp_GetResultSummary'), ('sp_GetStudentResults'), ('sp_GetReportCard'),
        ('sp_GetTranscript'), ('sp_GetResultPublicationDashboard'),
        ('sp_CalculateSubjectResults'), ('sp_CalculateExamResults'), ('sp_CalculateMerit'),
        ('sp_RecalculateResults'), ('sp_PublishResults'), ('sp_UnpublishResults'),
        -- ReportCard
        ('sp_BulkGenerateReportCards'), ('SP_ReportCard_Generate'),
        -- AdmitCard
        ('sp_GenerateAdmitCard'), ('sp_BulkGenerateAdmitCards'),
        -- Analytics
        ('sp_GetClassSummary'), ('sp_GetStudentTrend'), ('sp_GetGroupSummary'),
        -- Academic
        ('sp_GetAcademicYearList'), ('sp_GetClassList'), ('sp_GetSectionList'), ('sp_GetSubjectList'),
        ('sp_SeedClassSubjectMappings_BD'), ('sp_AssignStudentToSection'),
        -- Identity
        ('sp_GetStudentIdCardList'), ('sp_GetStudentIdCardBulkData'),
        ('sp_GetEmployeeIdCardList'), ('sp_GetEmployeeIdCardBulkData'),
        -- Attendance
        ('sp_GetAttendanceList'), ('sp_GetAttendanceSummary'), ('sp_GetAttendanceSessions'),
        ('sp_GetAttendanceHistory'), ('sp_GetAttendanceRevisionHistory'),
        ('sp_GetAttendanceDashboardSummary'), ('sp_GetAttendanceAnalytics'),
        ('sp_GetClassAttendanceAnalytics'), ('sp_GetEmployeeAttendanceAnalytics'),
        ('sp_GetStudentAttendanceList'), ('sp_GetEmployeeAttendanceList'),
        ('sp_GetAbsentStudents'), ('sp_GetLateStudents'),
        -- Students
        ('sp_GetStudentList'),
        -- Teacher
        ('sp_GetTeacherList'), ('sp_GetTeacherAssignedExams'), ('sp_GetTeacherAssignedSubjects'),
        -- Guardian
        ('sp_GetGuardianList'), ('sp_GetGuardianDetails'), ('sp_GetGuardianDashboard'),
        ('sp_GetGuardianChildren'), ('sp_GetGuardianAttendance'), ('sp_GetGuardianResults'),
        ('sp_GetGuardianFees'), ('sp_GetGuardianNotifications'), ('sp_GetGuardianLeaveApplications'),
        ('sp_VerifyGuardianDataIntegrity'),
        -- User / Role / Employee / Admission
        ('sp_GetUserList'), ('sp_GetRoleList'), ('sp_GetEmployeeInvitationList'), ('sp_GetAdmissionList'),
        -- Fees
        ('sp_GetFeeStructureList'), ('sp_GetFeeInvoiceList');

    -- Missing procedures
    SELECT ep.ProcName AS MissingProcedure
    FROM @ExpectedProcs ep
    LEFT JOIN #ActualProcs ap ON ep.ProcName = ap.StoredProcedureName
    WHERE ap.StoredProcedureName IS NULL;

    -- Summary
    SELECT
        (SELECT COUNT(*) FROM @ExpectedProcs) AS TotalExpected,
        (SELECT COUNT(*) FROM #ActualProcs) AS TotalDeployed,
        (SELECT COUNT(*) FROM @ExpectedProcs ep
         LEFT JOIN #ActualProcs ap ON ep.ProcName = ap.StoredProcedureName
         WHERE ap.StoredProcedureName IS NULL) AS MissingCount,
        CASE
            WHEN (SELECT COUNT(*) FROM @ExpectedProcs ep
                  LEFT JOIN #ActualProcs ap ON ep.ProcName = ap.StoredProcedureName
                  WHERE ap.StoredProcedureName IS NULL) = 0
            THEN 'ALL DEPLOYED'
            ELSE 'MISSING PROCEDURES'
        END AS DeploymentStatus;

    DROP TABLE #ActualProcs;
END;
GO
