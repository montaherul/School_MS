-- =====================================================================
-- STORED PROCEDURES FOR ENTERPRISE EXAM CREATION WIZARD
-- =====================================================================
-- Architecture: Controller → Service → Repository → Stored Procedure → SQL Server
-- =====================================================================

-- 1. sp_GetExamCreationPreview
-- Loads complete preview data after class selection
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamCreationPreview]
    @AcademicYearId INT,
    @SelectedClassIds NVARCHAR(MAX)  -- JSON array of class IDs
AS
BEGIN
    SET NOCOUNT ON;

    -- Parse JSON class IDs
    DECLARE @ClassIds TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIds (ClassId)
    SELECT [value] FROM OPENJSON(@SelectedClassIds);

    -- Result Set 1: Class Hierarchy with Sections, Subjects, Components
    SELECT 
        c.Id AS ClassId,
        c.Name AS ClassName,
        c.IsGroupBased,
        c.SortOrder AS ClassSortOrder,
        s.Id AS SectionId,
        s.Name AS SectionName,
        s.ParentSectionId,
        s.StudentGroupId,
        sg.Name AS StudentGroupName,
        sg.Code AS StudentGroupCode,
        cs.Id AS ClassSubjectId,
        cs.SubjectId,
        sub.Code AS SubjectCode,
        sub.Name AS SubjectName,
        sub.NameBn AS SubjectNameBn,
        sub.Category AS SubjectCategory,
        sub.SubjectGroup AS SubjectGroupName,
        sub.IsMandatory,
        sub.IsOptional,
        sub.IsReligionSubject,
        sub.IsPractical,
        sub.ReligionType,
        sub.DefaultFullMarks,
        sub.DefaultPassMarks,
        sub.TheoryMarks,
        sub.PracticalMarks,
        sub.Credit,
        sub.NctbCode,
        cs.FullMarks AS ClassSubjectFullMarks,
        cs.PassMarks AS ClassSubjectPassMarks,
        cs.IsOptional AS ClassSubjectIsOptional
    FROM SchoolClasses c
    INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
    LEFT JOIN Sections s ON s.SchoolClassId = c.Id AND s.IsDeleted = 0
    LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
    LEFT JOIN ClassSubjects cs ON cs.SchoolClassId = c.Id AND cs.IsDeleted = 0 AND cs.IsActive = 1
        AND (c.IsGroupBased = 0 OR (c.IsGroupBased = 1 AND (cs.StudentGroupId IS NULL OR cs.StudentGroupId = s.StudentGroupId)))
    LEFT JOIN Subjects sub ON sub.Id = cs.SubjectId AND sub.IsDeleted = 0 AND sub.IsActive = 1
    WHERE c.IsDeleted = 0
    ORDER BY c.SortOrder, s.Name, cs.DisplayOrder, sub.DisplayOrder;

    -- Result Set 2: Subject Components (from SubjectMarkStructure)
    SELECT 
        sms.Id AS SubjectMarkStructureId,
        sms.SubjectId,
        sms.ClassId,
        sms.StudentGroupId,
        ec.Id AS ComponentId,
        ec.Code AS ComponentCode,
        ec.Name AS ComponentName,
        ec.Description AS ComponentDescription,
        ec.IsPractical,
        ec.IsOptional AS ComponentIsOptional,
        ec.DisplayOrder AS ComponentDisplayOrder,
        ec.DefaultFullMarks,
        ec.DefaultPassMarks,
        sms.FullMarks,
        sms.PassMarks,
        sms.DisplayOrder AS ComponentDisplayOrder,
        sms.IsActive AS StructureIsActive
    FROM SubjectMarkStructures sms
    INNER JOIN ExamComponents ec ON ec.Id = sms.ComponentId AND ec.IsDeleted = 0 AND ec.IsActive = 1
    WHERE sms.IsDeleted = 0 AND sms.IsActive = 1
        AND EXISTS (SELECT 1 FROM @ClassIds ci WHERE ci.ClassId = COALESCE(sms.ClassId, (SELECT SchoolClassId FROM ClassSubjects WHERE SubjectId = sms.SubjectId AND IsDeleted = 0 AND IsActive = 1)))
    ORDER BY sms.SubjectId, sms.DisplayOrder, ec.DisplayOrder;

    -- Result Set 3: Teacher Assignments
    SELECT 
        tsa.TeacherId,
        tsa.SubjectId,
        tsa.ClassId,
        tsa.SectionId,
        tsa.StudentGroupId,
        tsa.AcademicYearId,
        e.Id AS EmployeeId,
        e.EmployeeCode,
        e.FirstName + ' ' + e.LastName AS TeacherName,
        e.Email AS TeacherEmail
    FROM TeacherSubjectAssignments tsa
    INNER JOIN Teachers t ON t.Id = tsa.TeacherId AND t.IsDeleted = 0
    INNER JOIN Employees e ON e.Id = t.EmployeeId AND e.IsDeleted = 0
    WHERE tsa.IsDeleted = 0 AND tsa.IsActive = 1
        AND tsa.AcademicYearId = @AcademicYearId
        AND EXISTS (SELECT 1 FROM @ClassIds ci WHERE ci.ClassId = tsa.ClassId);

    -- Result Set 4: Validation Summary
    SELECT 
        c.Id AS ClassId,
        c.Name AS ClassName,
        COUNT(DISTINCT s.Id) AS SectionCount,
        COUNT(DISTINCT cs.SubjectId) AS SubjectCount,
        COUNT(DISTINCT sms.ComponentId) AS ComponentCount,
        COUNT(DISTINCT tsa.TeacherId) AS TeacherCount,
        SUM(CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END) AS MissingTeacherCount,
        CASE 
            WHEN COUNT(DISTINCT s.Id) = 0 THEN 'MISSING_SECTIONS'
            WHEN COUNT(DISTINCT cs.SubjectId) = 0 THEN 'MISSING_SUBJECTS'
            WHEN COUNT(DISTINCT sms.ComponentId) = 0 THEN 'MISSING_COMPONENTS'
            WHEN SUM(CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END) > 0 THEN 'MISSING_TEACHERS'
            ELSE 'READY'
        END AS ValidationStatus,
        CASE 
            WHEN COUNT(DISTINCT s.Id) > 0 
                AND COUNT(DISTINCT cs.SubjectId) > 0 
                AND COUNT(DISTINCT sms.ComponentId) > 0 
                AND SUM(CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END) = 0 
            THEN 1 ELSE 0 END AS IsReady
    FROM SchoolClasses c
    INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
    LEFT JOIN Sections s ON s.SchoolClassId = c.Id AND s.IsDeleted = 0
    LEFT JOIN ClassSubjects cs ON cs.SchoolClassId = c.Id AND cs.IsDeleted = 0 AND cs.IsActive = 1
        AND (c.IsGroupBased = 0 OR (c.IsGroupBased = 1 AND (cs.StudentGroupId IS NULL OR cs.StudentGroupId = s.StudentGroupId)))
    LEFT JOIN SubjectMarkStructures sms ON sms.SubjectId = cs.SubjectId AND sms.IsDeleted = 0 AND sms.IsActive = 1
    LEFT JOIN TeacherSubjectAssignments tsa ON tsa.SubjectId = cs.SubjectId 
        AND tsa.ClassId = c.Id 
        AND tsa.AcademicYearId = @AcademicYearId
        AND tsa.IsDeleted = 0 AND tsa.IsActive = 1
        AND (tsa.SectionId IS NULL OR tsa.SectionId = s.Id)
        AND (tsa.StudentGroupId IS NULL OR tsa.StudentGroupId = s.StudentGroupId)
    WHERE c.IsDeleted = 0
    GROUP BY c.Id, c.Name
    ORDER BY c.SortOrder;

    -- Result Set 5: Overall Statistics
    SELECT 
        (SELECT COUNT(*) FROM @ClassIds) AS TotalClasses,
        (SELECT COUNT(*) FROM Sections s WHERE s.SchoolClassId IN (SELECT ClassId FROM @ClassIds) AND s.IsDeleted = 0) AS TotalSections,
        (SELECT COUNT(DISTINCT cs.SubjectId) FROM ClassSubjects cs WHERE cs.SchoolClassId IN (SELECT ClassId FROM @ClassIds) AND cs.IsDeleted = 0 AND cs.IsActive = 1) AS TotalSubjects,
        (SELECT COUNT(DISTINCT sms.ComponentId) FROM SubjectMarkStructures sms 
            INNER JOIN ClassSubjects cs ON cs.SubjectId = sms.SubjectId 
            WHERE cs.SchoolClassId IN (SELECT ClassId FROM @ClassIds) AND cs.IsDeleted = 0 AND cs.IsActive = 1
            AND sms.IsDeleted = 0 AND sms.IsActive = 1) AS TotalComponents,
        (SELECT COUNT(DISTINCT tsa.TeacherId) FROM TeacherSubjectAssignments tsa 
            WHERE tsa.ClassId IN (SELECT ClassId FROM @ClassIds) AND tsa.AcademicYearId = @AcademicYearId
            AND tsa.IsDeleted = 0 AND tsa.IsActive = 1) AS TotalTeachersAssigned;
