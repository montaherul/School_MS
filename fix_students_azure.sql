IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='Students' AND COLUMN_NAME='OptionalSubjectId')
BEGIN
    ALTER TABLE Students ADD OptionalSubjectId INT NULL;
    PRINT 'OptionalSubjectId column added.';
END
ELSE
    PRINT 'OptionalSubjectId already exists.';

UPDATE Students SET AssignedReligionSubjectId = 30 WHERE AssignedReligionSubjectId IS NULL AND Religion = 'Islam';
PRINT 'Backfilled AssignedReligionSubjectId for Islam students.';
