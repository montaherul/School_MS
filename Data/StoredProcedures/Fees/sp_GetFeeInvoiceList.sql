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

    WITH FilteredInvoices AS (
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
            ROW_NUMBER() OVER (ORDER BY fi.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeInvoices fi
        JOIN 
            Students s ON fi.StudentId = s.Id
        LEFT JOIN 
            AcademicYears ay ON fi.AcademicYearId = ay.Id
        WHERE 
            fi.IsDeleted = 0
            AND (@StudentId = 0 OR fi.StudentId = @StudentId)
            AND (@Status = 0 OR fi.[Status] = @Status)
            AND (
                @SearchTerm IS NULL 
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR s.FullName LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id, InvoiceNo, StudentId, StudentName,
        AcademicYearId, AcademicYearName,
        DueDate, TotalAmount, PaidAmount, DiscountAmount, LateFee,
        [Status], Remarks,
        TotalCount AS TotalRecords
    FROM 
        FilteredInvoices
    WHERE 
        RowNum > @Offset 
        AND RowNum <= @Offset + @PageSize
    ORDER BY 
        RowNum;
END;
GO