END;
GO


-- 2. sp_GetExamClassHierarchy
-- Returns class → section → subject → component hierarchy in one query
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamClassHierarchy]
    @AcademicYearId INT,
    @ClassIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassIds TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIds (ClassId)
    SELECT [value] FROM OPENJSON(@ClassIds);

    WITH Hierarchy AS (
        SELECT 
            c.Id AS ClassId,
            c.Name AS ClassName,
            c.IsGroupBased,
            c.SortOrder AS ClassSortOrder,
            s.Id AS SectionId,
            s.Name AS SectionName,
            s.ParentSectionId,
            s.StudentGroupId,
            sg.Name AS StudentGroupName,
            sg.Code AS StudentGroupCode,
            cs.Id AS ClassSubjectId,
            cs.SubjectId,
            sub.Code AS SubjectCode,
            sub.Name AS SubjectName,
            sub.NameBn AS SubjectNameBn,
            sub.Category AS SubjectCategory,
            sub.SubjectGroup AS SubjectGroupName,
            sub.IsMandatory,
            sub.IsOptional,
            sub.IsReligionSubject,
            sub.IsPractical,
            sub.ReligionType,
            cs.FullMarks AS ClassSubjectFullMarks,
            cs.PassMarks AS ClassSubjectPassMarks,
            cs.IsOptional AS ClassSubjectIsOptional,
            ec.Id AS ComponentId,
            ec.Code AS ComponentCode,
            ec.Name AS ComponentName,
            ec.IsPractical AS ComponentIsPractical,
            ec.DefaultFullMarks,
            ec.DefaultPassMarks,
            sms.FullMarks AS ComponentFullMarks,
            sms.PassMarks AS ComponentPassMarks,
            sms.DisplayOrder AS ComponentDisplayOrder,
            ROW_NUMBER() OVER (PARTITION BY c.Id ORDER BY s.Name, cs.DisplayOrder, sub.DisplayOrder, sms.DisplayOrder) AS HierarchyOrder
        FROM SchoolClasses c
        INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
        LEFT JOIN Sections s ON s.SchoolClassId = c.Id AND s.IsDeleted = 0
        LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
        LEFT JOIN ClassSubjects cs ON cs.SchoolClassId = c.Id AND cs.IsDeleted = 0 AND cs.IsActive = 1
            AND (c.IsGroupBased = 0 OR (c.IsGroupBased = 1 AND (cs.StudentGroupId IS NULL OR cs.StudentGroupId = s.StudentGroupId)))
        LEFT JOIN Subjects sub ON sub.Id = cs.SubjectId AND sub.IsDeleted = 0 AND sub.IsActive = 1
        LEFT JOIN SubjectMarkStructures sms ON sms.SubjectId = sub.Id AND sms.IsDeleted = 0 AND sms.IsActive = 1
        LEFT JOIN ExamComponents ec ON ec.Id = sms.ComponentId AND ec.IsDeleted = 0 AND ec.IsActive = 1
        WHERE c.IsDeleted = 0
    )
    SELECT *
    FROM Hierarchy
    ORDER BY ClassSortOrder, SectionName, SubjectName, ComponentDisplayOrder;
END;
GO


-- 3. sp_GetExamTeacherAssignments
-- Returns assigned teachers with missing assignment flags
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamTeacherAssignments]
    @AcademicYearId INT,
    @ClassIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassIds TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIds (ClassId)
    SELECT [value] FROM OPENJSON(@ClassIds);

    SELECT 
        cs.SubjectId,
        sub.Code AS SubjectCode,
        sub.Name AS SubjectName,
        cs.SchoolClassId AS ClassId,
        c.Name AS ClassName,
        s.Id AS SectionId,
        s.Name AS SectionName,
        s.StudentGroupId,
        sg.Name AS StudentGroupName,
        tsa.TeacherId,
        e.Id AS EmployeeId,
        e.EmployeeCode,
        e.FirstName + ' ' + e.LastName AS TeacherName,
        e.Email AS TeacherEmail,
        CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END AS IsMissingTeacher
    FROM ClassSubjects cs
    INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId
    INNER JOIN Subjects sub ON sub.Id = cs.SubjectId AND sub.IsDeleted = 0 AND sub.IsActive = 1
    INNER JOIN SchoolClasses c ON c.Id = cs.SchoolClassId AND c.IsDeleted = 0
    LEFT JOIN Sections s ON s.SchoolClassId = c.Id AND s.IsDeleted = 0
        AND (c.IsGroupBased = 0 OR (c.IsGroupBased = 1 AND (cs.StudentGroupId IS NULL OR cs.StudentGroupId = s.StudentGroupId)))
    LEFT JOIN StudentGroups sg ON s.StudentGroupId = sg.Id
    LEFT JOIN TeacherSubjectAssignments tsa ON tsa.SubjectId = cs.SubjectId 
        AND tsa.ClassId = cs.SchoolClassId 
        AND tsa.AcademicYearId = @AcademicYearId
        AND tsa.IsDeleted = 0 AND tsa.IsActive = 1
        AND (tsa.SectionId IS NULL OR tsa.SectionId = s.Id)
        AND (tsa.StudentGroupId IS NULL OR tsa.StudentGroupId = s.StudentGroupId)
    LEFT JOIN Teachers t ON t.Id = tsa.TeacherId AND t.IsDeleted = 0
    LEFT JOIN Employees e ON e.Id = t.EmployeeId AND e.IsDeleted = 0
    WHERE cs.IsDeleted = 0 AND cs.IsActive = 1
    ORDER BY c.SortOrder, s.Name, sub.DisplayOrder;
END;
GO


