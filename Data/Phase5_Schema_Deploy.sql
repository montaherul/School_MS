-- ============================================================
-- Phase 5 Schema Deployment: Promotion Engine & FinalResults
-- ============================================================
-- Run against: SchoolManagementSystemDb
-- Usage: sqlcmd -S MONTAHERUL\SQLEXPRESS -d SchoolManagementSystemDb -E -i Phase5_Schema_Deploy.sql
-- ============================================================

PRINT '=== Phase 5 Schema Deploy ===';
GO

-- ============================================================
-- 1. FinalResults: Add missing columns
-- ============================================================
PRINT '1. Adding missing columns to FinalResults...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FinalResults' AND COLUMN_NAME = 'WeightedTotalMarks')
    ALTER TABLE [FinalResults] ADD [WeightedTotalMarks] decimal(18,2) NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FinalResults' AND COLUMN_NAME = 'FinalSectionPosition')
    ALTER TABLE [FinalResults] ADD [FinalSectionPosition] int NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FinalResults' AND COLUMN_NAME = 'FinalGroupPosition')
    ALTER TABLE [FinalResults] ADD [FinalGroupPosition] int NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FinalResults' AND COLUMN_NAME = 'TotalPassedSubjects')
    ALTER TABLE [FinalResults] ADD [TotalPassedSubjects] int NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FinalResults' AND COLUMN_NAME = 'AttendancePercentage')
    ALTER TABLE [FinalResults] ADD [AttendancePercentage] decimal(18,2) NOT NULL DEFAULT 0;
GO

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'FinalResults' AND COLUMN_NAME = 'GeneratedRollNumber')
    ALTER TABLE [FinalResults] ADD [GeneratedRollNumber] int NULL;
GO

PRINT '   FinalResults columns OK.';
GO

-- ============================================================
-- 2. PromotionPolicies
-- ============================================================
PRINT '2. Creating PromotionPolicies...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PromotionPolicies')
BEGIN
    CREATE TABLE [PromotionPolicies] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL DEFAULT '',
        [PrimaryMethod] int NOT NULL DEFAULT 1,
        [MinimumGpa] decimal(18,2) NOT NULL DEFAULT 1.00,
        [MaxPositionForPromotion] int NULL,
        [TopPercentagePromote] decimal(18,2) NULL,
        [MinimumAttendancePercentage] decimal(18,2) NULL,
        [MinimumPassedSubjects] int NULL,
        [UseCombinedRules] bit NOT NULL DEFAULT 0,
        [CriticalSubjectsJson] nvarchar(2000) NULL,
        [MaxCriticalSubjectFailures] int NOT NULL DEFAULT 0,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(64) NOT NULL DEFAULT 'system',
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_PromotionPolicies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionPolicies_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionPolicies_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_PromotionPolicies_AcademicYearId_SchoolClassId] ON [PromotionPolicies] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0;

    PRINT '   PromotionPolicies created.';
END
ELSE
    PRINT '   PromotionPolicies already exists.';
GO

-- ============================================================
-- 3. PromotionPolicyRules
-- ============================================================
PRINT '3. Creating PromotionPolicyRules...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PromotionPolicyRules')
BEGIN
    CREATE TABLE [PromotionPolicyRules] (
        [Id] int NOT NULL IDENTITY,
        [PromotionPolicyId] int NOT NULL,
        [CriterionType] nvarchar(100) NOT NULL DEFAULT '',
        [Operator] nvarchar(100) NOT NULL DEFAULT '',
        [ThresholdValue] decimal(18,2) NOT NULL DEFAULT 0,
        [LogicalOperator] nvarchar(100) NOT NULL DEFAULT 'AND',
        [IsInverse] bit NOT NULL DEFAULT 0,
        [DisplayOrder] int NOT NULL DEFAULT 0,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(64) NOT NULL DEFAULT 'system',
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_PromotionPolicyRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionPolicyRules_PromotionPolicies_PromotionPolicyId] FOREIGN KEY ([PromotionPolicyId]) REFERENCES [PromotionPolicies] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_PromotionPolicyRules_PromotionPolicyId] ON [PromotionPolicyRules] ([PromotionPolicyId]);

    PRINT '   PromotionPolicyRules created.';
END
ELSE
    PRINT '   PromotionPolicyRules already exists.';
GO

-- ============================================================
-- 4. PromotionExecutions
-- ============================================================
PRINT '4. Creating PromotionExecutions...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'PromotionExecutions')
BEGIN
    CREATE TABLE [PromotionExecutions] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NOT NULL,
        [PromotionPolicyId] int NULL,
        [TotalStudents] int NOT NULL DEFAULT 0,
        [PromotedCount] int NOT NULL DEFAULT 0,
        [RepeatCount] int NOT NULL DEFAULT 0,
        [FailedCount] int NOT NULL DEFAULT 0,
        [Notes] nvarchar(500) NULL,
        [ExecutedByUserId] int NOT NULL DEFAULT 0,
        [ExecutedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [IsApproved] bit NOT NULL DEFAULT 0,
        [ApprovedByUserId] int NULL,
        [ApprovedAt] datetime2 NULL,
        [CreatedBy] nvarchar(64) NOT NULL DEFAULT 'system',
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_PromotionExecutions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionExecutions_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionExecutions_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionExecutions_PromotionPolicies_PromotionPolicyId] FOREIGN KEY ([PromotionPolicyId]) REFERENCES [PromotionPolicies] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_PromotionExecutions_AcademicYearId_SchoolClassId] ON [PromotionExecutions] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0;

    PRINT '   PromotionExecutions created.';
