-- ============================================================================
-- Stored Procedure: sp_GetFeeInvoiceList
-- Purpose: Get paginated fee invoices with student details
-- Author: School Management System
-- Created: May 4, 2026
-- ============================================================================

CREATE OR ALTER PROCEDURE sp_GetFeeInvoiceList
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT = 0
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
            fi.DueDate,
            fi.TotalAmount,
            fi.PaidAmount,
            fi.[Status],
            ROW_NUMBER() OVER (ORDER BY fi.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM 
            FeeInvoices fi
        JOIN 
            Students s ON fi.StudentId = s.Id
        WHERE 
            fi.IsDeleted = 0
            AND (@StudentId = 0 OR fi.StudentId = @StudentId)
            AND (
                @SearchTerm IS NULL 
                OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%'
                OR s.FullName LIKE '%' + @SearchTerm + '%'
            )
    )
    SELECT 
        Id,
        InvoiceNo,
        StudentId,
        StudentName,
        DueDate,
        TotalAmount,
        PaidAmount,
        [Status],
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