-- 4. sp_GetExamValidation
-- Full validation with readiness checks
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamValidation]
    @AcademicYearId INT,
    @ExamName NVARCHAR(200),
    @ExamTerm INT,
    @ClassIds NVARCHAR(MAX),
    @StartDate DATE,
    @EndDate DATE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassIds TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIds (ClassId)
    SELECT [value] FROM OPENJSON(@ClassIds);

    -- Validation messages
    DECLARE @ValidationMessages TABLE (
        Severity NVARCHAR(20),  -- ERROR, WARNING, INFO
        Category NVARCHAR(50),
        Message NVARCHAR(MAX),
        ClassId INT NULL,
        SectionId INT NULL,
        SubjectId INT NULL,
        FixAction NVARCHAR(100) NULL  -- Action to fix this issue
    );

    -- 1. Duplicate Exam Check
    IF EXISTS (
        SELECT 1 FROM Exams e 
        WHERE e.Name = @ExamName 
            AND e.AcademicYearId = @AcademicYearId 
            AND e.Term = @ExamTerm
            AND e.IsDeleted = 0
    )
    BEGIN
        INSERT INTO @ValidationMessages (Severity, Category, Message, FixAction)
        VALUES ('ERROR', 'DUPLICATE_EXAM', 'An exam with this name already exists for the selected academic year and term.', 'RENAME_EXAM');
    END

    -- 2. Date Range Validation
    IF @StartDate > @EndDate
    BEGIN
        INSERT INTO @ValidationMessages (Severity, Category, Message, FixAction)
        VALUES ('ERROR', 'INVALID_DATES', 'Start date must be before or equal to end date.', 'FIX_DATES');
    END

    IF @StartDate < (SELECT StartsOn FROM AcademicYears WHERE Id = @AcademicYearId AND IsDeleted = 0)
    BEGIN
        INSERT INTO @ValidationMessages (Severity, Category, Message, FixAction)
        VALUES ('WARNING', 'DATE_OUTSIDE_YEAR', 'Exam start date is before the academic year start date.', 'FIX_DATES');
    END

    IF @EndDate > (SELECT EndsOn FROM AcademicYears WHERE Id = @AcademicYearId AND IsDeleted = 0)
    BEGIN
        INSERT INTO @ValidationMessages (Severity, Category, Message, FixAction)
        VALUES ('WARNING', 'DATE_OUTSIDE_YEAR', 'Exam end date is after the academic year end date.', 'FIX_DATES');
    END

    -- 3. Class-level validation with fix actions
    INSERT INTO @ValidationMessages (Severity, Category, Message, ClassId, FixAction)
    SELECT 
        CASE WHEN SectionCount = 0 THEN 'ERROR' ELSE 'WARNING' END,
        'MISSING_SECTIONS',
        'Class has no sections configured.',
        c.Id,
        'CONFIGURE_SECTIONS'
    FROM SchoolClasses c
    INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
    LEFT JOIN (
        SELECT SchoolClassId, COUNT(*) AS SectionCount
        FROM Sections 
        WHERE IsDeleted = 0
        GROUP BY SchoolClassId
    ) s ON s.SchoolClassId = c.Id
    WHERE c.IsDeleted = 0 AND ISNULL(s.SectionCount, 0) = 0;

    INSERT INTO @ValidationMessages (Severity, Category, Message, ClassId, FixAction)
    SELECT 
        CASE WHEN SubjectCount = 0 THEN 'ERROR' ELSE 'WARNING' END,
        'MISSING_SUBJECTS',
        'Class has no subjects mapped.',
        c.Id,
        'MAP_SUBJECTS'
    FROM SchoolClasses c
    INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
    LEFT JOIN (
        SELECT SchoolClassId, COUNT(DISTINCT SubjectId) AS SubjectCount
        FROM ClassSubjects 
        WHERE IsDeleted = 0 AND IsActive = 1
        GROUP BY SchoolClassId
    ) cs ON cs.SchoolClassId = c.Id
    WHERE c.IsDeleted = 0 AND ISNULL(cs.SubjectCount, 0) = 0;

    INSERT INTO @ValidationMessages (Severity, Category, Message, ClassId, FixAction)
    SELECT 
        CASE WHEN ComponentCount = 0 THEN 'ERROR' ELSE 'WARNING' END,
        'MISSING_COMPONENTS',
        'One or more subjects lack component mark structures.',
        c.Id,
        'CONFIGURE_COMPONENTS'
    FROM SchoolClasses c
    INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
    LEFT JOIN (
        SELECT cs.SchoolClassId, COUNT(DISTINCT sms.ComponentId) AS ComponentCount
        FROM ClassSubjects cs
        INNER JOIN SubjectMarkStructures sms ON sms.SubjectId = cs.SubjectId AND sms.IsDeleted = 0 AND sms.IsActive = 1
        WHERE cs.IsDeleted = 0 AND cs.IsActive = 1
        GROUP BY cs.SchoolClassId
    ) sc ON sc.SchoolClassId = c.Id
    WHERE c.IsDeleted = 0 AND ISNULL(sc.ComponentCount, 0) = 0;

    INSERT INTO @ValidationMessages (Severity, Category, Message, ClassId, FixAction)
    SELECT 
        'WARNING',
        'MISSING_TEACHERS',
        'Some subjects have no assigned teachers.',
        c.Id,
        'ASSIGN_TEACHERS'
    FROM SchoolClasses c
    INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
    LEFT JOIN (
        SELECT cs.SchoolClassId, 
            SUM(CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END) AS MissingTeacherCount
        FROM ClassSubjects cs
        LEFT JOIN TeacherSubjectAssignments tsa ON tsa.SubjectId = cs.SubjectId 
            AND tsa.ClassId = cs.SchoolClassId 
            AND tsa.AcademicYearId = @AcademicYearId
            AND tsa.IsDeleted = 0 AND tsa.IsActive = 1
        WHERE cs.IsDeleted = 0 AND cs.IsActive = 1
        GROUP BY cs.SchoolClassId
    ) mt ON mt.SchoolClassId = c.Id
    WHERE c.IsDeleted = 0 AND ISNULL(mt.MissingTeacherCount, 0) > 0;

    -- 4. Subject-level duplicate check
    INSERT INTO @ValidationMessages (Severity, Category, Message, ClassId, SubjectId, FixAction)
    SELECT 
        'ERROR',
        'DUPLICATE_SUBJECT',
        'Duplicate subject mapping found for class.',
        cs.SchoolClassId,
        cs.SubjectId,
        'REMOVE_DUPLICATE_SUBJECT'
    FROM ClassSubjects cs
    INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId
    WHERE cs.IsDeleted = 0 AND cs.IsActive = 1
    GROUP BY cs.SchoolClassId, cs.SubjectId
    HAVING COUNT(*) > 1;

    -- 5. Component duplicate check per subject
    INSERT INTO @ValidationMessages (Severity, Category, Message, SubjectId, FixAction)
    SELECT 
        'ERROR',
        'DUPLICATE_COMPONENT',
        'Duplicate component in subject mark structure.',
        sms.SubjectId,
        'REMOVE_DUPLICATE_COMPONENT'
    FROM SubjectMarkStructures sms
    WHERE sms.IsDeleted = 0 AND sms.IsActive = 1
        AND EXISTS (SELECT 1 FROM ClassSubjects cs INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId WHERE cs.SubjectId = sms.SubjectId AND cs.IsDeleted = 0 AND cs.IsActive = 1)
    GROUP BY sms.SubjectId, sms.ComponentId
    HAVING COUNT(*) > 1;

    -- 6. Invalid marks validation
    INSERT INTO @ValidationMessages (Severity, Category, Message, SubjectId, FixAction)
    SELECT 
        'ERROR',
        'INVALID_MARKS',
        'Pass marks exceed full marks in component structure.',
        sms.SubjectId,
        'FIX_COMPONENT_MARKS'
    FROM SubjectMarkStructures sms
    WHERE sms.IsDeleted = 0 AND sms.IsActive = 1
        AND sms.PassMarks > sms.FullMarks
        AND EXISTS (SELECT 1 FROM ClassSubjects cs INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId WHERE cs.SubjectId = sms.SubjectId AND cs.IsDeleted = 0 AND cs.IsActive = 1);

    -- 7. Zero/negative marks
    INSERT INTO @ValidationMessages (Severity, Category, Message, SubjectId, FixAction)
    SELECT 
        'ERROR',
        'INVALID_MARKS',
        'Full marks must be greater than zero.',
        sms.SubjectId,
        'FIX_COMPONENT_MARKS'
    FROM SubjectMarkStructures sms
    WHERE sms.IsDeleted = 0 AND sms.IsActive = 1
        AND (sms.FullMarks <= 0 OR sms.PassMarks < 0)
        AND EXISTS (SELECT 1 FROM ClassSubjects cs INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId WHERE cs.SubjectId = sms.SubjectId AND cs.IsDeleted = 0 AND cs.IsActive = 1);

    -- 8. Component total validation (total max marks = subject full marks)
    INSERT INTO @ValidationMessages (Severity, Category, Message, ClassId, SubjectId, FixAction)
    SELECT 
        'ERROR',
        'COMPONENT_TOTAL_MISMATCH',
        'Total component max marks (' + CAST(TotalMaxMarks AS NVARCHAR) + ') does not match subject full marks (' + CAST(cs.FullMarks AS NVARCHAR) + ').',
        cs.SchoolClassId,
        cs.SubjectId,
        'FIX_COMPONENT_TOTAL'
    FROM (
        SELECT cs.SchoolClassId, cs.SubjectId, cs.FullMarks, SUM(sms.FullMarks) AS TotalMaxMarks
        FROM ClassSubjects cs
        INNER JOIN SubjectMarkStructures sms ON sms.SubjectId = cs.SubjectId AND sms.IsDeleted = 0 AND sms.IsActive = 1
        INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId
        WHERE cs.IsDeleted = 0 AND cs.IsActive = 1
        GROUP BY cs.SchoolClassId, cs.SubjectId, cs.FullMarks
        HAVING SUM(sms.FullMarks) != cs.FullMarks
    ) x;

    -- 9. Display order duplicates
    INSERT INTO @ValidationMessages (Severity, Category, Message, SubjectId, FixAction)
    SELECT 
        'WARNING',
        'DUPLICATE_DISPLAY_ORDER',
        'Duplicate display order in component structure.',
        sms.SubjectId,
        'FIX_DISPLAY_ORDER'
    FROM SubjectMarkStructures sms
    WHERE sms.IsDeleted = 0 AND sms.IsActive = 1
        AND EXISTS (SELECT 1 FROM ClassSubjects cs INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId WHERE cs.SubjectId = sms.SubjectId AND cs.IsDeleted = 0 AND cs.IsActive = 1)
    GROUP BY sms.SubjectId, sms.DisplayOrder
    HAVING COUNT(*) > 1;

    -- 10. Weight total validation (if weights are used)
    INSERT INTO @ValidationMessages (Severity, Category, Message, SubjectId, FixAction)
    SELECT 
        'WARNING',
        'WEIGHT_TOTAL_INVALID',
        'Component weights do not sum to 100.',
        sms.SubjectId,
        'FIX_WEIGHTS'
    FROM (
        SELECT SubjectId, SUM(Weight) AS TotalWeight
        FROM SubjectMarkStructures
        WHERE IsDeleted = 0 AND IsActive = 1 AND Weight > 0
        GROUP BY SubjectId
        HAVING SUM(Weight) != 100 AND SUM(Weight) > 0
    ) x
    INNER JOIN SubjectMarkStructures sms ON sms.SubjectId = x.SubjectId AND sms.IsDeleted = 0 AND sms.IsActive = 1
        AND EXISTS (SELECT 1 FROM ClassSubjects cs INNER JOIN @ClassIds ci ON cs.SchoolClassId = ci.ClassId WHERE cs.SubjectId = sms.SubjectId AND cs.IsDeleted = 0 AND cs.IsActive = 1);

    -- Return validation messages
    SELECT * FROM @ValidationMessages ORDER BY 
        CASE Severity WHEN 'ERROR' THEN 1 WHEN 'WARNING' THEN 2 ELSE 3 END,
        Category;

    -- Return readiness score with category breakdown
    DECLARE @TotalClasses INT = (SELECT COUNT(*) FROM @ClassIds);
    DECLARE @ReadyClasses INT = (
        SELECT COUNT(DISTINCT ClassId) 
        FROM @ClassIds ci
        WHERE NOT EXISTS (SELECT 1 FROM @ValidationMessages vm WHERE vm.ClassId = ci.ClassId AND vm.Severity = 'ERROR')
    );

    -- Per-category readiness
    SELECT 
        'ACADEMIC_YEAR' AS Category,
        CASE WHEN EXISTS (SELECT 1 FROM AcademicYears WHERE Id = @AcademicYearId AND IsDeleted = 0) THEN 1 ELSE 0 END AS IsReady,
        'Academic Year' AS Label
    UNION ALL
    SELECT 
        'CLASSES' AS Category,
        CASE WHEN @TotalClasses > 0 THEN 1 ELSE 0 END AS IsReady,
        'Classes Selected' AS Label
    UNION ALL
    SELECT 
        'SECTIONS' AS Category,
        CASE WHEN NOT EXISTS (SELECT 1 FROM @ValidationMessages WHERE Category = 'MISSING_SECTIONS' AND Severity = 'ERROR') THEN 1 ELSE 0 END AS IsReady,
        'Sections Configured' AS Label
    UNION ALL
    SELECT 
        'SUBJECTS' AS Category,
        CASE WHEN NOT EXISTS (SELECT 1 FROM @ValidationMessages WHERE Category = 'MISSING_SUBJECTS' AND Severity = 'ERROR') THEN 1 ELSE 0 END AS IsReady,
        'Subjects Mapped' AS Label
    UNION ALL
    SELECT 
        'COMPONENTS' AS Category,
        CASE WHEN NOT EXISTS (SELECT 1 FROM @ValidationMessages WHERE Category = 'MISSING_COMPONENTS' AND Severity = 'ERROR') THEN 1 ELSE 0 END AS IsReady,
        'Components Configured' AS Label
    UNION ALL
    SELECT 
        'TEACHERS' AS Category,
        CASE WHEN NOT EXISTS (SELECT 1 FROM @ValidationMessages WHERE Category = 'MISSING_TEACHERS' AND Severity = 'ERROR') THEN 1 ELSE 0 END AS IsReady,
        'Teachers Assigned' AS Label
    UNION ALL
    SELECT 
        'NO_ERRORS' AS Category,
        CASE WHEN (SELECT COUNT(*) FROM @ValidationMessages WHERE Severity = 'ERROR') = 0 THEN 1 ELSE 0 END AS IsReady,
        'No Validation Errors' AS Label;

    -- Overall readiness
    SELECT 
        @TotalClasses AS TotalClasses,
        @ReadyClasses AS ReadyClasses,
        @TotalClasses - @ReadyClasses AS NotReadyClasses,
        CASE WHEN @TotalClasses > 0 THEN CAST(@ReadyClasses * 100.0 / @TotalClasses AS DECIMAL(5,2)) ELSE 100 END AS ReadinessPercentage,
        (SELECT COUNT(*) FROM @ValidationMessages WHERE Severity = 'ERROR') AS ErrorCount,
        (SELECT COUNT(*) FROM @ValidationMessages WHERE Severity = 'WARNING') AS WarningCount,
        CASE WHEN (SELECT COUNT(*) FROM @ValidationMessages WHERE Severity = 'ERROR') = 0 THEN 1 ELSE 0 END AS Is100PercentReady;
