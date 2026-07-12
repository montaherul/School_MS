CREATE OR ALTER PROCEDURE sp_GetPromotionCandidates
    @ClassId INT,
    @AcademicYearId INT,
    @MinGPA DECIMAL(4,2) = 1.00,
    @MinAttendance DECIMAL(5,2) = 60.00,
    @SearchTerm NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        s.Id AS StudentId, s.StudentNo, s.FullName, s.RollNumber,
        sc.Name AS ClassName, sec.Name AS SectionName,
        fr.FinalGpa AS GPA, fr.AttendancePercentage,
        fr.TotalFailedSubjects, fr.IsPassed,
        CASE 
            WHEN fr.IsPassed = 1 AND fr.FinalGpa >= @MinGPA AND fr.AttendancePercentage >= @MinAttendance THEN 'Eligible'
            WHEN fr.IsPassed = 0 THEN 'Failed'
            WHEN fr.FinalGpa < @MinGPA THEN 'LowGPA'
            WHEN fr.AttendancePercentage < @MinAttendance THEN 'LowAttendance'
            ELSE 'Pending'
        END AS EligibilityStatus,
        COUNT(*) OVER() AS TotalRecords
    FROM Students s WITH(NOLOCK)
    INNER JOIN FinalResults fr WITH(NOLOCK) ON fr.StudentId = s.Id AND fr.AcademicYearId = @AcademicYearId
    INNER JOIN SchoolClasses sc WITH(NOLOCK) ON sc.Id = s.ClassId
    INNER JOIN Sections sec WITH(NOLOCK) ON sec.Id = s.SectionId
    WHERE s.IsDeleted = 0 AND s.ClassId = @ClassId
      AND s.Status = 'Active'
      AND (@SearchTerm IS NULL OR s.FullName LIKE '%' + @SearchTerm + '%')
    ORDER BY fr.FinalGpa DESC, s.RollNumber ASC;
END;
