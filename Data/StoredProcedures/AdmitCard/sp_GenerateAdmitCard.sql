CREATE OR ALTER PROCEDURE [dbo].[sp_GenerateAdmitCard]
    @ExamId INT,
    @StudentId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CardNo NVARCHAR(50);
    SELECT @CardNo = 'AC-' + CAST(@ExamId AS NVARCHAR) + '-' + CAST(@StudentId AS NVARCHAR);

    MERGE AdmitCards AS target
    USING (SELECT @ExamId AS ExamId, @StudentId AS StudentId) AS source
    ON (target.ExamId = source.ExamId AND target.StudentId = source.StudentId AND target.IsDeleted = 0)
    WHEN MATCHED THEN
        UPDATE SET
            IsGenerated = 1,
            IsIssued = 1,
            IssuedAt = GETUTCDATE(),
            PrintedAt = GETUTCDATE(),
            CardNo = @CardNo,
            UpdatedAt = GETUTCDATE()
    WHEN NOT MATCHED THEN
        INSERT (ExamId, StudentId, CardNo, IsIssued, IsGenerated, IssuedAt, PrintedAt, CreatedAt, IsDeleted)
        VALUES (@ExamId, @StudentId, @CardNo, 1, 1, GETUTCDATE(), GETUTCDATE(), GETUTCDATE(), 0);

    -- Return admit card data
    SELECT
        ac.Id,
        ac.CardNo,
        ac.AdmitCardNumber,
        ac.RollNumber,
        ac.SeatNumber,
        s.FullName AS StudentName,
        s.StudentNo,
        s.RollNumber AS StudentRoll,
        c.Name AS ClassName,
        sec.Name AS SectionName,
        sg.Name AS GroupName,
        s.ProfilePicturePath AS PhotoPath,
        e.Name AS ExamName,
        e.StartsOn,
        e.EndsOn
FROM AdmitCards ac WITH(NOLOCK)
INNER JOIN Students s WITH(NOLOCK) ON ac.StudentId = s.Id
INNER JOIN Exams e WITH(NOLOCK) ON ac.ExamId = e.Id
INNER JOIN Classes c WITH(NOLOCK) ON s.ClassId = c.Id
LEFT JOIN Sections sec WITH(NOLOCK) ON s.SectionId = sec.Id
LEFT JOIN StudentGroups sg WITH(NOLOCK) ON s.StudentGroupId = sg.Id
    WHERE ac.ExamId = @ExamId AND ac.StudentId = @StudentId AND ac.IsDeleted = 0;
END;
GO
