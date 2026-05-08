using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations;

/// <inheritdoc />
public partial class AddStoredProcedures : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            CREATE PROCEDURE sp_GetAdmissionsWithPagination
                @PageNumber INT = 1,
                @PageSize INT = 10,
                @SearchTerm NVARCHAR(MAX) = NULL,
                @ClassId INT = NULL,
                @StatusFilter NVARCHAR(20) = NULL,
                @TotalCount INT OUTPUT
            AS
            BEGIN
                SET NOCOUNT ON;

                -- Validate input parameters
                IF @PageNumber < 1 SET @PageNumber = 1;
                IF @PageSize < 1 SET @PageSize = 10;
                IF @PageSize > 1000 SET @PageSize = 1000;

                DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;

                -- Get total count
                SELECT @TotalCount = COUNT(*)
                FROM Admissions aa
                WHERE aa.IsDeleted = 0
                    AND (@SearchTerm IS NULL OR 
                        aa.ApplicationNo LIKE '%' + @SearchTerm + '%' OR
                        aa.ApplicantName LIKE '%' + @SearchTerm + '%' OR
                        aa.ApplicantMobileNumber LIKE '%' + @SearchTerm + '%' OR
                        aa.ApplicantEmail LIKE '%' + @SearchTerm + '%')
                    AND (@ClassId IS NULL OR aa.AppliedClassId = @ClassId)
                    AND (@StatusFilter IS NULL OR aa.Status = @StatusFilter);

                -- Get paginated data
                SELECT 
                    aa.Id,
                    aa.ApplicationNo,
                    aa.ApplicantName,
                    aa.DateOfBirth,
                    aa.Gender,
                    aa.ApplicantMobileNumber,
                    aa.ApplicantEmail,
                    aa.ProfilePicturePath,
                    aa.Status,
                    c.Name AS ClassName,
                    aa.CreatedAt,
                    aa.CreatedBy,
                    DATEDIFF(DAY, aa.CreatedAt, GETUTCDATE()) AS DaysApplied,
                    CAST(CASE WHEN aa.Status = 'Approved' THEN 1 WHEN aa.Status = 'Rejected' THEN 2 ELSE 0 END AS INT) AS StatusOrder
                FROM Admissions aa
                LEFT JOIN SchoolClasses c ON aa.AppliedClassId = c.Id
                WHERE aa.IsDeleted = 0
                    AND (@SearchTerm IS NULL OR 
                        aa.ApplicationNo LIKE '%' + @SearchTerm + '%' OR
                        aa.ApplicantName LIKE '%' + @SearchTerm + '%' OR
                        aa.ApplicantMobileNumber LIKE '%' + @SearchTerm + '%' OR
                        aa.ApplicantEmail LIKE '%' + @SearchTerm + '%')
                    AND (@ClassId IS NULL OR aa.AppliedClassId = @ClassId)
                    AND (@StatusFilter IS NULL OR aa.Status = @StatusFilter)
                ORDER BY aa.CreatedAt DESC
                OFFSET @Offset ROWS
                FETCH NEXT @PageSize ROWS ONLY;
            END;
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP PROCEDURE IF EXISTS sp_GetAdmissionsWithPagination;");
    }
}
