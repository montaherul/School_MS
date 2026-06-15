-- ============================================================================
-- Stored Procedure: sp_GetEmployeeIdCardList
-- Purpose: Get paginated employee records for ID Card management with filters
-- Author: School Management System
-- Created: June 14, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetEmployeeIdCardList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @DepartmentId INT = 0,
    @DesignationId INT = 0,
    @Status NVARCHAR(20) = NULL,
    @EmploymentType NVARCHAR(50) = NULL,
    @JoiningFrom DATETIME = NULL,
    @JoiningTo DATETIME = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT;
    SET @Offset = (@PageNumber - 1) * @PageSize;

    ;WITH FilteredEmployees AS (
        SELECT 
            e.Id,
            e.EmployeeCode,
            e.FullName AS EmployeeName,
            e.ProfilePicturePath AS PhotoPath,
            e.Phone,
            e.Email,
            e.Status,
            e.IsTeachingStaff,
            e.EmployeeType AS EmploymentType,
            e.JoiningDate,
            e.EmployeeCardNumber,
            e.CardIssueDate,
            e.CardExpiryDate,
            e.CardPrintedAt,
            e.CardVersion,
            COALESCE(d.Name, '') AS DepartmentName,
            COALESCE(desig.Name, '') AS DesignationName,
            ROW_NUMBER() OVER (ORDER BY e.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            Employees e
        LEFT JOIN 
            Departments d ON e.DepartmentId = d.Id AND d.IsDeleted = 0
        LEFT JOIN 
            Designations desig ON e.DesignationId = desig.Id AND desig.IsDeleted = 0
        WHERE 
            e.IsDeleted = 0
            AND (@DepartmentId = 0 OR e.DepartmentId = @DepartmentId)
            AND (@DesignationId = 0 OR e.DesignationId = @DesignationId)
            AND (@Status IS NULL OR @Status = '' OR e.Status = @Status)
            AND (@EmploymentType IS NULL OR @EmploymentType = '' OR e.EmployeeType = @EmploymentType)
            AND (@JoiningFrom IS NULL OR e.JoiningDate >= @JoiningFrom)
            AND (@JoiningTo IS NULL OR e.JoiningDate <= @JoiningTo)
            AND (
                @SearchTerm IS NULL OR @SearchTerm = ''
                OR e.FullName LIKE '%' + @SearchTerm + '%'
                OR e.EmployeeCode LIKE '%' + @SearchTerm + '%'
                OR e.Phone LIKE '%' + @SearchTerm + '%'
                OR e.Email LIKE '%' + @SearchTerm + '%'
                OR COALESCE(d.Name, '') LIKE '%' + @SearchTerm + '%'
                OR COALESCE(desig.Name, '') LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        EmployeeCode,
        EmployeeName,
        PhotoPath,
        Phone,
        Email,
        Status,
        IsTeachingStaff,
        EmploymentType,
        JoiningDate,
        EmployeeCardNumber,
        CardIssueDate,
        CardExpiryDate,
        CardPrintedAt,
        CardVersion,
        DepartmentName,
        DesignationName,
        TotalCount AS TotalRecords
    FROM 
        FilteredEmployees
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
