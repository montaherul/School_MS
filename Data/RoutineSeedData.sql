-- ============================================================
-- ROUTINE MODULE MASTER SEED DATA
-- Idempotent: safe to run multiple times
-- ============================================================
SET NOCOUNT ON;

DECLARE @AcademicYearId INT = 1;
DECLARE @CreatedBy NVARCHAR(64) = 'system';
DECLARE @Now DATETIME2 = SYSDATETIME();

-- ============================================================
-- 1. ROUTINE PERIODS (Time Slots)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM RoutinePeriods WHERE IsDeleted = 0)
BEGIN
    INSERT INTO RoutinePeriods (Name, StartTime, EndTime, PeriodNumber, IsBreak, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        ('Period 1', '08:00:00', '08:45:00', 1, 0, 1, @CreatedBy, @Now, 0),
        ('Period 2', '08:45:00', '09:30:00', 2, 0, 1, @CreatedBy, @Now, 0),
        ('Morning Break', '09:30:00', '09:45:00', 3, 1, 1, @CreatedBy, @Now, 0),
        ('Period 3', '09:45:00', '10:30:00', 4, 0, 1, @CreatedBy, @Now, 0),
        ('Period 4', '10:30:00', '11:15:00', 5, 0, 1, @CreatedBy, @Now, 0),
        ('Period 5', '11:15:00', '12:00:00', 6, 0, 1, @CreatedBy, @Now, 0),
        ('Lunch Break', '12:00:00', '12:45:00', 7, 1, 1, @CreatedBy, @Now, 0),
        ('Period 6', '12:45:00', '13:30:00', 8, 0, 1, @CreatedBy, @Now, 0),
        ('Period 7', '13:30:00', '14:15:00', 9, 0, 1, @CreatedBy, @Now, 0),
        ('Period 8', '14:15:00', '15:00:00', 10, 0, 1, @CreatedBy, @Now, 0),
        ('Asr Break', '15:00:00', '15:15:00', 11, 1, 1, @CreatedBy, @Now, 0),
        ('Period 9', '15:15:00', '16:00:00', 12, 0, 1, @CreatedBy, @Now, 0)
    PRINT '✅ Seeded: RoutinePeriods (12)';
END
ELSE
    PRINT '⏭️ Skipped: RoutinePeriods (already has data)';

-- ============================================================
-- 2. WORKING DAYS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM WorkingDays WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId)
BEGIN
    INSERT INTO WorkingDays (AcademicYearId, DayName, DayNumber, IsWorkingDay, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 'Saturday', 1, 1, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Sunday',   2, 1, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Monday',   3, 1, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Tuesday',  4, 1, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Wednesday',5, 1, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Thursday', 6, 1, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Friday',   7, 0, @CreatedBy, @Now, 0)
    PRINT '✅ Seeded: WorkingDays (7)';
END
ELSE
    PRINT '⏭️ Skipped: WorkingDays (already has data)';

