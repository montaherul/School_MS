-- Seed PromotionPolicies for AcademicYear 2026
DECLARE @AcademicYearId INT = 1;

-- Class One - Five: Simple GPA >= 1.00
INSERT INTO [PromotionPolicies] ([AcademicYearId],[SchoolClassId],[Name],[PrimaryMethod],[MinimumGpa],[IsActive],[CreatedBy],[CreatedAt])
SELECT @AcademicYearId, Id, Name + ' Promotion Policy', 1, 1.00, 1, 'seed', GETUTCDATE()
FROM Classes WHERE Id IN (1,2,3,4,5) AND IsDeleted = 0;

-- Class Six - Eight: GPA >= 1.00, max 2 failed subjects
INSERT INTO [PromotionPolicies] ([AcademicYearId],[SchoolClassId],[Name],[PrimaryMethod],[MinimumGpa],[MinimumPassedSubjects],[IsActive],[CreatedBy],[CreatedAt])
SELECT @AcademicYearId, Id, Name + ' Promotion Policy', 1, 1.00, 4, 1, 'seed', GETUTCDATE()
FROM Classes WHERE Id IN (6,7,8) AND IsDeleted = 0;

-- Class Nine - Ten: GPA >= 1.00, critical subjects enforced
INSERT INTO [PromotionPolicies] ([AcademicYearId],[SchoolClassId],[Name],[PrimaryMethod],[MinimumGpa],[CriticalSubjectsJson],[MaxCriticalSubjectFailures],[UseCombinedRules],[IsActive],[CreatedBy],[CreatedAt])
SELECT @AcademicYearId, Id, Name + ' Promotion Policy', 1, 1.00, '["Mathematics","English","Bangla"]', 0, 1, 1, 'seed', GETUTCDATE()
FROM Classes WHERE Id IN (9,10) AND IsDeleted = 0;

PRINT 'PromotionPolicies seeded: ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + ' rows';
GO

-- Verify
SELECT pp.Id, c.Name AS Class, pp.Name, pp.PrimaryMethod, pp.MinimumGpa, pp.MinimumPassedSubjects, pp.CriticalSubjectsJson
FROM PromotionPolicies pp
INNER JOIN Classes c ON pp.SchoolClassId = c.Id
WHERE pp.AcademicYearId = 1 AND pp.IsDeleted = 0
ORDER BY c.Id;
GO