END;
GO


-- 5. sp_CreateExamHierarchy
-- Creates complete exam hierarchy in single transaction
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_CreateExamHierarchy]
    @AcademicYearId INT,
    @ExamName NVARCHAR(100),
    @ExamTerm INT,
    @ExamType NVARCHAR(100),
    @StartDate DATE,
    @EndDate DATE,
    @ClassIds NVARCHAR(MAX),  -- JSON array
    @Subjects NVARCHAR(MAX),  -- JSON: [{SubjectId, ClassId, SectionId, StudentGroupId, FullMarks, PassMarks, IsOptional, TeacherId, Components:[{ComponentId, MaxMarks, PassMarks, DisplayOrder}]}]
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ExamId INT;
    DECLARE @ClassIdsTable TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIdsTable (ClassId)
    SELECT [value] FROM OPENJSON(@ClassIds);

    BEGIN TRY
        BEGIN TRANSACTION;

        -- 1. Create Exam
        INSERT INTO Exams (Name, Term, AcademicYearId, ClassId, StartsOn, EndsOn, Status, IsLocked, IsPublished, IsArchived, CreatedBy, CreatedAt)
        OUTPUT INSERTED.Id INTO @ExamIdTable
        VALUES (@ExamName, @ExamTerm, @AcademicYearId, 
            (SELECT TOP 1 ClassId FROM @ClassIdsTable), 
            @StartDate, @EndDate, 0, 0, 0, 0, @UserId, GETUTCDATE());

        SELECT @ExamId = Id FROM @ExamIdTable;

        -- 2. Create ExamClasses
        INSERT INTO ExamClasses (ExamId, ClassId, ClassName, SortOrder, CreatedBy, CreatedAt)
        SELECT @ExamId, c.Id, c.Name, c.SortOrder, @UserId, GETUTCDATE()
        FROM SchoolClasses c
        INNER JOIN @ClassIdsTable ci ON c.Id = ci.ClassId
        WHERE c.IsDeleted = 0;

        -- 3. Create ExamSections
        INSERT INTO ExamSections (ExamClassId, SectionId, SectionName, CreatedBy, CreatedAt)
        SELECT ec.Id, s.Id, s.Name, @UserId, GETUTCDATE()
        FROM ExamClasses ec
        INNER JOIN Sections s ON s.SchoolClassId = ec.ClassId AND s.IsDeleted = 0
        WHERE ec.ExamId = @ExamId;

        -- 4. Create ExamSubjects with teacher assignments
        -- Parse JSON subjects
        DECLARE @Subjects TABLE (
            SubjectId INT,
            ClassId INT,
            SectionId INT,
            StudentGroupId INT,
            FullMarks DECIMAL(10,2),
            PassMarks DECIMAL(10,2),
            IsOptional BIT,
            TeacherId INT
        );

        INSERT INTO @Subjects (SubjectId, ClassId, SectionId, StudentGroupId, FullMarks, PassMarks, IsOptional, TeacherId)
        SELECT 
            JSON_VALUE(value, '$.SubjectId'),
            JSON_VALUE(value, '$.ClassId'),
            JSON_VALUE(value, '$.SectionId'),
            JSON_VALUE(value, '$.StudentGroupId'),
            JSON_VALUE(value, '$.FullMarks'),
            JSON_VALUE(value, '$.PassMarks'),
            JSON_VALUE(value, '$.IsOptional'),
            JSON_VALUE(value, '$.TeacherId')
        FROM OPENJSON(@Subjects);

        INSERT INTO ExamSubjects (
            ExamId, SubjectId, ClassId, StudentGroupId,
            FullMarks, PassMarks, IsOptional, TeacherId,
            IsActive, CreatedBy, CreatedAt,
            -- Snapshot fields for historical integrity
            SubjectName, SubjectCode, SubjectType, SubjectGroup,
            TheoryMarks, PracticalMarks, Credit, NCTBCode,
            TeacherName, TeacherEmployeeCode
        )
        SELECT 
            @ExamId, s.SubjectId, s.ClassId, s.StudentGroupId,
            s.FullMarks, s.PassMarks, s.IsOptional, s.TeacherId,
            1, @UserId, GETUTCDATE(),
            sub.Name, sub.Code, sub.Category, sub.SubjectGroup,
            sub.TheoryMarks, sub.PracticalMarks, sub.Credit, sub.NctbCode,
            e.FirstName + ' ' + e.LastName, e.EmployeeCode
        FROM @Subjects s
        INNER JOIN Subjects sub ON sub.Id = s.SubjectId AND sub.IsDeleted = 0
        LEFT JOIN Teachers t ON t.Id = s.TeacherId AND t.IsDeleted = 0
        LEFT JOIN Employees e ON e.Id = t.EmployeeId AND e.IsDeleted = 0;

        -- 5. Create ExamSubjectComponents
        -- Parse JSON components
        DECLARE @Components TABLE (
            SubjectId INT,
            ComponentId INT,
            MaxMarks DECIMAL(10,2),
            PassMarks DECIMAL(10,2),
            DisplayOrder INT
        );

        -- Extract components from the nested JSON
        INSERT INTO @Components (SubjectId, ComponentId, MaxMarks, PassMarks, DisplayOrder)
        SELECT 
            JSON_VALUE(s.value, '$.SubjectId'),
            JSON_VALUE(c.value, '$.ComponentId'),
            JSON_VALUE(c.value, '$.MaxMarks'),
            JSON_VALUE(c.value, '$.PassMarks'),
            JSON_VALUE(c.value, '$.DisplayOrder')
        FROM OPENJSON(@Subjects) s
        CROSS APPLY OPENJSON(JSON_QUERY(s.value, '$.Components')) c;

        INSERT INTO ExamSubjectComponents (
            ExamSubjectId, ComponentId, MaxMarks, PassMarks, DisplayOrder,
            ComponentName, ComponentCode, Weight,
            CreatedBy, CreatedAt
        )
        SELECT 
            es.Id, c.ComponentId, c.MaxMarks, c.PassMarks, c.DisplayOrder,
            ec.Name, ec.Code, 1.0,
            @UserId, GETUTCDATE()
        FROM @Components c
        INNER JOIN ExamSubjects es ON es.ExamId = @ExamId AND es.SubjectId = c.SubjectId AND es.IsDeleted = 0
        INNER JOIN ExamComponents ec ON ec.Id = c.ComponentId AND ec.IsDeleted = 0;

        -- 6. Teacher Assignment Snapshot (Audit)
        INSERT INTO TeacherAssignmentLogs (ExamId, TeacherId, SubjectId, ClassId, SectionId, StudentGroupId, Action, ActionBy, ActionAt)
        SELECT 
            @ExamId, es.TeacherId, es.SubjectId, es.ClassId, 
            sec.SectionId, es.StudentGroupId, 'SNAPSHOT', @UserId, GETUTCDATE()
        FROM ExamSubjects es
        LEFT JOIN ExamSections sec ON sec.ExamClassId IN (SELECT Id FROM ExamClasses WHERE ExamId = @ExamId AND ClassId = es.ClassId)
            AND (sec.SectionId IS NULL OR sec.StudentGroupId = es.StudentGroupId)
        WHERE es.ExamId = @ExamId AND es.TeacherId IS NOT NULL AND es.IsDeleted = 0;

        -- 7. Room/Shift Snapshot (for future scheduling)
        -- This can be extended when rooms are assigned during scheduling

        COMMIT TRANSACTION;

        SELECT @ExamId AS ExamId, @ExamName AS ExamName, 'SUCCESS' AS Status;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 
            0 AS ExamId, 
            @ExamName AS ExamName, 
            ERROR_MESSAGE() AS Status,
            ERROR_NUMBER() AS ErrorNumber,
            ERROR_SEVERITY() AS ErrorSeverity,
            ERROR_STATE() AS ErrorState;
    END CATCH;