-- ============================================================
-- 3. ROOMS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM Rooms WHERE IsDeleted = 0)
BEGIN
    INSERT INTO Rooms (RoomNo, Name, Capacity, Building, Floor, RoomType, IsLab, RequiresDoublePeriod, IsActive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        ('C-101', 'Classroom 101', 45, 'Main Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-102', 'Classroom 102', 45, 'Main Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-103', 'Classroom 103', 40, 'Main Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-104', 'Classroom 104', 40, 'Main Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-201', 'Classroom 201', 45, 'Main Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-202', 'Classroom 202', 45, 'Main Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-203', 'Classroom 203', 40, 'Main Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-204', 'Classroom 204', 40, 'Main Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-301', 'Classroom 301', 35, 'Science Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-302', 'Classroom 302', 35, 'Science Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-303', 'Classroom 303', 35, 'Science Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-304', 'Classroom 304', 35, 'Science Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-305', 'Classroom 305', 30, 'Science Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-306', 'Classroom 306', 30, 'Science Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-401', 'Classroom 401', 30, 'Arts Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-402', 'Classroom 402', 30, 'Arts Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-403', 'Classroom 403', 25, 'Arts Building', 1, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-404', 'Classroom 404', 25, 'Arts Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-405', 'Classroom 405', 25, 'Arts Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('C-406', 'Classroom 406', 25, 'Arts Building', 2, 'Classroom', 0, 0, 1, @CreatedBy, @Now, 0),
        ('LB-1', 'Computer Lab 1', 30, 'Science Building', 3, 'Laboratory', 1, 1, 1, @CreatedBy, @Now, 0),
        ('LB-2', 'Computer Lab 2', 30, 'Science Building', 3, 'Laboratory', 1, 1, 1, @CreatedBy, @Now, 0),
        ('LB-3', 'Computer Lab 3', 25, 'Science Building', 3, 'Laboratory', 1, 1, 1, @CreatedBy, @Now, 0),
        ('LB-4', 'Physics Lab', 25, 'Science Building', 2, 'Laboratory', 1, 1, 1, @CreatedBy, @Now, 0),
        ('LB-5', 'Chemistry Lab', 25, 'Science Building', 2, 'Laboratory', 1, 1, 1, @CreatedBy, @Now, 0),
        ('LB-6', 'Biology Lab', 25, 'Science Building', 2, 'Laboratory', 1, 1, 1, @CreatedBy, @Now, 0),
        ('SR-1', 'Library', 60, 'Main Building', 1, 'Library', 0, 0, 1, @CreatedBy, @Now, 0),
        ('SR-2', 'Auditorium', 200, 'Main Building', 1, 'Auditorium', 0, 0, 1, @CreatedBy, @Now, 0),
        ('SR-3', 'Seminar Hall', 80, 'Main Building', 2, 'Seminar Hall', 0, 0, 1, @CreatedBy, @Now, 0),
        ('SR-4', 'Staff Room', 20, 'Main Building', 1, 'Staff Room', 0, 0, 1, @CreatedBy, @Now, 0)
    PRINT '✅ Seeded: Rooms (30)';
END
ELSE
    PRINT '⏭️ Skipped: Rooms (already has data)';

-- ============================================================
-- 4. TEACHER AVAILABILITY
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM TeacherAvailabilities WHERE IsDeleted = 0)
BEGIN
    DECLARE @TId INT, @PId INT, @DNum INT;
    DECLARE teacher_cursor CURSOR FOR SELECT Id FROM Teachers WHERE IsDeleted = 0;
    OPEN teacher_cursor;
    FETCH NEXT FROM teacher_cursor INTO @TId;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        DECLARE period_cursor CURSOR FOR SELECT Id, PeriodNumber FROM RoutinePeriods WHERE IsDeleted = 0 AND IsActive = 1 AND IsBreak = 0;
        OPEN period_cursor;
        FETCH NEXT FROM period_cursor INTO @PId, @DNum;
        WHILE @@FETCH_STATUS = 0
        BEGIN
            DECLARE day_cursor CURSOR FOR SELECT DayNumber FROM WorkingDays WHERE IsDeleted = 0 AND AcademicYearId = @AcademicYearId AND IsWorkingDay = 1;
            OPEN day_cursor;
            FETCH NEXT FROM day_cursor INTO @DNum;
            WHILE @@FETCH_STATUS = 0
            BEGIN
                INSERT INTO TeacherAvailabilities (TeacherId, RoutinePeriodId, DayNumber, IsAvailable, CreatedBy, CreatedAt, IsDeleted)
                VALUES (@TId, @PId, @DNum, 1, @CreatedBy, @Now, 0);
                FETCH NEXT FROM day_cursor INTO @DNum;
            END
            CLOSE day_cursor; DEALLOCATE day_cursor;
            FETCH NEXT FROM period_cursor INTO @PId, @DNum;
        END
        CLOSE period_cursor; DEALLOCATE period_cursor;
        FETCH NEXT FROM teacher_cursor INTO @TId;
    END
    CLOSE teacher_cursor; DEALLOCATE teacher_cursor;
    PRINT '✅ Seeded: TeacherAvailabilities';
END
ELSE
    PRINT '⏭️ Skipped: TeacherAvailabilities (already has data)';

-- ============================================================
-- 5. SUBJECT REQUIREMENTS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM SubjectRequirements WHERE IsDeleted = 0)
BEGIN
    -- Class 6 (Id=6): Sections A(11), B(12) - No groups
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 6, 11, NULL, 1, 1, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),  -- Bangla
        (@AcademicYearId, 6, 11, NULL, 2, 2, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),  -- English
        (@AcademicYearId, 6, 11, NULL, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),  -- Math
        (@AcademicYearId, 6, 11, NULL, 4, 4, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),  -- General Science
        (@AcademicYearId, 6, 11, NULL, 5, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),  -- BGS
        (@AcademicYearId, 6, 11, NULL, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0), -- ICT
        (@AcademicYearId, 6, 11, NULL, 6, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),  -- Religion
        (@AcademicYearId, 6, 11, NULL, 8, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),  -- PE
        -- Class 6 Section B (Id=12)
        (@AcademicYearId, 6, 12, NULL, 1, 1, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 2, 2, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 4, 4, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 5, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 6, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 8, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 7 (Id=7): Sections A(13), B(14)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 7, 13, NULL, 1, 1, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 2, 2, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 13, 4, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Science
        (@AcademicYearId, 7, 13, NULL, 5, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 6, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 8, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 1, 1, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 2, 2, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 13, 4, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 5, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 6, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 14, NULL, 8, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 8 (Id=8): Sections A(15), B(16)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 8, 15, NULL, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0), -- Bangla 1st
        (@AcademicYearId, 8, 15, NULL, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0), -- Bangla 2nd
        (@AcademicYearId, 8, 15, NULL, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0), -- English 1st
        (@AcademicYearId, 8, 15, NULL, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0), -- English 2nd
        (@AcademicYearId, 8, 15, NULL, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 13, 4, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 5, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 6, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 8, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 13, 4, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 5, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 6, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 16, NULL, 8, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 9 Science (Id=9, Group=Science/1): Sections A(18), B(19)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 9, 18, 1, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 16, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0), -- Physics
        (@AcademicYearId, 9, 18, 1, 17, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0), -- Chemistry
        (@AcademicYearId, 9, 18, 1, 18, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0), -- Biology
        (@AcademicYearId, 9, 18, 1, 19, 3, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Higher Math
        (@AcademicYearId, 9, 18, 1, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0), -- Islam
        (@AcademicYearId, 9, 18, 1, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0), -- PE
        (@AcademicYearId, 9, 19, 1, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 16, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 17, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 18, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 19, 3, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 19, 1, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 9 Business Studies (Id=9, Group=BusinessStudies/2): Sections A(21), B(22)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 9, 21, 2, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 20, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Accounting
        (@AcademicYearId, 9, 21, 2, 22, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Business Entrepreneurship
        (@AcademicYearId, 9, 21, 2, 21, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Finance & Banking
        (@AcademicYearId, 9, 21, 2, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 21, 2, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 20, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 22, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 21, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 22, 2, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 9 Humanities (Id=9, Group=Humanities/3): Sections A(24), B(25)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 9, 24, 3, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 23, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- History
        (@AcademicYearId, 9, 24, 3, 24, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Geography
        (@AcademicYearId, 9, 24, 3, 26, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0), -- Civics
        (@AcademicYearId, 9, 24, 3, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 24, 3, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 23, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 24, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 26, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 25, 3, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 10 Science (Id=10, Group=Science/1): Sections A(27), B(28)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 10, 27, 1, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 16, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 17, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 18, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 19, 3, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 16, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 17, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 18, 4, 3, 1, 1, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 19, 3, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 28, 1, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 10 Business Studies (Id=10, Group=BusinessStudies/2): Sections A(30), B(31)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 10, 30, 2, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 20, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 22, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 21, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 30, 2, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 20, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 22, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 21, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 31, 2, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    -- Class 10 Humanities (Id=10, Group=Humanities/3): Sections A(33), B(34)
    INSERT INTO SubjectRequirements (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, PeriodsPerWeek, RequiresLab, RequiresDoublePeriod, Priority, MaxConsecutive, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 10, 33, 3, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 23, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 24, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 26, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 33, 3, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 9, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 10, 1, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 11, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 12, 2, 3, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 3, 3, 4, 0, 0, 1, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 23, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 24, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 26, 5, 3, 0, 0, 2, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 14, 4, 2, 1, 1, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 30, 6, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 34, 3, 28, 7, 2, 0, 0, 3, 2, @CreatedBy, @Now, 0);

    PRINT '✅ Seeded: SubjectRequirements';
END
ELSE
    PRINT '⏭️ Skipped: SubjectRequirements (already has data)';

-- ============================================================
-- 6. ROUTINE VERSIONS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM RoutineVersions WHERE IsDeleted = 0)
BEGIN
    INSERT INTO RoutineVersions (AcademicYearId, Name, Status, PublishedAt, ApprovedAt, EntryCount, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 'Draft v1.0', 'Draft', NULL, NULL, 0, @CreatedBy, @Now, 0),
        (@AcademicYearId, 'Published v1.0', 'Published', DATEADD(DAY, -2, @Now), DATEADD(DAY, -3, @Now), 0, @CreatedBy, DATEADD(DAY, -5, @Now), 0),
        (@AcademicYearId, 'Archived v0.9', 'Archived', DATEADD(DAY, -10, @Now), DATEADD(DAY, -11, @Now), 0, @CreatedBy, DATEADD(DAY, -15, @Now), 0)
    PRINT '✅ Seeded: RoutineVersions (3)';
END
ELSE
    PRINT '⏭️ Skipped: RoutineVersions (already has data)';

-- ============================================================
-- 7. ROUTINE GENERATIONS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM RoutineGenerations WHERE IsDeleted = 0)
BEGIN
    INSERT INTO RoutineGenerations (AcademicYearId, Status, StartedAt, CompletedAt, TotalAssignments, SuccessfulAssignments, FailedAssignments, ConflictsDetected, ErrorMessage, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 'Completed', DATEADD(DAY, -20, @Now), DATEADD(DAY, -20, @Now), 120, 118, 2, 3, NULL, 'Admin', @Now, 0),
        (@AcademicYearId, 'Completed', DATEADD(DAY, -15, @Now), DATEADD(DAY, -15, @Now), 120, 120, 0, 0, NULL, 'Admin', @Now, 0),
        (@AcademicYearId, 'Completed', DATEADD(DAY, -10, @Now), DATEADD(DAY, -10, @Now), 120, 119, 1, 1, NULL, 'Admin', @Now, 0),
        (@AcademicYearId, 'Completed', DATEADD(DAY, -7, @Now), DATEADD(DAY, -7, @Now), 120, 120, 0, 0, NULL, 'Admin', @Now, 0),
        (@AcademicYearId, 'Completed', DATEADD(DAY, -3, @Now), DATEADD(DAY, -3, @Now), 120, 120, 0, 0, NULL, 'Admin', @Now, 0),
        (@AcademicYearId, 'Failed', DATEADD(DAY, -5, @Now), DATEADD(DAY, -5, @Now), 0, 0, 0, 0, 'Insufficient teachers for Class 10 Science Physics requirement', 'Admin', @Now, 0),
        (@AcademicYearId, 'Failed', DATEADD(DAY, -4, @Now), DATEADD(DAY, -4, @Now), 0, 0, 0, 0, 'Room conflict detected: Lab LB-1 double-booked', 'Admin', @Now, 0),
        (@AcademicYearId, 'Cancelled', DATEADD(DAY, -2, @Now), NULL, 45, 30, 15, 5, 'Cancelled by admin due to configuration changes', 'Admin', @Now, 0)
    PRINT '✅ Seeded: RoutineGenerations (8)';
END
ELSE
    PRINT '⏭️ Skipped: RoutineGenerations (already has data)';

-- ============================================================
-- 8. ROUTINE ENTRIES (Timetable)
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM RoutineEntries WHERE IsDeleted = 0)
BEGIN
    -- Get generation and version IDs
    DECLARE @GenId1 INT = (SELECT TOP 1 Id FROM RoutineGenerations WHERE Status = 'Completed' AND IsDeleted = 0 ORDER BY Id DESC);
    DECLARE @VerIdPublished INT = (SELECT TOP 1 Id FROM RoutineVersions WHERE Status = 'Published' AND IsDeleted = 0);
    DECLARE @VerIdDraft INT = (SELECT TOP 1 Id FROM RoutineVersions WHERE Status = 'Draft' AND IsDeleted = 0);

    -- Class 6 Section A (Id=11), Day 1 (Saturday)
    INSERT INTO RoutineEntries (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, RoomId, RoutinePeriodId, DayNumber, IsLab, GenerationId, VersionId, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 6, 11, NULL, 1, 1, 1, 1, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 2, 2, 2, 2, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 3, 3, 3, 4, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 4, 4, 4, 5, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 5, 5, 5, 6, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 6, 6, 6, 8, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 8, 7, 7, 9, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        -- Class 6 Section A, Day 2 (Sunday)
        (@AcademicYearId, 6, 11, NULL, 3, 3, 8, 1, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 1, 1, 9, 2, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 2, 2, 10, 4, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 4, 4, 11, 5, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 14, 4, 21, 8, 2, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 5, 5, 12, 9, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 11, NULL, 6, 6, 13, 10, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0);

    -- Class 6 Section B (Id=12)
    INSERT INTO RoutineEntries (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, RoomId, RoutinePeriodId, DayNumber, IsLab, GenerationId, VersionId, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 6, 12, NULL, 2, 2, 14, 1, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 1, 1, 15, 2, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 4, 4, 16, 4, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 3, 3, 17, 5, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 5, 5, 18, 6, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 8, 7, 19, 8, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 6, 12, NULL, 6, 6, 20, 9, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0);

    -- Class 7 Section A (Id=13)
    INSERT INTO RoutineEntries (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, RoomId, RoutinePeriodId, DayNumber, IsLab, GenerationId, VersionId, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 7, 13, NULL, 3, 3, 1, 1, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 1, 1, 2, 2, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 2, 2, 3, 4, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 13, 4, 4, 5, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 5, 5, 5, 6, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 14, 4, 21, 8, 3, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 6, 6, 6, 9, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 8, 7, 7, 10, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        -- Day 4
        (@AcademicYearId, 7, 13, NULL, 1, 1, 8, 1, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 3, 3, 9, 2, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 2, 2, 10, 4, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 13, 4, 11, 5, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 5, 5, 12, 6, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 6, 6, 13, 8, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 7, 13, NULL, 8, 7, 14, 9, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0);

    -- Class 8 Section A (Id=15)
    INSERT INTO RoutineEntries (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, RoomId, RoutinePeriodId, DayNumber, IsLab, GenerationId, VersionId, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 8, 15, NULL, 9, 1, 15, 1, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 11, 2, 16, 2, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 3, 3, 17, 4, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 13, 4, 18, 5, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 5, 5, 19, 6, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 10, 1, 20, 8, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 12, 2, 1, 9, 1, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 8, 15, NULL, 14, 4, 22, 10, 1, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0);

    -- Class 9 Science Section A (Id=18, Group=1)
    INSERT INTO RoutineEntries (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, RoomId, RoutinePeriodId, DayNumber, IsLab, GenerationId, VersionId, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 9, 18, 1, 9, 1, 2, 1, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 11, 2, 3, 2, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 3, 3, 4, 4, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 16, 4, 24, 5, 2, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 17, 4, 25, 8, 2, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 19, 3, 5, 9, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 30, 6, 6, 10, 2, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        -- Day 3
        (@AcademicYearId, 9, 18, 1, 10, 1, 7, 1, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 12, 2, 8, 2, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 3, 3, 9, 4, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 18, 4, 26, 5, 3, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 14, 4, 23, 8, 3, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 9, 18, 1, 28, 7, 10, 9, 3, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0);

    -- Class 10 Science Section A (Id=27, Group=1)
    INSERT INTO RoutineEntries (AcademicYearId, ClassId, SectionId, GroupId, SubjectId, TeacherId, RoomId, RoutinePeriodId, DayNumber, IsLab, GenerationId, VersionId, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@AcademicYearId, 10, 27, 1, 9, 1, 11, 1, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 11, 2, 12, 2, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 3, 3, 13, 4, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 16, 4, 24, 5, 4, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 18, 4, 26, 8, 4, 1, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 19, 3, 14, 9, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0),
        (@AcademicYearId, 10, 27, 1, 30, 6, 15, 10, 4, 0, @GenId1, @VerIdPublished, @CreatedBy, @Now, 0);

    -- Update version entry counts
    UPDATE RoutineVersions SET EntryCount = (SELECT COUNT(*) FROM RoutineEntries WHERE VersionId = @VerIdPublished AND IsDeleted = 0) WHERE Id = @VerIdPublished;
    UPDATE RoutineVersions SET EntryCount = (SELECT COUNT(*) FROM RoutineEntries WHERE VersionId = @VerIdDraft AND IsDeleted = 0) WHERE Id = @VerIdDraft;

    PRINT '✅ Seeded: RoutineEntries';
END
ELSE
    PRINT '⏭️ Skipped: RoutineEntries (already has data)';

-- ============================================================
-- 9. ROUTINE CONFLICTS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM RoutineConflicts WHERE IsDeleted = 0)
BEGIN
    DECLARE @GenIdLatest INT = (SELECT TOP 1 Id FROM RoutineGenerations WHERE IsDeleted = 0 ORDER BY Id DESC);
    DECLARE @GenIdPrev INT = (SELECT Id FROM RoutineGenerations WHERE IsDeleted = 0 ORDER BY Id OFFSET 1 ROW FETCH NEXT 1 ROW ONLY);

    INSERT INTO RoutineConflicts (GenerationId, ConflictType, Description, TeacherId, RoomId, SubjectId, ClassId, RoutinePeriodId, DayNumber, IsResolved, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@GenIdPrev, 'TeacherConflict', 'Teacher Rafiqul Hasan double-booked: Class 6 Math and Class 7 Math at same time', 5, NULL, 3, 6, 1, 1, 1, @CreatedBy, @Now, 0),
        (@GenIdPrev, 'RoomConflict', 'Room C-101 double-booked: Class 6 Bangla and Class 7 English', NULL, 1, NULL, NULL, 2, 1, 1, @CreatedBy, @Now, 0),
        (@GenIdPrev, 'StudentConflict', 'Class 9 Science Section A has two subjects at same day-period', NULL, NULL, NULL, 9, 4, 2, 1, @CreatedBy, @Now, 0),
        (@GenIdPrev, 'HolidayConflict', 'Entry scheduled on Friday (holiday) for Class 8', NULL, NULL, NULL, 8, 5, 7, 1, @CreatedBy, @Now, 0),
        (@GenIdPrev, 'TeacherConflict', 'Teacher Shamim Ara overloaded: exceeds max 6 periods/day', 7, NULL, NULL, NULL, 8, 3, 0, @CreatedBy, @Now, 0),
        (@GenIdPrev, 'RoomConflict', 'Computer Lab LB-1 overbooked for ICT lab sessions', NULL, 21, NULL, NULL, 9, 2, 0, @CreatedBy, @Now, 0),
        (@GenIdPrev, 'HolidayConflict', 'Entry scheduled on weekly holiday for Class 7', NULL, NULL, NULL, 7, 10, 7, 0, @CreatedBy, @Now, 0)
    PRINT '✅ Seeded: RoutineConflicts (7)';
END
ELSE
    PRINT '⏭️ Skipped: RoutineConflicts (already has data)';

-- ============================================================
-- 10. SUBSTITUTE ASSIGNMENTS
-- ============================================================
IF NOT EXISTS (SELECT 1 FROM SubstituteAssignments WHERE IsDeleted = 0)
BEGIN
    DECLARE @EntryId1 INT = (SELECT TOP 1 Id FROM RoutineEntries WHERE IsDeleted = 0 ORDER BY Id);
    DECLARE @AdminUserId INT = (SELECT TOP 1 Id FROM Users WHERE IsDeleted = 0 ORDER BY Id);

    INSERT INTO SubstituteAssignments (RoutineEntryId, OriginalTeacherId, SubstituteTeacherId, AssignedById, AssignmentDate, EffectiveDate, PeriodNumber, DayNumber, Status, Reason, ApprovedAt, Notes, CreatedBy, CreatedAt, IsDeleted)
    VALUES
        (@EntryId1, 1, 3, ISNULL(@AdminUserId, 1), @Now, DATEADD(DAY, 1, @Now), 1, 1, 'Approved', 'Teacher on medical leave', @Now, 'Approved by admin', @CreatedBy, @Now, 0),
        (@EntryId1 + 1, 2, 1, ISNULL(@AdminUserId, 1), @Now, DATEADD(DAY, 1, @Now), 2, 1, 'Approved', 'Personal emergency', @Now, NULL, @CreatedBy, @Now, 0),
        (@EntryId1 + 2, 3, 5, ISNULL(@AdminUserId, 1), DATEADD(DAY, -1, @Now), DATEADD(DAY, 2, @Now), 4, 3, 'Approved', 'Training workshop attendance', DATEADD(DAY, -1, @Now), 'Substitute confirmed', @CreatedBy, @Now, 0),
        (@EntryId1 + 3, 4, 2, ISNULL(@AdminUserId, 1), DATEADD(DAY, -2, @Now), DATEADD(DAY, -1, @Now), 5, 4, 'Approved', 'Family event', DATEADD(DAY, -2, @Now), NULL, @CreatedBy, @Now, 0),
        (@EntryId1 + 4, 5, 6, ISNULL(@AdminUserId, 1), DATEADD(DAY, -2, @Now), @Now, 6, 1, 'Approved', 'Curriculum development meeting', @Now, 'Handled by substitute', @CreatedBy, @Now, 0),
        (@EntryId1 + 5, 6, 7, ISNULL(@AdminUserId, 1), DATEADD(DAY, -3, @Now), DATEADD(DAY, -2, @Now), 8, 2, 'Approved', 'Sick leave', DATEADD(DAY, -3, @Now), NULL, @CreatedBy, @Now, 0),
        (@EntryId1 + 6, 7, 4, ISNULL(@AdminUserId, 1), DATEADD(DAY, -3, @Now), DATEADD(DAY, -1, @Now), 9, 3, 'Approved', 'Department head meeting', DATEADD(DAY, -3, @Now), 'Rescheduled', @CreatedBy, @Now, 0),
        (@EntryId1, 1, 5, ISNULL(@AdminUserId, 1), DATEADD(DAY, -5, @Now), DATEADD(DAY, -4, @Now), 1, 5, 'Approved', 'External exam duty', DATEADD(DAY, -5, @Now), NULL, @CreatedBy, @Now, 0),
        (@EntryId1 + 2, 3, 1, ISNULL(@AdminUserId, 1), DATEADD(DAY, -5, @Now), @Now, 4, 6, 'Pending', 'Requested swap', NULL, 'Awaiting approval', @CreatedBy, @Now, 0),
        (@EntryId1 + 3, 4, 6, ISNULL(@AdminUserId, 1), DATEADD(DAY, -1, @Now), DATEADD(DAY, 3, @Now), 5, 1, 'Pending', 'Professional development', NULL, 'Pending confirmation', @CreatedBy, @Now, 0)
    PRINT '✅ Seeded: SubstituteAssignments (10)';
END
ELSE
    PRINT '⏭️ Skipped: SubstituteAssignments (already has data)';

PRINT '';
PRINT '============================================';
PRINT '✅ ROUTINE MASTER SEED DATA COMPLETE';
PRINT '============================================';
PRINT '';
GO
