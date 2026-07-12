CREATE OR ALTER PROCEDURE sp_CopyExam
    @SourceExamId INT,
    @TargetAcademicYearId INT,
    @NewName NVARCHAR(100) = NULL,
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @NewExamId INT;
    DECLARE @SourceName NVARCHAR(100);
    DECLARE @TargetYearName NVARCHAR(100);

    -- Get source exam name
    SELECT @SourceName = Name FROM Exams WHERE Id = @SourceExamId AND IsDeleted = 0;
    IF @SourceName IS NULL
    BEGIN
        RAISERROR('Source exam not found', 16, 1);
        RETURN;
    END

    -- Get target academic year name
    SELECT @TargetYearName = Name FROM AcademicYears WHERE Id = @TargetAcademicYearId AND IsDeleted = 0;
    IF @TargetYearName IS NULL
    BEGIN
        RAISERROR('Target academic year not found', 16, 1);
        RETURN;
    END

    -- Generate new name if not provided
    IF @NewName IS NULL OR LEN(@NewName) = 0
        SET @NewName = @SourceName + N' (Copy - ' + @TargetYearName + N')';

    -- Check for duplicate
    IF EXISTS (SELECT 1 FROM Exams WHERE AcademicYearId = @TargetAcademicYearId AND Name = @NewName AND IsDeleted = 0)
    BEGIN
        RAISERROR('An exam with this name already exists for the target academic year', 16, 1);
        RETURN;
    END

    BEGIN TRANSACTION;
    BEGIN TRY
        -- 1. Create new Exam
        INSERT INTO Exams (Name, AcademicYearId, Term, StartsOn, EndsOn, 
            Status, IsPublished, IsLocked, ClassId, SectionId, StudentGroupId,
            CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
        SELECT @NewName, @TargetAcademicYearId, Term, StartsOn, EndsOn,
            0, 0, 0, ClassId, SectionId, StudentGroupId,
            @UserId, GETUTCDATE(), @UserId, GETUTCDATE()
        FROM Exams WHERE Id = @SourceExamId;

        SET @NewExamId = SCOPE_IDENTITY();

        -- 2. Copy ExamClasses
        INSERT INTO ExamClasses (ExamId, ClassId, ClassName, SortOrder, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
        SELECT @NewExamId, ClassId, ClassName, SortOrder, @UserId, GETUTCDATE(), @UserId, GETUTCDATE()
        FROM ExamClasses WHERE ExamId = @SourceExamId AND IsDeleted = 0;

        -- 3. Copy ExamSections
        INSERT INTO ExamSections (ExamClassId, SectionId, SectionName, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
        SELECT nc.Id, oc.SectionId, oc.SectionName, @UserId, GETUTCDATE(), @UserId, GETUTCDATE()
        FROM ExamSections oc
        INNER JOIN ExamClasses oc_class ON oc_class.Id = oc.ExamClassId
        INNER JOIN ExamClasses nc ON nc.ExamId = @NewExamId AND nc.ClassId = oc_class.ClassId
        WHERE oc_class.ExamId = @SourceExamId AND oc.IsDeleted = 0;

        -- 4. Copy ExamSubjects (with full snapshots)
        INSERT INTO ExamSubjects (ExamId, SubjectId, ClassId, StudentGroupId,
            FullMarks, PassMarks, IsOptional, IsReligionSubject, TeacherId,
            SubjectName, SubjectCode, SubjectType, SubjectGroup,
            TheoryMarks, PracticalMarks, TeacherName, TeacherEmployeeCode,
            Credit, NCTBCode, IsActive,
            CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
        SELECT @NewExamId, oc.SubjectId, oc.ClassId, oc.StudentGroupId,
            oc.FullMarks, oc.PassMarks, oc.IsOptional, oc.IsReligionSubject, oc.TeacherId,
            oc.SubjectName, oc.SubjectCode, oc.SubjectType, oc.SubjectGroup,
            oc.TheoryMarks, oc.PracticalMarks, oc.TeacherName, oc.TeacherEmployeeCode,
            oc.Credit, oc.NCTBCode, 1,
            @UserId, GETUTCDATE(), @UserId, GETUTCDATE()
        FROM ExamSubjects oc
        WHERE oc.ExamId = @SourceExamId AND oc.IsDeleted = 0;

        -- 5. Copy ExamSubjectComponents
        INSERT INTO ExamSubjectComponents (ExamSubjectId, ComponentId,
            MaxMarks, PassMarks, DisplayOrder,
            ComponentName, ComponentCode, Weight,
            CreatedBy, CreatedAt, UpdatedBy, UpdatedAt)
        SELECT ncs.Id, oc.ComponentId,
            oc.MaxMarks, oc.PassMarks, oc.DisplayOrder,
            oc.ComponentName, oc.ComponentCode, oc.Weight,
            @UserId, GETUTCDATE(), @UserId, GETUTCDATE()
        FROM ExamSubjectComponents oc
        INNER JOIN ExamSubjects oc_subj ON oc_subj.Id = oc.ExamSubjectId
        INNER JOIN ExamSubjects ncs ON ncs.ExamId = @NewExamId AND ncs.SubjectId = oc_subj.SubjectId
        WHERE oc_subj.ExamId = @SourceExamId AND oc.IsDeleted = 0;

        COMMIT TRANSACTION;

        -- Return new exam info
        SELECT @NewExamId AS NewExamId, @NewName AS NewExamName;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH;
END;
