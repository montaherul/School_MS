CREATE OR ALTER PROCEDURE [dbo].[sp_GetGuardianList]
    @SearchTerm NVARCHAR(100) = NULL,
    @Status NVARCHAR(20) = NULL,
    @PageNumber INT = 1,
    @PageSize INT = 20
AS
BEGIN
    SET NOCOUNT ON;

    -- Get total count
    SELECT COUNT(*) FROM Guardians g
    WHERE g.IsDeleted = 0
      AND (@SearchTerm IS NULL OR 
           g.FirstName LIKE '%' + @SearchTerm + '%' OR 
           g.LastName LIKE '%' + @SearchTerm + '%' OR 
           g.GuardianCode LIKE '%' + @SearchTerm + '%' OR 
           g.MobileNumber LIKE '%' + @SearchTerm + '%' OR
           g.Email LIKE '%' + @SearchTerm + '%')
      AND (@Status IS NULL OR
           CASE g.Status
                WHEN 1 THEN 'Active'
                WHEN 2 THEN 'Inactive'
                WHEN 3 THEN 'PendingActivation'
                ELSE 'Unknown'
           END = @Status);

    -- Get paginated list
    SELECT 
        g.Id,
        g.GuardianCode,
        (g.FirstName + ' ' + g.LastName) AS FullName,
        g.MobileNumber,
        g.Email,
        CASE g.RelationType
            WHEN 1 THEN 'Father'
            WHEN 2 THEN 'Mother'
            WHEN 3 THEN 'LegalGuardian'
            WHEN 4 THEN 'Grandfather'
            WHEN 5 THEN 'Grandmother'
            WHEN 6 THEN 'Uncle'
            WHEN 7 THEN 'Aunt'
            WHEN 8 THEN 'Brother'
            WHEN 9 THEN 'Sister'
            WHEN 10 THEN 'Other'
            ELSE 'Other'
        END AS RelationType,
        CASE g.Status
            WHEN 1 THEN 'Active'
            WHEN 2 THEN 'Inactive'
            WHEN 3 THEN 'PendingActivation'
            ELSE 'Unknown'
        END AS Status,
        (SELECT COUNT(*) FROM StudentGuardians sg WHERE sg.GuardianId = g.Id) AS ChildrenCount,
        g.CreatedAt
    FROM Guardians g
    WHERE g.IsDeleted = 0
      AND (@SearchTerm IS NULL OR 
           g.FirstName LIKE '%' + @SearchTerm + '%' OR 
           g.LastName LIKE '%' + @SearchTerm + '%' OR 
           g.GuardianCode LIKE '%' + @SearchTerm + '%' OR 
           g.MobileNumber LIKE '%' + @SearchTerm + '%' OR
           g.Email LIKE '%' + @SearchTerm + '%')
      AND (@Status IS NULL OR
           CASE g.Status
                WHEN 1 THEN 'Active'
                WHEN 2 THEN 'Inactive'
                WHEN 3 THEN 'PendingActivation'
                ELSE 'Unknown'
           END = @Status)
    ORDER BY g.CreatedAt DESC, g.Id DESC
    OFFSET (@PageNumber - 1) * @PageSize ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END
GO
