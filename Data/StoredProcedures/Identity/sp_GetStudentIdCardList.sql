-- ============================================================================
-- Stored Procedure: sp_GetStudentIdCardList
-- Purpose: Get paginated student records for ID Card management with filters
-- Author: School Management System
-- Created: June 14, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetStudentIdCardList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @ClassId INT = 0,
    @SectionId INT = 0,
    @GroupId INT = 0,
    @Status NVARCHAR(20) = NULL,
    @Gender NVARCHAR(20) = NULL,
    @AdmissionFrom DATETIME = NULL,
    @AdmissionTo DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT;
    SET @Offset = (@PageNumber - 1) * @PageSize;

    ;WITH FilteredStudents AS (
        SELECT 
            s.Id,
            s.StudentNo AS StudentCode,
            s.RollNumber AS RollNumber,
            s.FullName AS StudentName,
            s.ProfilePicturePath AS PhotoPath,
            c.Name AS ClassName,
            sec.Name AS SectionName,
            sg.Name AS GroupName,
            s.Gender,
            s.MobileNumber AS Phone,
            s.EmailAddress AS Email,
            s.Status,
            s.CreatedAt AS AdmissionDate,
            COALESCE(g.FullName, s.FatherName, '') AS GuardianName,
            ROW_NUMBER() OVER (ORDER BY s.CreatedAt DESC, s.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Students s
        LEFT JOIN 
            Classes c ON s.ClassId = c.Id AND c.IsDeleted = 0
        LEFT JOIN 
            Sections sec ON s.SectionId = sec.Id AND sec.IsDeleted = 0
        LEFT JOIN 
            StudentGroups sg ON s.StudentGroupId = sg.Id AND sg.IsDeleted = 0
        LEFT JOIN
            StudentGuardians sg_guard ON sg_guard.StudentId = s.Id AND sg_guard.IsPrimaryGuardian = 1 AND sg_guard.IsDeleted = 0
        LEFT JOIN
            Guardians g ON sg_guard.GuardianId = g.Id AND g.IsDeleted = 0
        WHERE 
            s.IsDeleted = 0
            AND (@ClassId = 0 OR s.ClassId = @ClassId)
            AND (@SectionId = 0 OR s.SectionId = @SectionId)
            AND (@GroupId = 0 OR s.StudentGroupId = @GroupId)
            AND (@Status IS NULL OR @Status = '' OR 
                CASE s.[Status]
                    WHEN 1 THEN 'Active'
                    WHEN 2 THEN 'Inactive'
                    WHEN 3 THEN 'Graduated'
                    WHEN 4 THEN 'Transferred'
                    WHEN 5 THEN 'Dropped'
                    ELSE 'Unknown'
                END = @Status)
            AND (@Gender IS NULL OR @Gender = '' OR s.Gender = @Gender)
            AND (@AdmissionFrom IS NULL OR s.CreatedAt >= @AdmissionFrom)
            AND (@AdmissionTo IS NULL OR s.CreatedAt <= @AdmissionTo)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR s.FullName LIKE '%' + @SearchTerm + '%'
                OR s.StudentNo LIKE '%' + @SearchTerm + '%'
                OR CAST(s.RollNumber AS NVARCHAR) LIKE '%' + @SearchTerm + '%'
                OR s.MobileNumber LIKE '%' + @SearchTerm + '%'
                OR s.EmailAddress LIKE '%' + @SearchTerm + '%'
                OR COALESCE(g.FullName, s.FatherName, '') LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        StudentCode,
        RollNumber,
        StudentName,
        PhotoPath,
        ClassName,
        SectionName,
        GroupName,
        Gender,
        Phone,
        Email,
        CASE [Status]
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'Graduated'
            WHEN 4 THEN 'Transferred'
            WHEN 5 THEN 'Dropped'
            ELSE 'Unknown'
        END AS [Status],
        GuardianName,
        AdmissionDate,
        TotalCount AS TotalRecords
    FROM 
        FilteredStudents
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
