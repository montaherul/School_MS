CREATE OR ALTER PROCEDURE sp_AssignStudentToSection
    @StudentId INT,
    @SectionId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @CurrentCount INT;
    DECLARE @Capacity INT;

    -- Get the capacity of the section (default to 50 if somehow null)
    SELECT @Capacity = ISNULL(Capacity, 50)
FROM Sections WITH(NOLOCK)
    WHERE Id = @SectionId AND IsDeleted = 0;

    IF @Capacity IS NULL
    BEGIN
        RAISERROR ('Invalid Section', 16, 1);
        RETURN;
    END

    -- Count active students currently in this section
    SELECT @CurrentCount = COUNT(*) 
FROM Students WITH(NOLOCK) 
    WHERE SectionId = @SectionId AND IsDeleted = 0 AND Status = 1; -- 1 = Active

    -- Check if section is full
    IF @CurrentCount >= @Capacity
    BEGIN
        DECLARE @ErrorMsg NVARCHAR(200);
        SET @ErrorMsg = 'Section is full (Max ' + CAST(@Capacity AS NVARCHAR(10)) + ' students allowed)';
        RAISERROR (@ErrorMsg, 16, 1);
        RETURN;
    END

    -- Safe to assign
    UPDATE Students
    SET SectionId = @SectionId
    WHERE Id = @StudentId;
END
