CREATE OR ALTER PROCEDURE sp_GetRoutineDashboard
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TotalTeachers INT, @TotalRooms INT, @TotalClasses INT,
            @TotalSubjects INT, @TotalEntries INT, @TotalConflicts INT,
            @LastGenerationId INT, @LastGenerationStatus NVARCHAR(50),
            @LastGenerationDate DATETIME,
            @PublishedVersionId INT, @PublishedVersionName NVARCHAR(100);

    SELECT @TotalTeachers = COUNT(DISTINCT TeacherId)
    FROM RoutineEntries WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId;

    SELECT @TotalRooms = COUNT(DISTINCT RoomId)
    FROM RoutineEntries WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId;

    SELECT @TotalClasses = COUNT(DISTINCT ClassId)
    FROM RoutineEntries WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId;

    SELECT @TotalSubjects = COUNT(DISTINCT SubjectId)
    FROM RoutineEntries WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId;

    SELECT @TotalEntries = COUNT(*)
    FROM RoutineEntries WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId;

    SELECT @TotalConflicts = COUNT(*)
    FROM RoutineConflicts WITH(NOLOCK)
    WHERE IsDeleted = 0 AND IsResolved = 0;

    SELECT TOP 1
        @LastGenerationId = Id,
        @LastGenerationStatus = Status,
        @LastGenerationDate = CompletedAt
    FROM RoutineGenerations WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId
    ORDER BY CreatedAt DESC;

    SELECT TOP 1
        @PublishedVersionId = Id,
        @PublishedVersionName = Name
    FROM RoutineVersions WITH(NOLOCK)
    WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId AND Status = 'Published'
    ORDER BY PublishedAt DESC;

    SELECT
        @TotalTeachers AS TotalTeachers,
        @TotalRooms AS TotalRooms,
        @TotalClasses AS TotalClasses,
        @TotalSubjects AS TotalSubjects,
        @TotalEntries AS TotalEntries,
        @TotalConflicts AS TotalConflicts,
        @LastGenerationId AS LastGenerationId,
        @LastGenerationStatus AS LastGenerationStatus,
        @LastGenerationDate AS LastGenerationDate,
        @PublishedVersionId AS PublishedVersionId,
        @PublishedVersionName AS PublishedVersionName;
END;
GO