END
ELSE
    PRINT '   PromotionExecutions already exists.';
GO

-- ============================================================
-- 5. RollGenerationConfigs
-- ============================================================
PRINT '5. Creating RollGenerationConfigs...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'RollGenerationConfigs')
BEGIN
    CREATE TABLE [RollGenerationConfigs] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NOT NULL,
        [Strategy] int NOT NULL DEFAULT 1,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(64) NOT NULL DEFAULT 'system',
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_RollGenerationConfigs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RollGenerationConfigs_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RollGenerationConfigs_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );

    PRINT '   RollGenerationConfigs created.';
END
ELSE
    PRINT '   RollGenerationConfigs already exists.';
GO

-- ============================================================
-- 6. GroupPromotionConfigs
-- ============================================================
PRINT '6. Creating GroupPromotionConfigs...';

IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'GroupPromotionConfigs')
BEGIN
    CREATE TABLE [GroupPromotionConfigs] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [FromClassId] int NOT NULL,
        [ToClassId] int NOT NULL,
        [AssignmentMethod] int NOT NULL DEFAULT 1,
        [ConfigurationJson] nvarchar(2000) NULL,
        [IsActive] bit NOT NULL DEFAULT 1,
        [CreatedBy] nvarchar(64) NOT NULL DEFAULT 'system',
        [CreatedAt] datetime2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL DEFAULT 0,
        CONSTRAINT [PK_GroupPromotionConfigs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GroupPromotionConfigs_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GroupPromotionConfigs_Classes_FromClassId] FOREIGN KEY ([FromClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GroupPromotionConfigs_Classes_ToClassId] FOREIGN KEY ([ToClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX [IX_GroupPromotionConfigs_AcademicYearId_FromClassId] ON [GroupPromotionConfigs] ([AcademicYearId], [FromClassId]) WHERE [IsDeleted] = 0;

    PRINT '   GroupPromotionConfigs created.';
END
ELSE
    PRINT '   GroupPromotionConfigs already exists.';
GO

-- ============================================================
-- 7. Deploy sp_CalculateFinalPositions (which now has columns)
-- ============================================================
PRINT '7. Deploying sp_CalculateFinalPositions...';
GO

CREATE OR ALTER PROCEDURE [dbo].[sp_CalculateFinalPositions]
    @AcademicYearId INT
AS
BEGIN
    SET NOCOUNT ON;

    -- School-wide position
    ;WITH SchoolRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS SchoolPosition
        FROM FinalResults fr
        INNER JOIN Students s ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
    )
    UPDATE fr
    SET fr.FinalPosition = sr.SchoolPosition,
        fr.UpdatedAt = GETUTCDATE()
    FROM FinalResults fr
    INNER JOIN SchoolRank sr ON fr.Id = sr.Id;

    -- Class position
    ;WITH ClassRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId, fr.SchoolClassId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS ClassPosition
        FROM FinalResults fr
        INNER JOIN Students s ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
    )
    UPDATE fr
    SET fr.FinalClassPosition = cr.ClassPosition,
        fr.UpdatedAt = GETUTCDATE()
    FROM FinalResults fr
    INNER JOIN ClassRank cr ON fr.Id = cr.Id;

    -- Section position
    ;WITH SectionRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId, fr.SchoolClassId, fr.SectionId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS SectionPosition
        FROM FinalResults fr
        INNER JOIN Students s ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
    )
    UPDATE fr
    SET fr.FinalSectionPosition = sr.SectionPosition,
        fr.UpdatedAt = GETUTCDATE()
    FROM FinalResults fr
    INNER JOIN SectionRank sr ON fr.Id = sr.Id;

    -- Group position (only for students with assigned groups)
    ;WITH GroupRank AS (
        SELECT 
            fr.Id,
            ROW_NUMBER() OVER (
                PARTITION BY fr.AcademicYearId, fr.StudentGroupId
                ORDER BY fr.FinalGpa DESC, fr.WeightedTotalMarks DESC, s.RollNumber ASC
            ) AS GroupPosition
        FROM FinalResults fr
        INNER JOIN Students s ON fr.StudentId = s.Id
        WHERE fr.AcademicYearId = @AcademicYearId
            AND fr.IsDeleted = 0
            AND fr.StudentGroupId IS NOT NULL
    )
    UPDATE fr
    SET fr.FinalGroupPosition = gr.GroupPosition,
        fr.UpdatedAt = GETUTCDATE()
    FROM FinalResults fr
    INNER JOIN GroupRank gr ON fr.Id = gr.Id;
END
GO

PRINT '   sp_CalculateFinalPositions deployed.';
GO

PRINT '=== Phase 5 Schema Deploy Complete ===';
GO
