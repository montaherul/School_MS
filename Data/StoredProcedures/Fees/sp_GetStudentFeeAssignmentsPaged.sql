-- ============================================================================
-- Stored Procedure: sp_GetStudentFeeAssignmentsPaged
-- Purpose: Get paginated student fee assignments
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetStudentFeeAssignmentsPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0,
    @FeeStructureId INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

    WITH Data AS (
        SELECT 
            sfa.Id,
            sfa.StudentId,
            st.FullName AS StudentName,
            st.StudentNo,
            sfa.FeeStructureId,
            fs.FeeName AS FeeStructureName,
            sfa.AcademicYearId,
            ay.Name AS AcademicYearName,
            sfa.CustomAmount,
            sfa.IsActive,
            sfa.ValidFrom,
            sfa.ValidTo,
            ROW_NUMBER() OVER (ORDER BY st.FullName, fs.FeeName) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            StudentFeeAssignments sfa
        INNER JOIN 
            Students st ON sfa.StudentId = st.Id
        INNER JOIN 
            FeeStructures fs ON sfa.FeeStructureId = fs.Id
        LEFT JOIN 
            AcademicYears ay ON sfa.AcademicYearId = ay.Id
        WHERE 
            sfa.IsDeleted = 0
            AND (@StudentId = 0 OR sfa.StudentId = @StudentId)
            AND (@FeeStructureId = 0 OR sfa.FeeStructureId = @FeeStructureId)
            AND (
                @SearchTerm IS NULL 
                OR st.FullName LIKE '%' + @SearchTerm + '%'
                OR st.StudentNo LIKE '%' + @SearchTerm + '%'
                OR fs.FeeName LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, StudentId, StudentName, StudentNo,
        FeeStructureId, FeeStructureName,
        AcademicYearId, AcademicYearName,
        CustomAmount, IsActive, ValidFrom, ValidTo,
        TotalCount AS TotalRecords
    FROM 
        Data
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
