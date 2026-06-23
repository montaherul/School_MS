BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    ALTER TABLE [FinalResults] ADD [AttendancePercentage] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    ALTER TABLE [FinalResults] ADD [FinalGroupPosition] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    ALTER TABLE [FinalResults] ADD [FinalSectionPosition] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    ALTER TABLE [FinalResults] ADD [GeneratedRollNumber] int NULL;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    ALTER TABLE [FinalResults] ADD [TotalPassedSubjects] int NOT NULL DEFAULT 0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    ALTER TABLE [FinalResults] ADD [WeightedTotalMarks] decimal(18,2) NOT NULL DEFAULT 0.0;
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [GroupPromotionConfigs] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [FromClassId] int NOT NULL,
        [ToClassId] int NOT NULL,
        [AssignmentMethod] int NOT NULL,
        [ConfigurationJson] nvarchar(2000) NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_GroupPromotionConfigs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GroupPromotionConfigs_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GroupPromotionConfigs_Classes_FromClassId] FOREIGN KEY ([FromClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_GroupPromotionConfigs_Classes_ToClassId] FOREIGN KEY ([ToClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [PromotionPolicies] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NOT NULL,
        [Name] nvarchar(100) NOT NULL,
        [PrimaryMethod] int NOT NULL,
        [MinimumGpa] decimal(18,2) NOT NULL,
        [MaxPositionForPromotion] int NULL,
        [TopPercentagePromote] decimal(18,2) NULL,
        [MinimumAttendancePercentage] decimal(18,2) NULL,
        [MinimumPassedSubjects] int NULL,
        [UseCombinedRules] bit NOT NULL,
        [CriticalSubjectsJson] nvarchar(2000) NULL,
        [MaxCriticalSubjectFailures] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PromotionPolicies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionPolicies_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionPolicies_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [RankingRules] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NULL,
        [Name] nvarchar(100) NOT NULL,
        [TieBreakersJson] nvarchar(2000) NOT NULL,
        [IsDefault] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RankingRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RankingRules_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RankingRules_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [ResultPolicies] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NULL,
        [Name] nvarchar(100) NOT NULL,
        [Description] nvarchar(500) NULL,
        [IsDefault] bit NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ResultPolicies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResultPolicies_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPolicies_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [RollGenerationConfigs] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NOT NULL,
        [Strategy] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_RollGenerationConfigs] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RollGenerationConfigs_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_RollGenerationConfigs_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [PromotionExecutions] (
        [Id] int NOT NULL IDENTITY,
        [AcademicYearId] int NOT NULL,
        [SchoolClassId] int NOT NULL,
        [PromotionPolicyId] int NULL,
        [TotalStudents] int NOT NULL,
        [PromotedCount] int NOT NULL,
        [RepeatCount] int NOT NULL,
        [FailedCount] int NOT NULL,
        [Notes] nvarchar(500) NULL,
        [ExecutedByUserId] int NOT NULL,
        [ExecutedAt] datetime2 NOT NULL,
        [IsApproved] bit NOT NULL,
        [ApprovedByUserId] int NULL,
        [ApprovedAt] datetime2 NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PromotionExecutions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionExecutions_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionExecutions_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_PromotionExecutions_PromotionPolicies_PromotionPolicyId] FOREIGN KEY ([PromotionPolicyId]) REFERENCES [PromotionPolicies] ([Id]) ON DELETE NO ACTION
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [PromotionPolicyRules] (
        [Id] int NOT NULL IDENTITY,
        [PromotionPolicyId] int NOT NULL,
        [CriterionType] nvarchar(100) NOT NULL,
        [Operator] nvarchar(100) NOT NULL,
        [ThresholdValue] decimal(18,2) NOT NULL,
        [LogicalOperator] nvarchar(100) NOT NULL,
        [IsInverse] bit NOT NULL,
        [DisplayOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_PromotionPolicyRules] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PromotionPolicyRules_PromotionPolicies_PromotionPolicyId] FOREIGN KEY ([PromotionPolicyId]) REFERENCES [PromotionPolicies] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE TABLE [ResultPolicyExamWeights] (
        [Id] int NOT NULL IDENTITY,
        [ResultPolicyId] int NOT NULL,
        [ExamTypeId] int NOT NULL,
        [WeightPercentage] decimal(18,2) NOT NULL,
        [DisplayOrder] int NOT NULL,
        [IsActive] bit NOT NULL,
        [CreatedBy] nvarchar(64) NOT NULL,
        [CreatedAt] datetime2 NOT NULL,
        [UpdatedBy] nvarchar(64) NULL,
        [UpdatedAt] datetime2 NULL,
        [IsDeleted] bit NOT NULL,
        CONSTRAINT [PK_ResultPolicyExamWeights] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResultPolicyExamWeights_ExamTypes_ExamTypeId] FOREIGN KEY ([ExamTypeId]) REFERENCES [ExamTypes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ResultPolicyExamWeights_ResultPolicies_ResultPolicyId] FOREIGN KEY ([ResultPolicyId]) REFERENCES [ResultPolicies] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_GroupPromotionConfigs_AcademicYearId_FromClassId] ON [GroupPromotionConfigs] ([AcademicYearId], [FromClassId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_GroupPromotionConfigs_FromClassId] ON [GroupPromotionConfigs] ([FromClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_GroupPromotionConfigs_ToClassId] ON [GroupPromotionConfigs] ([ToClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_PromotionExecutions_AcademicYearId_SchoolClassId] ON [PromotionExecutions] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_PromotionExecutions_PromotionPolicyId] ON [PromotionExecutions] ([PromotionPolicyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_PromotionExecutions_SchoolClassId] ON [PromotionExecutions] ([SchoolClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_PromotionPolicies_AcademicYearId_SchoolClassId] ON [PromotionPolicies] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_PromotionPolicies_SchoolClassId] ON [PromotionPolicies] ([SchoolClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_PromotionPolicyRules_PromotionPolicyId] ON [PromotionPolicyRules] ([PromotionPolicyId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_RankingRules_AcademicYearId_SchoolClassId] ON [RankingRules] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_RankingRules_SchoolClassId] ON [RankingRules] ([SchoolClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ResultPolicies_AcademicYearId_SchoolClassId] ON [ResultPolicies] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_ResultPolicies_SchoolClassId] ON [ResultPolicies] ([SchoolClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_ResultPolicyExamWeights_ExamTypeId] ON [ResultPolicyExamWeights] ([ExamTypeId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_ResultPolicyExamWeights_ResultPolicyId_ExamTypeId] ON [ResultPolicyExamWeights] ([ResultPolicyId], [ExamTypeId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_RollGenerationConfigs_AcademicYearId_SchoolClassId] ON [RollGenerationConfigs] ([AcademicYearId], [SchoolClassId]) WHERE [IsDeleted] = 0');
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    CREATE INDEX [IX_RollGenerationConfigs_SchoolClassId] ON [RollGenerationConfigs] ([SchoolClassId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260623180046_AddPhase5PromotionEngine'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260623180046_AddPhase5PromotionEngine', N'8.0.0');
END;
GO

COMMIT;
GO