END;
GO

-- Helper table variable for output
DECLARE @ExamIdTable TABLE (Id INT);
GO


-- 6. sp_GetExamReadiness
-- Returns readiness percentage and missing items
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamReadiness]
    @AcademicYearId INT,
    @ClassIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassIds TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIds (ClassId)
    SELECT [value] FROM OPENJSON(@ClassIds);

    WITH ClassReadiness AS (
        SELECT 
            c.Id AS ClassId,
            c.Name AS ClassName,
            COUNT(DISTINCT s.Id) AS SectionCount,
            COUNT(DISTINCT cs.SubjectId) AS SubjectCount,
            COUNT(DISTINCT sms.ComponentId) AS ComponentCount,
            COUNT(DISTINCT tsa.TeacherId) AS TeacherAssignedCount,
            SUM(CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END) AS MissingTeacherCount,
            CASE 
                WHEN COUNT(DISTINCT s.Id) = 0 THEN 'MISSING_SECTIONS'
                WHEN COUNT(DISTINCT cs.SubjectId) = 0 THEN 'MISSING_SUBJECTS'
                WHEN COUNT(DISTINCT sms.ComponentId) = 0 THEN 'MISSING_COMPONENTS'
                WHEN SUM(CASE WHEN tsa.TeacherId IS NULL THEN 1 ELSE 0 END) > 0 THEN 'MISSING_TEACHERS'
                ELSE 'READY'
            END AS Status
        FROM SchoolClasses c
        INNER JOIN @ClassIds ci ON c.Id = ci.ClassId
        LEFT JOIN Sections s ON s.SchoolClassId = c.Id AND s.IsDeleted = 0
        LEFT JOIN ClassSubjects cs ON cs.SchoolClassId = c.Id AND cs.IsDeleted = 0 AND cs.IsActive = 1
            AND (c.IsGroupBased = 0 OR (c.IsGroupBased = 1 AND (cs.StudentGroupId IS NULL OR cs.StudentGroupId = s.StudentGroupId)))
        LEFT JOIN SubjectMarkStructures sms ON sms.SubjectId = cs.SubjectId AND sms.IsDeleted = 0 AND sms.IsActive = 1
        LEFT JOIN TeacherSubjectAssignments tsa ON tsa.SubjectId = cs.SubjectId 
            AND tsa.ClassId = c.Id 
            AND tsa.AcademicYearId = @AcademicYearId
            AND tsa.IsDeleted = 0 AND tsa.IsActive = 1
            AND (tsa.SectionId IS NULL OR tsa.SectionId = s.Id)
            AND (tsa.StudentGroupId IS NULL OR tsa.StudentGroupId = s.StudentGroupId)
        WHERE c.IsDeleted = 0
        GROUP BY c.Id, c.Name
    )
    SELECT 
        ClassId,
        ClassName,
        SectionCount,
        SubjectCount,
        ComponentCount,
        TeacherAssignedCount,
        MissingTeacherCount,
        Status,
        CASE WHEN Status = 'READY' THEN 1 ELSE 0 END AS IsReady
    FROM ClassReadiness
    ORDER BY ClassId;

    -- Overall readiness
    SELECT 
        COUNT(*) AS TotalClasses,
        SUM(CASE WHEN Status = 'READY' THEN 1 ELSE 0 END) AS ReadyClasses,
        CAST(SUM(CASE WHEN Status = 'READY' THEN 1 ELSE 0 END) * 100.0 / COUNT(*) AS DECIMAL(5,2)) AS ReadinessPercentage,
        SUM(SectionCount) AS TotalSections,
        SUM(SubjectCount) AS TotalSubjects,
        SUM(ComponentCount) AS TotalComponents,
        SUM(TeacherAssignedCount) AS TotalTeachersAssigned,
        SUM(MissingTeacherCount) AS TotalMissingTeachers
    FROM ClassReadiness;
