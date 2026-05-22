CREATE OR ALTER PROCEDURE sp_CalculateExamRanking
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    MERGE StudentExamResults AS target
    USING (
        SELECT 
            m.StudentId,
            SUM(m.MarksObtained) AS TotalMarks,
            AVG(CAST(m.GradePoint AS DECIMAL(18,2))) AS Gpa,
            CASE 
                WHEN MIN(CASE WHEN m.Grade = 'F' THEN 0 ELSE 1 END) = 0 
                THEN 0 
                ELSE 1 
            END AS IsPassed
        FROM Marks m
        WHERE m.ExamId = @ExamId 
          AND m.IsDeleted = 0
        GROUP BY m.StudentId
    ) AS source
    ON (
        target.StudentId = source.StudentId 
        AND target.ExamId = @ExamId
    )

    WHEN MATCHED THEN
        UPDATE SET 
            TotalMarks = source.TotalMarks,
            Gpa = source.Gpa,
            IsPassed = source.IsPassed,
            Status = 4,
            PublishedAt = GETUTCDATE(),
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = 'system'

    WHEN NOT MATCHED THEN
        INSERT (
            ExamId,
            StudentId,
            TotalMarks,
            Gpa,
            IsPassed,
            Position,
            Status,
            PublishedAt,
            CreatedAt,
            CreatedBy,
            IsDeleted,
            Grade
        )
        VALUES (
            @ExamId,
            source.StudentId,
            source.TotalMarks,
            source.Gpa,
            source.IsPassed,
            0,
            4,
            GETUTCDATE(),
            GETUTCDATE(),
            'system',
            0,
            ''
        );

    WITH RankedResults AS (
        SELECT 
            r.Id,
            RANK() OVER (
                PARTITION BY s.ClassId 
                ORDER BY r.TotalMarks DESC, r.Gpa DESC
            ) AS NewPosition
        FROM StudentExamResults r
        JOIN Students s 
            ON r.StudentId = s.Id
        WHERE r.ExamId = @ExamId 
          AND r.IsDeleted = 0
    )

    UPDATE r
    SET r.Position = rr.NewPosition
    FROM StudentExamResults r
    JOIN RankedResults rr 
        ON r.Id = rr.Id;
END;
GO