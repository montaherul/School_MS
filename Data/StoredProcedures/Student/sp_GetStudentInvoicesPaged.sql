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

            COUNT(*) OVER () AS TotalRecords
FROM FeeInvoices fi WITH(NOLOCK)
        WHERE fi.IsDeleted = 0
          AND fi.StudentId = @StudentId
          AND (@Status = 0 OR fi.[Status] = @Status)
          AND (@SearchTerm IS NULL OR fi.InvoiceNo LIKE '%' + @SearchTerm + '%')
    
ORDER BY fi.Id DESC
OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;

END;
GO