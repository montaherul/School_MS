SET QUOTED_IDENTIFIER ON;
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_SaveSubjectMarkStructure]
    @ComponentId INT,
    @SubjectId INT = NULL,
    @ClassId INT = NULL,
    @StudentGroupId INT = NULL,
    @FullMarks DECIMAL(18,2),
    @PassMarks DECIMAL(18,2),
    @DisplayOrder INT = 0,
    @CreatedBy NVARCHAR(100) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ExistingId INT;

    SELECT @ExistingId = Id
    FROM SubjectMarkStructures
    WHERE ComponentId = @ComponentId
      AND (@SubjectId IS NULL OR SubjectId = @SubjectId)
      AND (@ClassId IS NULL OR ClassId = @ClassId)
      AND (@StudentGroupId IS NULL OR StudentGroupId = @StudentGroupId)
      AND IsDeleted = 0;

    -- Ensure existing record is not soft-deleted
    IF @ExistingId IS NOT NULL
    BEGIN
        UPDATE SubjectMarkStructures
        SET IsDeleted = 0
        WHERE Id = @ExistingId AND IsDeleted = 1;

        -- Reset the flag if we just restored
        SELECT @ExistingId = Id
        FROM SubjectMarkStructures
        WHERE Id = @ExistingId;
    END

    IF @ExistingId IS NOT NULL
    BEGIN
        UPDATE SubjectMarkStructures
        SET FullMarks = @FullMarks,
            PassMarks = @PassMarks,
            DisplayOrder = @DisplayOrder,
            UpdatedAt = GETUTCDATE(),
            UpdatedBy = @CreatedBy
        WHERE Id = @ExistingId;

        SELECT @ExistingId AS Id, 'Updated' AS Action;
    END
    ELSE
    BEGIN
        INSERT INTO SubjectMarkStructures
            (ComponentId, SubjectId, ClassId, StudentGroupId,
             FullMarks, PassMarks, DisplayOrder, IsActive,
             IsDeleted, CreatedAt, CreatedBy)
        VALUES
            (@ComponentId, @SubjectId, @ClassId, @StudentGroupId,
             @FullMarks, @PassMarks, @DisplayOrder, 1,
             0, GETUTCDATE(), @CreatedBy);

        SELECT SCOPE_IDENTITY() AS Id, 'Created' AS Action;
    END
END;
GO