END;
GO


-- 7. sp_GetExamStatistics
-- Summary counts for preview/dashboard
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamStatistics]
    @AcademicYearId INT,
    @ClassIds NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @ClassIds TABLE (ClassId INT PRIMARY KEY);
    INSERT INTO @ClassIds (ClassId)
    SELECT [value] FROM OPENJSON(@ClassIds);

    SELECT 
        (SELECT COUNT(*) FROM @ClassIds) AS ClassCount,
        (SELECT COUNT(*) FROM Sections s WHERE s.SchoolClassId IN (SELECT ClassId FROM @ClassIds) AND s.IsDeleted = 0) AS SectionCount,
        (SELECT COUNT(DISTINCT cs.SubjectId) FROM ClassSubjects cs WHERE cs.SchoolClassId IN (SELECT ClassId FROM @ClassIds) AND cs.IsDeleted = 0 AND cs.IsActive = 1) AS SubjectCount,
        (SELECT COUNT(DISTINCT sms.ComponentId) FROM SubjectMarkStructures sms 
            INNER JOIN ClassSubjects cs ON cs.SubjectId = sms.SubjectId 
            WHERE cs.SchoolClassId IN (SELECT ClassId FROM @ClassIds) AND cs.IsDeleted = 0 AND cs.IsActive = 1
            AND sms.IsDeleted = 0 AND sms.IsActive = 1) AS ComponentCount,
        (SELECT COUNT(DISTINCT tsa.TeacherId) FROM TeacherSubjectAssignments tsa 
            WHERE tsa.ClassId IN (SELECT ClassId FROM @ClassIds) AND tsa.AcademicYearId = @AcademicYearId
            AND tsa.IsDeleted = 0 AND tsa.IsActive = 1) AS TeacherCount,
        (SELECT COUNT(*) FROM Students s 
            INNER JOIN Sections sec ON sec.Id = s.SectionId
            INNER JOIN SchoolClasses sc ON sc.Id = sec.SchoolClassId
            WHERE sc.Id IN (SELECT ClassId FROM @ClassIds) AND s.IsDeleted = 0) AS StudentCount;
END;
GO


-- 8. sp_GenerateExamSchedule
-- Auto-generate exam schedule
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GenerateExamSchedule]
    @ExamId INT,
    @StartDate DATE,
    @EndDate DATE,
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Exam TABLE (AcademicYearId INT, Term INT);
        INSERT INTO @Exam SELECT AcademicYearId, Term FROM Exams WHERE Id = @ExamId AND IsDeleted = 0;

        -- Get all exam subjects with class/section/group
        DECLARE @SubjectsToSchedule TABLE (
            ExamSubjectId INT,
            SubjectId INT,
            ClassId INT,
            SectionId INT,
            StudentGroupId INT,
            SubjectName NVARCHAR(100)
        );

        INSERT INTO @SubjectsToSchedule
        SELECT es.Id, es.SubjectId, es.ClassId, 
            COALESCE(es2.SectionId, s.Id) AS SectionId,
            es.StudentGroupId,
            sub.Name
        FROM ExamSubjects es
        INNER JOIN Subjects sub ON sub.Id = es.SubjectId
        INNER JOIN SchoolClasses sc ON sc.Id = es.ClassId
        LEFT JOIN Sections s ON s.SchoolClassId = es.ClassId AND s.IsDeleted = 0
            AND (sc.IsGroupBased = 0 OR s.StudentGroupId = es.StudentGroupId)
        LEFT JOIN ExamSections es2 ON es2.ExamClassId = (SELECT TOP 1 Id FROM ExamClasses WHERE ExamId = @ExamId AND ClassId = es.ClassId)
            AND (es2.SectionId = s.Id OR (es2.StudentGroupId = es.StudentGroupId AND es2.SectionId IS NULL))
        WHERE es.ExamId = @ExamId AND es.IsDeleted = 0 AND es.IsActive = 1;

        -- Simple scheduling: one subject per day, sequential
        DECLARE @CurrentDate DATE = @StartDate;
        DECLARE @SubjectCount INT = (SELECT COUNT(*) FROM @SubjectsToSchedule);
        DECLARE @DayOffset INT = 0;

        WHILE @DayOffset < @SubjectCount AND @CurrentDate <= @EndDate
        BEGIN
            -- Skip weekends (Friday = 5, Saturday = 6 in SQL Server DATEPART)
            IF DATEPART(WEEKDAY, @CurrentDate) NOT IN (6, 7) -- Adjust based on @@DATEFIRST
            BEGIN
                UPDATE TOP (1) sts SET 
                    sts.ExamDate = @CurrentDate
                FROM @SubjectsToSchedule sts
                WHERE sts.ExamDate IS NULL;

                SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
                SET @DayOffset = @DayOffset + 1;
            END
            ELSE
            BEGIN
                SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
            END
        END

        -- Insert schedules
        INSERT INTO ExamSchedules (ExamId, SubjectId, ClassId, StudentGroupId, SectionId, ExamDate, StartsAt, EndsAt, RoomNo, CreatedBy, CreatedAt)
        SELECT 
            @ExamId, sts.SubjectId, sts.ClassId, sts.StudentGroupId, sts.SectionId,
            sts.ExamDate, '09:00', '12:00', 'AUTO', @UserId, GETUTCDATE()
        FROM @SubjectsToSchedule sts
        WHERE sts.ExamDate IS NOT NULL;

        COMMIT TRANSACTION;
        SELECT 'SUCCESS' AS Status, @SubjectCount AS ScheduledCount;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Status, ERROR_MESSAGE() AS Message;
    END CATCH;
END;
GO


-- 9. sp_GetExamConflicts
-- Teacher, room, and time conflict detection
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_GetExamConflicts]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Teacher Conflicts
    SELECT 
        'TEACHER_CONFLICT' AS ConflictType,
        es1.ExamDate,
        es1.StartsAt,
        es1.EndsAt,
        t.EmployeeCode,
        t.FirstName + ' ' + t.LastName AS TeacherName,
        es1.SubjectName AS Subject1,
        es2.SubjectName AS Subject2,
        es1.RoomNo AS Room1,
        es2.RoomNo AS Room2,
        es1.ClassName AS Class1,
        es2.ClassName AS Class2
    FROM ExamSchedules es1
    INNER JOIN ExamSchedules es2 ON es2.ExamId = @ExamId AND es2.Id > es1.Id
    INNER JOIN ExamSubjects esub1 ON esub1.Id = es1.ExamSubjectId
    INNER JOIN ExamSubjects esub2 ON esub2.Id = es2.ExamSubjectId
    INNER JOIN Teachers t1 ON t1.Id = esub1.TeacherId
    INNER JOIN Teachers t2 ON t2.Id = esub2.TeacherId AND t2.Id = t1.Id
    INNER JOIN Employees e ON e.Id = t1.EmployeeId
    WHERE es1.ExamId = @ExamId 
        AND es1.ExamDate = es2.ExamDate
        AND es1.IsDeleted = 0 AND es2.IsDeleted = 0
        AND ((es1.StartsAt < es2.EndsAt) AND (es1.EndsAt > es2.StartsAt))
    ORDER BY es1.ExamDate, es1.StartsAt;

    -- Room Conflicts
    SELECT 
        'ROOM_CONFLICT' AS ConflictType,
        es1.ExamDate,
        es1.StartsAt,
        es1.EndsAt,
        es1.RoomNo,
        es1.SubjectName AS Subject1,
        es2.SubjectName AS Subject2,
        es1.ClassName AS Class1,
        es2.ClassName AS Class2
    FROM ExamSchedules es1
    INNER JOIN ExamSchedules es2 ON es2.ExamId = @ExamId AND es2.Id > es1.Id
    WHERE es1.ExamId = @ExamId 
        AND es1.ExamDate = es2.ExamDate
        AND es1.RoomNo = es2.RoomNo
        AND es1.IsDeleted = 0 AND es2.IsDeleted = 0
        AND ((es1.StartsAt < es2.EndsAt) AND (es1.EndsAt > es2.StartsAt))
    ORDER BY es1.ExamDate, es1.StartsAt;

    -- Student Group Conflicts (same student group scheduled at same time)
    SELECT 
        'GROUP_CONFLICT' AS ConflictType,
        es1.ExamDate,
        es1.StartsAt,
        es1.EndsAt,
        sg.Name AS StudentGroup,
        es1.SubjectName AS Subject1,
        es2.SubjectName AS Subject2
    FROM ExamSchedules es1
    INNER JOIN ExamSchedules es2 ON es2.ExamId = @ExamId AND es2.Id > es1.Id
    INNER JOIN ExamSubjects esub1 ON esub1.Id = es1.ExamSubjectId
    INNER JOIN ExamSubjects esub2 ON esub2.Id = es2.ExamSubjectId
    INNER JOIN StudentGroups sg ON sg.Id = esub1.StudentGroupId AND sg.Id = esub2.StudentGroupId
    WHERE es1.ExamId = @ExamId 
        AND es1.ExamDate = es2.ExamDate
        AND es1.IsDeleted = 0 AND es2.IsDeleted = 0
        AND esub1.StudentGroupId IS NOT NULL
        AND ((es1.StartsAt < es2.EndsAt) AND (es1.EndsAt > es2.StartsAt))
    ORDER BY es1.ExamDate, es1.StartsAt;
END;
GO


-- =====================================================================
-- FIX ISSUES STORED PROCEDURES
-- Allow admins to fix validation issues directly from the wizard
-- =====================================================================

-- 10. sp_AssignTeacherToExamSubject
-- Assign or update teacher for an exam subject
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_AssignTeacherToExamSubject]
    @AcademicYearId INT,
    @SubjectId INT,
    @ClassId INT,
    @SectionId INT = NULL,
    @StudentGroupId INT = NULL,
    @TeacherId INT,
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AssignmentId INT;

    -- Check if assignment already exists
    SELECT @AssignmentId = Id FROM TeacherSubjectAssignments
    WHERE AcademicYearId = @AcademicYearId
        AND SubjectId = @SubjectId
        AND ClassId = @ClassId
        AND (SectionId = @SectionId OR (SectionId IS NULL AND @SectionId IS NULL))
        AND (StudentGroupId = @StudentGroupId OR (StudentGroupId IS NULL AND @StudentGroupId IS NULL))
        AND IsDeleted = 0;

    IF @AssignmentId IS NOT NULL
    BEGIN
        -- Update existing assignment
        UPDATE TeacherSubjectAssignments
        SET TeacherId = @TeacherId,
            UpdatedBy = @UserId,
            UpdatedAt = GETUTCDATE()
        WHERE Id = @AssignmentId;

        SELECT 'UPDATED' AS Status, @AssignmentId AS AssignmentId;
    END
    ELSE
    BEGIN
        -- Create new assignment
        INSERT INTO TeacherSubjectAssignments (AcademicYearId, SubjectId, ClassId, SectionId, StudentGroupId, TeacherId, IsActive, CreatedBy, CreatedAt)
        VALUES (@AcademicYearId, @SubjectId, @ClassId, @SectionId, @StudentGroupId, @TeacherId, 1, @UserId, GETUTCDATE());

        SELECT 'CREATED' AS Status, SCOPE_IDENTITY() AS AssignmentId;
    END
END;
GO


-- 11. sp_ConfigureExamSubjectComponents
-- Update component marks for an exam subject
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ConfigureExamSubjectComponents]
    @ExamSubjectId INT,
    @Components NVARCHAR(MAX),  -- JSON: [{ComponentId, MaxMarks, PassMarks, DisplayOrder}]
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Parse JSON components
        DECLARE @Components TABLE (
            ComponentId INT,
            MaxMarks DECIMAL(10,2),
            PassMarks DECIMAL(10,2),
            DisplayOrder INT
        );

        INSERT INTO @Components (ComponentId, MaxMarks, PassMarks, DisplayOrder)
        SELECT 
            JSON_VALUE(value, '$.ComponentId'),
            JSON_VALUE(value, '$.MaxMarks'),
            JSON_VALUE(value, '$.PassMarks'),
            JSON_VALUE(value, '$.DisplayOrder')
        FROM OPENJSON(@Components);

        -- Validate: Total MaxMarks = ExamSubject.FullMarks
        DECLARE @TotalMaxMarks DECIMAL(10,2) = (SELECT SUM(MaxMarks) FROM @Components);
        DECLARE @ExamSubjectFullMarks DECIMAL(10,2) = (SELECT FullMarks FROM ExamSubjects WHERE Id = @ExamSubjectId AND IsDeleted = 0);

        IF @TotalMaxMarks != @ExamSubjectFullMarks
        BEGIN
            THROW 50001, 'Total component max marks must equal exam subject full marks.', 1;
        END

        -- Validate: PassMarks <= MaxMarks for each component
        IF EXISTS (SELECT 1 FROM @Components WHERE PassMarks > MaxMarks)
        BEGIN
            THROW 50002, 'Pass marks cannot exceed max marks for one or more components.', 1;
        END

        -- Update components
        UPDATE esc
        SET esc.MaxMarks = c.MaxMarks,
            esc.PassMarks = c.PassMarks,
            esc.DisplayOrder = c.DisplayOrder,
            esc.UpdatedBy = @UserId,
            esc.UpdatedAt = GETUTCDATE()
        FROM ExamSubjectComponents esc
        INNER JOIN @Components c ON c.ComponentId = esc.ComponentId
        WHERE esc.ExamSubjectId = @ExamSubjectId AND esc.IsDeleted = 0;

        COMMIT TRANSACTION;
        SELECT 'SUCCESS' AS Status;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Status, ERROR_MESSAGE() AS Message;
    END CATCH;
END;
GO


-- 12. sp_AddSectionsToClass
-- Add missing sections to a class
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_AddSectionsToClass]
    @ClassId INT,
    @SectionNames NVARCHAR(MAX),  -- JSON array of section names
    @StudentGroupId INT = NULL,
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Sections TABLE (SectionName NVARCHAR(50), StudentGroupId INT);
        INSERT INTO @Sections (SectionName, StudentGroupId)
        SELECT JSON_VALUE(value, '$.Name'), JSON_VALUE(value, '$.StudentGroupId')
        FROM OPENJSON(@SectionNames);

        INSERT INTO Sections (SchoolClassId, Name, ParentSectionId, StudentGroupId, CreatedBy, CreatedAt)
        SELECT @ClassId, s.SectionName, NULL, s.StudentGroupId, @UserId, GETUTCDATE()
        FROM @Sections s
        WHERE NOT EXISTS (
            SELECT 1 FROM Sections 
            WHERE SchoolClassId = @ClassId AND Name = s.SectionName AND IsDeleted = 0
        );

        COMMIT TRANSACTION;
        SELECT 'SUCCESS' AS Status, (SELECT COUNT(*) FROM @Sections) AS CreatedCount;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Status, ERROR_MESSAGE() AS Message;
    END CATCH;
END;
GO


-- 13. sp_MapSubjectToClass
-- Map a subject to a class (ClassSubject)
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_MapSubjectToClass]
    @SubjectId INT,
    @ClassId INT,
    @StudentGroupId INT = NULL,
    @FullMarks DECIMAL(10,2) = 100,
    @PassMarks DECIMAL(10,2) = 33,
    @IsOptional BIT = 0,
    @DisplayOrder INT = 0,
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        -- Check if mapping already exists
        IF EXISTS (
            SELECT 1 FROM ClassSubjects 
            WHERE SubjectId = @SubjectId AND SchoolClassId = @ClassId 
            AND (StudentGroupId = @StudentGroupId OR (StudentGroupId IS NULL AND @StudentGroupId IS NULL))
            AND IsDeleted = 0
        )
        BEGIN
            SELECT 'EXISTS' AS Status, 0 AS CreatedCount;
        END
        ELSE
        BEGIN
            INSERT INTO ClassSubjects (SubjectId, SchoolClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, DisplayOrder, IsActive, CreatedBy, CreatedAt)
            VALUES (@SubjectId, @ClassId, @StudentGroupId, @FullMarks, @PassMarks, @IsOptional, @DisplayOrder, 1, @UserId, GETUTCDATE());

            SELECT 'CREATED' AS Status, SCOPE_IDENTITY() AS ClassSubjectId;
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Status, ERROR_MESSAGE() AS Message;
    END CATCH;
