-- ============================================================================
-- Stored Procedure: sp_GetAttendanceList
-- Purpose: Get paginated attendance list with student and class details
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetAttendanceList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH FilteredAttendance AS (
        SELECT 
            a.Id,
            a.StudentId,
            s.FullName AS StudentName,
            a.SchoolClassId,
            c.Name AS ClassName,
            a.SectionId,
            sec.Name AS SectionName,
            a.[Status],
            a.Remarks,
            a.CreatedAt,
            ROW_NUMBER() OVER (ORDER BY a.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Attendance a
        JOIN 
            Students s ON a.StudentId = s.Id
        JOIN 
            Classes c ON a.SchoolClassId = c.Id
        JOIN 
            Sections sec ON a.SectionId = sec.Id
        WHERE 
            a.IsDeleted = 0
            AND (@StudentId = 0 OR a.StudentId = @StudentId)
            AND (
                @SearchTerm IS NULL 
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo LIKE '%' + @SearchTerm + '%'
                OR a.Remarks LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        StudentId,
        StudentName,
        SchoolClassId,
        ClassName,
        SectionId,
        SectionName,
        [Status],
        Remarks,
        CreatedAt,
        TotalCount AS TotalRecords
    FROM 
        FilteredAttendance
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
