CREATE OR ALTER PROCEDURE sp_GetStudentInvoicesPaged
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(MAX) = NULL,
    @StudentId INT,
    @Status INT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    WITH Filtered AS (
        SELECT
            fi.Id,
            fi.InvoiceNo,
            fi.DueDate,
            fi.TotalAmount,
            fi.PaidAmount,
            fi.DiscountAmount,
            fi.LateFee,
            fi.[Status],
            fi.CreatedAt AS InvoiceDate,
            ROW_NUMBER() OVER (ORDER BY fi.Id DESC) AS RowNum,
            COUNT(*) OVER () AS TotalCount
        FROM FeeInvoices fi
        WHERE fi.IsDeleted = 0
          AND fi.StudentId = @StudentId
          AND (@Status = 0 OR fi.[Status] = @Status)
          AND (@SearchTerm IS NULL OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%')
    )
    SELECT Id, InvoiceNo, InvoiceDate, DueDate, TotalAmount, PaidAmount,
           DiscountAmount, LateFee, [Status], TotalCount AS TotalRecords
    FROM Filtered
    WHERE RowNum > @Offset AND RowNum <= @Offset + @PageSize
    ORDER BY RowNum;
END;
GO