END;
GO


-- 14. sp_ConfigureSubjectMarkStructure
-- Configure component marks for a subject (SubjectMarkStructure)
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_ConfigureSubjectMarkStructure]
    @SubjectId INT,
    @ClassId INT = NULL,
    @StudentGroupId INT = NULL,
    @Components NVARCHAR(MAX),  -- JSON: [{ComponentId, FullMarks, PassMarks, DisplayOrder, IsActive}]
    @UserId NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @Components TABLE (
            ComponentId INT,
            FullMarks DECIMAL(10,2),
            PassMarks DECIMAL(10,2),
            DisplayOrder INT,
            IsActive BIT
        );

        INSERT INTO @Components (ComponentId, FullMarks, PassMarks, DisplayOrder, IsActive)
        SELECT 
            JSON_VALUE(value, '$.ComponentId'),
            JSON_VALUE(value, '$.FullMarks'),
            JSON_VALUE(value, '$.PassMarks'),
            JSON_VALUE(value, '$.DisplayOrder'),
            JSON_VALUE(value, '$.IsActive')
        FROM OPENJSON(@Components);

        -- Validate: PassMarks <= FullMarks
        IF EXISTS (SELECT 1 FROM @Components WHERE PassMarks > FullMarks)
        BEGIN
            THROW 50003, 'Pass marks cannot exceed full marks for one or more components.', 1;
        END

        -- Deactivate existing structures for this subject/class/group
        UPDATE SubjectMarkStructures
        SET IsActive = 0, UpdatedBy = @UserId, UpdatedAt = GETUTCDATE()
        WHERE SubjectId = @SubjectId 
            AND (ClassId = @ClassId OR (@ClassId IS NULL AND ClassId IS NULL))
            AND (StudentGroupId = @StudentGroupId OR (@StudentGroupId IS NULL AND StudentGroupId IS NULL))
            AND IsDeleted = 0;

        -- Insert new structures
        INSERT INTO SubjectMarkStructures (ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, DisplayOrder, IsActive, CreatedBy, CreatedAt)
        SELECT c.ComponentId, @ClassId, @SubjectId, @StudentGroupId, c.FullMarks, c.PassMarks, c.DisplayOrder, c.IsActive, @UserId, GETUTCDATE()
        FROM @Components c;

        COMMIT TRANSACTION;
        SELECT 'SUCCESS' AS Status, (SELECT COUNT(*) FROM @Components) AS ConfiguredCount;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        SELECT 'ERROR' AS Status, ERROR_MESSAGE() AS Message;
    END CATCH;
END;
GO


-- 15. sp_CheckExamPublishReadiness
-- Final check before allowing exam to be published
-- Returns 1 if 100% ready, 0 otherwise with detailed blockers
-- =====================================================================
CREATE OR ALTER PROCEDURE [dbo].[sp_CheckExamPublishReadiness]
    @ExamId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Exam TABLE (Id INT, Name NVARCHAR(100), Status INT, IsLocked BIT, AcademicYearId INT);
    INSERT INTO @Exam SELECT Id, Name, Status, IsLocked, AcademicYearId FROM Exams WHERE Id = @ExamId AND IsDeleted = 0;

    IF NOT EXISTS (SELECT 1 FROM @Exam)
    BEGIN
        SELECT 0 AS IsReady, 'Exam not found' AS Blocker;
        RETURN;
    END

    DECLARE @Blockers TABLE (Blocker NVARCHAR(MAX));
    
    -- Check exam status
    IF (SELECT Status FROM @Exam) != 0 -- Draft = 0
    BEGIN
        INSERT INTO @Blockers VALUES ('Exam is not in Draft status');
    END

    IF (SELECT IsLocked FROM @Exam) = 1
    BEGIN
        INSERT INTO @Blockers VALUES ('Exam is locked');
    END

    -- Check for missing sections
    IF EXISTS (
        SELECT 1 FROM ExamClasses ec
        LEFT JOIN ExamSections es ON es.ExamClassId = ec.Id AND es.IsDeleted = 0
        WHERE ec.ExamId = @ExamId AND ec.IsDeleted = 0 AND es.Id IS NULL
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('One or more classes have no sections');
    END

    -- Check for missing subjects
    IF EXISTS (
        SELECT 1 FROM ExamClasses ec
        LEFT JOIN ExamSubjects es ON es.ExamId = ec.ExamId AND es.ClassId = ec.ClassId AND es.IsDeleted = 0
        WHERE ec.ExamId = @ExamId AND ec.IsDeleted = 0 AND es.Id IS NULL
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('One or more classes have no subjects');
    END

    -- Check for missing components
    IF EXISTS (
        SELECT 1 FROM ExamSubjects es
        LEFT JOIN ExamSubjectComponents esc ON esc.ExamSubjectId = es.Id AND esc.IsDeleted = 0
        WHERE es.ExamId = @ExamId AND es.IsDeleted = 0 AND esc.Id IS NULL
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('One or more subjects have no components');
    END

    -- Check for missing teachers
    IF EXISTS (
        SELECT 1 FROM ExamSubjects 
        WHERE ExamId = @ExamId AND IsDeleted = 0 AND IsActive = 1 AND TeacherId IS NULL
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('One or more subjects have no assigned teacher');
    END

    -- Check for missing schedules
    IF NOT EXISTS (
        SELECT 1 FROM ExamSchedules WHERE ExamId = @ExamId AND IsDeleted = 0
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('Exam schedule has not been generated');
    END

    -- Check for schedule conflicts
    IF EXISTS (
        SELECT 1 FROM ExamSchedules es1
        INNER JOIN ExamSchedules es2 ON es2.ExamId = @ExamId AND es2.Id > es1.Id
        WHERE es1.ExamId = @ExamId AND es1.IsDeleted = 0 AND es2.IsDeleted = 0
        AND es1.ExamDate = es2.ExamDate
        AND ((es1.StartsAt < es2.EndsAt) AND (es1.EndsAt > es2.StartsAt))
        AND (
            -- Teacher conflict
            EXISTS (SELECT 1 FROM ExamSubjects esub1 
                INNER JOIN ExamSubjects esub2 ON esub2.Id = es2.ExamSubjectId
                WHERE esub1.Id = es1.ExamSubjectId AND esub1.TeacherId = esub2.TeacherId AND esub1.TeacherId IS NOT NULL)
            OR
            -- Room conflict
            (es1.RoomNo = es2.RoomNo)
            OR
            -- Group conflict
            EXISTS (SELECT 1 FROM ExamSubjects esub1 
                INNER JOIN ExamSubjects esub2 ON esub2.Id = es2.ExamSubjectId
                WHERE esub1.Id = es1.ExamSubjectId AND esub1.StudentGroupId = esub2.StudentGroupId AND esub1.StudentGroupId IS NOT NULL)
        )
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('Schedule has conflicts (teacher, room, or group)');
    END

    -- Check total marks validation
    IF EXISTS (
        SELECT 1 FROM ExamSubjects es
        WHERE es.ExamId = @ExamId AND es.IsDeleted = 0
        AND es.FullMarks != (
            SELECT ISNULL(SUM(esc.MaxMarks), 0) FROM ExamSubjectComponents esc 
            WHERE esc.ExamSubjectId = es.Id AND esc.IsDeleted = 0
        )
    )
    BEGIN
        INSERT INTO @Blockers VALUES ('Component marks do not sum to subject full marks');
    END

    -- Final result
    IF EXISTS (SELECT 1 FROM @Blockers)
    BEGIN
        SELECT 0 AS IsReady, Blocker FROM @Blockers;
    END
    ELSE
    BEGIN
        SELECT 1 AS IsReady, 'Ready to publish' AS Blocker;
    END
END;
GO