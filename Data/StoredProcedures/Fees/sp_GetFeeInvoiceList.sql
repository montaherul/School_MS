-- ============================================================================
-- Stored Procedure: sp_GetFeeInvoiceList
-- Purpose: Get paginated fee invoices with student details
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeInvoiceList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0,
    @Status INT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;


        SELECT 
            fi.Id,
            fi.InvoiceNo,
            fi.StudentId,
            s.FullName AS StudentName,
            fi.AcademicYearId,
            ay.Name AS AcademicYearName,
            fi.DueDate,
            fi.TotalAmount,
            fi.PaidAmount,
            fi.DiscountAmount,
            fi.LateFee,
            fi.[Status],
            fi.Remarks,

            COUNT(*) OVER () AS TotalRecords
        FROM 
FeeInvoices fi WITH(NOLOCK)
        JOIN 
Students s WITH(NOLOCK) ON fi.StudentId = s.Id
        LEFT JOIN 
AcademicYears ay WITH(NOLOCK) ON fi.AcademicYearId = ay.Id
        WHERE 
            fi.IsDeleted = 0
            AND (@StudentId = 0 OR fi.StudentId = @StudentId)
            AND (@Status = 0 OR fi.[Status] = @Status)
            AND (
                @SearchTerm IS NULL 
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR s.FullName LIKE '%' + @SearchTerm + '%'
            )
    
ORDER BY fi.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO
