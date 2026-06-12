IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AcademicYears] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(30) NOT NULL,
    [StartsOn] datetime2 NOT NULL,
    [EndsOn] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AcademicYears] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AdmissionListResults] (
    [Id] int NOT NULL,
    [ApplicationNo] nvarchar(max) NOT NULL,
    [ApplicantName] nvarchar(max) NOT NULL,
    [ApplicantNameBangla] nvarchar(max) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(max) NOT NULL,
    [AppliedClassId] int NOT NULL,
    [ClassName] nvarchar(max) NOT NULL,
    [ApplicantMobileNumber] nvarchar(max) NOT NULL,
    [FatherOrGuardianMobileNo] nvarchar(max) NOT NULL,
    [AlternativeNumber] nvarchar(max) NOT NULL,
    [ApplicantEmail] nvarchar(max) NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FatherName] nvarchar(max) NOT NULL,
    [FatherOccupation] nvarchar(max) NOT NULL,
    [MotherName] nvarchar(max) NOT NULL,
    [MotherOccupation] nvarchar(max) NOT NULL,
    [GuardianName] nvarchar(max) NOT NULL,
    [GuardianOccupation] nvarchar(max) NOT NULL,
    [Nationality] nvarchar(max) NOT NULL,
    [Religion] nvarchar(max) NOT NULL,
    [BloodGroup] nvarchar(max) NOT NULL,
    [BirthCertificateNo] nvarchar(max) NOT NULL,
    [BirthCertificatePath] nvarchar(max) NOT NULL,
    [PaymentSlipPath] nvarchar(max) NOT NULL,
    [PaymentMethod] nvarchar(max) NOT NULL,
    [TransactionDetails] nvarchar(max) NOT NULL,
    [PresentVillage] nvarchar(max) NOT NULL,
    [PresentPostOffice] nvarchar(max) NOT NULL,
    [PresentThana] nvarchar(max) NOT NULL,
    [PresentDistrict] nvarchar(max) NOT NULL,
    [PermanentVillage] nvarchar(max) NOT NULL,
    [PermanentPostOffice] nvarchar(max) NOT NULL,
    [PermanentThana] nvarchar(max) NOT NULL,
    [PermanentDistrict] nvarchar(max) NOT NULL,
    [ProfilePicturePath] nvarchar(max) NOT NULL,
    [TotalRecords] int NOT NULL,
    [CreatedBy] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL
);
GO

CREATE TABLE [Admissions] (
    [Id] int NOT NULL IDENTITY,
    [ApplicationNo] nvarchar(30) NOT NULL,
    [ApplicantName] nvarchar(120) NOT NULL,
    [ApplicantNameBangla] nvarchar(120) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [FatherName] nvarchar(120) NOT NULL,
    [FatherOccupation] nvarchar(100) NULL,
    [MotherName] nvarchar(120) NOT NULL,
    [MotherOccupation] nvarchar(100) NULL,
    [GuardianName] nvarchar(120) NULL,
    [GuardianOccupation] nvarchar(100) NULL,
    [FatherOrGuardianMobileNo] nvarchar(30) NOT NULL,
    [ApplicantMobileNumber] nvarchar(30) NOT NULL,
    [AlternativeNumber] nvarchar(30) NULL,
    [ApplicantEmail] nvarchar(160) NULL,
    [Nationality] nvarchar(50) NOT NULL,
    [Country] nvarchar(50) NOT NULL,
    [MaritalStatus] nvarchar(30) NOT NULL,
    [Religion] nvarchar(30) NOT NULL,
    [BloodGroup] nvarchar(10) NULL,
    [BirthCertificateNo] nvarchar(50) NULL,
    [BirthCertificatePath] nvarchar(260) NULL,
    [PaymentMethod] nvarchar(50) NULL,
    [TransactionDetails] nvarchar(250) NULL,
    [PaymentSlipPath] nvarchar(260) NULL,
    [PresentVillage] nvarchar(150) NULL,
    [PresentPostOffice] nvarchar(150) NULL,
    [PresentThana] nvarchar(150) NULL,
    [PresentDistrict] nvarchar(100) NULL,
    [PermanentVillage] nvarchar(150) NULL,
    [PermanentPostOffice] nvarchar(150) NULL,
    [PermanentThana] nvarchar(150) NULL,
    [PermanentDistrict] nvarchar(100) NULL,
    [AppliedClassId] int NOT NULL,
    [Status] int NOT NULL,
    [AdmissionFee] decimal(18,2) NOT NULL,
    [AdmissionFeePaid] bit NOT NULL,
    [ReviewedAt] datetime2 NULL,
    [ReviewedByUserId] int NULL,
    [ProfilePicturePath] nvarchar(260) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Admissions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AdmitCards] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [CardNo] nvarchar(40) NOT NULL,
    [PrintedAt] datetime2 NULL,
    [IsGenerated] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AdmitCards] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Assignments] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [TeacherProfileId] int NOT NULL,
    [Title] nvarchar(160) NOT NULL,
    [Instructions] nvarchar(2000) NOT NULL,
    [Deadline] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [AttachmentPath] nvarchar(260) NULL,
    [CreatedByUserId] int NULL,
    [UpdatedByUserId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Assignments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Attendance] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [SchoolClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [AttendanceDate] date NOT NULL,
    [PeriodNo] int NULL,
    [Status] int NOT NULL,
    [Remarks] nvarchar(240) NULL,
    [CreatedByUserId] int NULL,
    [UpdatedByUserId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Attendance] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BackupRecords] (
    [Id] int NOT NULL IDENTITY,
    [FilePath] nvarchar(260) NOT NULL,
    [BackupAt] datetime2 NOT NULL,
    [Restored] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_BackupRecords] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BookIssues] (
    [Id] int NOT NULL IDENTITY,
    [BookId] int NOT NULL,
    [StudentId] int NOT NULL,
    [IssueDate] date NOT NULL,
    [DueDate] date NOT NULL,
    [ReturnedDate] date NULL,
    [FineAmount] decimal(18,2) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_BookIssues] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BookReservations] (
    [Id] int NOT NULL IDENTITY,
    [BookId] int NOT NULL,
    [StudentId] int NOT NULL,
    [ReservedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_BookReservations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Books] (
    [Id] int NOT NULL IDENTITY,
    [AccessionNo] nvarchar(30) NOT NULL,
    [Title] nvarchar(160) NOT NULL,
    [Author] nvarchar(120) NOT NULL,
    [TotalCopies] int NOT NULL,
    [AvailableCopies] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Books] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Circulars] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(160) NOT NULL,
    [FilePath] nvarchar(260) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Circulars] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Classes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(60) NOT NULL,
    [SortOrder] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Departments] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Designations] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [RoleLevel] int NOT NULL,
    [IsTeachingRole] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Designations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Drivers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(120) NOT NULL,
    [Phone] nvarchar(30) NOT NULL,
    [LicenseNo] nvarchar(60) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Drivers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Exams] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Term] int NOT NULL,
    [Status] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [StartsOn] date NOT NULL,
    [EndsOn] date NOT NULL,
    [IsLocked] bit NOT NULL,
    [LockedAt] datetime2 NULL,
    [LockedByUserId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Exams] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ExamTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ExamTypes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FeeInvoices] (
    [Id] int NOT NULL IDENTITY,
    [InvoiceNo] nvarchar(40) NOT NULL,
    [StudentId] int NOT NULL,
    [DueDate] date NOT NULL,
    [TotalAmount] decimal(18,2) NOT NULL,
    [PaidAmount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_FeeInvoices] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FeeStructures] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [FeeName] nvarchar(100) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [IsRecurring] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_FeeStructures] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FineRules] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [GraceDays] int NOT NULL,
    [FinePerDay] decimal(18,2) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_FineRules] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [GpaConfigurations] (
    [Id] int NOT NULL IDENTITY,
    [Grade] nvarchar(10) NOT NULL,
    [MinMarks] decimal(18,2) NOT NULL,
    [MaxMarks] decimal(18,2) NOT NULL,
    [GradePoint] decimal(18,2) NOT NULL,
    [Description] nvarchar(50) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_GpaConfigurations] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [GradingRules] (
    [Id] int NOT NULL IDENTITY,
    [Grade] nvarchar(10) NOT NULL,
    [MinMarks] decimal(18,2) NOT NULL,
    [MaxMarks] decimal(18,2) NOT NULL,
    [GradePoint] decimal(18,2) NOT NULL,
    [Description] nvarchar(50) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_GradingRules] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LeaveApplications] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [FromDate] date NOT NULL,
    [ToDate] date NOT NULL,
    [Reason] nvarchar(500) NOT NULL,
    [Status] int NOT NULL,
    [ApprovedByUserId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_LeaveApplications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MedicalRecords] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [BloodGroup] nvarchar(120) NOT NULL,
    [Allergies] nvarchar(1000) NULL,
    [EmergencyContactName] nvarchar(120) NOT NULL,
    [EmergencyContactPhone] nvarchar(30) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_MedicalRecords] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MessageItems] (
    [Id] int NOT NULL IDENTITY,
    [MessageThreadId] int NOT NULL,
    [SenderUserId] int NOT NULL,
    [ReceiverUserId] int NOT NULL,
    [Body] nvarchar(2000) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_MessageItems] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MessageThreads] (
    [Id] int NOT NULL IDENTITY,
    [Subject] nvarchar(160) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_MessageThreads] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Notices] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(160) NOT NULL,
    [Body] nvarchar(3000) NOT NULL,
    [AudienceRole] nvarchar(80) NOT NULL,
    [PublishAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Notices] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Notifications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [Channel] int NOT NULL,
    [Title] nvarchar(160) NOT NULL,
    [Body] nvarchar(1000) NOT NULL,
    [IsRead] bit NOT NULL,
    [SentAt] datetime2 NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Permissions] (
    [Id] int NOT NULL IDENTITY,
    [Module] nvarchar(80) NOT NULL,
    [ModuleName] nvarchar(80) NOT NULL,
    [Action] nvarchar(80) NOT NULL,
    [Code] nvarchar(160) NOT NULL,
    [CanCreate] bit NOT NULL,
    [CanRead] bit NOT NULL,
    [CanUpdate] bit NOT NULL,
    [CanDelete] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Permissions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(80) NOT NULL,
    [Description] nvarchar(200) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SchoolProfiles] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(160) NOT NULL,
    [Address] nvarchar(300) NOT NULL,
    [Phone] nvarchar(30) NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SchoolProfiles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SeatingPlans] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SeatNo] nvarchar(40) NOT NULL,
    [HallNo] nvarchar(100) NOT NULL,
    [BlockNo] int NULL,
    [RowNo] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SeatingPlans] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudentGroups] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NOT NULL,
    [MinClass] int NOT NULL,
    [MaxClass] int NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentGroups] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudentListItemResults] (
    [Id] int NOT NULL,
    [StudentNo] nvarchar(max) NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [FullNameBangla] nvarchar(max) NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [ClassName] nvarchar(max) NOT NULL,
    [SectionName] nvarchar(max) NOT NULL,
    [RollNumber] int NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [FatherName] nvarchar(max) NULL,
    [FatherOccupation] nvarchar(max) NULL,
    [MotherName] nvarchar(max) NULL,
    [MotherOccupation] nvarchar(max) NULL,
    [MobileNumber] nvarchar(max) NULL,
    [EmailAddress] nvarchar(max) NULL,
    [PresentVillage] nvarchar(max) NULL,
    [PresentPostOffice] nvarchar(max) NULL,
    [PresentThana] nvarchar(max) NULL,
    [PresentDistrict] nvarchar(max) NULL,
    [PermanentVillage] nvarchar(max) NULL,
    [PermanentPostOffice] nvarchar(max) NULL,
    [PermanentThana] nvarchar(max) NULL,
    [PermanentDistrict] nvarchar(max) NULL,
    [BloodGroup] nvarchar(max) NULL,
    [Religion] nvarchar(max) NULL,
    [Nationality] nvarchar(max) NULL,
    [BirthCertificateNo] nvarchar(max) NULL,
    [FatherOrGuardianMobileNo] nvarchar(max) NULL,
    [ProfilePicturePath] nvarchar(max) NULL,
    [TotalRecords] int NOT NULL
);
GO

CREATE TABLE [StudentPromotions] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [FromClassId] int NOT NULL,
    [ToClassId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [PromotedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentPromotions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudentRouteAssignments] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [TransportRouteId] int NOT NULL,
    [VehicleId] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentRouteAssignments] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudyMaterials] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [Title] nvarchar(160) NOT NULL,
    [ResourceUrl] nvarchar(260) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudyMaterials] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Subjects] (
    [Id] int NOT NULL IDENTITY,
    [Code] nvarchar(30) NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [NameBn] nvarchar(100) NOT NULL,
    [SubjectGroup] nvarchar(50) NOT NULL,
    [IsMandatory] bit NOT NULL,
    [IsOptional] bit NOT NULL,
    [IsReligionSubject] bit NOT NULL,
    [IsPractical] bit NOT NULL,
    [HasWritten] bit NOT NULL,
    [HasMCQ] bit NOT NULL,
    [HasCQ] bit NOT NULL,
    [HasPractical] bit NOT NULL,
    [HasLab] bit NOT NULL,
    [HasViva] bit NOT NULL,
    [HasOral] bit NOT NULL,
    [HasAssignment] bit NOT NULL,
    [HasContinuousAssessment] bit NOT NULL,
    [ReligionType] nvarchar(50) NULL,
    [DefaultFullMarks] decimal(18,2) NOT NULL,
    [DefaultPassMarks] decimal(18,2) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Subjects] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Syllabi] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [FilePath] nvarchar(260) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Syllabi] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SystemLogs] (
    [Id] int NOT NULL IDENTITY,
    [Level] nvarchar(40) NOT NULL,
    [Message] nvarchar(1000) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SystemLogs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TransferCertificates] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [CertificateNo] nvarchar(40) NOT NULL,
    [IssueDate] datetime2 NOT NULL,
    [Reason] nvarchar(500) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TransferCertificates] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TransportRoutes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [PickupDropSchedule] nvarchar(300) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TransportRoutes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(80) NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [PhoneNumber] nvarchar(30) NULL,
    [PasswordHash] nvarchar(512) NOT NULL,
    [Status] int NOT NULL,
    [IsEmailConfirmed] bit NOT NULL,
    [ActivationToken] nvarchar(64) NULL,
    [ActivationTokenExpiry] datetime2 NULL,
    [LastLoginAt] datetime2 NULL,
    [FailedLoginAttempts] int NOT NULL,
    [LockoutUntil] datetime2 NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [VaccinationRecords] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [VaccineName] nvarchar(120) NOT NULL,
    [VaccinatedOn] date NOT NULL,
    [NextDueOn] date NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_VaccinationRecords] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Vehicles] (
    [Id] int NOT NULL IDENTITY,
    [RegistrationNo] nvarchar(40) NOT NULL,
    [Capacity] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AdmissionDocuments] (
    [Id] int NOT NULL IDENTITY,
    [AdmissionApplicationId] int NOT NULL,
    [DocumentType] nvarchar(80) NOT NULL,
    [FilePath] nvarchar(260) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AdmissionDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AdmissionDocuments_Admissions_AdmissionApplicationId] FOREIGN KEY ([AdmissionApplicationId]) REFERENCES [Admissions] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AssignmentSubmissions] (
    [Id] int NOT NULL IDENTITY,
    [AssignmentTaskId] int NOT NULL,
    [StudentId] int NOT NULL,
    [FilePath] nvarchar(260) NOT NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [Marks] decimal(18,2) NULL,
    [Feedback] nvarchar(1000) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AssignmentSubmissions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AssignmentSubmissions_Assignments_AssignmentTaskId] FOREIGN KEY ([AssignmentTaskId]) REFERENCES [Assignments] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Sections] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [Name] nvarchar(20) NOT NULL,
    [Capacity] int NOT NULL,
    [ParentSectionId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Sections] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sections_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Sections_Sections_ParentSectionId] FOREIGN KEY ([ParentSectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ResultLocks] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [LockedByUserId] int NOT NULL,
    [LockedAt] datetime2 NOT NULL,
    [Reason] nvarchar(260) NULL,
    [CanUnlock] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ResultLocks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResultLocks_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ResultPublications] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [Status] int NOT NULL,
    [PublishedAt] datetime2 NULL,
    [ApprovedByUserId] int NULL,
    [IsLocked] bit NOT NULL,
    [LockedAt] datetime2 NULL,
    [PublicationNotes] nvarchar(500) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ResultPublications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResultPublications_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ExamConfigurations] (
    [Id] int NOT NULL IDENTITY,
    [ExamTypeId] int NOT NULL,
    [ClassId] int NOT NULL,
    [DisplayName] nvarchar(100) NOT NULL,
    [ExamWeightage] decimal(18,2) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ExamConfigurations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamConfigurations_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ExamConfigurations_ExamTypes_ExamTypeId] FOREIGN KEY ([ExamTypeId]) REFERENCES [ExamTypes] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Payments] (
    [Id] int NOT NULL IDENTITY,
    [FeeInvoiceId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Method] int NOT NULL,
    [ReferenceNo] nvarchar(80) NULL,
    [PaidAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Payments_FeeInvoices_FeeInvoiceId] FOREIGN KEY ([FeeInvoiceId]) REFERENCES [FeeInvoices] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [RolePermissions] (
    [RoleId] int NOT NULL,
    [PermissionId] int NOT NULL,
    CONSTRAINT [PK_RolePermissions] PRIMARY KEY ([RoleId], [PermissionId]),
    CONSTRAINT [FK_RolePermissions_Permissions_PermissionId] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RolePermissions_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ExamSchedules] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [ExamDate] date NOT NULL,
    [StartsAt] time NOT NULL,
    [EndsAt] time NOT NULL,
    [RoomNo] nvarchar(80) NOT NULL,
    [Instructions] nvarchar(500) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ExamSchedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamSchedules_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ExamSchedules_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ExamSubjects] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [FullMarks] decimal(18,2) NOT NULL,
    [PassMarks] decimal(18,2) NOT NULL,
    [IsOptional] bit NOT NULL,
    [WrittenMarks] decimal(18,2) NULL,
    [MCQMarks] decimal(18,2) NULL,
    [PracticalMarks] decimal(18,2) NULL,
    [VivaMarks] decimal(18,2) NULL,
    [LabMarks] decimal(18,2) NULL,
    [ContinuousAssessmentMarks] decimal(18,2) NULL,
    [OralMarks] decimal(18,2) NULL,
    [AssignmentMarks] decimal(18,2) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ExamSubjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ExamSubjects_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ExamSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ActivityLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [Action] nvarchar(100) NOT NULL,
    [Module] nvarchar(100) NOT NULL,
    [RecordId] int NULL,
    [OldValues] nvarchar(max) NULL,
    [NewValues] nvarchar(max) NULL,
    [IpAddress] nvarchar(64) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ActivityLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ActivityLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NULL,
    [Module] nvarchar(80) NOT NULL,
    [Action] nvarchar(80) NOT NULL,
    [IpAddress] nvarchar(64) NULL,
    [Details] nvarchar(1000) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AuditLogs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeCode] nvarchar(50) NOT NULL,
    [FullName] nvarchar(120) NOT NULL,
    [FatherName] nvarchar(120) NULL,
    [MotherName] nvarchar(120) NULL,
    [Gender] nvarchar(20) NOT NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [BloodGroup] nvarchar(10) NULL,
    [Religion] nvarchar(50) NULL,
    [Nationality] nvarchar(50) NOT NULL,
    [NIDNumber] nvarchar(50) NULL,
    [BirthCertificateNo] nvarchar(50) NULL,
    [Phone] nvarchar(30) NOT NULL,
    [Email] nvarchar(160) NULL,
    [PresentAddress] nvarchar(500) NULL,
    [PermanentAddress] nvarchar(500) NULL,
    [JoiningDate] datetime2 NOT NULL,
    [DepartmentId] int NOT NULL,
    [DesignationId] int NOT NULL,
    [EmployeeType] nvarchar(50) NOT NULL,
    [IsTeachingStaff] bit NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [UserId] int NULL,
    [ProfilePicturePath] nvarchar(260) NULL,
    [SignaturePath] nvarchar(260) NULL,
    [EmergencyContactName] nvarchar(120) NULL,
    [EmergencyContactPhone] nvarchar(30) NULL,
    [Remarks] nvarchar(500) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Employees_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PasswordResetTokens] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Otp] nvarchar(12) NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [Used] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_PasswordResetTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PasswordResetTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [UserRoles] (
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [UserSessions] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [SessionId] nvarchar(64) NOT NULL,
    [LoginAt] datetime2 NOT NULL,
    [LogoutAt] datetime2 NULL,
    [IpAddress] nvarchar(64) NULL,
    [UserAgent] nvarchar(512) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_UserSessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserSessions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ClassSubjects] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [StudentGroupId] int NULL,
    [SectionId] int NULL,
    [FullMarks] decimal(18,2) NOT NULL,
    [PassMarks] decimal(18,2) NOT NULL,
    [WrittenMarks] decimal(18,2) NULL,
    [MCQMarks] decimal(18,2) NULL,
    [CQMarks] decimal(18,2) NULL,
    [PracticalMarks] decimal(18,2) NULL,
    [VivaMarks] decimal(18,2) NULL,
    [LabMarks] decimal(18,2) NULL,
    [OralMarks] decimal(18,2) NULL,
    [AssignmentMarks] decimal(18,2) NULL,
    [ContinuousAssessmentMarks] decimal(18,2) NULL,
    [CompetencyMarks] decimal(18,2) NULL,
    [BehaviourMarks] decimal(18,2) NULL,
    [ParticipationMarks] decimal(18,2) NULL,
    [IsMandatory] bit NOT NULL,
    [IsOptional] bit NOT NULL,
    [IsGroupSubject] bit NOT NULL,
    [IsReligionSubject] bit NOT NULL,
    [ReligionType] nvarchar(50) NULL,
    [GroupName] nvarchar(50) NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ClassSubjects] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassSubjects_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassSubjects_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassSubjects_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassSubjects_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Students] (
    [Id] int NOT NULL IDENTITY,
    [StudentNo] nvarchar(30) NOT NULL,
    [FullName] nvarchar(120) NOT NULL,
    [FullNameBangla] nvarchar(120) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [FatherName] nvarchar(120) NOT NULL,
    [FatherOccupation] nvarchar(100) NULL,
    [MotherName] nvarchar(120) NOT NULL,
    [MotherOccupation] nvarchar(100) NULL,
    [MobileNumber] nvarchar(30) NOT NULL,
    [AlternativeNumber] nvarchar(30) NULL,
    [EmailAddress] nvarchar(160) NULL,
    [Nationality] nvarchar(50) NOT NULL,
    [Country] nvarchar(50) NOT NULL,
    [MaritalStatus] nvarchar(30) NOT NULL,
    [Religion] nvarchar(30) NOT NULL,
    [BloodGroup] nvarchar(10) NULL,
    [BirthCertificateNo] nvarchar(50) NULL,
    [ProfilePicturePath] nvarchar(260) NULL,
    [PresentVillage] nvarchar(150) NULL,
    [PresentPostOffice] nvarchar(150) NULL,
    [PresentThana] nvarchar(150) NULL,
    [PresentDistrict] nvarchar(100) NULL,
    [PermanentVillage] nvarchar(150) NULL,
    [PermanentPostOffice] nvarchar(150) NULL,
    [PermanentThana] nvarchar(150) NULL,
    [PermanentDistrict] nvarchar(100) NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [RollNumber] int NOT NULL,
    [Status] int NOT NULL,
    [UserId] int NULL,
    [AssignedReligionSubjectId] int NULL,
    [StudentGroupId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Students] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Students_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_Subjects_AssignedReligionSubjectId] FOREIGN KEY ([AssignedReligionSubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Students_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeAcademicAssignments] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeAcademicAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeAcademicAssignments_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeAttendances] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [AttendanceDate] datetime2 NOT NULL,
    [CheckInTime] time NULL,
    [CheckOutTime] time NULL,
    [Status] nvarchar(20) NOT NULL,
    [Remarks] nvarchar(255) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeAttendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeAttendances_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeDocuments] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [DocumentType] nvarchar(50) NOT NULL,
    [DocumentName] nvarchar(150) NOT NULL,
    [FilePath] nvarchar(260) NOT NULL,
    [ExpiryDate] datetime2 NULL,
    [Remarks] nvarchar(255) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeDocuments_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeExperiences] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [OrganizationName] nvarchar(150) NOT NULL,
    [Designation] nvarchar(100) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NULL,
    [Remarks] nvarchar(500) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeExperiences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeExperiences_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeLeaves] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [LeaveType] nvarchar(50) NOT NULL,
    [FromDate] datetime2 NOT NULL,
    [ToDate] datetime2 NOT NULL,
    [Reason] nvarchar(500) NOT NULL,
    [ApprovalStatus] nvarchar(20) NOT NULL,
    [ApprovedByUserId] int NULL,
    [ApprovedAt] datetime2 NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeLeaves] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeLeaves_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_EmployeeLeaves_Users_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeQualifications] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [ExamName] nvarchar(100) NOT NULL,
    [BoardOrUniversity] nvarchar(150) NULL,
    [InstituteName] nvarchar(150) NULL,
    [GroupOrSubject] nvarchar(100) NULL,
    [PassingYear] nvarchar(10) NULL,
    [Result] nvarchar(50) NULL,
    [CGPAOrDivision] nvarchar(50) NULL,
    [CertificateFilePath] nvarchar(260) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeQualifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeQualifications_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [EmployeeSalaries] (
    [Id] int NOT NULL IDENTITY,
    [EmployeeId] int NOT NULL,
    [BasicSalary] decimal(18,2) NOT NULL,
    [HouseRent] decimal(18,2) NOT NULL,
    [MedicalAllowance] decimal(18,2) NOT NULL,
    [TransportAllowance] decimal(18,2) NOT NULL,
    [OtherAllowance] decimal(18,2) NOT NULL,
    [Deduction] decimal(18,2) NOT NULL,
    [TotalSalary] decimal(18,2) NOT NULL,
    [EffectiveFrom] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeSalaries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeSalaries_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Teachers] (
    [Id] int NOT NULL IDENTITY,
    [TeacherNo] nvarchar(30) NOT NULL,
    [FullName] nvarchar(120) NOT NULL,
    [FullNameBangla] nvarchar(120) NULL,
    [DateOfBirth] datetime2 NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [MobileNumber] nvarchar(30) NOT NULL,
    [AlternativeNumber] nvarchar(30) NULL,
    [EmailAddress] nvarchar(160) NULL,
    [Nationality] nvarchar(50) NOT NULL,
    [Country] nvarchar(50) NOT NULL,
    [MaritalStatus] nvarchar(30) NOT NULL,
    [Religion] nvarchar(30) NOT NULL,
    [BloodGroup] nvarchar(10) NULL,
    [PassportNo] nvarchar(50) NULL,
    [PassportPath] nvarchar(260) NULL,
    [NationalIdNo] nvarchar(50) NULL,
    [NationalIdPath] nvarchar(260) NULL,
    [Designation] nvarchar(100) NOT NULL,
    [Department] nvarchar(100) NULL,
    [Qualification] nvarchar(200) NULL,
    [Specialization] nvarchar(200) NULL,
    [JoiningDate] datetime2 NULL,
    [FatherName] nvarchar(120) NULL,
    [MotherName] nvarchar(120) NULL,
    [SpouseName] nvarchar(120) NULL,
    [PresentVillage] nvarchar(150) NULL,
    [PresentPostOffice] nvarchar(150) NULL,
    [PresentThana] nvarchar(150) NULL,
    [PresentDistrict] nvarchar(100) NULL,
    [PermanentVillage] nvarchar(150) NULL,
    [PermanentPostOffice] nvarchar(150) NULL,
    [PermanentThana] nvarchar(150) NULL,
    [PermanentDistrict] nvarchar(100) NULL,
    [ProfilePicturePath] nvarchar(260) NULL,
    [Status] int NOT NULL,
    [UserId] int NULL,
    [EmployeeId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Teachers_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Teachers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [SubjectComponents] (
    [Id] int NOT NULL IDENTITY,
    [ClassSubjectId] int NOT NULL,
    [ComponentName] nvarchar(100) NOT NULL,
    [MaxMarks] decimal(18,2) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsRequired] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SubjectComponents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SubjectComponents_ClassSubjects_ClassSubjectId] FOREIGN KEY ([ClassSubjectId]) REFERENCES [ClassSubjects] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FinalResults] (
    [Id] int NOT NULL IDENTITY,
    [AcademicYearId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SchoolClassId] int NOT NULL,
    [FinalGpa] decimal(18,2) NOT NULL,
    [FinalPosition] int NOT NULL,
    [FinalClassPosition] int NOT NULL,
    [FinalGrade] nvarchar(10) NOT NULL,
    [PromotionStatus] int NOT NULL,
    [IsPassed] bit NOT NULL,
    [TotalFailedSubjects] int NOT NULL,
    [PromotionRemarks] nvarchar(500) NULL,
    [CalculatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_FinalResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FinalResults_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FinalResults_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_FinalResults_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Guardians] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [Name] nvarchar(120) NOT NULL,
    [Relation] nvarchar(40) NOT NULL,
    [Phone] nvarchar(30) NOT NULL,
    [Occupation] nvarchar(100) NULL,
    [Email] nvarchar(160) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Guardians] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Guardians_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [MarkEntryDrafts] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [WrittenMarks] decimal(18,2) NULL,
    [MCQMarks] decimal(18,2) NULL,
    [CQMarks] decimal(18,2) NULL,
    [PracticalMarks] decimal(18,2) NULL,
    [VivaMarks] decimal(18,2) NULL,
    [LabMarks] decimal(18,2) NULL,
    [OralMarks] decimal(18,2) NULL,
    [TotalMarks] decimal(18,2) NOT NULL,
    [Notes] nvarchar(500) NULL,
    [IsApproved] bit NOT NULL,
    [IsRejected] bit NOT NULL,
    [RejectionReason] nvarchar(260) NULL,
    [CreatedByTeacherId] int NOT NULL,
    [DraftSavedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_MarkEntryDrafts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MarkEntryDrafts_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MarkEntryDrafts_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MarkEntryDrafts_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [MeritResults] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SectionId] int NOT NULL,
    [Position] int NOT NULL,
    [ClassPosition] int NOT NULL,
    [GroupPosition] int NULL,
    [TotalMarks] decimal(18,2) NOT NULL,
    [Gpa] decimal(18,2) NOT NULL,
    [Grade] nvarchar(10) NOT NULL,
    [CalculatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_MeritResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MeritResults_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MeritResults_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MeritResults_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [PromotionHistories] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [FromClassId] int NOT NULL,
    [ToClassId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [Status] int NOT NULL,
    [PromotedAt] datetime2 NOT NULL,
    [PromotedByUserId] int NULL,
    [Remarks] nvarchar(500) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_PromotionHistories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PromotionHistories_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PromotionHistories_Classes_FromClassId] FOREIGN KEY ([FromClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PromotionHistories_Classes_ToClassId] FOREIGN KEY ([ToClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_PromotionHistories_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ReEvaluationRequests] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [RequestedByUserId] int NOT NULL,
    [Status] int NOT NULL,
    [OldMarks] decimal(18,2) NOT NULL,
    [NewMarks] decimal(18,2) NULL,
    [RequestReason] nvarchar(400) NULL,
    [Notes] nvarchar(400) NULL,
    [ApprovedByUserId] int NULL,
    [ApprovedAt] datetime2 NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ReEvaluationRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReEvaluationRequests_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ReEvaluationRequests_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ReEvaluationRequests_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ReportCards] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [PdfPath] nvarchar(260) NOT NULL,
    [Gpa] decimal(18,2) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ReportCards] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ReportCards_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ReportCards_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ResultAuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [OldMarks] decimal(18,2) NOT NULL,
    [NewMarks] decimal(18,2) NOT NULL,
    [OldGpa] int NULL,
    [NewGpa] int NULL,
    [ChangedByUserId] int NOT NULL,
    [Reason] nvarchar(260) NULL,
    [ChangeType] nvarchar(100) NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ResultAuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResultAuditLogs_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ResultAuditLogs_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ResultAuditLogs_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [RollNumberAssignments] (
    [Id] int NOT NULL IDENTITY,
    [AcademicYearId] int NOT NULL,
    [StudentId] int NOT NULL,
    [FromClassId] int NOT NULL,
    [ToClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [RollNumber] int NOT NULL,
    [MeritPosition] int NOT NULL,
    [MeritValue] decimal(18,2) NOT NULL,
    [GeneratedByUserId] int NOT NULL,
    [GeneratedAt] datetime2 NOT NULL,
    [Remarks] nvarchar(260) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_RollNumberAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RollNumberAssignments_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RollNumberAssignments_Classes_FromClassId] FOREIGN KEY ([FromClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RollNumberAssignments_Classes_ToClassId] FOREIGN KEY ([ToClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RollNumberAssignments_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RollNumberAssignments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentDocuments] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [DocumentType] nvarchar(80) NOT NULL,
    [FilePath] nvarchar(260) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentDocuments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentExamResults] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [TotalMarks] decimal(18,2) NOT NULL,
    [TotalFullMarks] decimal(18,2) NOT NULL,
    [Gpa] decimal(18,2) NOT NULL,
    [Grade] nvarchar(10) NOT NULL,
    [Position] int NOT NULL,
    [ClassPosition] int NOT NULL,
    [GroupPosition] int NULL,
    [IsPassed] bit NOT NULL,
    [FailedSubjectCount] int NOT NULL,
    [PassedSubjectCount] int NOT NULL,
    [Status] int NOT NULL,
    [PublishedAt] datetime2 NULL,
    [CalculatedAt] datetime2 NOT NULL,
    [Remarks] nvarchar(500) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentExamResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentExamResults_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentExamResults_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentGroupAssignments] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [StudentGroupId] int NOT NULL,
    [SchoolClassId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [AssignedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentGroupAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentGroupAssignments_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentGroupAssignments_Classes_SchoolClassId] FOREIGN KEY ([SchoolClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentGroupAssignments_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentGroupAssignments_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentSubjectResults] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [MarksObtained] decimal(18,2) NOT NULL,
    [FullMarks] decimal(18,2) NOT NULL,
    [PassMarks] decimal(18,2) NOT NULL,
    [Grade] nvarchar(10) NOT NULL,
    [GradePoint] decimal(18,2) NOT NULL,
    [IsPassed] bit NOT NULL,
    [Remarks] nvarchar(500) NULL,
    [CalculatedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentSubjectResults] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentSubjectResults_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentSubjectResults_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentSubjectResults_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ClassSubjectTeachers] (
    [Id] int NOT NULL IDENTITY,
    [ClassSubjectId] int NOT NULL,
    [TeacherId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ClassSubjectTeachers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ClassSubjectTeachers_ClassSubjects_ClassSubjectId] FOREIGN KEY ([ClassSubjectId]) REFERENCES [ClassSubjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ClassSubjectTeachers_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [LessonPlans] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [LessonDate] datetime2 NOT NULL,
    [Topic] nvarchar(max) NOT NULL,
    [Plan] nvarchar(max) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_LessonPlans] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_LessonPlans_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Marks] (
    [Id] int NOT NULL IDENTITY,
    [ExamId] int NOT NULL,
    [StudentId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [WrittenMarks] decimal(18,2) NULL,
    [MCQMarks] decimal(18,2) NULL,
    [CQMarks] decimal(18,2) NULL,
    [PracticalMarks] decimal(18,2) NULL,
    [VivaMarks] decimal(18,2) NULL,
    [LabMarks] decimal(18,2) NULL,
    [OralMarks] decimal(18,2) NULL,
    [AssignmentMarks] decimal(18,2) NULL,
    [ContinuousAssessmentMarks] decimal(18,2) NULL,
    [CompetencyMarks] decimal(18,2) NULL,
    [BehaviourMarks] decimal(18,2) NULL,
    [ParticipationMarks] decimal(18,2) NULL,
    [MarksObtained] decimal(18,2) NOT NULL,
    [Grade] nvarchar(10) NULL,
    [GradePoint] decimal(18,2) NULL,
    [EnteredByTeacherId] int NOT NULL,
    [Status] int NOT NULL,
    [IsLocked] bit NOT NULL,
    [LockedAt] datetime2 NULL,
    [SubmittedAt] datetime2 NULL,
    [CreatedByUserId] int NULL,
    [UpdatedByUserId] int NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Marks] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Marks_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Marks_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Marks_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Marks_Teachers_EnteredByTeacherId] FOREIGN KEY ([EnteredByTeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherAttendances] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [AttendanceDate] datetime2 NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [Remarks] nvarchar(255) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherAttendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherAttendances_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherClassAssignments] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [IsClassTeacher] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherClassAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherClassAssignments_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherClassAssignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherClassAssignments_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherClassAssignments_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherDocuments] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [DocumentType] nvarchar(50) NOT NULL,
    [FilePath] nvarchar(255) NOT NULL,
    [UploadedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherDocuments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherDocuments_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherLeaves] (
    [Id] int NOT NULL IDENTITY,
    [TeacherProfileId] int NOT NULL,
    [TeacherId] int NULL,
    [LeaveType] nvarchar(50) NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [Reason] nvarchar(500) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [ApproverRemarks] nvarchar(255) NULL,
    [ApprovedByUserId] int NULL,
    [ApprovedDate] datetime2 NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherLeaves] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherLeaves_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherLeaves_Users_ApprovedByUserId] FOREIGN KEY ([ApprovedByUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherPerformances] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [EvaluationDate] datetime2 NOT NULL,
    [EvaluatorUserId] int NULL,
    [Rating] int NOT NULL,
    [Comments] nvarchar(500) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherPerformances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherPerformances_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherPerformances_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherPerformances_Users_EvaluatorUserId] FOREIGN KEY ([EvaluatorUserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherSalaries] (
    [Id] int NOT NULL IDENTITY,
    [TeacherProfileId] int NOT NULL,
    [TeacherId] int NULL,
    [MonthYear] datetime2 NOT NULL,
    [BasicSalary] decimal(18,2) NOT NULL,
    [Allowances] decimal(18,2) NOT NULL,
    [Deductions] decimal(18,2) NOT NULL,
    [NetSalary] decimal(18,2) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherSalaries] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherSalaries_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherSubjectAssignments] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [AcademicYearId] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherSubjectAssignments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherSubjectAssignments_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherSubjectAssignments_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherSubjectAssignments_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherSubjectAssignments_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherSubjectAssignments_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherTimetables] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [SubjectId] int NOT NULL,
    [DayOfWeek] nvarchar(20) NOT NULL,
    [StartTime] time NOT NULL,
    [EndTime] time NOT NULL,
    [RoomNo] nvarchar(50) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherTimetables] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherTimetables_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherTimetables_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherTimetables_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_TeacherTimetables_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [MarkAuditLogs] (
    [Id] int NOT NULL IDENTITY,
    [MarkEntryId] int NOT NULL,
    [OldMarks] decimal(18,2) NOT NULL,
    [NewMarks] decimal(18,2) NOT NULL,
    [ChangedByUserId] int NOT NULL,
    [Reason] nvarchar(260) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_MarkAuditLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MarkAuditLogs_Marks_MarkEntryId] FOREIGN KEY ([MarkEntryId]) REFERENCES [Marks] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'EndsOn', N'IsActive', N'IsDeleted', N'Name', N'StartsOn', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[AcademicYears]'))
    SET IDENTITY_INSERT [AcademicYears] ON;
INSERT INTO [AcademicYears] ([Id], [CreatedAt], [CreatedBy], [EndsOn], [IsActive], [IsDeleted], [Name], [StartsOn], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', '2026-12-31T00:00:00.0000000', CAST(1 AS bit), CAST(0 AS bit), N'2026', '2026-01-01T00:00:00.0000000', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'EndsOn', N'IsActive', N'IsDeleted', N'Name', N'StartsOn', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[AcademicYears]'))
    SET IDENTITY_INSERT [AcademicYears] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AdmissionFee', N'AdmissionFeePaid', N'AlternativeNumber', N'ApplicantEmail', N'ApplicantMobileNumber', N'ApplicantName', N'ApplicantNameBangla', N'ApplicationNo', N'AppliedClassId', N'BirthCertificateNo', N'BirthCertificatePath', N'BloodGroup', N'Country', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'FatherName', N'FatherOccupation', N'FatherOrGuardianMobileNo', N'Gender', N'GuardianName', N'GuardianOccupation', N'IsDeleted', N'MaritalStatus', N'MotherName', N'MotherOccupation', N'Nationality', N'PaymentMethod', N'PaymentSlipPath', N'PermanentDistrict', N'PermanentPostOffice', N'PermanentThana', N'PermanentVillage', N'PresentDistrict', N'PresentPostOffice', N'PresentThana', N'PresentVillage', N'ProfilePicturePath', N'Religion', N'ReviewedAt', N'ReviewedByUserId', N'Status', N'TransactionDetails', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Admissions]'))
    SET IDENTITY_INSERT [Admissions] ON;
INSERT INTO [Admissions] ([Id], [AdmissionFee], [AdmissionFeePaid], [AlternativeNumber], [ApplicantEmail], [ApplicantMobileNumber], [ApplicantName], [ApplicantNameBangla], [ApplicationNo], [AppliedClassId], [BirthCertificateNo], [BirthCertificatePath], [BloodGroup], [Country], [CreatedAt], [CreatedBy], [DateOfBirth], [FatherName], [FatherOccupation], [FatherOrGuardianMobileNo], [Gender], [GuardianName], [GuardianOccupation], [IsDeleted], [MaritalStatus], [MotherName], [MotherOccupation], [Nationality], [PaymentMethod], [PaymentSlipPath], [PermanentDistrict], [PermanentPostOffice], [PermanentThana], [PermanentVillage], [PresentDistrict], [PresentPostOffice], [PresentThana], [PresentVillage], [ProfilePicturePath], [Religion], [ReviewedAt], [ReviewedByUserId], [Status], [TransactionDetails], [UpdatedAt], [UpdatedBy])
VALUES (1, 1500.0, CAST(0 AS bit), NULL, NULL, N'01800000010', N'Pending Applicant', NULL, N'APP-2026-0001', 1, NULL, NULL, NULL, N'Bangladesh', '2026-01-01T00:00:00.0000000Z', N'system', '2019-04-01T00:00:00.0000000', N'Applicant Father', NULL, N'01800000001', N'Female', N'Applicant Guardian', NULL, CAST(0 AS bit), N'Single', N'Applicant Mother', NULL, N'Bangladeshi', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Islam', NULL, NULL, 1, NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AdmissionFee', N'AdmissionFeePaid', N'AlternativeNumber', N'ApplicantEmail', N'ApplicantMobileNumber', N'ApplicantName', N'ApplicantNameBangla', N'ApplicationNo', N'AppliedClassId', N'BirthCertificateNo', N'BirthCertificatePath', N'BloodGroup', N'Country', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'FatherName', N'FatherOccupation', N'FatherOrGuardianMobileNo', N'Gender', N'GuardianName', N'GuardianOccupation', N'IsDeleted', N'MaritalStatus', N'MotherName', N'MotherOccupation', N'Nationality', N'PaymentMethod', N'PaymentSlipPath', N'PermanentDistrict', N'PermanentPostOffice', N'PermanentThana', N'PermanentVillage', N'PresentDistrict', N'PresentPostOffice', N'PresentThana', N'PresentVillage', N'ProfilePicturePath', N'Religion', N'ReviewedAt', N'ReviewedByUserId', N'Status', N'TransactionDetails', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Admissions]'))
    SET IDENTITY_INSERT [Admissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AttendanceDate', N'CreatedAt', N'CreatedBy', N'CreatedByUserId', N'IsDeleted', N'PeriodNo', N'Remarks', N'SchoolClassId', N'SectionId', N'Status', N'StudentId', N'UpdatedAt', N'UpdatedBy', N'UpdatedByUserId') AND [object_id] = OBJECT_ID(N'[Attendance]'))
    SET IDENTITY_INSERT [Attendance] ON;
INSERT INTO [Attendance] ([Id], [AttendanceDate], [CreatedAt], [CreatedBy], [CreatedByUserId], [IsDeleted], [PeriodNo], [Remarks], [SchoolClassId], [SectionId], [Status], [StudentId], [UpdatedAt], [UpdatedBy], [UpdatedByUserId])
VALUES (1, '2026-04-25', '2026-01-01T00:00:00.0000000Z', N'system', NULL, CAST(0 AS bit), NULL, NULL, 1, 1, 1, 1, NULL, NULL, NULL),
(2, '2026-04-25', '2026-01-01T00:00:00.0000000Z', N'system', NULL, CAST(0 AS bit), NULL, NULL, 1, 1, 2, 2, NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AttendanceDate', N'CreatedAt', N'CreatedBy', N'CreatedByUserId', N'IsDeleted', N'PeriodNo', N'Remarks', N'SchoolClassId', N'SectionId', N'Status', N'StudentId', N'UpdatedAt', N'UpdatedBy', N'UpdatedByUserId') AND [object_id] = OBJECT_ID(N'[Attendance]'))
    SET IDENTITY_INSERT [Attendance] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessionNo', N'Author', N'AvailableCopies', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Title', N'TotalCopies', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Books]'))
    SET IDENTITY_INSERT [Books] ON;
INSERT INTO [Books] ([Id], [AccessionNo], [Author], [AvailableCopies], [CreatedAt], [CreatedBy], [IsDeleted], [Title], [TotalCopies], [UpdatedAt], [UpdatedBy])
VALUES (1, N'B-0001', N'Academic Board', 8, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Primary Mathematics', 10, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AccessionNo', N'Author', N'AvailableCopies', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Title', N'TotalCopies', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Books]'))
    SET IDENTITY_INSERT [Books] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'SortOrder', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Classes]'))
    SET IDENTITY_INSERT [Classes] ON;
INSERT INTO [Classes] ([Id], [CreatedAt], [CreatedBy], [IsDeleted], [Name], [SortOrder], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class One', 1, NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Two', 2, NULL, NULL),
(3, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Three', 3, NULL, NULL),
(4, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Four', 4, NULL, NULL),
(5, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Five', 5, NULL, NULL),
(6, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Six', 6, NULL, NULL),
(7, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Seven', 7, NULL, NULL),
(8, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Eight', 8, NULL, NULL),
(9, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Nine', 9, NULL, NULL),
(10, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Class Ten', 10, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'SortOrder', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Classes]'))
    SET IDENTITY_INSERT [Classes] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AcademicYearId', N'CreatedAt', N'CreatedBy', N'EndsOn', N'IsDeleted', N'IsLocked', N'LockedAt', N'LockedByUserId', N'Name', N'StartsOn', N'Status', N'Term', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Exams]'))
    SET IDENTITY_INSERT [Exams] ON;
INSERT INTO [Exams] ([Id], [AcademicYearId], [CreatedAt], [CreatedBy], [EndsOn], [IsDeleted], [IsLocked], [LockedAt], [LockedByUserId], [Name], [StartsOn], [Status], [Term], [UpdatedAt], [UpdatedBy])
VALUES (1, 1, '2026-01-01T00:00:00.0000000Z', N'system', '2026-06-12', CAST(0 AS bit), CAST(0 AS bit), NULL, NULL, N'Midterm', '2026-06-01', 1, 8, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AcademicYearId', N'CreatedAt', N'CreatedBy', N'EndsOn', N'IsDeleted', N'IsLocked', N'LockedAt', N'LockedByUserId', N'Name', N'StartsOn', N'Status', N'Term', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Exams]'))
    SET IDENTITY_INSERT [Exams] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DueDate', N'InvoiceNo', N'IsDeleted', N'PaidAmount', N'Status', N'StudentId', N'TotalAmount', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[FeeInvoices]'))
    SET IDENTITY_INSERT [FeeInvoices] ON;
INSERT INTO [FeeInvoices] ([Id], [CreatedAt], [CreatedBy], [DueDate], [InvoiceNo], [IsDeleted], [PaidAmount], [Status], [StudentId], [TotalAmount], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', '2026-05-10', N'INV-2026-0001', CAST(0 AS bit), 2500.0, 3, 1, 2500.0, NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'system', '2026-05-10', N'INV-2026-0002', CAST(0 AS bit), 1000.0, 2, 2, 2500.0, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'DueDate', N'InvoiceNo', N'IsDeleted', N'PaidAmount', N'Status', N'StudentId', N'TotalAmount', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[FeeInvoices]'))
    SET IDENTITY_INSERT [FeeInvoices] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Description', N'DisplayOrder', N'Grade', N'GradePoint', N'IsActive', N'IsDeleted', N'MaxMarks', N'MinMarks', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[GradingRules]'))
    SET IDENTITY_INSERT [GradingRules] ON;
INSERT INTO [GradingRules] ([Id], [CreatedAt], [CreatedBy], [Description], [DisplayOrder], [Grade], [GradePoint], [IsActive], [IsDeleted], [MaxMarks], [MinMarks], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'A+', 5.0, CAST(1 AS bit), CAST(0 AS bit), 100.0, 80.0, NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'A', 4.0, CAST(1 AS bit), CAST(0 AS bit), 79.0, 70.0, NULL, NULL),
(3, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'A-', 3.5, CAST(1 AS bit), CAST(0 AS bit), 69.0, 60.0, NULL, NULL),
(4, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'B', 3.0, CAST(1 AS bit), CAST(0 AS bit), 59.0, 50.0, NULL, NULL),
(5, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'C', 2.0, CAST(1 AS bit), CAST(0 AS bit), 49.0, 40.0, NULL, NULL),
(6, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'D', 1.0, CAST(1 AS bit), CAST(0 AS bit), 39.0, 33.0, NULL, NULL),
(7, '2026-01-01T00:00:00.0000000Z', N'system', N'', 0, N'F', 0.0, CAST(1 AS bit), CAST(0 AS bit), 32.0, 0.0, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Description', N'DisplayOrder', N'Grade', N'GradePoint', N'IsActive', N'IsDeleted', N'MaxMarks', N'MinMarks', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[GradingRules]'))
    SET IDENTITY_INSERT [GradingRules] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AudienceRole', N'Body', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'PublishAt', N'Title', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Notices]'))
    SET IDENTITY_INSERT [Notices] ON;
INSERT INTO [Notices] ([Id], [AudienceRole], [Body], [CreatedAt], [CreatedBy], [IsDeleted], [PublishAt], [Title], [UpdatedAt], [UpdatedBy])
VALUES (1, N'All', N'Classes and office activities are active.', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), '2026-01-01T00:00:00.0000000Z', N'Welcome to the 2026 academic session', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AudienceRole', N'Body', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'PublishAt', N'Title', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Notices]'))
    SET IDENTITY_INSERT [Notices] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (1, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Dashboard.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(2, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Dashboard.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(3, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Dashboard.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(4, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Dashboard.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(5, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Dashboard.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(6, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Dashboard.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(7, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Dashboard.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(8, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Dashboard.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(9, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Dashboard.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Dashboard', N'Dashboard', NULL, NULL),
(10, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Users.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(11, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Users.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(12, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Users.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(13, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Users.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(14, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Users.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(15, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Users.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(16, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Users.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(17, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Users.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(18, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Users.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Users', N'Users', NULL, NULL),
(19, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Roles.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(20, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Roles.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(21, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Roles.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(22, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Roles.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(23, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Roles.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(24, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Roles.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(25, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Roles.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(26, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Roles.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(27, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Roles.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Roles', N'Roles', NULL, NULL),
(28, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Permissions.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(29, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Permissions.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(30, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Permissions.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(31, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Permissions.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(32, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Permissions.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(33, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Permissions.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(34, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Permissions.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(35, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Permissions.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(36, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Permissions.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Permissions', N'Permissions', NULL, NULL),
(37, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admissions.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(38, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Admissions.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(39, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admissions.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(40, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Admissions.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(41, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admissions.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(42, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admissions.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (43, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admissions.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(44, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admissions.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(45, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Admissions.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admissions', N'Admissions', NULL, NULL),
(46, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Students.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(47, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Students.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(48, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Students.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(49, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Students.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(50, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Students.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(51, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Students.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(52, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Students.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(53, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Students.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(54, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Students.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Students', N'Students', NULL, NULL),
(55, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Teachers.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(56, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Teachers.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(57, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Teachers.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(58, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Teachers.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(59, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Teachers.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(60, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Teachers.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(61, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Teachers.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(62, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Teachers.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(63, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Teachers.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Teachers', N'Teachers', NULL, NULL),
(64, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Classes.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(65, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Classes.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(66, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Classes.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(67, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Classes.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(68, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Classes.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(69, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Classes.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(70, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Classes.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(71, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Classes.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(72, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Classes.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Classes', N'Classes', NULL, NULL),
(73, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Sections.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(74, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Sections.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(75, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Sections.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(76, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Sections.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(77, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Sections.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(78, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Sections.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(79, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Sections.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(80, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Sections.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(81, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Sections.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Sections', N'Sections', NULL, NULL),
(82, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Subjects.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(83, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Subjects.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(84, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Subjects.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (85, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Subjects.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(86, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Subjects.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(87, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Subjects.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(88, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Subjects.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(89, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Subjects.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(90, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Subjects.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Subjects', N'Subjects', NULL, NULL),
(91, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Attendance.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(92, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Attendance.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(93, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Attendance.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(94, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Attendance.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(95, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Attendance.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(96, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Attendance.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(97, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Attendance.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(98, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Attendance.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(99, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Attendance.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Attendance', N'Attendance', NULL, NULL),
(100, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exams.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(101, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Exams.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(102, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exams.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(103, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Exams.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(104, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exams.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(105, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exams.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(106, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exams.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(107, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exams.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(108, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Exams.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exams', N'Exams', NULL, NULL),
(109, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Marks.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(110, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Marks.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(111, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Marks.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(112, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Marks.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(113, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Marks.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(114, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Marks.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(115, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Marks.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(116, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Marks.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(117, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Marks.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Marks', N'Marks', NULL, NULL),
(118, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Assignments.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(119, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Assignments.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(120, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Assignments.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(121, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Assignments.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(122, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Assignments.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(123, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Assignments.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(124, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Assignments.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(125, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Assignments.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL),
(126, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Assignments.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Assignments', N'Assignments', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (127, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Fees.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(128, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Fees.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(129, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Fees.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(130, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Fees.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(131, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Fees.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(132, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Fees.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(133, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Fees.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(134, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Fees.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(135, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Fees.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Fees', N'Fees', NULL, NULL),
(136, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Payments.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(137, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Payments.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(138, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Payments.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(139, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Payments.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(140, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Payments.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(141, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Payments.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(142, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Payments.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(143, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Payments.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(144, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Payments.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Payments', N'Payments', NULL, NULL),
(145, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Library.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(146, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Library.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(147, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Library.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(148, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Library.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(149, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Library.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(150, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Library.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(151, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Library.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(152, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Library.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(153, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Library.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(154, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Transport.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(155, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Transport.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(156, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Transport.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(157, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Transport.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(158, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Transport.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(159, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Transport.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(160, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Transport.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(161, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Transport.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(162, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Transport.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Transport', N'Transport', NULL, NULL),
(163, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Health.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(164, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Health.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(165, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Health.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(166, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Health.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(167, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Health.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(168, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Health.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (169, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Health.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(170, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Health.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(171, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Health.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Health', N'Health', NULL, NULL),
(172, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notifications.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(173, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Notifications.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(174, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notifications.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(175, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Notifications.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(176, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notifications.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(177, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notifications.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(178, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notifications.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(179, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notifications.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(180, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Notifications.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notifications', N'Notifications', NULL, NULL),
(181, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Reports.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(182, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Reports.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(183, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Reports.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(184, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Reports.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(185, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Reports.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(186, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Reports.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(187, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Reports.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(188, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Reports.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(189, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Reports.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(190, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(191, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Settings.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(192, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(193, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Settings.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(194, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(195, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(196, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(197, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(198, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Settings.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(199, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(200, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Academic.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(201, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(202, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Academic.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(203, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(204, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(205, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(206, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(207, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Academic.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(208, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(209, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Admission.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(210, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (211, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Admission.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(212, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(213, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(214, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(215, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(216, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Admission.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(217, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(218, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Student.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(219, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(220, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Student.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(221, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(222, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(223, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(224, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(225, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Student.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(226, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(227, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Exam.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(228, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(229, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Exam.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(230, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(231, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(232, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(233, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(234, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Exam.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(235, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(236, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Result.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(237, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(238, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Result.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(239, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(240, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(241, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(242, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(243, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Result.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(244, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(245, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Communication.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(246, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(247, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Communication.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(248, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(249, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(250, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(251, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(252, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Communication.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (253, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(254, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'System.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(255, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(256, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'System.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(257, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(258, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(259, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(260, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(261, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'System.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(262, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(263, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'AuditLogs.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(264, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(265, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'AuditLogs.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(266, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(267, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(268, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(269, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(270, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'AuditLogs.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Description', N'IsDeleted', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [CreatedAt], [CreatedBy], [Description], [IsDeleted], [Name], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', N'System owner with all permissions', CAST(0 AS bit), N'Super Admin', NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'system', N'Final approval and all modules', CAST(0 AS bit), N'Principal', NULL, NULL),
(3, '2026-01-01T00:00:00.0000000Z', N'system', N'Academic operations', CAST(0 AS bit), N'Assistant Head', NULL, NULL),
(4, '2026-01-01T00:00:00.0000000Z', N'system', N'Teaching and review', CAST(0 AS bit), N'Senior Lecturer', NULL, NULL),
(5, '2026-01-01T00:00:00.0000000Z', N'system', N'Teaching operations', CAST(0 AS bit), N'Lecturer', NULL, NULL),
(6, '2026-01-01T00:00:00.0000000Z', N'system', N'Admission, fees, reports', CAST(0 AS bit), N'Office Staff', NULL, NULL),
(7, '2026-01-01T00:00:00.0000000Z', N'system', N'Student portal access', CAST(0 AS bit), N'Student', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Description', N'IsDeleted', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'CreatedAt', N'CreatedBy', N'Email', N'IsDeleted', N'Name', N'Phone', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[SchoolProfiles]'))
    SET IDENTITY_INSERT [SchoolProfiles] ON;
INSERT INTO [SchoolProfiles] ([Id], [Address], [CreatedAt], [CreatedBy], [Email], [IsDeleted], [Name], [Phone], [UpdatedAt], [UpdatedBy])
VALUES (1, N'Dhaka, Bangladesh', '2026-01-01T00:00:00.0000000Z', N'system', N'info@school.local', CAST(0 AS bit), N'School Management System', N'01000000000', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Address', N'CreatedAt', N'CreatedBy', N'Email', N'IsDeleted', N'Name', N'Phone', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[SchoolProfiles]'))
    SET IDENTITY_INSERT [SchoolProfiles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreatedAt', N'CreatedBy', N'Description', N'DisplayOrder', N'IsActive', N'IsDeleted', N'MaxClass', N'MinClass', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[StudentGroups]'))
    SET IDENTITY_INSERT [StudentGroups] ON;
INSERT INTO [StudentGroups] ([Id], [Code], [CreatedAt], [CreatedBy], [Description], [DisplayOrder], [IsActive], [IsDeleted], [MaxClass], [MinClass], [Name], [UpdatedAt], [UpdatedBy])
VALUES (1, N'SCI', '2026-01-01T00:00:00.0000000Z', N'system', N'Science Group', 1, CAST(1 AS bit), CAST(0 AS bit), 10, 9, N'Science', NULL, NULL),
(2, N'BS', '2026-01-01T00:00:00.0000000Z', N'system', N'Business Studies Group', 2, CAST(1 AS bit), CAST(0 AS bit), 10, 9, N'Business Studies', NULL, NULL),
(3, N'HUM', '2026-01-01T00:00:00.0000000Z', N'system', N'Humanities Group', 3, CAST(1 AS bit), CAST(0 AS bit), 10, 9, N'Humanities', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreatedAt', N'CreatedBy', N'Description', N'DisplayOrder', N'IsActive', N'IsDeleted', N'MaxClass', N'MinClass', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[StudentGroups]'))
    SET IDENTITY_INSERT [StudentGroups] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreatedAt', N'CreatedBy', N'DefaultFullMarks', N'DefaultPassMarks', N'DisplayOrder', N'HasAssignment', N'HasCQ', N'HasContinuousAssessment', N'HasLab', N'HasMCQ', N'HasOral', N'HasPractical', N'HasViva', N'HasWritten', N'IsActive', N'IsDeleted', N'IsMandatory', N'IsOptional', N'IsPractical', N'IsReligionSubject', N'Name', N'NameBn', N'ReligionType', N'SubjectGroup', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Subjects]'))
    SET IDENTITY_INSERT [Subjects] ON;
INSERT INTO [Subjects] ([Id], [Code], [CreatedAt], [CreatedBy], [DefaultFullMarks], [DefaultPassMarks], [DisplayOrder], [HasAssignment], [HasCQ], [HasContinuousAssessment], [HasLab], [HasMCQ], [HasOral], [HasPractical], [HasViva], [HasWritten], [IsActive], [IsDeleted], [IsMandatory], [IsOptional], [IsPractical], [IsReligionSubject], [Name], [NameBn], [ReligionType], [SubjectGroup], [UpdatedAt], [UpdatedBy])
VALUES (1, N'BAN', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'বাংলা', N'', NULL, N'', NULL, NULL),
(2, N'ENG', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ইংরেজি', N'', NULL, N'', NULL, NULL),
(3, N'MAT', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'গণিত', N'', NULL, N'', NULL, NULL),
(4, N'GSCI', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'সাধারণ বিজ্ঞান', N'', NULL, N'', NULL, NULL),
(5, N'SOC', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'বাংলাদেশ ও বিশ্ব পরিচয়', N'', NULL, N'', NULL, NULL),
(6, N'REL', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ধর্ম ও নৈতিক শিক্ষা', N'', NULL, N'', NULL, NULL),
(7, N'ART', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'চারুকলা', N'', NULL, N'', NULL, NULL),
(8, N'PE', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'শারীরিক শিক্ষা', N'', NULL, N'', NULL, NULL),
(9, N'BAN1', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'বাংলা ১ম পত্র', N'', NULL, N'', NULL, NULL),
(10, N'BAN2', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'বাংলা ২য় পত্র', N'', NULL, N'', NULL, NULL),
(11, N'ENG1', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ইংরেজি ১ম পত্র', N'', NULL, N'', NULL, NULL),
(12, N'ENG2', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ইংরেজি ২য় পত্র', N'', NULL, N'', NULL, NULL),
(13, N'SCI', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'বিজ্ঞান', N'', NULL, N'', NULL, NULL),
(14, N'ICT', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'তথ্য ও যোগাযোগ প্রযুক্তি', N'', NULL, N'', NULL, NULL),
(15, N'AGR', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'কৃষি শিক্ষা', N'', NULL, N'', NULL, NULL),
(16, N'PHY', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'পদার্থবিজ্ঞান', N'', NULL, N'', NULL, NULL),
(17, N'CHE', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'রসায়ন', N'', NULL, N'', NULL, NULL),
(18, N'BIO', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'জীববিজ্ঞান', N'', NULL, N'', NULL, NULL),
(19, N'HMA', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'উচ্চতর গণিত', N'', NULL, N'', NULL, NULL),
(20, N'ACC', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'হিসাববিজ্ঞান', N'', NULL, N'', NULL, NULL),
(21, N'FIN', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ফাইন্যান্স', N'', NULL, N'', NULL, NULL),
(22, N'BUS', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ব্যবসায় উদ্যোগ', N'', NULL, N'', NULL, NULL),
(23, N'HIS', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ইতিহাস', N'', NULL, N'', NULL, NULL),
(24, N'GEO', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ভূগোল ও পরিবেশ', N'', NULL, N'', NULL, NULL),
(25, N'ECO', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'অর্থনীতি', N'', NULL, N'', NULL, NULL),
(26, N'CIV', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'নাগরিকতা', N'', NULL, N'', NULL, NULL),
(27, N'CAREER', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ক্যারিয়ার শিক্ষা', N'', NULL, N'', NULL, NULL),
(28, N'HEALTH', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা', N'', NULL, N'', NULL, NULL),
(29, N'HSC', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'গার্হস্থ্য বিজ্ঞান', N'', NULL, N'', NULL, NULL),
(30, N'IRE', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'ইসলাম ও নৈতিক শিক্ষা', N'', NULL, N'', NULL, NULL),
(31, N'HRE', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'হিন্দুধর্ম ও নৈতিক শিক্ষা', N'', NULL, N'', NULL, NULL),
(32, N'BRE', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'বৌদ্ধধর্ম ও নৈতিক শিক্ষা', N'', NULL, N'', NULL, NULL),
(33, N'CRE', '2026-01-01T00:00:00.0000000Z', N'system', 100.0, 33.0, 0, CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'খ্রিস্টধর্ম ও নৈতিক শিক্ষা', N'', NULL, N'', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Code', N'CreatedAt', N'CreatedBy', N'DefaultFullMarks', N'DefaultPassMarks', N'DisplayOrder', N'HasAssignment', N'HasCQ', N'HasContinuousAssessment', N'HasLab', N'HasMCQ', N'HasOral', N'HasPractical', N'HasViva', N'HasWritten', N'IsActive', N'IsDeleted', N'IsMandatory', N'IsOptional', N'IsPractical', N'IsReligionSubject', N'Name', N'NameBn', N'ReligionType', N'SubjectGroup', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Subjects]'))
    SET IDENTITY_INSERT [Subjects] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AlternativeNumber', N'BloodGroup', N'Country', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'Department', N'Designation', N'EmailAddress', N'EmployeeId', N'FatherName', N'FullName', N'FullNameBangla', N'Gender', N'IsDeleted', N'JoiningDate', N'MaritalStatus', N'MobileNumber', N'MotherName', N'NationalIdNo', N'NationalIdPath', N'Nationality', N'PassportNo', N'PassportPath', N'PermanentDistrict', N'PermanentPostOffice', N'PermanentThana', N'PermanentVillage', N'PresentDistrict', N'PresentPostOffice', N'PresentThana', N'PresentVillage', N'ProfilePicturePath', N'Qualification', N'Religion', N'Specialization', N'SpouseName', N'Status', N'TeacherNo', N'UpdatedAt', N'UpdatedBy', N'UserId') AND [object_id] = OBJECT_ID(N'[Teachers]'))
    SET IDENTITY_INSERT [Teachers] ON;
INSERT INTO [Teachers] ([Id], [AlternativeNumber], [BloodGroup], [Country], [CreatedAt], [CreatedBy], [DateOfBirth], [Department], [Designation], [EmailAddress], [EmployeeId], [FatherName], [FullName], [FullNameBangla], [Gender], [IsDeleted], [JoiningDate], [MaritalStatus], [MobileNumber], [MotherName], [NationalIdNo], [NationalIdPath], [Nationality], [PassportNo], [PassportPath], [PermanentDistrict], [PermanentPostOffice], [PermanentThana], [PermanentVillage], [PresentDistrict], [PresentPostOffice], [PresentThana], [PresentVillage], [ProfilePicturePath], [Qualification], [Religion], [Specialization], [SpouseName], [Status], [TeacherNo], [UpdatedAt], [UpdatedBy], [UserId])
VALUES (1, NULL, NULL, N'Bangladesh', '2026-01-01T00:00:00.0000000Z', N'system', '0001-01-01T00:00:00.0000000', NULL, N'Senior Lecturer', NULL, NULL, NULL, N'Senior Lecturer', NULL, N'', CAST(0 AS bit), NULL, N'', N'01000000001', NULL, NULL, NULL, N'Bangladeshi', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, 1, N'T-0001', NULL, NULL, NULL),
(2, NULL, NULL, N'Bangladesh', '2026-01-01T00:00:00.0000000Z', N'system', '0001-01-01T00:00:00.0000000', NULL, N'Lecturer', NULL, NULL, NULL, N'Class Teacher', NULL, N'', CAST(0 AS bit), NULL, N'', N'01000000002', NULL, NULL, NULL, N'Bangladeshi', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, 1, N'T-0002', NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AlternativeNumber', N'BloodGroup', N'Country', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'Department', N'Designation', N'EmailAddress', N'EmployeeId', N'FatherName', N'FullName', N'FullNameBangla', N'Gender', N'IsDeleted', N'JoiningDate', N'MaritalStatus', N'MobileNumber', N'MotherName', N'NationalIdNo', N'NationalIdPath', N'Nationality', N'PassportNo', N'PassportPath', N'PermanentDistrict', N'PermanentPostOffice', N'PermanentThana', N'PermanentVillage', N'PresentDistrict', N'PresentPostOffice', N'PresentThana', N'PresentVillage', N'ProfilePicturePath', N'Qualification', N'Religion', N'Specialization', N'SpouseName', N'Status', N'TeacherNo', N'UpdatedAt', N'UpdatedBy', N'UserId') AND [object_id] = OBJECT_ID(N'[Teachers]'))
    SET IDENTITY_INSERT [Teachers] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ActivationToken', N'ActivationTokenExpiry', N'CreatedAt', N'CreatedBy', N'Email', N'FailedLoginAttempts', N'IsDeleted', N'IsEmailConfirmed', N'LastLoginAt', N'LockoutUntil', N'PasswordHash', N'PhoneNumber', N'Status', N'UpdatedAt', N'UpdatedBy', N'UserName') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] ON;
INSERT INTO [Users] ([Id], [ActivationToken], [ActivationTokenExpiry], [CreatedAt], [CreatedBy], [Email], [FailedLoginAttempts], [IsDeleted], [IsEmailConfirmed], [LastLoginAt], [LockoutUntil], [PasswordHash], [PhoneNumber], [Status], [UpdatedAt], [UpdatedBy], [UserName])
VALUES (1, NULL, NULL, '2026-01-01T00:00:00.0000000Z', N'system', N'admin@school.local', 0, CAST(0 AS bit), CAST(1 AS bit), NULL, NULL, N'ChangeThisHash', NULL, 1, NULL, NULL, N'admin');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ActivationToken', N'ActivationTokenExpiry', N'CreatedAt', N'CreatedBy', N'Email', N'FailedLoginAttempts', N'IsDeleted', N'IsEmailConfirmed', N'LastLoginAt', N'LockoutUntil', N'PasswordHash', N'PhoneNumber', N'Status', N'UpdatedAt', N'UpdatedBy', N'UserName') AND [object_id] = OBJECT_ID(N'[Users]'))
    SET IDENTITY_INSERT [Users] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'CreatedAt', N'CreatedBy', N'FeeInvoiceId', N'IsDeleted', N'Method', N'PaidAt', N'ReferenceNo', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] ON;
INSERT INTO [Payments] ([Id], [Amount], [CreatedAt], [CreatedBy], [FeeInvoiceId], [IsDeleted], [Method], [PaidAt], [ReferenceNo], [UpdatedAt], [UpdatedBy])
VALUES (1, 2500.0, '2026-01-01T00:00:00.0000000Z', N'system', 1, CAST(0 AS bit), 1, '2026-01-01T00:00:00.0000000Z', NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Amount', N'CreatedAt', N'CreatedBy', N'FeeInvoiceId', N'IsDeleted', N'Method', N'PaidAt', N'ReferenceNo', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Payments]'))
    SET IDENTITY_INSERT [Payments] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (1, 1),
(2, 1),
(3, 1),
(4, 1),
(5, 1),
(6, 1),
(7, 1),
(8, 1),
(9, 1),
(10, 1),
(11, 1),
(12, 1),
(13, 1),
(14, 1),
(15, 1),
(16, 1),
(17, 1),
(18, 1),
(19, 1),
(20, 1),
(21, 1),
(22, 1),
(23, 1),
(24, 1),
(25, 1),
(26, 1),
(27, 1),
(28, 1),
(29, 1),
(30, 1),
(31, 1),
(32, 1),
(33, 1),
(34, 1),
(35, 1),
(36, 1),
(37, 1),
(38, 1),
(39, 1),
(40, 1),
(41, 1),
(42, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (43, 1),
(44, 1),
(45, 1),
(46, 1),
(47, 1),
(48, 1),
(49, 1),
(50, 1),
(51, 1),
(52, 1),
(53, 1),
(54, 1),
(55, 1),
(56, 1),
(57, 1),
(58, 1),
(59, 1),
(60, 1),
(61, 1),
(62, 1),
(63, 1),
(64, 1),
(65, 1),
(66, 1),
(67, 1),
(68, 1),
(69, 1),
(70, 1),
(71, 1),
(72, 1),
(73, 1),
(74, 1),
(75, 1),
(76, 1),
(77, 1),
(78, 1),
(79, 1),
(80, 1),
(81, 1),
(82, 1),
(83, 1),
(84, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (85, 1),
(86, 1),
(87, 1),
(88, 1),
(89, 1),
(90, 1),
(91, 1),
(92, 1),
(93, 1),
(94, 1),
(95, 1),
(96, 1),
(97, 1),
(98, 1),
(99, 1),
(100, 1),
(101, 1),
(102, 1),
(103, 1),
(104, 1),
(105, 1),
(106, 1),
(107, 1),
(108, 1),
(109, 1),
(110, 1),
(111, 1),
(112, 1),
(113, 1),
(114, 1),
(115, 1),
(116, 1),
(117, 1),
(118, 1),
(119, 1),
(120, 1),
(121, 1),
(122, 1),
(123, 1),
(124, 1),
(125, 1),
(126, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (127, 1),
(128, 1),
(129, 1),
(130, 1),
(131, 1),
(132, 1),
(133, 1),
(134, 1),
(135, 1),
(136, 1),
(137, 1),
(138, 1),
(139, 1),
(140, 1),
(141, 1),
(142, 1),
(143, 1),
(144, 1),
(145, 1),
(146, 1),
(147, 1),
(148, 1),
(149, 1),
(150, 1),
(151, 1),
(152, 1),
(153, 1),
(154, 1),
(155, 1),
(156, 1),
(157, 1),
(158, 1),
(159, 1),
(160, 1),
(161, 1),
(162, 1),
(163, 1),
(164, 1),
(165, 1),
(166, 1),
(167, 1),
(168, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (169, 1),
(170, 1),
(171, 1),
(172, 1),
(173, 1),
(174, 1),
(175, 1),
(176, 1),
(177, 1),
(178, 1),
(179, 1),
(180, 1),
(181, 1),
(182, 1),
(183, 1),
(184, 1),
(185, 1),
(186, 1),
(187, 1),
(188, 1),
(189, 1),
(190, 1),
(191, 1),
(192, 1),
(193, 1),
(194, 1),
(195, 1),
(196, 1),
(197, 1),
(198, 1),
(199, 1),
(200, 1),
(201, 1),
(202, 1),
(203, 1),
(204, 1),
(205, 1),
(206, 1),
(207, 1),
(208, 1),
(209, 1),
(210, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (211, 1),
(212, 1),
(213, 1),
(214, 1),
(215, 1),
(216, 1),
(217, 1),
(218, 1),
(219, 1),
(220, 1),
(221, 1),
(222, 1),
(223, 1),
(224, 1),
(225, 1),
(226, 1),
(227, 1),
(228, 1),
(229, 1),
(230, 1),
(231, 1),
(232, 1),
(233, 1),
(234, 1),
(235, 1),
(236, 1),
(237, 1),
(238, 1),
(239, 1),
(240, 1),
(241, 1),
(242, 1),
(243, 1),
(244, 1),
(245, 1),
(246, 1),
(247, 1),
(248, 1),
(249, 1),
(250, 1),
(251, 1),
(252, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (253, 1),
(254, 1),
(255, 1),
(256, 1),
(257, 1),
(258, 1),
(259, 1),
(260, 1),
(261, 1),
(262, 1),
(263, 1),
(264, 1),
(265, 1),
(266, 1),
(267, 1),
(268, 1),
(269, 1),
(270, 1),
(1, 2),
(2, 2),
(3, 2),
(4, 2),
(5, 2),
(6, 2),
(7, 2),
(8, 2),
(9, 2),
(10, 2),
(11, 2),
(12, 2),
(13, 2),
(14, 2),
(15, 2),
(16, 2),
(17, 2),
(18, 2),
(19, 2),
(20, 2),
(21, 2),
(22, 2),
(23, 2),
(24, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (25, 2),
(26, 2),
(27, 2),
(28, 2),
(29, 2),
(30, 2),
(31, 2),
(32, 2),
(33, 2),
(34, 2),
(35, 2),
(36, 2),
(37, 2),
(38, 2),
(39, 2),
(40, 2),
(41, 2),
(42, 2),
(43, 2),
(44, 2),
(45, 2),
(46, 2),
(47, 2),
(48, 2),
(49, 2),
(50, 2),
(51, 2),
(52, 2),
(53, 2),
(54, 2),
(55, 2),
(56, 2),
(57, 2),
(58, 2),
(59, 2),
(60, 2),
(61, 2),
(62, 2),
(63, 2),
(64, 2),
(65, 2),
(66, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (67, 2),
(68, 2),
(69, 2),
(70, 2),
(71, 2),
(72, 2),
(73, 2),
(74, 2),
(75, 2),
(76, 2),
(77, 2),
(78, 2),
(79, 2),
(80, 2),
(81, 2),
(82, 2),
(83, 2),
(84, 2),
(85, 2),
(86, 2),
(87, 2),
(88, 2),
(89, 2),
(90, 2),
(91, 2),
(92, 2),
(93, 2),
(94, 2),
(95, 2),
(96, 2),
(97, 2),
(98, 2),
(99, 2),
(100, 2),
(101, 2),
(102, 2),
(103, 2),
(104, 2),
(105, 2),
(106, 2),
(107, 2),
(108, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (109, 2),
(110, 2),
(111, 2),
(112, 2),
(113, 2),
(114, 2),
(115, 2),
(116, 2),
(117, 2),
(118, 2),
(119, 2),
(120, 2),
(121, 2),
(122, 2),
(123, 2),
(124, 2),
(125, 2),
(126, 2),
(127, 2),
(128, 2),
(129, 2),
(130, 2),
(131, 2),
(132, 2),
(133, 2),
(134, 2),
(135, 2),
(136, 2),
(137, 2),
(138, 2),
(139, 2),
(140, 2),
(141, 2),
(142, 2),
(143, 2),
(144, 2),
(145, 2),
(146, 2),
(147, 2),
(148, 2),
(149, 2),
(150, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (151, 2),
(152, 2),
(153, 2),
(154, 2),
(155, 2),
(156, 2),
(157, 2),
(158, 2),
(159, 2),
(160, 2),
(161, 2),
(162, 2),
(163, 2),
(164, 2),
(165, 2),
(166, 2),
(167, 2),
(168, 2),
(169, 2),
(170, 2),
(171, 2),
(172, 2),
(173, 2),
(174, 2),
(175, 2),
(176, 2),
(177, 2),
(178, 2),
(179, 2),
(180, 2),
(181, 2),
(182, 2),
(183, 2),
(184, 2),
(185, 2),
(186, 2),
(187, 2),
(188, 2),
(189, 2),
(190, 2),
(191, 2),
(192, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (193, 2),
(194, 2),
(195, 2),
(196, 2),
(197, 2),
(198, 2),
(199, 2),
(200, 2),
(201, 2),
(202, 2),
(203, 2),
(204, 2),
(205, 2),
(206, 2),
(207, 2),
(208, 2),
(209, 2),
(210, 2),
(211, 2),
(212, 2),
(213, 2),
(214, 2),
(215, 2),
(216, 2),
(217, 2),
(218, 2),
(219, 2),
(220, 2),
(221, 2),
(222, 2),
(223, 2),
(224, 2),
(225, 2),
(226, 2),
(227, 2),
(228, 2),
(229, 2),
(230, 2),
(231, 2),
(232, 2),
(233, 2),
(234, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (235, 2),
(236, 2),
(237, 2),
(238, 2),
(239, 2),
(240, 2),
(241, 2),
(242, 2),
(243, 2),
(244, 2),
(245, 2),
(246, 2),
(247, 2),
(248, 2),
(249, 2),
(250, 2),
(251, 2),
(252, 2),
(253, 2),
(254, 2),
(255, 2),
(256, 2),
(257, 2),
(258, 2),
(259, 2),
(260, 2),
(261, 2),
(262, 2),
(263, 2),
(264, 2),
(265, 2),
(266, 2),
(267, 2),
(268, 2),
(269, 2),
(270, 2),
(1, 3),
(2, 3),
(3, 3),
(4, 3),
(5, 3),
(6, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (7, 3),
(8, 3),
(9, 3),
(37, 3),
(38, 3),
(39, 3),
(40, 3),
(41, 3),
(42, 3),
(43, 3),
(44, 3),
(45, 3),
(46, 3),
(47, 3),
(48, 3),
(49, 3),
(50, 3),
(51, 3),
(52, 3),
(53, 3),
(54, 3),
(64, 3),
(65, 3),
(66, 3),
(67, 3),
(68, 3),
(69, 3),
(70, 3),
(71, 3),
(72, 3),
(73, 3),
(74, 3),
(75, 3),
(76, 3),
(77, 3),
(78, 3),
(79, 3),
(80, 3),
(81, 3),
(82, 3),
(83, 3),
(84, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (85, 3),
(86, 3),
(87, 3),
(88, 3),
(89, 3),
(90, 3),
(91, 3),
(92, 3),
(93, 3),
(94, 3),
(95, 3),
(96, 3),
(97, 3),
(98, 3),
(99, 3),
(100, 3),
(101, 3),
(102, 3),
(103, 3),
(104, 3),
(105, 3),
(106, 3),
(107, 3),
(108, 3),
(109, 3),
(110, 3),
(111, 3),
(112, 3),
(113, 3),
(114, 3),
(115, 3),
(116, 3),
(117, 3),
(181, 3),
(182, 3),
(183, 3),
(184, 3),
(185, 3),
(186, 3),
(187, 3),
(188, 3),
(189, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (199, 3),
(200, 3),
(201, 3),
(202, 3),
(203, 3),
(204, 3),
(205, 3),
(206, 3),
(207, 3),
(208, 3),
(209, 3),
(210, 3),
(211, 3),
(212, 3),
(213, 3),
(214, 3),
(215, 3),
(216, 3),
(217, 3),
(218, 3),
(219, 3),
(220, 3),
(221, 3),
(222, 3),
(223, 3),
(224, 3),
(225, 3),
(226, 3),
(227, 3),
(228, 3),
(229, 3),
(230, 3),
(231, 3),
(232, 3),
(233, 3),
(234, 3),
(235, 3),
(236, 3),
(237, 3),
(238, 3),
(239, 3),
(240, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (241, 3),
(242, 3),
(243, 3),
(244, 3),
(245, 3),
(246, 3),
(247, 3),
(248, 3),
(249, 3),
(250, 3),
(251, 3),
(252, 3),
(1, 5),
(46, 5),
(64, 5),
(91, 5),
(92, 5),
(100, 5),
(109, 5),
(110, 5),
(118, 5),
(119, 5),
(181, 5),
(226, 5),
(1, 6),
(2, 6),
(3, 6),
(5, 6),
(6, 6),
(7, 6),
(8, 6),
(9, 6),
(37, 6),
(38, 6),
(39, 6),
(41, 6),
(42, 6),
(43, 6),
(44, 6),
(45, 6),
(46, 6),
(47, 6);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (48, 6),
(50, 6),
(51, 6),
(52, 6),
(53, 6),
(54, 6),
(127, 6),
(128, 6),
(129, 6),
(131, 6),
(132, 6),
(133, 6),
(134, 6),
(135, 6),
(136, 6),
(137, 6),
(138, 6),
(140, 6),
(141, 6),
(142, 6),
(143, 6),
(144, 6),
(181, 6),
(182, 6),
(183, 6),
(185, 6),
(186, 6),
(187, 6),
(188, 6),
(189, 6),
(208, 6),
(209, 6),
(210, 6),
(212, 6),
(213, 6),
(214, 6),
(215, 6),
(216, 6),
(217, 6),
(218, 6),
(219, 6),
(221, 6);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (222, 6),
(223, 6),
(224, 6),
(225, 6),
(1, 7),
(46, 7),
(91, 7),
(109, 7),
(118, 7),
(119, 7),
(127, 7),
(172, 7),
(217, 7);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'ParentSectionId', N'SchoolClassId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Sections]'))
    SET IDENTITY_INSERT [Sections] ON;
INSERT INTO [Sections] ([Id], [Capacity], [CreatedAt], [CreatedBy], [IsDeleted], [Name], [ParentSectionId], [SchoolClassId], [UpdatedAt], [UpdatedBy])
VALUES (1, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 1, NULL, NULL),
(2, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 1, NULL, NULL),
(3, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 2, NULL, NULL),
(4, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 2, NULL, NULL),
(5, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 3, NULL, NULL),
(6, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 3, NULL, NULL),
(7, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 4, NULL, NULL),
(8, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 4, NULL, NULL),
(9, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 5, NULL, NULL),
(10, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 5, NULL, NULL),
(11, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 6, NULL, NULL),
(12, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 6, NULL, NULL),
(13, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 7, NULL, NULL),
(14, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 7, NULL, NULL),
(15, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'A', NULL, 8, NULL, NULL),
(16, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'B', NULL, 8, NULL, NULL),
(17, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Science', NULL, 9, NULL, NULL),
(20, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Business Studies', NULL, 9, NULL, NULL),
(23, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Humanities', NULL, 9, NULL, NULL),
(26, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Science', NULL, 10, NULL, NULL),
(29, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Business Studies', NULL, 10, NULL, NULL),
(32, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Humanities', NULL, 10, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'ParentSectionId', N'SchoolClassId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Sections]'))
    SET IDENTITY_INSERT [Sections] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[UserRoles]'))
    SET IDENTITY_INSERT [UserRoles] ON;
INSERT INTO [UserRoles] ([RoleId], [UserId])
VALUES (1, 1);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'UserId') AND [object_id] = OBJECT_ID(N'[UserRoles]'))
    SET IDENTITY_INSERT [UserRoles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'ParentSectionId', N'SchoolClassId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Sections]'))
    SET IDENTITY_INSERT [Sections] ON;
INSERT INTO [Sections] ([Id], [Capacity], [CreatedAt], [CreatedBy], [IsDeleted], [Name], [ParentSectionId], [SchoolClassId], [UpdatedAt], [UpdatedBy])
VALUES (18, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Science A', 17, 9, NULL, NULL),
(19, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Science B', 17, 9, NULL, NULL),
(21, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Business Studies A', 20, 9, NULL, NULL),
(22, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Business Studies B', 20, 9, NULL, NULL),
(24, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Humanities A', 23, 9, NULL, NULL),
(25, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Humanities B', 23, 9, NULL, NULL),
(27, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Science A', 26, 10, NULL, NULL),
(28, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Science B', 26, 10, NULL, NULL),
(30, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Business Studies A', 29, 10, NULL, NULL),
(31, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Business Studies B', 29, 10, NULL, NULL),
(33, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Humanities A', 32, 10, NULL, NULL),
(34, 50, '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Humanities B', 32, 10, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Capacity', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Name', N'ParentSectionId', N'SchoolClassId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Sections]'))
    SET IDENTITY_INSERT [Sections] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AlternativeNumber', N'AssignedReligionSubjectId', N'BirthCertificateNo', N'BloodGroup', N'ClassId', N'Country', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'EmailAddress', N'FatherName', N'FatherOccupation', N'FullName', N'FullNameBangla', N'Gender', N'IsDeleted', N'MaritalStatus', N'MobileNumber', N'MotherName', N'MotherOccupation', N'Nationality', N'PermanentDistrict', N'PermanentPostOffice', N'PermanentThana', N'PermanentVillage', N'PresentDistrict', N'PresentPostOffice', N'PresentThana', N'PresentVillage', N'ProfilePicturePath', N'Religion', N'RollNumber', N'SectionId', N'Status', N'StudentGroupId', N'StudentNo', N'UpdatedAt', N'UpdatedBy', N'UserId') AND [object_id] = OBJECT_ID(N'[Students]'))
    SET IDENTITY_INSERT [Students] ON;
INSERT INTO [Students] ([Id], [AlternativeNumber], [AssignedReligionSubjectId], [BirthCertificateNo], [BloodGroup], [ClassId], [Country], [CreatedAt], [CreatedBy], [DateOfBirth], [EmailAddress], [FatherName], [FatherOccupation], [FullName], [FullNameBangla], [Gender], [IsDeleted], [MaritalStatus], [MobileNumber], [MotherName], [MotherOccupation], [Nationality], [PermanentDistrict], [PermanentPostOffice], [PermanentThana], [PermanentVillage], [PresentDistrict], [PresentPostOffice], [PresentThana], [PresentVillage], [ProfilePicturePath], [Religion], [RollNumber], [SectionId], [Status], [StudentGroupId], [StudentNo], [UpdatedAt], [UpdatedBy], [UserId])
VALUES (1, NULL, NULL, NULL, NULL, 1, N'Bangladesh', '2026-01-01T00:00:00.0000000Z', N'system', '2018-02-01T00:00:00.0000000', NULL, N'Father One', NULL, N'Sample Student One', NULL, N'Male', CAST(0 AS bit), N'Single', N'01700000010', N'Mother One', NULL, N'Bangladeshi', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Islam', 1, 1, 1, NULL, N'STU-2026-0001', NULL, NULL, NULL),
(2, NULL, NULL, NULL, NULL, 1, N'Bangladesh', '2026-01-01T00:00:00.0000000Z', N'system', '2018-05-11T00:00:00.0000000', NULL, N'Father Two', NULL, N'Sample Student Two', NULL, N'Female', CAST(0 AS bit), N'Single', N'01700000020', N'Mother Two', NULL, N'Bangladeshi', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'Islam', 2, 1, 1, NULL, N'STU-2026-0002', NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AlternativeNumber', N'AssignedReligionSubjectId', N'BirthCertificateNo', N'BloodGroup', N'ClassId', N'Country', N'CreatedAt', N'CreatedBy', N'DateOfBirth', N'EmailAddress', N'FatherName', N'FatherOccupation', N'FullName', N'FullNameBangla', N'Gender', N'IsDeleted', N'MaritalStatus', N'MobileNumber', N'MotherName', N'MotherOccupation', N'Nationality', N'PermanentDistrict', N'PermanentPostOffice', N'PermanentThana', N'PermanentVillage', N'PresentDistrict', N'PresentPostOffice', N'PresentThana', N'PresentVillage', N'ProfilePicturePath', N'Religion', N'RollNumber', N'SectionId', N'Status', N'StudentGroupId', N'StudentNo', N'UpdatedAt', N'UpdatedBy', N'UserId') AND [object_id] = OBJECT_ID(N'[Students]'))
    SET IDENTITY_INSERT [Students] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Email', N'IsDeleted', N'Name', N'Occupation', N'Phone', N'Relation', N'StudentId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Guardians]'))
    SET IDENTITY_INSERT [Guardians] ON;
INSERT INTO [Guardians] ([Id], [CreatedAt], [CreatedBy], [Email], [IsDeleted], [Name], [Occupation], [Phone], [Relation], [StudentId], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', NULL, CAST(0 AS bit), N'Guardian One', NULL, N'01700000001', N'Father', 1, NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'system', NULL, CAST(0 AS bit), N'Guardian Two', NULL, N'01700000002', N'Mother', 2, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Email', N'IsDeleted', N'Name', N'Occupation', N'Phone', N'Relation', N'StudentId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Guardians]'))
    SET IDENTITY_INSERT [Guardians] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssignmentMarks', N'BehaviourMarks', N'CQMarks', N'CompetencyMarks', N'ContinuousAssessmentMarks', N'CreatedAt', N'CreatedBy', N'CreatedByUserId', N'EnteredByTeacherId', N'ExamId', N'Grade', N'GradePoint', N'IsDeleted', N'IsLocked', N'LabMarks', N'LockedAt', N'MCQMarks', N'MarksObtained', N'OralMarks', N'ParticipationMarks', N'PracticalMarks', N'Status', N'StudentId', N'SubjectId', N'SubmittedAt', N'UpdatedAt', N'UpdatedBy', N'UpdatedByUserId', N'VivaMarks', N'WrittenMarks') AND [object_id] = OBJECT_ID(N'[Marks]'))
    SET IDENTITY_INSERT [Marks] ON;
INSERT INTO [Marks] ([Id], [AssignmentMarks], [BehaviourMarks], [CQMarks], [CompetencyMarks], [ContinuousAssessmentMarks], [CreatedAt], [CreatedBy], [CreatedByUserId], [EnteredByTeacherId], [ExamId], [Grade], [GradePoint], [IsDeleted], [IsLocked], [LabMarks], [LockedAt], [MCQMarks], [MarksObtained], [OralMarks], [ParticipationMarks], [PracticalMarks], [Status], [StudentId], [SubjectId], [SubmittedAt], [UpdatedAt], [UpdatedBy], [UpdatedByUserId], [VivaMarks], [WrittenMarks])
VALUES (1, NULL, NULL, NULL, NULL, NULL, '2026-01-01T00:00:00.0000000Z', N'system', NULL, 1, 1, NULL, NULL, CAST(0 AS bit), CAST(0 AS bit), NULL, NULL, NULL, 86.0, NULL, NULL, NULL, 4, 1, 1, NULL, NULL, NULL, NULL, NULL, NULL),
(2, NULL, NULL, NULL, NULL, NULL, '2026-01-01T00:00:00.0000000Z', N'system', NULL, 1, 1, NULL, NULL, CAST(0 AS bit), CAST(0 AS bit), NULL, NULL, NULL, 78.0, NULL, NULL, NULL, 4, 2, 1, NULL, NULL, NULL, NULL, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'AssignmentMarks', N'BehaviourMarks', N'CQMarks', N'CompetencyMarks', N'ContinuousAssessmentMarks', N'CreatedAt', N'CreatedBy', N'CreatedByUserId', N'EnteredByTeacherId', N'ExamId', N'Grade', N'GradePoint', N'IsDeleted', N'IsLocked', N'LabMarks', N'LockedAt', N'MCQMarks', N'MarksObtained', N'OralMarks', N'ParticipationMarks', N'PracticalMarks', N'Status', N'StudentId', N'SubjectId', N'SubmittedAt', N'UpdatedAt', N'UpdatedBy', N'UpdatedByUserId', N'VivaMarks', N'WrittenMarks') AND [object_id] = OBJECT_ID(N'[Marks]'))
    SET IDENTITY_INSERT [Marks] OFF;
GO

CREATE INDEX [IX_ActivityLogs_UserId] ON [ActivityLogs] ([UserId]);
GO

CREATE INDEX [IX_AdmissionDocuments_AdmissionApplicationId] ON [AdmissionDocuments] ([AdmissionApplicationId]);
GO

CREATE UNIQUE INDEX [IX_Admissions_ApplicationNo] ON [Admissions] ([ApplicationNo]);
GO

CREATE INDEX [IX_AssignmentSubmissions_AssignmentTaskId] ON [AssignmentSubmissions] ([AssignmentTaskId]);
GO

CREATE INDEX [IX_AuditLogs_UserId] ON [AuditLogs] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_Books_AccessionNo] ON [Books] ([AccessionNo]);
GO

CREATE INDEX [IX_ClassSubjects_SchoolClassId] ON [ClassSubjects] ([SchoolClassId]);
GO

CREATE INDEX [IX_ClassSubjects_SectionId] ON [ClassSubjects] ([SectionId]);
GO

CREATE INDEX [IX_ClassSubjects_StudentGroupId] ON [ClassSubjects] ([StudentGroupId]);
GO

CREATE INDEX [IX_ClassSubjects_SubjectId] ON [ClassSubjects] ([SubjectId]);
GO

CREATE INDEX [IX_ClassSubjectTeachers_ClassSubjectId] ON [ClassSubjectTeachers] ([ClassSubjectId]);
GO

CREATE INDEX [IX_ClassSubjectTeachers_TeacherId] ON [ClassSubjectTeachers] ([TeacherId]);
GO

CREATE INDEX [IX_EmployeeAcademicAssignments_EmployeeId] ON [EmployeeAcademicAssignments] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeAttendances_EmployeeId] ON [EmployeeAttendances] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeDocuments_EmployeeId] ON [EmployeeDocuments] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeExperiences_EmployeeId] ON [EmployeeExperiences] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeLeaves_ApprovedByUserId] ON [EmployeeLeaves] ([ApprovedByUserId]);
GO

CREATE INDEX [IX_EmployeeLeaves_EmployeeId] ON [EmployeeLeaves] ([EmployeeId]);
GO

CREATE INDEX [IX_EmployeeQualifications_EmployeeId] ON [EmployeeQualifications] ([EmployeeId]);
GO

CREATE INDEX [IX_Employees_DepartmentId] ON [Employees] ([DepartmentId]);
GO

CREATE INDEX [IX_Employees_DesignationId] ON [Employees] ([DesignationId]);
GO

CREATE UNIQUE INDEX [IX_Employees_Email] ON [Employees] ([Email]) WHERE [Email] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Employees_EmployeeCode] ON [Employees] ([EmployeeCode]);
GO

CREATE UNIQUE INDEX [IX_Employees_NIDNumber] ON [Employees] ([NIDNumber]) WHERE [NIDNumber] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Employees_Phone] ON [Employees] ([Phone]);
GO

CREATE INDEX [IX_Employees_UserId] ON [Employees] ([UserId]);
GO

CREATE INDEX [IX_EmployeeSalaries_EmployeeId] ON [EmployeeSalaries] ([EmployeeId]);
GO

CREATE INDEX [IX_ExamConfigurations_ClassId] ON [ExamConfigurations] ([ClassId]);
GO

CREATE UNIQUE INDEX [IX_ExamConfigurations_ExamTypeId_ClassId] ON [ExamConfigurations] ([ExamTypeId], [ClassId]);
GO

CREATE INDEX [IX_ExamSchedules_ExamId] ON [ExamSchedules] ([ExamId]);
GO

CREATE INDEX [IX_ExamSchedules_SubjectId] ON [ExamSchedules] ([SubjectId]);
GO

CREATE INDEX [IX_ExamSubjects_ExamId] ON [ExamSubjects] ([ExamId]);
GO

CREATE INDEX [IX_ExamSubjects_SubjectId] ON [ExamSubjects] ([SubjectId]);
GO

CREATE UNIQUE INDEX [IX_ExamTypes_Code] ON [ExamTypes] ([Code]);
GO

CREATE UNIQUE INDEX [IX_FeeInvoices_InvoiceNo] ON [FeeInvoices] ([InvoiceNo]);
GO

CREATE UNIQUE INDEX [IX_FinalResults_AcademicYearId_StudentId] ON [FinalResults] ([AcademicYearId], [StudentId]);
GO

CREATE INDEX [IX_FinalResults_SchoolClassId] ON [FinalResults] ([SchoolClassId]);
GO

CREATE INDEX [IX_FinalResults_StudentId] ON [FinalResults] ([StudentId]);
GO

CREATE UNIQUE INDEX [IX_GpaConfigurations_Grade] ON [GpaConfigurations] ([Grade]);
GO

CREATE UNIQUE INDEX [IX_GpaConfigurations_MinMarks_MaxMarks] ON [GpaConfigurations] ([MinMarks], [MaxMarks]);
GO

CREATE INDEX [IX_Guardians_StudentId] ON [Guardians] ([StudentId]);
GO

CREATE INDEX [IX_LessonPlans_TeacherId] ON [LessonPlans] ([TeacherId]);
GO

CREATE INDEX [IX_MarkAuditLogs_MarkEntryId] ON [MarkAuditLogs] ([MarkEntryId]);
GO

CREATE INDEX [IX_MarkEntryDrafts_ExamId_StudentId_SubjectId] ON [MarkEntryDrafts] ([ExamId], [StudentId], [SubjectId]);
GO

CREATE INDEX [IX_MarkEntryDrafts_StudentId] ON [MarkEntryDrafts] ([StudentId]);
GO

CREATE INDEX [IX_MarkEntryDrafts_SubjectId] ON [MarkEntryDrafts] ([SubjectId]);
GO

CREATE INDEX [IX_Marks_EnteredByTeacherId] ON [Marks] ([EnteredByTeacherId]);
GO

CREATE UNIQUE INDEX [IX_Marks_ExamId_StudentId_SubjectId] ON [Marks] ([ExamId], [StudentId], [SubjectId]);
GO

CREATE INDEX [IX_Marks_StudentId] ON [Marks] ([StudentId]);
GO

CREATE INDEX [IX_Marks_SubjectId] ON [Marks] ([SubjectId]);
GO

CREATE INDEX [IX_MeritResults_ExamId_SectionId_Position] ON [MeritResults] ([ExamId], [SectionId], [Position]);
GO

CREATE UNIQUE INDEX [IX_MeritResults_ExamId_StudentId] ON [MeritResults] ([ExamId], [StudentId]);
GO

CREATE INDEX [IX_MeritResults_SectionId] ON [MeritResults] ([SectionId]);
GO

CREATE INDEX [IX_MeritResults_StudentId] ON [MeritResults] ([StudentId]);
GO

CREATE INDEX [IX_PasswordResetTokens_UserId] ON [PasswordResetTokens] ([UserId]);
GO

CREATE INDEX [IX_Payments_FeeInvoiceId] ON [Payments] ([FeeInvoiceId]);
GO

CREATE INDEX [IX_PromotionHistories_AcademicYearId] ON [PromotionHistories] ([AcademicYearId]);
GO

CREATE INDEX [IX_PromotionHistories_FromClassId] ON [PromotionHistories] ([FromClassId]);
GO

CREATE UNIQUE INDEX [IX_PromotionHistories_StudentId_AcademicYearId] ON [PromotionHistories] ([StudentId], [AcademicYearId]);
GO

CREATE INDEX [IX_PromotionHistories_ToClassId] ON [PromotionHistories] ([ToClassId]);
GO

CREATE UNIQUE INDEX [IX_ReEvaluationRequests_ExamId_StudentId_SubjectId] ON [ReEvaluationRequests] ([ExamId], [StudentId], [SubjectId]);
GO

CREATE INDEX [IX_ReEvaluationRequests_StudentId] ON [ReEvaluationRequests] ([StudentId]);
GO

CREATE INDEX [IX_ReEvaluationRequests_SubjectId] ON [ReEvaluationRequests] ([SubjectId]);
GO

CREATE INDEX [IX_ReportCards_ExamId] ON [ReportCards] ([ExamId]);
GO

CREATE INDEX [IX_ReportCards_StudentId] ON [ReportCards] ([StudentId]);
GO

CREATE INDEX [IX_ResultAuditLogs_ExamId] ON [ResultAuditLogs] ([ExamId]);
GO

CREATE INDEX [IX_ResultAuditLogs_StudentId] ON [ResultAuditLogs] ([StudentId]);
GO

CREATE INDEX [IX_ResultAuditLogs_SubjectId] ON [ResultAuditLogs] ([SubjectId]);
GO

CREATE INDEX [IX_ResultLocks_ExamId] ON [ResultLocks] ([ExamId]);
GO

CREATE INDEX [IX_ResultPublications_ExamId] ON [ResultPublications] ([ExamId]);
GO

CREATE INDEX [IX_RolePermissions_PermissionId] ON [RolePermissions] ([PermissionId]);
GO

CREATE UNIQUE INDEX [IX_RollNumberAssignments_AcademicYearId_StudentId_ToClassId] ON [RollNumberAssignments] ([AcademicYearId], [StudentId], [ToClassId]);
GO

CREATE INDEX [IX_RollNumberAssignments_FromClassId] ON [RollNumberAssignments] ([FromClassId]);
GO

CREATE INDEX [IX_RollNumberAssignments_SectionId] ON [RollNumberAssignments] ([SectionId]);
GO

CREATE INDEX [IX_RollNumberAssignments_StudentId] ON [RollNumberAssignments] ([StudentId]);
GO

CREATE INDEX [IX_RollNumberAssignments_ToClassId] ON [RollNumberAssignments] ([ToClassId]);
GO

CREATE INDEX [IX_Sections_ParentSectionId] ON [Sections] ([ParentSectionId]);
GO

CREATE INDEX [IX_Sections_SchoolClassId] ON [Sections] ([SchoolClassId]);
GO

CREATE INDEX [IX_StudentDocuments_StudentId] ON [StudentDocuments] ([StudentId]);
GO

CREATE UNIQUE INDEX [IX_StudentExamResults_ExamId_StudentId] ON [StudentExamResults] ([ExamId], [StudentId]);
GO

CREATE INDEX [IX_StudentExamResults_StudentId] ON [StudentExamResults] ([StudentId]);
GO

CREATE INDEX [IX_StudentGroupAssignments_AcademicYearId] ON [StudentGroupAssignments] ([AcademicYearId]);
GO

CREATE INDEX [IX_StudentGroupAssignments_SchoolClassId] ON [StudentGroupAssignments] ([SchoolClassId]);
GO

CREATE INDEX [IX_StudentGroupAssignments_StudentGroupId] ON [StudentGroupAssignments] ([StudentGroupId]);
GO

CREATE UNIQUE INDEX [IX_StudentGroupAssignments_StudentId_SchoolClassId_AcademicYearId] ON [StudentGroupAssignments] ([StudentId], [SchoolClassId], [AcademicYearId]);
GO

CREATE UNIQUE INDEX [IX_StudentGroups_Code] ON [StudentGroups] ([Code]);
GO

CREATE INDEX [IX_Students_AssignedReligionSubjectId] ON [Students] ([AssignedReligionSubjectId]);
GO

CREATE UNIQUE INDEX [IX_Students_ClassId_SectionId_RollNumber] ON [Students] ([ClassId], [SectionId], [RollNumber]);
GO

CREATE INDEX [IX_Students_SectionId] ON [Students] ([SectionId]);
GO

CREATE INDEX [IX_Students_StudentGroupId] ON [Students] ([StudentGroupId]);
GO

CREATE UNIQUE INDEX [IX_Students_StudentNo] ON [Students] ([StudentNo]);
GO

CREATE INDEX [IX_Students_UserId] ON [Students] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_StudentSubjectResults_ExamId_StudentId_SubjectId] ON [StudentSubjectResults] ([ExamId], [StudentId], [SubjectId]);
GO

CREATE INDEX [IX_StudentSubjectResults_StudentId] ON [StudentSubjectResults] ([StudentId]);
GO

CREATE INDEX [IX_StudentSubjectResults_SubjectId] ON [StudentSubjectResults] ([SubjectId]);
GO

CREATE UNIQUE INDEX [IX_SubjectComponents_ClassSubjectId_ComponentName] ON [SubjectComponents] ([ClassSubjectId], [ComponentName]);
GO

CREATE UNIQUE INDEX [IX_Subjects_Code] ON [Subjects] ([Code]);
GO

CREATE INDEX [IX_TeacherAttendances_TeacherId] ON [TeacherAttendances] ([TeacherId]);
GO

CREATE INDEX [IX_TeacherClassAssignments_AcademicYearId] ON [TeacherClassAssignments] ([AcademicYearId]);
GO

CREATE INDEX [IX_TeacherClassAssignments_ClassId] ON [TeacherClassAssignments] ([ClassId]);
GO

CREATE INDEX [IX_TeacherClassAssignments_SectionId] ON [TeacherClassAssignments] ([SectionId]);
GO

CREATE UNIQUE INDEX [IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments] ([TeacherId], [ClassId], [SectionId], [AcademicYearId]);
GO

CREATE INDEX [IX_TeacherDocuments_TeacherId] ON [TeacherDocuments] ([TeacherId]);
GO

CREATE INDEX [IX_TeacherLeaves_ApprovedByUserId] ON [TeacherLeaves] ([ApprovedByUserId]);
GO

CREATE INDEX [IX_TeacherLeaves_TeacherId] ON [TeacherLeaves] ([TeacherId]);
GO

CREATE INDEX [IX_TeacherPerformances_AcademicYearId] ON [TeacherPerformances] ([AcademicYearId]);
GO

CREATE INDEX [IX_TeacherPerformances_EvaluatorUserId] ON [TeacherPerformances] ([EvaluatorUserId]);
GO

CREATE INDEX [IX_TeacherPerformances_TeacherId] ON [TeacherPerformances] ([TeacherId]);
GO

CREATE INDEX [IX_Teachers_EmployeeId] ON [Teachers] ([EmployeeId]);
GO

CREATE UNIQUE INDEX [IX_Teachers_TeacherNo] ON [Teachers] ([TeacherNo]);
GO

CREATE INDEX [IX_Teachers_UserId] ON [Teachers] ([UserId]);
GO

CREATE INDEX [IX_TeacherSalaries_TeacherId] ON [TeacherSalaries] ([TeacherId]);
GO

CREATE INDEX [IX_TeacherSubjectAssignments_AcademicYearId] ON [TeacherSubjectAssignments] ([AcademicYearId]);
GO

CREATE INDEX [IX_TeacherSubjectAssignments_ClassId] ON [TeacherSubjectAssignments] ([ClassId]);
GO

CREATE INDEX [IX_TeacherSubjectAssignments_SectionId] ON [TeacherSubjectAssignments] ([SectionId]);
GO

CREATE INDEX [IX_TeacherSubjectAssignments_SubjectId] ON [TeacherSubjectAssignments] ([SubjectId]);
GO

CREATE UNIQUE INDEX [IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId] ON [TeacherSubjectAssignments] ([TeacherId], [SubjectId], [ClassId], [SectionId], [AcademicYearId]);
GO

CREATE INDEX [IX_TeacherTimetables_ClassId] ON [TeacherTimetables] ([ClassId]);
GO

CREATE INDEX [IX_TeacherTimetables_SectionId] ON [TeacherTimetables] ([SectionId]);
GO

CREATE INDEX [IX_TeacherTimetables_SubjectId] ON [TeacherTimetables] ([SubjectId]);
GO

CREATE UNIQUE INDEX [IX_TeacherTimetables_TeacherId_DayOfWeek_StartTime] ON [TeacherTimetables] ([TeacherId], [DayOfWeek], [StartTime]);
GO

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]);
GO

CREATE UNIQUE INDEX [IX_Users_UserName] ON [Users] ([UserName]);
GO

CREATE UNIQUE INDEX [IX_UserSessions_SessionId] ON [UserSessions] ([SessionId]);
GO

CREATE INDEX [IX_UserSessions_UserId] ON [UserSessions] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260517141930_AddEmployeeModule', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Notices] ADD [AttachmentPath] nvarchar(260) NULL;
GO

ALTER TABLE [Notices] ADD [IsPublished] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [Announcements] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(260) NOT NULL,
    [Content] nvarchar(2000) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Announcements] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Events] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(160) NOT NULL,
    [Description] nvarchar(4000) NOT NULL,
    [EventDate] datetime2 NOT NULL,
    [EventLocation] nvarchar(160) NULL,
    [CoverImagePath] nvarchar(260) NULL,
    [IsUpcoming] bit NOT NULL,
    [IsPublished] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Events] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Galleries] (
    [Id] int NOT NULL IDENTITY,
    [AlbumName] nvarchar(160) NOT NULL,
    [Description] nvarchar(500) NULL,
    [CoverImagePath] nvarchar(260) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Galleries] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SchoolSettings] (
    [Id] int NOT NULL IDENTITY,
    [SchoolName] nvarchar(160) NOT NULL,
    [ShortName] nvarchar(80) NOT NULL,
    [EIIN] nvarchar(20) NOT NULL,
    [Address] nvarchar(300) NOT NULL,
    [Phone] nvarchar(30) NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [Website] nvarchar(260) NOT NULL,
    [FacebookUrl] nvarchar(260) NULL,
    [YouTubeUrl] nvarchar(260) NULL,
    [LogoPath] nvarchar(260) NULL,
    [FaviconPath] nvarchar(260) NULL,
    [PrincipalName] nvarchar(160) NULL,
    [PrincipalMessage] nvarchar(4000) NULL,
    [PrincipalImagePath] nvarchar(260) NULL,
    [Mission] nvarchar(2000) NULL,
    [Vision] nvarchar(2000) NULL,
    [FooterText] nvarchar(500) NULL,
    [GoogleMapEmbed] nvarchar(1000) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SchoolSettings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Sliders] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(160) NOT NULL,
    [Subtitle] nvarchar(260) NULL,
    [ButtonText] nvarchar(50) NULL,
    [ButtonUrl] nvarchar(260) NULL,
    [ImagePath] nvarchar(260) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_Sliders] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [WebsitePages] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(160) NOT NULL,
    [Slug] nvarchar(160) NOT NULL,
    [Content] nvarchar(max) NOT NULL,
    [MetaTitle] nvarchar(160) NULL,
    [MetaDescription] nvarchar(260) NULL,
    [IsPublished] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_WebsitePages] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [GalleryImages] (
    [Id] int NOT NULL IDENTITY,
    [GalleryId] int NOT NULL,
    [ImagePath] nvarchar(260) NOT NULL,
    [Caption] nvarchar(260) NULL,
    [DisplayOrder] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_GalleryImages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GalleryImages_Galleries_GalleryId] FOREIGN KEY ([GalleryId]) REFERENCES [Galleries] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [Notices] SET [AttachmentPath] = NULL, [IsPublished] = CAST(1 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_GalleryImages_GalleryId] ON [GalleryImages] ([GalleryId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260517143516_AddWebsiteModule', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260517165448_AddWebsitePortalTables', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [EmployeeLeaves];
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'ApprovedByUserId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [LeaveApplications] DROP COLUMN [ApprovedByUserId];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'CreatedBy');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [LeaveApplications] DROP COLUMN [CreatedBy];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'IsDeleted');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [LeaveApplications] DROP COLUMN [IsDeleted];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'UpdatedBy');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [LeaveApplications] DROP COLUMN [UpdatedBy];
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'CreatedBy');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [EmployeeAttendances] DROP COLUMN [CreatedBy];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'IsDeleted');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [EmployeeAttendances] DROP COLUMN [IsDeleted];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'UpdatedAt');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [EmployeeAttendances] DROP COLUMN [UpdatedAt];
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'UpdatedBy');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [EmployeeAttendances] DROP COLUMN [UpdatedBy];
GO

EXEC sp_rename N'[LeaveApplications].[UpdatedAt]', N'ApprovedAt', N'COLUMN';
GO

EXEC sp_rename N'[LeaveApplications].[StudentId]', N'TotalDays', N'COLUMN';
GO

EXEC sp_rename N'[LeaveApplications].[Status]', N'LeaveTypeId', N'COLUMN';
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'ToDate');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [LeaveApplications] ALTER COLUMN [ToDate] datetime2 NOT NULL;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'Reason');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [LeaveApplications] ALTER COLUMN [Reason] nvarchar(500) NULL;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LeaveApplications]') AND [c].[name] = N'FromDate');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [LeaveApplications] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [LeaveApplications] ALTER COLUMN [FromDate] datetime2 NOT NULL;
GO

ALTER TABLE [LeaveApplications] ADD [ApprovalStatus] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [LeaveApplications] ADD [ApprovedBy] nvarchar(100) NULL;
GO

ALTER TABLE [LeaveApplications] ADD [AttachmentPath] nvarchar(260) NULL;
GO

ALTER TABLE [LeaveApplications] ADD [EmployeeId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [LeaveApplications] ADD [Remarks] nvarchar(500) NULL;
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'Status');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [EmployeeAttendances] ALTER COLUMN [Status] int NOT NULL;
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'Remarks');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [EmployeeAttendances] ALTER COLUMN [Remarks] nvarchar(500) NULL;
GO

ALTER TABLE [EmployeeAttendances] ADD [RecordedBy] nvarchar(max) NOT NULL DEFAULT N'';
GO

CREATE TABLE [AttendanceLogs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(100) NOT NULL,
    [Action] nvarchar(100) NOT NULL,
    [EntityName] nvarchar(100) NOT NULL,
    [EntityId] int NOT NULL,
    [Timestamp] datetime2 NOT NULL,
    [IPAddress] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_AttendanceLogs] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AttendanceSettings] (
    [Id] int NOT NULL IDENTITY,
    [SchoolStartTime] time NOT NULL,
    [LateAfterMinutes] int NOT NULL,
    [WorkingDays] nvarchar(100) NOT NULL,
    [AttendanceLockAfterHours] int NOT NULL,
    [AutoAbsentEnabled] bit NOT NULL,
    CONSTRAINT [PK_AttendanceSettings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [LeaveTypes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [MaxDays] int NOT NULL,
    [IsPaid] bit NOT NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_LeaveTypes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [StudentAttendances] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [ClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [AttendanceDate] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [Remarks] nvarchar(500) NULL,
    [RecordedBy] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_StudentAttendances] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentAttendances_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentAttendances_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentAttendances_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_LeaveApplications_EmployeeId] ON [LeaveApplications] ([EmployeeId]);
GO

CREATE INDEX [IX_LeaveApplications_LeaveTypeId] ON [LeaveApplications] ([LeaveTypeId]);
GO

CREATE INDEX [IX_StudentAttendances_ClassId] ON [StudentAttendances] ([ClassId]);
GO

CREATE INDEX [IX_StudentAttendances_SectionId] ON [StudentAttendances] ([SectionId]);
GO

CREATE INDEX [IX_StudentAttendances_StudentId] ON [StudentAttendances] ([StudentId]);
GO

ALTER TABLE [LeaveApplications] ADD CONSTRAINT [FK_LeaveApplications_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [LeaveApplications] ADD CONSTRAINT [FK_LeaveApplications_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260517183028_AddAttendanceAndLeaveTables', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Teachers] DROP CONSTRAINT [FK_Teachers_Users_UserId];
GO

DROP INDEX [IX_Teachers_TeacherNo] ON [Teachers];
GO

DROP INDEX [IX_Teachers_UserId] ON [Teachers];
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'AlternativeNumber');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [Teachers] DROP COLUMN [AlternativeNumber];
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'BloodGroup');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [Teachers] DROP COLUMN [BloodGroup];
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Country');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Country];
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'DateOfBirth');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [Teachers] DROP COLUMN [DateOfBirth];
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Department');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Department];
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Designation');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Designation];
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'EmailAddress');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [Teachers] DROP COLUMN [EmailAddress];
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'FatherName');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [Teachers] DROP COLUMN [FatherName];
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'FullName');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [Teachers] DROP COLUMN [FullName];
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'FullNameBangla');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [Teachers] DROP COLUMN [FullNameBangla];
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Gender');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Gender];
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'JoiningDate');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [Teachers] DROP COLUMN [JoiningDate];
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'MaritalStatus');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Teachers] DROP COLUMN [MaritalStatus];
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'MobileNumber');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [Teachers] DROP COLUMN [MobileNumber];
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'MotherName');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [Teachers] DROP COLUMN [MotherName];
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'NationalIdNo');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [Teachers] DROP COLUMN [NationalIdNo];
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'NationalIdPath');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [Teachers] DROP COLUMN [NationalIdPath];
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PassportNo');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PassportNo];
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PassportPath');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PassportPath];
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PermanentDistrict');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PermanentDistrict];
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PermanentPostOffice');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PermanentPostOffice];
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PermanentThana');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PermanentThana];
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PermanentVillage');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PermanentVillage];
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PresentPostOffice');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PresentPostOffice];
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PresentThana');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PresentThana];
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PresentVillage');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PresentVillage];
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'ProfilePicturePath');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [Teachers] DROP COLUMN [ProfilePicturePath];
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Qualification');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Qualification];
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Religion');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Religion];
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'SpouseName');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [Teachers] DROP COLUMN [SpouseName];
GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'TeacherNo');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [Teachers] DROP COLUMN [TeacherNo];
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'UserId');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [Teachers] DROP COLUMN [UserId];
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Status');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Status];
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Specialization');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Specialization];
GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'PresentDistrict');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [Teachers] DROP COLUMN [PresentDistrict];
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Teachers]') AND [c].[name] = N'Nationality');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [Teachers] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [Teachers] DROP COLUMN [Nationality];
GO

ALTER TABLE [Teachers] ADD [TeachingExperienceYears] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Teachers] ADD [SubjectSpecialization] nvarchar(200) NULL;
GO

ALTER TABLE [Teachers] ADD [TeachingLevel] nvarchar(100) NULL;
GO

ALTER TABLE [Teachers] ADD [TeacherCode] nvarchar(50) NOT NULL DEFAULT N'';
GO

UPDATE Teachers SET TeacherCode = 'MIG-' + CAST(Id AS VARCHAR(10))
GO

ALTER TABLE [Teachers] ADD [IsClassTeacher] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Teachers] ADD [IsExamController] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Teachers] ADD [IsRoutineCoordinator] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Teachers] ADD [Remarks] nvarchar(500) NULL;
GO

CREATE UNIQUE INDEX [IX_Teachers_TeacherCode] ON [Teachers] ([TeacherCode]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518112609_RefactorTeacherToEmployeeExtension', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_TeacherClassAssignments_ClassId] ON [TeacherClassAssignments];
GO

ALTER TABLE [TeacherSubjectAssignments] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [TeacherSubjectAssignments] ADD [Remarks] nvarchar(250) NULL;
GO

ALTER TABLE [TeacherClassAssignments] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [TeacherClassAssignments] ADD [Remarks] nvarchar(250) NULL;
GO

CREATE TABLE [TeacherAcademicProfiles] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NOT NULL,
    [SubjectSpecialization] nvarchar(100) NULL,
    [TeachingLevel] nvarchar(50) NULL,
    [IsExamController] bit NOT NULL,
    [IsRoutineCoordinator] bit NOT NULL,
    [IsClassTeacherEligible] bit NOT NULL,
    [ExperienceYears] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherAcademicProfiles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherAcademicProfiles_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [TeacherAssignmentLogs] (
    [Id] int NOT NULL IDENTITY,
    [TeacherId] int NULL,
    [Action] nvarchar(50) NOT NULL,
    [EntityName] nvarchar(50) NOT NULL,
    [EntityId] int NULL,
    [Timestamp] datetime2 NOT NULL,
    [IPAddress] nvarchar(45) NULL,
    [Remarks] nvarchar(500) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_TeacherAssignmentLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_TeacherAssignmentLogs_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments] ([ClassId], [SectionId], [AcademicYearId]) WHERE [IsActive] = 1;
GO

CREATE INDEX [IX_TeacherAcademicProfiles_TeacherId] ON [TeacherAcademicProfiles] ([TeacherId]);
GO

CREATE INDEX [IX_TeacherAssignmentLogs_TeacherId] ON [TeacherAssignmentLogs] ([TeacherId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518122123_AddTeacherAssignmentExtensions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [TeacherSubjectAssignments] ADD [AssignedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [TeacherSubjectAssignments] ADD [AssignedBy] nvarchar(64) NULL;
GO

ALTER TABLE [TeacherClassAssignments] ADD [AssignedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [TeacherClassAssignments] ADD [AssignedBy] nvarchar(64) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518132206_AddTeacherAssignmentFields', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId] ON [TeacherSubjectAssignments];
GO

DROP INDEX [IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments];
GO

DROP INDEX [IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments];
GO

CREATE UNIQUE INDEX [IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId] ON [TeacherSubjectAssignments] ([TeacherId], [SubjectId], [ClassId], [SectionId], [AcademicYearId]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments] ([ClassId], [SectionId], [AcademicYearId]) WHERE [IsActive] = 1 AND [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments] ([TeacherId], [ClassId], [SectionId], [AcademicYearId]) WHERE [IsDeleted] = 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518133457_FixTeacherAssignmentSoftDelete', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Employees] DROP CONSTRAINT [FK_Employees_Users_UserId];
GO

DROP INDEX [IX_Employees_UserId] ON [Employees];
GO

ALTER TABLE [Users] ADD [EmployeeId] int NULL;
GO

ALTER TABLE [Users] ADD [MustChangePassword] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Employees] ADD [UserId1] int NULL;
GO

ALTER TABLE [Designations] ADD [IsAdministrativeRole] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Designations] ADD [RequiresLogin] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [DesignationRoleMappings] (
    [Id] int NOT NULL IDENTITY,
    [DesignationId] int NOT NULL,
    [RoleId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_DesignationRoleMappings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DesignationRoleMappings_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DesignationRoleMappings_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Description', N'IsDeleted', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([Id], [CreatedAt], [CreatedBy], [Description], [IsDeleted], [Name], [UpdatedAt], [UpdatedBy])
VALUES (20, '2026-01-01T00:00:00.0000000Z', N'system', N'Accounts and finance', CAST(0 AS bit), N'Accountant', NULL, NULL),
(21, '2026-01-01T00:00:00.0000000Z', N'system', N'Library services', CAST(0 AS bit), N'Librarian', NULL, NULL),
(22, '2026-01-01T00:00:00.0000000Z', N'system', N'Lab assistance', CAST(0 AS bit), N'LabAssistant', NULL, NULL),
(23, '2026-01-01T00:00:00.0000000Z', N'system', N'Transport services', CAST(0 AS bit), N'TransportStaff', NULL, NULL),
(24, '2026-01-01T00:00:00.0000000Z', N'system', N'Support and cleaning', CAST(0 AS bit), N'SupportStaff', NULL, NULL),
(25, '2026-01-01T00:00:00.0000000Z', N'system', N'Guardian portal access', CAST(0 AS bit), N'Guardian', NULL, NULL),
(26, '2026-01-01T00:00:00.0000000Z', N'system', N'Administrator', CAST(0 AS bit), N'Admin', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'Description', N'IsDeleted', N'Name', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

UPDATE [Users] SET [EmployeeId] = NULL, [MustChangePassword] = CAST(0 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_Employees_UserId1] ON [Employees] ([UserId1]);
GO

CREATE UNIQUE INDEX [IX_DesignationRoleMappings_DesignationId_RoleId] ON [DesignationRoleMappings] ([DesignationId], [RoleId]);
GO

CREATE INDEX [IX_DesignationRoleMappings_RoleId] ON [DesignationRoleMappings] ([RoleId]);
GO

ALTER TABLE [Employees] ADD CONSTRAINT [FK_Employees_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260518190501_AddEnterpriseRbacAndOnboarding', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Sections_SchoolClassId] ON [Sections];
GO

DROP INDEX [IX_EmployeeAttendances_EmployeeId] ON [EmployeeAttendances];
GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 198 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 199 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 200 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 201 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 202 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 203 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 204 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 205 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 206 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 207 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 208 AND [RoleId] = 2;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 37 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 38 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 39 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 40 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 41 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 42 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 43 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 44 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 45 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 46 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 47 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 48 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 49 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 50 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 51 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 52 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 79 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 80 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 81 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 82 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 83 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 84 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 85 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 86 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 87 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 88 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 89 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 90 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 91 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 181 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 182 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 183 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 184 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 185 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 186 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 187 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 188 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 189 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 199 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 200 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 201 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 202 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 203 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 204 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 205 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 206 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 207 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 208 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 209 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 210 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 211 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 212 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 213 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 214 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 215 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 216 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 217 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 218 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 219 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 220 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 221 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 222 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 223 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 224 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 225 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 226 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 227 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 228 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 229 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 230 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 231 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 232 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 233 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 234 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 235 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 236 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 237 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 238 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 239 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 240 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 241 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 242 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 243 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 244 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 245 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 246 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 247 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 248 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 249 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 250 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 251 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 252 AND [RoleId] = 3;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 46 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 64 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 91 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 100 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 109 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 110 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 118 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 119 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 181 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 226 AND [RoleId] = 5;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 6 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 37 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 38 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 39 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 41 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 42 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 43 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 44 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 45 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 46 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 47 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 48 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 50 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 51 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 52 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 127 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 128 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 129 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 131 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 132 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 133 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 134 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 135 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 136 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 137 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 138 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 140 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 141 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 142 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 143 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 144 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 181 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 182 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 188 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 209 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 210 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 212 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 213 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 214 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 215 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 216 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 217 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 218 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 219 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 221 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 222 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 223 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 224 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 225 AND [RoleId] = 6;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 46 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 91 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 109 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 118 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 119 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 127 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 217 AND [RoleId] = 7;
SELECT @@ROWCOUNT;

GO

ALTER TABLE [Sections] ADD [StudentGroupId] int NULL;
GO

                -- 1. Update Child sections' StudentGroupId, Name, and set ParentSectionId = NULL
                UPDATE Child
                SET 
                    Child.StudentGroupId = CASE 
                        WHEN Parent.Name = 'Science' THEN 1
                        WHEN Parent.Name = 'Business Studies' THEN 2
                        WHEN Parent.Name = 'Humanities' THEN 3
                        ELSE NULL
                    END,
                    Child.Name = LTRIM(RTRIM(REPLACE(Child.Name, Parent.Name, ''))),
                    Child.ParentSectionId = NULL
                FROM Sections Child
                INNER JOIN Sections Parent ON Child.ParentSectionId = Parent.Id
                WHERE Child.ParentSectionId IS NOT NULL;
                -- 2. Delete the parent sections from the Sections table
                DELETE FROM Sections
                WHERE ParentSectionId IS NULL 
                  AND Name IN ('Science', 'Business Studies', 'Humanities')
                  AND SchoolClassId IN (9, 10);
                -- 3. Sync Students.StudentGroupId with Students.Section.StudentGroupId for consistency
                UPDATE S
                SET S.StudentGroupId = SEC.StudentGroupId
                FROM Students S
                INNER JOIN Sections SEC ON S.SectionId = SEC.Id
                WHERE SEC.StudentGroupId IS NOT NULL;
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'TransactionDetails');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [TransactionDetails] nvarchar(max) NULL;
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'ProfilePicturePath');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [ProfilePicturePath] nvarchar(max) NULL;
GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PresentVillage');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var51 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PresentVillage] nvarchar(max) NULL;
GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PresentThana');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PresentThana] nvarchar(max) NULL;
GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PresentPostOffice');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PresentPostOffice] nvarchar(max) NULL;
GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PresentDistrict');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PresentDistrict] nvarchar(max) NULL;
GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PermanentVillage');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PermanentVillage] nvarchar(max) NULL;
GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PermanentThana');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PermanentThana] nvarchar(max) NULL;
GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PermanentPostOffice');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PermanentPostOffice] nvarchar(max) NULL;
GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PermanentDistrict');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var58 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PermanentDistrict] nvarchar(max) NULL;
GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PaymentSlipPath');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var59 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PaymentSlipPath] nvarchar(max) NULL;
GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'PaymentMethod');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [PaymentMethod] nvarchar(max) NULL;
GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'MotherOccupation');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [MotherOccupation] nvarchar(max) NULL;
GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'GuardianOccupation');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var62 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [GuardianOccupation] nvarchar(max) NULL;
GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'GuardianName');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var63 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [GuardianName] nvarchar(max) NULL;
GO

DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'FatherOccupation');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var64 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [FatherOccupation] nvarchar(max) NULL;
GO

DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'BloodGroup');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var65 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [BloodGroup] nvarchar(max) NULL;
GO

DECLARE @var66 sysname;
SELECT @var66 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'BirthCertificatePath');
IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var66 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [BirthCertificatePath] nvarchar(max) NULL;
GO

DECLARE @var67 sysname;
SELECT @var67 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'BirthCertificateNo');
IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var67 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [BirthCertificateNo] nvarchar(max) NULL;
GO

DECLARE @var68 sysname;
SELECT @var68 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'ApplicantEmail');
IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var68 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [ApplicantEmail] nvarchar(max) NULL;
GO

DECLARE @var69 sysname;
SELECT @var69 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AdmissionListResults]') AND [c].[name] = N'AlternativeNumber');
IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [AdmissionListResults] DROP CONSTRAINT [' + @var69 + '];');
ALTER TABLE [AdmissionListResults] ALTER COLUMN [AlternativeNumber] nvarchar(max) NULL;
GO

CREATE TABLE [AttendanceNotificationLogs] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [AttendanceDate] date NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [NotificationType] nvarchar(60) NOT NULL,
    [IsSent] bit NOT NULL,
    [SentAt] datetime2 NULL,
    [ErrorMessage] nvarchar(1000) NULL,
    [NotificationChannel] nvarchar(40) NOT NULL,
    [NotificationStatus] nvarchar(40) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AttendanceNotificationLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AttendanceNotificationLogs_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [Permissions] SET [Action] = N'Read', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Dashboard.Read'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Dashboard.Create'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Dashboard.Edit'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Dashboard.Update'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Dashboard.Delete'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Dashboard.Approve'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Dashboard.Assign'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Dashboard.Publish'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [Code] = N'Dashboard.Export', [Module] = N'Dashboard', [ModuleName] = N'Dashboard'
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Dashboard.Print', [Module] = N'Dashboard', [ModuleName] = N'Dashboard'
WHERE [Id] = 11;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Dashboard.Generate', [Module] = N'Dashboard', [ModuleName] = N'Dashboard'
WHERE [Id] = 12;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Dashboard.Manage', [Module] = N'Dashboard', [ModuleName] = N'Dashboard'
WHERE [Id] = 13;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Users.View'
WHERE [Id] = 14;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Users.Read'
WHERE [Id] = 15;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Users.Create'
WHERE [Id] = 16;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Users.Edit'
WHERE [Id] = 17;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Users.Update'
WHERE [Id] = 18;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Users.Delete', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 19;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Users.Approve', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 20;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Users.Assign', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 21;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Users.Publish', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 22;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Users.Export', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 23;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Users.Print', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 24;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Users.Generate', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 25;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Users.Manage', [Module] = N'Users', [ModuleName] = N'Users'
WHERE [Id] = 26;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Roles.View'
WHERE [Id] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [Code] = N'Roles.Read', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Roles.Create', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Roles.Edit', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 30;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Roles.Update', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 31;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Roles.Delete', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 32;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Roles.Approve', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 33;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Roles.Assign', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 34;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Roles.Publish', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 35;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Roles.Export', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 36;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [Code] = N'Roles.Print', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 37;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanRead] = CAST(1 AS bit), [Code] = N'Roles.Generate', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 38;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Roles.Manage', [Module] = N'Roles', [ModuleName] = N'Roles'
WHERE [Id] = 39;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Permissions.View', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 40;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Permissions.Read', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 41;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Permissions.Create', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 42;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [Code] = N'Permissions.Edit', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 43;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Permissions.Update', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 44;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Permissions.Delete', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 45;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Permissions.Approve', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 46;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Permissions.Assign', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 47;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [Code] = N'Permissions.Publish', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 48;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Permissions.Export', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 49;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Permissions.Print', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 50;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Permissions.Generate', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 51;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Permissions.Manage', [Module] = N'Permissions', [ModuleName] = N'Permissions'
WHERE [Id] = 52;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [Code] = N'Admissions.View', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 53;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Admissions.Read', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 54;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Admissions.Create', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 55;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Admissions.Edit', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 56;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Admissions.Update', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 57;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Admissions.Delete', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 58;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Admissions.Approve', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 59;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Admissions.Assign', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 60;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Admissions.Publish', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 61;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Admissions.Export', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 62;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Admissions.Print', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 63;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [Code] = N'Admissions.Generate', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 64;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Admissions.Manage', [Module] = N'Admissions', [ModuleName] = N'Admissions'
WHERE [Id] = 65;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Students.View', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 66;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Students.Read', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 67;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Students.Create', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 68;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [Code] = N'Students.Edit', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 69;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Students.Update', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 70;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Students.Delete', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 71;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Students.Approve', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 72;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Students.Assign', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 73;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Students.Publish', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 74;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Students.Export', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 75;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Students.Print', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 76;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Students.Generate', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 77;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Students.Manage', [Module] = N'Students', [ModuleName] = N'Students'
WHERE [Id] = 78;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Teachers.View', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 79;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [Code] = N'Teachers.Read', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 80;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Teachers.Create', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 81;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Teachers.Edit', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 82;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Teachers.Update', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 83;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Teachers.Delete', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 84;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Teachers.Approve', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 85;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Teachers.Assign', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 86;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [Code] = N'Teachers.Publish', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 87;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Teachers.Export', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 88;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [Code] = N'Teachers.Print', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 89;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Teachers.Generate', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 90;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Teachers.Manage', [Module] = N'Teachers', [ModuleName] = N'Teachers'
WHERE [Id] = 91;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Classes.View', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 92;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Classes.Read', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 93;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(0 AS bit), [Code] = N'Classes.Create', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 94;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [Code] = N'Classes.Edit', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 95;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Classes.Update', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 96;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Classes.Delete', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 97;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Classes.Approve', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 98;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Classes.Assign', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 99;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Classes.Publish', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 100;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Classes.Export', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 101;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Classes.Print', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 102;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Classes.Generate', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 103;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Classes.Manage', [Module] = N'Classes', [ModuleName] = N'Classes'
WHERE [Id] = 104;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Sections.View', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 105;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Sections.Read', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 106;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Sections.Create', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 107;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Sections.Edit', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 108;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Sections.Update', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 109;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(1 AS bit), [Code] = N'Sections.Delete', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 110;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Sections.Approve', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 111;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Sections.Assign', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 112;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [Code] = N'Sections.Publish', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 113;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Sections.Export', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 114;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Sections.Print', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 115;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [Code] = N'Sections.Generate', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 116;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Sections.Manage', [Module] = N'Sections', [ModuleName] = N'Sections'
WHERE [Id] = 117;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Subjects.View', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 118;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Subjects.Read', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 119;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Subjects.Create', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 120;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Subjects.Edit', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 121;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Subjects.Update', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 122;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Subjects.Delete', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 123;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Subjects.Approve', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 124;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Subjects.Assign', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 125;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Subjects.Publish', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 126;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [Code] = N'Subjects.Export', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 127;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Subjects.Print', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 128;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Subjects.Generate', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 129;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Subjects.Manage', [Module] = N'Subjects', [ModuleName] = N'Subjects'
WHERE [Id] = 130;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Attendance.View', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 131;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Attendance.Read', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 132;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Attendance.Create', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 133;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Attendance.Edit', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 134;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Attendance.Update', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 135;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Attendance.Delete', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 136;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Attendance.Approve', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 137;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Attendance.Assign', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 138;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Attendance.Publish', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 139;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Attendance.Export', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 140;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Attendance.Print', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 141;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Attendance.Generate', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 142;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Attendance.Manage', [Module] = N'Attendance', [ModuleName] = N'Attendance'
WHERE [Id] = 143;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Exams.View', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 144;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [Code] = N'Exams.Read', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 145;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Exams.Create', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 146;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Exams.Edit', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 147;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Exams.Update', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 148;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Exams.Delete', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 149;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Exams.Approve', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 150;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Exams.Assign', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 151;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Exams.Publish', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 152;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Exams.Export', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 153;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [Code] = N'Exams.Print', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 154;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanRead] = CAST(1 AS bit), [Code] = N'Exams.Generate', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 155;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Exams.Manage', [Module] = N'Exams', [ModuleName] = N'Exams'
WHERE [Id] = 156;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Marks.View', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 157;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Marks.Read', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 158;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Marks.Create', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 159;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [Code] = N'Marks.Edit', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 160;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Marks.Update', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 161;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Marks.Delete', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 162;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Marks.Approve', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 163;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Marks.Assign', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 164;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [Code] = N'Marks.Publish', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 165;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Marks.Export', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 166;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Marks.Print', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 167;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Marks.Generate', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 168;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Marks.Manage', [Module] = N'Marks', [ModuleName] = N'Marks'
WHERE [Id] = 169;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [Code] = N'Assignments.View', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 170;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Assignments.Read', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 171;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Assignments.Create', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 172;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Assignments.Edit', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 173;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Assignments.Update', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 174;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Assignments.Delete', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 175;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Assignments.Approve', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 176;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Assignments.Assign', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 177;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Assignments.Publish', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 178;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Assignments.Export', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 179;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Assignments.Print', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 180;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [Code] = N'Assignments.Generate', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 181;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Assignments.Manage', [Module] = N'Assignments', [ModuleName] = N'Assignments'
WHERE [Id] = 182;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Fees.View', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 183;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Fees.Read', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 184;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Fees.Create', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 185;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [Code] = N'Fees.Edit', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 186;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Fees.Update', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 187;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Fees.Delete', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 188;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Fees.Approve', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 189;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Fees.Assign', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 190;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Fees.Publish', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 191;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Fees.Export', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 192;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Fees.Print', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 193;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Fees.Generate', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 194;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Fees.Manage', [Module] = N'Fees', [ModuleName] = N'Fees'
WHERE [Id] = 195;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Payments.View', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 196;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [Code] = N'Payments.Read', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 197;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Payments.Create', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 198;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Payments.Edit', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 199;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Payments.Update', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 200;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Payments.Delete', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 201;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Payments.Approve', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 202;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Payments.Assign', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 203;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [Code] = N'Payments.Publish', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 204;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Payments.Export', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 205;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [Code] = N'Payments.Print', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 206;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Payments.Generate', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 207;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Payments.Manage', [Module] = N'Payments', [ModuleName] = N'Payments'
WHERE [Id] = 208;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Library.View', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 209;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Library.Read', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 210;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(0 AS bit), [Code] = N'Library.Create', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 211;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [Code] = N'Library.Edit', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 212;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Library.Update', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 213;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Library.Delete', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 214;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Library.Approve', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 215;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Library.Assign', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 216;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Library.Publish', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 217;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Library.Export', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 218;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Library.Print', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 219;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Library.Generate', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 220;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Library.Manage', [Module] = N'Library', [ModuleName] = N'Library'
WHERE [Id] = 221;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Transport.View', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 222;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Transport.Read', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 223;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Transport.Create', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 224;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Transport.Edit', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 225;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Transport.Update', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 226;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(1 AS bit), [Code] = N'Transport.Delete', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 227;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Transport.Approve', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 228;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Transport.Assign', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 229;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [Code] = N'Transport.Publish', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 230;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Transport.Export', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 231;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Transport.Print', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 232;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [Code] = N'Transport.Generate', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 233;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Transport.Manage', [Module] = N'Transport', [ModuleName] = N'Transport'
WHERE [Id] = 234;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Health.View', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 235;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Health.Read', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 236;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Health.Create', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 237;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Health.Edit', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 238;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [Code] = N'Health.Update', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 239;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Health.Delete', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 240;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Health.Approve', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 241;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Health.Assign', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 242;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Health.Publish', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 243;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [Code] = N'Health.Export', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 244;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanCreate] = CAST(0 AS bit), [CanRead] = CAST(1 AS bit), [Code] = N'Health.Print', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 245;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Health.Generate', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 246;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Health.Manage', [Module] = N'Health', [ModuleName] = N'Health'
WHERE [Id] = 247;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Notifications.View', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 248;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Notifications.Read', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 249;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Create', [CanCreate] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Notifications.Create', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 250;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Edit', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Notifications.Edit', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 251;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Notifications.Update', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 252;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanRead] = CAST(0 AS bit), [Code] = N'Notifications.Delete', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 253;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [CanCreate] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Notifications.Approve', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 254;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Notifications.Assign', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 255;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Notifications.Publish', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 256;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Notifications.Export', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 257;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Print', [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Notifications.Print', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 258;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Generate', [CanCreate] = CAST(1 AS bit), [CanRead] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Notifications.Generate', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 259;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Manage', [CanCreate] = CAST(1 AS bit), [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Notifications.Manage', [Module] = N'Notifications', [ModuleName] = N'Notifications'
WHERE [Id] = 260;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'View', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Reports.View', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 261;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Read', [Code] = N'Reports.Read', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 262;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Reports.Create', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 263;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Code] = N'Reports.Edit', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 264;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Update', [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Reports.Update', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 265;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Delete', [CanDelete] = CAST(1 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Reports.Delete', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 266;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Approve', [Code] = N'Reports.Approve', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 267;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Assign', [Code] = N'Reports.Assign', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 268;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Publish', [CanRead] = CAST(0 AS bit), [CanUpdate] = CAST(1 AS bit), [Code] = N'Reports.Publish', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 269;
SELECT @@ROWCOUNT;

GO

UPDATE [Permissions] SET [Action] = N'Export', [CanCreate] = CAST(0 AS bit), [CanDelete] = CAST(0 AS bit), [CanUpdate] = CAST(0 AS bit), [Code] = N'Reports.Export', [Module] = N'Reports', [ModuleName] = N'Reports'
WHERE [Id] = 270;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (271, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Reports.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(272, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Reports.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(273, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Reports.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Reports', N'Reports', NULL, NULL),
(274, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(275, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(276, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Settings.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(277, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(278, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(279, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Settings.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(280, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(281, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(282, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Settings.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(283, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(284, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(285, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Settings.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(286, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Settings.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Settings', N'Settings', NULL, NULL),
(287, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(288, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(289, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Academic.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(290, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(291, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(292, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Academic.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(293, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(294, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(295, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Academic.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(296, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(297, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(298, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Academic.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(299, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Academic.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Academic', N'Academic', NULL, NULL),
(300, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(301, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(302, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Admission.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(303, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(304, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(305, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Admission.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(306, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(307, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(308, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Admission.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(309, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(310, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(311, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Admission.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL),
(312, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Admission.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Admission', N'Admission', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (313, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(314, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(315, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Student.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(316, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(317, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(318, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Student.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(319, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(320, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(321, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Student.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(322, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(323, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(324, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Student.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(325, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Student.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Student', N'Student', NULL, NULL),
(326, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(327, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(328, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Exam.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(329, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(330, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(331, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Exam.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(332, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(333, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(334, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Exam.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(335, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(336, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(337, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Exam.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(338, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Exam.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Exam', N'Exam', NULL, NULL),
(339, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(340, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(341, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Result.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(342, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(343, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(344, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Result.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(345, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(346, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(347, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Result.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(348, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(349, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(350, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Result.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(351, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Result.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Result', N'Result', NULL, NULL),
(352, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(353, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(354, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Communication.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (355, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(356, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(357, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Communication.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(358, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(359, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(360, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Communication.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(361, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(362, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(363, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Communication.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(364, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Communication.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Communication', N'Communication', NULL, NULL),
(365, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(366, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(367, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'System.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(368, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(369, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(370, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'System.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(371, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(372, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(373, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'System.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(374, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(375, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(376, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'System.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(377, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'System.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'System', N'System', NULL, NULL),
(378, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(379, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(380, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'AuditLogs.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(381, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(382, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(383, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'AuditLogs.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(384, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(385, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(386, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'AuditLogs.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(387, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(388, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(389, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'AuditLogs.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(390, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'AuditLogs.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'AuditLogs', N'AuditLogs', NULL, NULL),
(391, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FeeStructures.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(392, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FeeStructures.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(393, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FeeStructures.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(394, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FeeStructures.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(395, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FeeStructures.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(396, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FeeStructures.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (397, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FeeStructures.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(398, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FeeStructures.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(399, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FeeStructures.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(400, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FeeStructures.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(401, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FeeStructures.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(402, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FeeStructures.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(403, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'FeeStructures.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FeeStructures', N'FeeStructures', NULL, NULL),
(404, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Invoices.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(405, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Invoices.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(406, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Invoices.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(407, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Invoices.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(408, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Invoices.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(409, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Invoices.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(410, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Invoices.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(411, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Invoices.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(412, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Invoices.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(413, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Invoices.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(414, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Invoices.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(415, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Invoices.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(416, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Invoices.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Invoices', N'Invoices', NULL, NULL),
(417, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Scholarships.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(418, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Scholarships.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(419, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Scholarships.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(420, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Scholarships.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(421, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Scholarships.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(422, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Scholarships.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(423, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Scholarships.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(424, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Scholarships.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(425, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Scholarships.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(426, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Scholarships.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(427, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Scholarships.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(428, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Scholarships.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(429, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Scholarships.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Scholarships', N'Scholarships', NULL, NULL),
(430, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Waivers.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(431, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Waivers.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(432, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Waivers.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(433, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Waivers.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(434, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Waivers.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(435, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Waivers.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(436, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Waivers.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(437, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Waivers.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(438, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Waivers.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (439, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Waivers.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(440, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Waivers.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(441, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Waivers.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(442, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Waivers.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Waivers', N'Waivers', NULL, NULL),
(443, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'StudentDues.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(444, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'StudentDues.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(445, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'StudentDues.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(446, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'StudentDues.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(447, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'StudentDues.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(448, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'StudentDues.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(449, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'StudentDues.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(450, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'StudentDues.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(451, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'StudentDues.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(452, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'StudentDues.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(453, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'StudentDues.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(454, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'StudentDues.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(455, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'StudentDues.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'StudentDues', N'StudentDues', NULL, NULL),
(456, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinancialTransactions.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(457, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinancialTransactions.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(458, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinancialTransactions.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(459, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinancialTransactions.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(460, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinancialTransactions.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(461, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinancialTransactions.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(462, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinancialTransactions.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(463, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinancialTransactions.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(464, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinancialTransactions.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(465, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinancialTransactions.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(466, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinancialTransactions.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(467, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinancialTransactions.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(468, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'FinancialTransactions.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinancialTransactions', N'FinancialTransactions', NULL, NULL),
(469, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceReports.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(470, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceReports.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(471, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinanceReports.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(472, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceReports.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(473, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceReports.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(474, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinanceReports.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(475, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceReports.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(476, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceReports.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(477, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceReports.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(478, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceReports.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(479, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceReports.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(480, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceReports.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (481, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'FinanceReports.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceReports', N'FinanceReports', NULL, NULL),
(482, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceConfiguration.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(483, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceConfiguration.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(484, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinanceConfiguration.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(485, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceConfiguration.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(486, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceConfiguration.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(487, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinanceConfiguration.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(488, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceConfiguration.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(489, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceConfiguration.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(490, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceConfiguration.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(491, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceConfiguration.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(492, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceConfiguration.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(493, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceConfiguration.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(494, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'FinanceConfiguration.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceConfiguration', N'FinanceConfiguration', NULL, NULL),
(495, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceDashboard.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(496, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceDashboard.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(497, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinanceDashboard.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(498, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceDashboard.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(499, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceDashboard.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(500, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'FinanceDashboard.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(501, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceDashboard.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(502, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceDashboard.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(503, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'FinanceDashboard.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(504, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceDashboard.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(505, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceDashboard.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(506, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'FinanceDashboard.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(507, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'FinanceDashboard.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'FinanceDashboard', N'FinanceDashboard', NULL, NULL),
(508, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Receipts.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(509, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Receipts.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(510, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Receipts.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(511, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Receipts.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(512, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Receipts.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(513, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Receipts.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(514, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Receipts.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(515, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Receipts.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(516, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Receipts.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(517, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Receipts.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(518, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Receipts.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(519, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Receipts.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL),
(520, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Receipts.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Receipts', N'Receipts', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (10, 3),
(11, 3),
(12, 3),
(13, 3),
(55, 3),
(56, 3),
(57, 3),
(58, 3),
(59, 3),
(60, 3),
(61, 3),
(62, 3),
(63, 3),
(118, 3),
(119, 3),
(120, 3),
(121, 3),
(122, 3),
(123, 3),
(124, 3),
(125, 3),
(126, 3),
(127, 3),
(128, 3),
(129, 3),
(130, 3),
(131, 3),
(132, 3),
(133, 3),
(134, 3),
(135, 3),
(136, 3),
(137, 3),
(138, 3),
(139, 3),
(140, 3),
(141, 3),
(142, 3),
(143, 3),
(144, 3),
(145, 3),
(146, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (147, 3),
(148, 3),
(149, 3),
(150, 3),
(151, 3),
(152, 3),
(153, 3),
(154, 3),
(155, 3),
(156, 3),
(157, 3),
(158, 3),
(159, 3),
(160, 3),
(161, 3),
(162, 3),
(163, 3),
(164, 3),
(165, 3),
(166, 3),
(167, 3),
(168, 3),
(169, 3),
(261, 3),
(262, 3),
(263, 3),
(264, 3),
(265, 3),
(266, 3),
(267, 3),
(268, 3),
(269, 3),
(270, 3),
(66, 5),
(131, 5),
(133, 5),
(144, 5),
(157, 5),
(159, 5),
(170, 5),
(172, 5),
(261, 5);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (4, 6),
(10, 6),
(11, 6),
(12, 6),
(13, 6),
(55, 6),
(56, 6),
(57, 6),
(59, 6),
(60, 6),
(61, 6),
(62, 6),
(63, 6),
(64, 6),
(65, 6),
(66, 6),
(67, 6),
(68, 6),
(69, 6),
(70, 6),
(72, 6),
(73, 6),
(74, 6),
(75, 6),
(76, 6),
(77, 6),
(78, 6),
(184, 6),
(190, 6),
(191, 6),
(192, 6),
(193, 6),
(194, 6),
(195, 6),
(196, 6),
(197, 6),
(198, 6),
(199, 6),
(200, 6),
(202, 6),
(203, 6),
(204, 6);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (205, 6),
(206, 6),
(207, 6),
(261, 6),
(262, 6),
(263, 6),
(264, 6),
(265, 6),
(267, 6),
(268, 6),
(269, 6),
(270, 6),
(2, 7),
(66, 7),
(131, 7),
(157, 7),
(170, 7),
(183, 7),
(196, 7),
(197, 7),
(248, 7),
(1, 20),
(2, 20),
(196, 20),
(197, 20),
(198, 20),
(199, 20),
(200, 20),
(201, 20),
(202, 20),
(203, 20),
(204, 20),
(205, 20),
(206, 20),
(207, 20),
(208, 20),
(1, 26),
(2, 26),
(3, 26),
(4, 26),
(5, 26),
(6, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (7, 26),
(8, 26),
(9, 26),
(10, 26),
(11, 26),
(12, 26),
(13, 26),
(14, 26),
(15, 26),
(16, 26),
(17, 26),
(18, 26),
(19, 26),
(20, 26),
(21, 26),
(22, 26),
(23, 26),
(24, 26),
(25, 26),
(26, 26),
(27, 26),
(28, 26),
(29, 26),
(30, 26),
(31, 26),
(32, 26),
(33, 26),
(34, 26),
(35, 26),
(36, 26),
(37, 26),
(38, 26),
(39, 26),
(40, 26),
(41, 26),
(42, 26),
(43, 26),
(44, 26),
(45, 26),
(46, 26),
(47, 26),
(48, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (49, 26),
(50, 26),
(51, 26),
(52, 26),
(53, 26),
(54, 26),
(55, 26),
(56, 26),
(57, 26),
(58, 26),
(59, 26),
(60, 26),
(61, 26),
(62, 26),
(63, 26),
(64, 26),
(65, 26),
(66, 26),
(67, 26),
(68, 26),
(69, 26),
(70, 26),
(71, 26),
(72, 26),
(73, 26),
(74, 26),
(75, 26),
(76, 26),
(77, 26),
(78, 26),
(79, 26),
(80, 26),
(81, 26),
(82, 26),
(83, 26),
(84, 26),
(85, 26),
(86, 26),
(87, 26),
(88, 26),
(89, 26),
(90, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (91, 26),
(92, 26),
(93, 26),
(94, 26),
(95, 26),
(96, 26),
(97, 26),
(98, 26),
(99, 26),
(100, 26),
(101, 26),
(102, 26),
(103, 26),
(104, 26),
(105, 26),
(106, 26),
(107, 26),
(108, 26),
(109, 26),
(110, 26),
(111, 26),
(112, 26),
(113, 26),
(114, 26),
(115, 26),
(116, 26),
(117, 26),
(118, 26),
(119, 26),
(120, 26),
(121, 26),
(122, 26),
(123, 26),
(124, 26),
(125, 26),
(126, 26),
(127, 26),
(128, 26),
(129, 26),
(130, 26),
(131, 26),
(132, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (133, 26),
(134, 26),
(135, 26),
(136, 26),
(137, 26),
(138, 26),
(139, 26),
(140, 26),
(141, 26),
(142, 26),
(143, 26),
(144, 26),
(145, 26),
(146, 26),
(147, 26),
(148, 26),
(149, 26),
(150, 26),
(151, 26),
(152, 26),
(153, 26),
(154, 26),
(155, 26),
(156, 26),
(157, 26),
(158, 26),
(159, 26),
(160, 26),
(161, 26),
(162, 26),
(163, 26),
(164, 26),
(165, 26),
(166, 26),
(167, 26),
(168, 26),
(169, 26),
(170, 26),
(171, 26),
(172, 26),
(173, 26),
(174, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (175, 26),
(176, 26),
(177, 26),
(178, 26),
(179, 26),
(180, 26),
(181, 26),
(182, 26),
(183, 26),
(184, 26),
(185, 26),
(186, 26),
(187, 26),
(188, 26),
(189, 26),
(190, 26),
(191, 26),
(192, 26),
(193, 26),
(194, 26),
(195, 26),
(196, 26),
(197, 26),
(198, 26),
(199, 26),
(200, 26),
(201, 26),
(202, 26),
(203, 26),
(204, 26),
(205, 26),
(206, 26),
(207, 26),
(208, 26),
(209, 26),
(210, 26),
(211, 26),
(212, 26),
(213, 26),
(214, 26),
(215, 26),
(216, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (217, 26),
(218, 26),
(219, 26),
(220, 26),
(221, 26),
(222, 26),
(223, 26),
(224, 26),
(225, 26),
(226, 26),
(227, 26),
(228, 26),
(229, 26),
(230, 26),
(231, 26),
(232, 26),
(233, 26),
(234, 26),
(235, 26),
(236, 26),
(237, 26),
(238, 26),
(239, 26),
(240, 26),
(241, 26),
(242, 26),
(243, 26),
(244, 26),
(245, 26),
(246, 26),
(247, 26),
(248, 26),
(249, 26),
(250, 26),
(251, 26),
(252, 26),
(253, 26),
(254, 26),
(255, 26),
(256, 26),
(257, 26),
(258, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (259, 26),
(260, 26),
(261, 26),
(262, 26),
(263, 26),
(264, 26),
(265, 26),
(266, 26),
(267, 26),
(268, 26),
(269, 26),
(270, 26);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;
GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 11;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 12;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 13;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 14;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 15;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 16;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 17;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 18;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 19;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 20;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 21;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 22;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 23;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 24;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 25;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 26;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 30;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 31;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 32;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 33;
SELECT @@ROWCOUNT;

GO

UPDATE [Sections] SET [StudentGroupId] = NULL
WHERE [Id] = 34;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [NameBn] = N'বাংলা'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'English', [NameBn] = N'ইংরেজি'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Mathematics', [NameBn] = N'গণিত'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'General Science', [NameBn] = N'সাধারণ বিজ্ঞান'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Bangladesh and Global Studies', [NameBn] = N'বাংলাদেশ ও বিশ্ব পরিচয়'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Religion and Moral Education', [NameBn] = N'ধর্ম ও নৈতিক শিক্ষা'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Arts and Crafts', [NameBn] = N'চারুকলা'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Physical Education', [NameBn] = N'শারীরিক শিক্ষা'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Bangla 1st Paper', [NameBn] = N'বাংলা ১ম পত্র'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Bangla 2nd Paper', [NameBn] = N'বাংলা ২য় পত্র'
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'English 1st Paper', [NameBn] = N'ইংরেজি ১ম পত্র'
WHERE [Id] = 11;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'English 2nd Paper', [NameBn] = N'ইংরেজি ২য় পত্র'
WHERE [Id] = 12;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Science', [NameBn] = N'বিজ্ঞান'
WHERE [Id] = 13;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Information and Communication Technology', [NameBn] = N'তথ্য ও যোগাযোগ প্রযুক্তি'
WHERE [Id] = 14;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Agriculture Studies', [NameBn] = N'কৃষি শিক্ষা'
WHERE [Id] = 15;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Physics', [NameBn] = N'পদার্থবিজ্ঞান', [SubjectGroup] = N'Science'
WHERE [Id] = 16;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Chemistry', [NameBn] = N'রসায়ন', [SubjectGroup] = N'Science'
WHERE [Id] = 17;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Biology', [NameBn] = N'জীববিজ্ঞান', [SubjectGroup] = N'Science'
WHERE [Id] = 18;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Higher Mathematics', [NameBn] = N'উচ্চতর গণিত', [SubjectGroup] = N'Science'
WHERE [Id] = 19;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Accounting', [NameBn] = N'হিসাববিজ্ঞান', [SubjectGroup] = N'Business Studies'
WHERE [Id] = 20;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Finance and Banking', [NameBn] = N'ফাইন্যান্স', [SubjectGroup] = N'Business Studies'
WHERE [Id] = 21;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Business Entrepreneurship', [NameBn] = N'ব্যবসায় উদ্যোগ', [SubjectGroup] = N'Business Studies'
WHERE [Id] = 22;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'History', [NameBn] = N'ইতিহাস', [SubjectGroup] = N'Humanities'
WHERE [Id] = 23;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Geography and Environment', [NameBn] = N'ভূগোল ও পরিবেশ', [SubjectGroup] = N'Humanities'
WHERE [Id] = 24;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Economics', [NameBn] = N'অর্থনীতি', [SubjectGroup] = N'Humanities'
WHERE [Id] = 25;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Civics', [NameBn] = N'নাগরিকতা', [SubjectGroup] = N'Humanities'
WHERE [Id] = 26;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Career Education', [NameBn] = N'ক্যারিয়ার শিক্ষা'
WHERE [Id] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Physical Education, Health and Sports', [NameBn] = N'শারীরিক শিক্ষা, স্বাস্থ্য ও খেলাধুলা'
WHERE [Id] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Home Science', [NameBn] = N'গার্হস্থ্য বিজ্ঞান'
WHERE [Id] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Islam and Moral Education', [NameBn] = N'ইসলাম ও নৈতিক শিক্ষা'
WHERE [Id] = 30;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Hindu Religion and Moral Education', [NameBn] = N'হিন্দুধর্ম ও নৈতিক শিক্ষা'
WHERE [Id] = 31;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Buddhist Religion and Moral Education', [NameBn] = N'বৌদ্ধধর্ম ও নৈতিক শিক্ষা'
WHERE [Id] = 32;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Name] = N'Christian Religion and Moral Education', [NameBn] = N'খ্রিস্টধর্ম ও নৈতিক শিক্ষা'
WHERE [Id] = 33;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (271, 1),
(272, 1),
(273, 1),
(274, 1),
(275, 1),
(276, 1),
(277, 1),
(278, 1),
(279, 1),
(280, 1),
(281, 1),
(282, 1),
(283, 1),
(284, 1),
(285, 1),
(286, 1),
(287, 1),
(288, 1),
(289, 1),
(290, 1),
(291, 1),
(292, 1),
(293, 1),
(294, 1),
(295, 1),
(296, 1),
(297, 1),
(298, 1),
(299, 1),
(300, 1),
(301, 1),
(302, 1),
(303, 1),
(304, 1),
(305, 1),
(306, 1),
(307, 1),
(308, 1),
(309, 1),
(310, 1),
(311, 1),
(312, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (313, 1),
(314, 1),
(315, 1),
(316, 1),
(317, 1),
(318, 1),
(319, 1),
(320, 1),
(321, 1),
(322, 1),
(323, 1),
(324, 1),
(325, 1),
(326, 1),
(327, 1),
(328, 1),
(329, 1),
(330, 1),
(331, 1),
(332, 1),
(333, 1),
(334, 1),
(335, 1),
(336, 1),
(337, 1),
(338, 1),
(339, 1),
(340, 1),
(341, 1),
(342, 1),
(343, 1),
(344, 1),
(345, 1),
(346, 1),
(347, 1),
(348, 1),
(349, 1),
(350, 1),
(351, 1),
(352, 1),
(353, 1),
(354, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (355, 1),
(356, 1),
(357, 1),
(358, 1),
(359, 1),
(360, 1),
(361, 1),
(362, 1),
(363, 1),
(364, 1),
(365, 1),
(366, 1),
(367, 1),
(368, 1),
(369, 1),
(370, 1),
(371, 1),
(372, 1),
(373, 1),
(374, 1),
(375, 1),
(376, 1),
(377, 1),
(378, 1),
(379, 1),
(380, 1),
(381, 1),
(382, 1),
(383, 1),
(384, 1),
(385, 1),
(386, 1),
(387, 1),
(388, 1),
(389, 1),
(390, 1),
(391, 1),
(392, 1),
(393, 1),
(394, 1),
(395, 1),
(396, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (397, 1),
(398, 1),
(399, 1),
(400, 1),
(401, 1),
(402, 1),
(403, 1),
(404, 1),
(405, 1),
(406, 1),
(407, 1),
(408, 1),
(409, 1),
(410, 1),
(411, 1),
(412, 1),
(413, 1),
(414, 1),
(415, 1),
(416, 1),
(417, 1),
(418, 1),
(419, 1),
(420, 1),
(421, 1),
(422, 1),
(423, 1),
(424, 1),
(425, 1),
(426, 1),
(427, 1),
(428, 1),
(429, 1),
(430, 1),
(431, 1),
(432, 1),
(433, 1),
(434, 1),
(435, 1),
(436, 1),
(437, 1),
(438, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (439, 1),
(440, 1),
(441, 1),
(442, 1),
(443, 1),
(444, 1),
(445, 1),
(446, 1),
(447, 1),
(448, 1),
(449, 1),
(450, 1),
(451, 1),
(452, 1),
(453, 1),
(454, 1),
(455, 1),
(456, 1),
(457, 1),
(458, 1),
(459, 1),
(460, 1),
(461, 1),
(462, 1),
(463, 1),
(464, 1),
(465, 1),
(466, 1),
(467, 1),
(468, 1),
(469, 1),
(470, 1),
(471, 1),
(472, 1),
(473, 1),
(474, 1),
(475, 1),
(476, 1),
(477, 1),
(478, 1),
(479, 1),
(480, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (481, 1),
(482, 1),
(483, 1),
(484, 1),
(485, 1),
(486, 1),
(487, 1),
(488, 1),
(489, 1),
(490, 1),
(491, 1),
(492, 1),
(493, 1),
(494, 1),
(495, 1),
(496, 1),
(497, 1),
(498, 1),
(499, 1),
(500, 1),
(501, 1),
(502, 1),
(503, 1),
(504, 1),
(505, 1),
(506, 1),
(507, 1),
(508, 1),
(509, 1),
(510, 1),
(511, 1),
(512, 1),
(513, 1),
(514, 1),
(515, 1),
(516, 1),
(517, 1),
(518, 1),
(519, 1),
(520, 1),
(271, 2),
(272, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (273, 2),
(274, 2),
(275, 2),
(276, 2),
(277, 2),
(278, 2),
(279, 2),
(280, 2),
(281, 2),
(282, 2),
(283, 2),
(284, 2),
(285, 2),
(286, 2),
(287, 2),
(288, 2),
(289, 2),
(290, 2),
(291, 2),
(292, 2),
(293, 2),
(294, 2),
(295, 2),
(296, 2),
(297, 2),
(298, 2),
(299, 2),
(300, 2),
(301, 2),
(302, 2),
(303, 2),
(304, 2),
(305, 2),
(306, 2),
(307, 2),
(308, 2),
(309, 2),
(310, 2),
(311, 2),
(312, 2),
(313, 2),
(314, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (315, 2),
(316, 2),
(317, 2),
(318, 2),
(319, 2),
(320, 2),
(321, 2),
(322, 2),
(323, 2),
(324, 2),
(325, 2),
(326, 2),
(327, 2),
(328, 2),
(329, 2),
(330, 2),
(331, 2),
(332, 2),
(333, 2),
(334, 2),
(335, 2),
(336, 2),
(337, 2),
(338, 2),
(339, 2),
(340, 2),
(341, 2),
(342, 2),
(343, 2),
(344, 2),
(345, 2),
(346, 2),
(347, 2),
(348, 2),
(349, 2),
(350, 2),
(351, 2),
(352, 2),
(353, 2),
(354, 2),
(355, 2),
(356, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (357, 2),
(358, 2),
(359, 2),
(360, 2),
(361, 2),
(362, 2),
(363, 2),
(364, 2),
(365, 2),
(366, 2),
(367, 2),
(368, 2),
(369, 2),
(370, 2),
(371, 2),
(372, 2),
(373, 2),
(374, 2),
(375, 2),
(376, 2),
(377, 2),
(378, 2),
(379, 2),
(380, 2),
(381, 2),
(382, 2),
(383, 2),
(384, 2),
(385, 2),
(386, 2),
(387, 2),
(388, 2),
(389, 2),
(390, 2),
(404, 2),
(405, 2),
(417, 2),
(418, 2),
(423, 2),
(430, 2),
(431, 2),
(436, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (443, 2),
(444, 2),
(469, 2),
(470, 2),
(478, 2),
(479, 2),
(495, 2),
(496, 2),
(271, 3),
(272, 3),
(273, 3),
(287, 3),
(288, 3),
(289, 3),
(290, 3),
(291, 3),
(292, 3),
(293, 3),
(294, 3),
(295, 3),
(296, 3),
(297, 3),
(298, 3),
(299, 3),
(300, 3),
(301, 3),
(302, 3),
(303, 3),
(304, 3),
(305, 3),
(306, 3),
(307, 3),
(308, 3),
(309, 3),
(310, 3),
(311, 3),
(312, 3),
(313, 3),
(314, 3),
(315, 3),
(316, 3),
(317, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (318, 3),
(319, 3),
(320, 3),
(321, 3),
(322, 3),
(323, 3),
(324, 3),
(325, 3),
(326, 3),
(327, 3),
(328, 3),
(329, 3),
(330, 3),
(331, 3),
(332, 3),
(333, 3),
(334, 3),
(335, 3),
(336, 3),
(337, 3),
(338, 3),
(339, 3),
(340, 3),
(341, 3),
(342, 3),
(343, 3),
(344, 3),
(345, 3),
(346, 3),
(347, 3),
(348, 3),
(349, 3),
(350, 3),
(351, 3),
(352, 3),
(353, 3),
(354, 3),
(355, 3),
(356, 3),
(357, 3),
(358, 3),
(359, 3);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (360, 3),
(361, 3),
(362, 3),
(363, 3),
(364, 3),
(326, 5),
(271, 6),
(272, 6),
(273, 6),
(300, 6),
(301, 6),
(302, 6),
(303, 6),
(304, 6),
(306, 6),
(307, 6),
(308, 6),
(309, 6),
(310, 6),
(311, 6),
(312, 6),
(313, 6),
(314, 6),
(315, 6),
(316, 6),
(317, 6),
(319, 6),
(320, 6),
(321, 6),
(322, 6),
(323, 6),
(324, 6),
(325, 6),
(313, 7),
(404, 7),
(405, 7),
(443, 7),
(444, 7),
(508, 7),
(509, 7),
(517, 7),
(518, 7);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (391, 20),
(392, 20),
(393, 20),
(394, 20),
(395, 20),
(396, 20),
(397, 20),
(398, 20),
(399, 20),
(400, 20),
(401, 20),
(402, 20),
(403, 20),
(404, 20),
(405, 20),
(406, 20),
(407, 20),
(408, 20),
(409, 20),
(410, 20),
(411, 20),
(412, 20),
(413, 20),
(414, 20),
(415, 20),
(416, 20),
(417, 20),
(418, 20),
(419, 20),
(420, 20),
(421, 20),
(422, 20),
(423, 20),
(424, 20),
(425, 20),
(426, 20),
(427, 20),
(428, 20),
(429, 20),
(430, 20),
(431, 20),
(432, 20);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (433, 20),
(434, 20),
(435, 20),
(436, 20),
(437, 20),
(438, 20),
(439, 20),
(440, 20),
(441, 20),
(442, 20),
(443, 20),
(444, 20),
(445, 20),
(446, 20),
(447, 20),
(448, 20),
(449, 20),
(450, 20),
(451, 20),
(452, 20),
(453, 20),
(454, 20),
(455, 20),
(456, 20),
(457, 20),
(458, 20),
(459, 20),
(460, 20),
(461, 20),
(462, 20),
(463, 20),
(464, 20),
(465, 20),
(466, 20),
(467, 20),
(468, 20),
(469, 20),
(470, 20),
(471, 20),
(472, 20),
(473, 20),
(474, 20);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (475, 20),
(476, 20),
(477, 20),
(478, 20),
(479, 20),
(480, 20),
(481, 20),
(482, 20),
(483, 20),
(484, 20),
(485, 20),
(486, 20),
(487, 20),
(488, 20),
(489, 20),
(490, 20),
(491, 20),
(492, 20),
(493, 20),
(494, 20),
(495, 20),
(496, 20),
(497, 20),
(498, 20),
(499, 20),
(500, 20),
(501, 20),
(502, 20),
(503, 20),
(504, 20),
(505, 20),
(506, 20),
(507, 20),
(508, 20),
(509, 20),
(510, 20),
(511, 20),
(512, 20),
(513, 20),
(514, 20),
(515, 20),
(516, 20);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (517, 20),
(518, 20),
(519, 20),
(520, 20),
(271, 26),
(272, 26),
(273, 26),
(274, 26),
(275, 26),
(276, 26),
(277, 26),
(278, 26),
(279, 26),
(280, 26),
(281, 26),
(282, 26),
(283, 26),
(284, 26),
(285, 26),
(286, 26),
(287, 26),
(288, 26),
(289, 26),
(290, 26),
(291, 26),
(292, 26),
(293, 26),
(294, 26),
(295, 26),
(296, 26),
(297, 26),
(298, 26),
(299, 26),
(300, 26),
(301, 26),
(302, 26),
(303, 26),
(304, 26),
(305, 26),
(306, 26),
(307, 26),
(308, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (309, 26),
(310, 26),
(311, 26),
(312, 26),
(313, 26),
(314, 26),
(315, 26),
(316, 26),
(317, 26),
(318, 26),
(319, 26),
(320, 26),
(321, 26),
(322, 26),
(323, 26),
(324, 26),
(325, 26),
(326, 26),
(327, 26),
(328, 26),
(329, 26),
(330, 26),
(331, 26),
(332, 26),
(333, 26),
(334, 26),
(335, 26),
(336, 26),
(337, 26),
(338, 26),
(339, 26),
(340, 26),
(341, 26),
(342, 26),
(343, 26),
(344, 26),
(345, 26),
(346, 26),
(347, 26),
(348, 26),
(349, 26),
(350, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (351, 26),
(352, 26),
(353, 26),
(354, 26),
(355, 26),
(356, 26),
(357, 26),
(358, 26),
(359, 26),
(360, 26),
(361, 26),
(362, 26),
(363, 26),
(364, 26),
(365, 26),
(366, 26),
(367, 26),
(368, 26),
(369, 26),
(370, 26),
(371, 26),
(372, 26),
(373, 26),
(374, 26),
(375, 26),
(376, 26),
(377, 26),
(378, 26),
(379, 26),
(380, 26),
(381, 26),
(382, 26),
(383, 26),
(384, 26),
(385, 26),
(386, 26),
(387, 26),
(388, 26),
(389, 26),
(390, 26),
(391, 26),
(392, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (393, 26),
(394, 26),
(395, 26),
(396, 26),
(397, 26),
(398, 26),
(399, 26),
(400, 26),
(401, 26),
(402, 26),
(403, 26),
(404, 26),
(405, 26),
(406, 26),
(407, 26),
(408, 26),
(409, 26),
(410, 26),
(411, 26),
(412, 26),
(413, 26),
(414, 26),
(415, 26),
(416, 26),
(417, 26),
(418, 26),
(419, 26),
(420, 26),
(421, 26),
(422, 26),
(423, 26),
(424, 26),
(425, 26),
(426, 26),
(427, 26),
(428, 26),
(429, 26),
(430, 26),
(431, 26),
(432, 26),
(433, 26),
(434, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (435, 26),
(436, 26),
(437, 26),
(438, 26),
(439, 26),
(440, 26),
(441, 26),
(442, 26),
(443, 26),
(444, 26),
(445, 26),
(446, 26),
(447, 26),
(448, 26),
(449, 26),
(450, 26),
(451, 26),
(452, 26),
(453, 26),
(454, 26),
(455, 26),
(456, 26),
(457, 26),
(458, 26),
(459, 26),
(460, 26),
(461, 26),
(462, 26),
(463, 26),
(464, 26),
(465, 26),
(466, 26),
(467, 26),
(468, 26),
(469, 26),
(470, 26),
(471, 26),
(472, 26),
(473, 26),
(474, 26),
(475, 26),
(476, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (477, 26),
(478, 26),
(479, 26),
(480, 26),
(481, 26),
(482, 26),
(483, 26),
(484, 26),
(485, 26),
(486, 26),
(487, 26),
(488, 26),
(489, 26),
(490, 26),
(497, 26),
(498, 26),
(499, 26),
(500, 26),
(501, 26),
(502, 26),
(503, 26),
(504, 26),
(505, 26),
(506, 26),
(507, 26),
(508, 26),
(509, 26),
(510, 26),
(511, 26),
(512, 26),
(513, 26),
(514, 26),
(515, 26),
(516, 26),
(517, 26),
(518, 26),
(519, 26),
(520, 26);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;
GO

CREATE INDEX [IX_Sections_StudentGroupId] ON [Sections] ([StudentGroupId]);
GO

CREATE UNIQUE INDEX [IX_EmployeeAttendances_EmployeeId_AttendanceDate] ON [EmployeeAttendances] ([EmployeeId], [AttendanceDate]);
GO

CREATE UNIQUE INDEX [IX_Attendance_StudentId_AttendanceDate] ON [Attendance] ([StudentId], [AttendanceDate]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel] ON [AttendanceNotificationLogs] ([StudentId], [AttendanceDate], [NotificationType], [NotificationChannel]) WHERE [IsDeleted] = 0;
GO

ALTER TABLE [Attendance] ADD CONSTRAINT [FK_Attendance_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Sections] ADD CONSTRAINT [FK_Sections_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260521060027_FixSectionGroupConflict', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [TeacherTimetables] ADD [GroupId] int NULL;
GO

ALTER TABLE [TeacherSubjectAssignments] ADD [GroupId] int NULL;
GO

ALTER TABLE [TeacherClassAssignments] ADD [GroupId] int NULL;
GO

CREATE INDEX [IX_TeacherTimetables_GroupId] ON [TeacherTimetables] ([GroupId]);
GO

CREATE INDEX [IX_TeacherSubjectAssignments_GroupId] ON [TeacherSubjectAssignments] ([GroupId]);
GO

CREATE INDEX [IX_TeacherClassAssignments_GroupId] ON [TeacherClassAssignments] ([GroupId]);
GO

ALTER TABLE [TeacherClassAssignments] ADD CONSTRAINT [FK_TeacherClassAssignments_StudentGroups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [TeacherSubjectAssignments] ADD CONSTRAINT [FK_TeacherSubjectAssignments_StudentGroups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [TeacherTimetables] ADD CONSTRAINT [FK_TeacherTimetables_StudentGroups_GroupId] FOREIGN KEY ([GroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260521074757_AddGroupIdToAssignments', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260522115005_AddMissingUserColumns', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'IsPaid', N'MaxDays', N'Name') AND [object_id] = OBJECT_ID(N'[LeaveTypes]'))
    SET IDENTITY_INSERT [LeaveTypes] ON;
INSERT INTO [LeaveTypes] ([Id], [IsActive], [IsPaid], [MaxDays], [Name])
VALUES (1, CAST(1 AS bit), CAST(1 AS bit), 14, N'Sick Leave'),
(2, CAST(1 AS bit), CAST(1 AS bit), 10, N'Casual Leave'),
(3, CAST(1 AS bit), CAST(1 AS bit), 180, N'Maternity Leave'),
(4, CAST(1 AS bit), CAST(1 AS bit), 15, N'Paternity Leave'),
(5, CAST(1 AS bit), CAST(0 AS bit), 30, N'Unpaid Leave');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'IsPaid', N'MaxDays', N'Name') AND [object_id] = OBJECT_ID(N'[LeaveTypes]'))
    SET IDENTITY_INSERT [LeaveTypes] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260522201352_SeedLeaveTypes2', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [EmployeeInvitations] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(120) NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [Mobile] nvarchar(30) NOT NULL,
    [InvitationToken] nvarchar(100) NOT NULL,
    [DepartmentId] int NOT NULL,
    [DesignationId] int NOT NULL,
    [JoiningDate] datetime2 NOT NULL,
    [EmploymentType] nvarchar(50) NOT NULL,
    [Status] nvarchar(20) NOT NULL,
    [IsTeachingStaff] bit NOT NULL,
    [Remarks] nvarchar(500) NULL,
    [ExpiresAt] datetime2 NOT NULL,
    [IsUsed] bit NOT NULL,
    [IsApproved] bit NOT NULL,
    [OnboardedAt] datetime2 NULL,
    [CreatedEmployeeId] int NULL,
    [InvitationStatus] nvarchar(50) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmployeeInvitations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmployeeInvitations_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_EmployeeInvitations_Designations_DesignationId] FOREIGN KEY ([DesignationId]) REFERENCES [Designations] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_EmployeeInvitations_DepartmentId] ON [EmployeeInvitations] ([DepartmentId]);
GO

CREATE INDEX [IX_EmployeeInvitations_DesignationId] ON [EmployeeInvitations] ([DesignationId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260522204203_AddEmployeeInvitations', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [EmployeeInvitations] ADD [CompletedAt] datetime2 NULL;
GO

ALTER TABLE [EmployeeInvitations] ADD [InvitationCode] nvarchar(30) NOT NULL DEFAULT N'';
GO

ALTER TABLE [EmployeeInvitations] ADD [OpenedAt] datetime2 NULL;
GO

ALTER TABLE [EmployeeInvitations] ADD [SentAt] datetime2 NULL;
GO

ALTER TABLE [EmployeeInvitations] ADD [OnboardedAt] datetime2 NULL;
GO

ALTER TABLE [EmployeeInvitations] ADD [CreatedEmployeeId] int NULL;
GO

ALTER TABLE [EmployeeInvitations] ADD [IsUsed] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [EmployeeInvitations] ADD [IsApproved] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [EmployeeInvitations] ADD [InvitationStatus] nvarchar(50) NOT NULL DEFAULT N'Started';
GO

CREATE UNIQUE INDEX [IX_EmployeeInvitations_InvitationCode] ON [EmployeeInvitations] ([InvitationCode]);
GO

CREATE UNIQUE INDEX [IX_EmployeeInvitations_InvitationToken] ON [EmployeeInvitations] ([InvitationToken]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260523091255_AddMissingInvitationFields', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AttendanceRevisions] (
    [Id] int NOT NULL IDENTITY,
    [AttendanceRecordId] int NOT NULL,
    [StudentId] int NOT NULL,
    [AttendanceDate] date NOT NULL,
    [OldStatus] nvarchar(max) NOT NULL,
    [NewStatus] nvarchar(max) NOT NULL,
    [Reason] nvarchar(512) NULL,
    [ChangedBy] nvarchar(128) NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AttendanceRevisions] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AttendanceSessions] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [SectionId] int NOT NULL,
    [StudentGroupId] int NULL,
    [AttendanceDate] date NOT NULL,
    [Status] int NOT NULL,
    [LockedBy] nvarchar(256) NULL,
    [LockedAt] datetime2 NULL,
    [Notes] nvarchar(512) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AttendanceSessions] PRIMARY KEY ([Id])
);
GO

CREATE UNIQUE INDEX [IX_AttendanceSessions_SchoolClassId_SectionId_StudentGroupId_AttendanceDate] ON [AttendanceSessions] ([SchoolClassId], [SectionId], [StudentGroupId], [AttendanceDate]) WHERE [IsDeleted] = 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260525075627_attendecesession', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [EmployeeAttendances] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [EmployeeAttendances] ADD [UpdatedAt] datetime2 NULL;
GO

ALTER TABLE [EmployeeAttendances] ADD [UpdatedBy] nvarchar(64) NULL;
GO

DROP INDEX [IX_EmployeeAttendances_EmployeeId_AttendanceDate] ON [EmployeeAttendances];
GO

CREATE UNIQUE INDEX [IX_EmployeeAttendances_EmployeeId_AttendanceDate] ON [EmployeeAttendances] ([EmployeeId], [AttendanceDate]) WHERE [IsDeleted] = 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260526_UpdateEmployeeAttendanceToBaseEntity', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_EmployeeAttendances_EmployeeId_AttendanceDate] ON [EmployeeAttendances];
GO

DECLARE @var70 sysname;
SELECT @var70 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[EmployeeAttendances]') AND [c].[name] = N'CreatedBy');
IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [EmployeeAttendances] DROP CONSTRAINT [' + @var70 + '];');
GO

ALTER TABLE [AttendanceSessions] ADD [ApprovedAt] datetime2 NULL;
GO

ALTER TABLE [AttendanceSessions] ADD [ApprovedBy] nvarchar(256) NULL;
GO

ALTER TABLE [AttendanceSessions] ADD [RevisedAt] datetime2 NULL;
GO

ALTER TABLE [AttendanceSessions] ADD [RevisedBy] nvarchar(256) NULL;
GO

ALTER TABLE [AttendanceSessions] ADD [SubmittedAt] datetime2 NULL;
GO

ALTER TABLE [AttendanceSessions] ADD [SubmittedBy] nvarchar(256) NULL;
GO

CREATE UNIQUE INDEX [IX_EmployeeAttendances_EmployeeId_AttendanceDate] ON [EmployeeAttendances] ([EmployeeId], [AttendanceDate]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260527153923_EnhanceAttendanceSessionWorkflowTracking', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Guardians] DROP CONSTRAINT [FK_Guardians_Students_StudentId];
GO

DROP INDEX [IX_Guardians_StudentId] ON [Guardians];
GO

DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Guardians]') AND [c].[name] = N'Name');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [Guardians] DROP CONSTRAINT [' + @var71 + '];');
ALTER TABLE [Guardians] DROP COLUMN [Name];
GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Guardians]') AND [c].[name] = N'Relation');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [Guardians] DROP CONSTRAINT [' + @var72 + '];');
ALTER TABLE [Guardians] DROP COLUMN [Relation];
GO

EXEC sp_rename N'[Guardians].[StudentId]', N'Status', N'COLUMN';
GO

EXEC sp_rename N'[Guardians].[Phone]', N'MobileNumber', N'COLUMN';
GO

ALTER TABLE [Guardians] ADD [AlternativeMobileNumber] nvarchar(30) NULL;
GO

ALTER TABLE [Guardians] ADD [DateOfBirth] datetime2 NULL;
GO

ALTER TABLE [Guardians] ADD [EmergencyContactName] nvarchar(100) NULL;
GO

ALTER TABLE [Guardians] ADD [EmergencyContactNumber] nvarchar(30) NULL;
GO

ALTER TABLE [Guardians] ADD [EmployerName] nvarchar(150) NULL;
GO

ALTER TABLE [Guardians] ADD [FirstName] nvarchar(80) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Guardians] ADD [FullName] nvarchar(160) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Guardians] ADD [Gender] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Guardians] ADD [GuardianCode] nvarchar(30) NULL;
GO

ALTER TABLE [Guardians] ADD [IsPrimaryGuardian] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Guardians] ADD [LastName] nvarchar(80) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Guardians] ADD [MonthlyIncome] decimal(18,2) NULL;
GO

ALTER TABLE [Guardians] ADD [NationalId] nvarchar(50) NULL;
GO

ALTER TABLE [Guardians] ADD [PassportNumber] nvarchar(50) NULL;
GO

ALTER TABLE [Guardians] ADD [PermanentAddress] nvarchar(250) NULL;
GO

ALTER TABLE [Guardians] ADD [PhotoPath] nvarchar(260) NULL;
GO

ALTER TABLE [Guardians] ADD [PortalAccessEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Guardians] ADD [PresentAddress] nvarchar(250) NULL;
GO

ALTER TABLE [Guardians] ADD [RelationType] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Guardians] ADD [Remarks] nvarchar(500) NULL;
GO

ALTER TABLE [Guardians] ADD [UserId] int NULL;
GO

ALTER TABLE [AttendanceNotificationLogs] ADD [GuardianId] int NULL;
GO

ALTER TABLE [Admissions] ADD [LinkedGuardianId] int NULL;
GO

CREATE TABLE [GuardianNotificationLogs] (
    [Id] int NOT NULL IDENTITY,
    [GuardianId] int NOT NULL,
    [Channel] nvarchar(50) NOT NULL,
    [Recipient] nvarchar(160) NOT NULL,
    [MessageContent] nvarchar(max) NOT NULL,
    [IsSent] bit NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    [SentAt] datetime2 NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_GuardianNotificationLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GuardianNotificationLogs_Guardians_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [GuardianNotifications] (
    [Id] int NOT NULL IDENTITY,
    [GuardianId] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Message] nvarchar(max) NOT NULL,
    [IsRead] bit NOT NULL,
    [ReadAt] datetime2 NULL,
    [Category] nvarchar(50) NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_GuardianNotifications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_GuardianNotifications_Guardians_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentGuardians] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [GuardianId] int NOT NULL,
    [Relationship] int NOT NULL,
    [IsPrimaryGuardian] bit NOT NULL,
    [ReceivesAttendanceNotifications] bit NOT NULL,
    [ReceivesResultNotifications] bit NOT NULL,
    [ReceivesFeeNotifications] bit NOT NULL,
    [ReceivesSMS] bit NOT NULL,
    [ReceivesEmail] bit NOT NULL,
    [ReceivesWhatsApp] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_StudentGuardians] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentGuardians_Guardians_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentGuardians_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [StudentLeaveApplications] (
    [Id] int NOT NULL IDENTITY,
    [StudentId] int NOT NULL,
    [GuardianId] int NOT NULL,
    [LeaveTypeId] int NOT NULL,
    [FromDate] datetime2 NOT NULL,
    [ToDate] datetime2 NOT NULL,
    [TotalDays] int NOT NULL,
    [Reason] nvarchar(500) NULL,
    [AttachmentPath] nvarchar(260) NULL,
    [ApprovalStatus] int NOT NULL,
    [ApprovedBy] nvarchar(100) NULL,
    [ApprovedAt] datetime2 NULL,
    [Remarks] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_StudentLeaveApplications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_StudentLeaveApplications_Guardians_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentLeaveApplications_LeaveTypes_LeaveTypeId] FOREIGN KEY ([LeaveTypeId]) REFERENCES [LeaveTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentLeaveApplications_Students_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Students] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [Admissions] SET [LinkedGuardianId] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Guardians] SET [AlternativeMobileNumber] = NULL, [DateOfBirth] = NULL, [EmergencyContactName] = NULL, [EmergencyContactNumber] = NULL, [EmployerName] = NULL, [FirstName] = N'Guardian', [FullName] = N'Guardian One', [Gender] = N'Male', [GuardianCode] = N'GRD-00001', [IsPrimaryGuardian] = CAST(0 AS bit), [LastName] = N'One', [MonthlyIncome] = NULL, [NationalId] = NULL, [PassportNumber] = NULL, [PermanentAddress] = NULL, [PhotoPath] = NULL, [PortalAccessEnabled] = CAST(0 AS bit), [PresentAddress] = NULL, [RelationType] = 1, [Remarks] = NULL, [UserId] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Guardians] SET [AlternativeMobileNumber] = NULL, [DateOfBirth] = NULL, [EmergencyContactName] = NULL, [EmergencyContactNumber] = NULL, [EmployerName] = NULL, [FirstName] = N'Guardian', [FullName] = N'Guardian Two', [Gender] = N'Female', [GuardianCode] = N'GRD-00002', [IsPrimaryGuardian] = CAST(0 AS bit), [LastName] = N'Two', [MonthlyIncome] = NULL, [NationalId] = NULL, [PassportNumber] = NULL, [PermanentAddress] = NULL, [PhotoPath] = NULL, [PortalAccessEnabled] = CAST(0 AS bit), [PresentAddress] = NULL, [RelationType] = 2, [Remarks] = NULL, [Status] = 1, [UserId] = NULL
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'GuardianId', N'IsDeleted', N'IsPrimaryGuardian', N'ReceivesAttendanceNotifications', N'ReceivesEmail', N'ReceivesFeeNotifications', N'ReceivesResultNotifications', N'ReceivesSMS', N'ReceivesWhatsApp', N'Relationship', N'StudentId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[StudentGuardians]'))
    SET IDENTITY_INSERT [StudentGuardians] ON;
INSERT INTO [StudentGuardians] ([Id], [CreatedAt], [CreatedBy], [GuardianId], [IsDeleted], [IsPrimaryGuardian], [ReceivesAttendanceNotifications], [ReceivesEmail], [ReceivesFeeNotifications], [ReceivesResultNotifications], [ReceivesSMS], [ReceivesWhatsApp], [Relationship], [StudentId], [UpdatedAt], [UpdatedBy])
VALUES (1, '2026-01-01T00:00:00.0000000Z', N'system', 1, CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), 1, 1, NULL, NULL),
(2, '2026-01-01T00:00:00.0000000Z', N'system', 2, CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(0 AS bit), 2, 2, NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'CreatedBy', N'GuardianId', N'IsDeleted', N'IsPrimaryGuardian', N'ReceivesAttendanceNotifications', N'ReceivesEmail', N'ReceivesFeeNotifications', N'ReceivesResultNotifications', N'ReceivesSMS', N'ReceivesWhatsApp', N'Relationship', N'StudentId', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[StudentGuardians]'))
    SET IDENTITY_INSERT [StudentGuardians] OFF;
GO

UPDATE Guardians SET GuardianCode = 'GRD-' + RIGHT('00000' + CAST(Id AS varchar(5)),5) WHERE GuardianCode IS NULL OR GuardianCode = ''
GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Guardians]') AND [c].[name] = N'GuardianCode');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [Guardians] DROP CONSTRAINT [' + @var73 + '];');
ALTER TABLE [Guardians] ALTER COLUMN [GuardianCode] nvarchar(30) NOT NULL;
GO

CREATE UNIQUE INDEX [IX_Guardians_GuardianCode] ON [Guardians] ([GuardianCode]);
GO

CREATE UNIQUE INDEX [IX_Guardians_MobileNumber] ON [Guardians] ([MobileNumber]);
GO

CREATE INDEX [IX_AttendanceNotificationLogs_GuardianId] ON [AttendanceNotificationLogs] ([GuardianId]);
GO

CREATE INDEX [IX_Admissions_LinkedGuardianId] ON [Admissions] ([LinkedGuardianId]);
GO

CREATE INDEX [IX_GuardianNotificationLogs_GuardianId] ON [GuardianNotificationLogs] ([GuardianId]);
GO

CREATE INDEX [IX_GuardianNotifications_GuardianId] ON [GuardianNotifications] ([GuardianId]);
GO

CREATE INDEX [IX_StudentGuardians_GuardianId] ON [StudentGuardians] ([GuardianId]);
GO

CREATE UNIQUE INDEX [IX_StudentGuardians_StudentId_GuardianId] ON [StudentGuardians] ([StudentId], [GuardianId]);
GO

CREATE INDEX [IX_StudentLeaveApplications_GuardianId] ON [StudentLeaveApplications] ([GuardianId]);
GO

CREATE INDEX [IX_StudentLeaveApplications_LeaveTypeId] ON [StudentLeaveApplications] ([LeaveTypeId]);
GO

CREATE INDEX [IX_StudentLeaveApplications_StudentId] ON [StudentLeaveApplications] ([StudentId]);
GO

ALTER TABLE [Admissions] ADD CONSTRAINT [FK_Admissions_Guardians_LinkedGuardianId] FOREIGN KEY ([LinkedGuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [AttendanceNotificationLogs] ADD CONSTRAINT [FK_AttendanceNotificationLogs_Guardians_GuardianId] FOREIGN KEY ([GuardianId]) REFERENCES [Guardians] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260602192030_Addgardianid', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [StoredProcedureDeploymentHistories] (
    [Id] int NOT NULL IDENTITY,
    [ProcedureName] nvarchar(max) NOT NULL,
    [FileName] nvarchar(max) NOT NULL,
    [Hash] nvarchar(max) NOT NULL,
    [DeployedAt] datetime2 NOT NULL,
    [Status] nvarchar(max) NOT NULL,
    [ErrorMessage] nvarchar(max) NULL,
    CONSTRAINT [PK_StoredProcedureDeploymentHistories] PRIMARY KEY ([Id])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (1, 25),
(2, 25),
(3, 25),
(4, 25),
(5, 25),
(6, 25),
(7, 25),
(8, 25),
(9, 25),
(10, 25),
(11, 25),
(12, 25),
(13, 25),
(14, 25),
(15, 25),
(16, 25),
(17, 25),
(18, 25),
(19, 25),
(20, 25),
(21, 25),
(22, 25),
(23, 25),
(24, 25),
(25, 25),
(26, 25),
(27, 25),
(28, 25),
(29, 25),
(30, 25),
(31, 25),
(32, 25),
(33, 25),
(34, 25),
(35, 25),
(36, 25),
(37, 25),
(38, 25),
(39, 25),
(40, 25),
(41, 25),
(42, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (43, 25),
(44, 25),
(45, 25),
(46, 25),
(47, 25),
(48, 25),
(49, 25),
(50, 25),
(51, 25),
(52, 25),
(53, 25),
(54, 25),
(55, 25),
(56, 25),
(57, 25),
(58, 25),
(59, 25),
(60, 25),
(61, 25),
(62, 25),
(63, 25),
(64, 25),
(65, 25),
(66, 25),
(67, 25),
(68, 25),
(69, 25),
(70, 25),
(71, 25),
(72, 25),
(73, 25),
(74, 25),
(75, 25),
(76, 25),
(77, 25),
(78, 25),
(79, 25),
(80, 25),
(81, 25),
(82, 25),
(83, 25),
(84, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (85, 25),
(86, 25),
(87, 25),
(88, 25),
(89, 25),
(90, 25),
(91, 25),
(92, 25),
(93, 25),
(94, 25),
(95, 25),
(96, 25),
(97, 25),
(98, 25),
(99, 25),
(100, 25),
(101, 25),
(102, 25),
(103, 25),
(104, 25),
(105, 25),
(106, 25),
(107, 25),
(108, 25),
(109, 25),
(110, 25),
(111, 25),
(112, 25),
(113, 25),
(114, 25),
(115, 25),
(116, 25),
(117, 25),
(118, 25),
(119, 25),
(120, 25),
(121, 25),
(122, 25),
(123, 25),
(124, 25),
(125, 25),
(126, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (127, 25),
(128, 25),
(129, 25),
(130, 25),
(131, 25),
(132, 25),
(133, 25),
(134, 25),
(135, 25),
(136, 25),
(137, 25),
(138, 25),
(139, 25),
(140, 25),
(141, 25),
(142, 25),
(143, 25),
(144, 25),
(145, 25),
(146, 25),
(147, 25),
(148, 25),
(149, 25),
(150, 25),
(151, 25),
(152, 25),
(153, 25),
(154, 25),
(155, 25),
(156, 25),
(157, 25),
(158, 25),
(159, 25),
(160, 25),
(161, 25),
(162, 25),
(163, 25),
(164, 25),
(165, 25),
(166, 25),
(167, 25),
(168, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (169, 25),
(170, 25),
(171, 25),
(172, 25),
(173, 25),
(174, 25),
(175, 25),
(176, 25),
(177, 25),
(178, 25),
(179, 25),
(180, 25),
(181, 25),
(182, 25),
(183, 25),
(184, 25),
(185, 25),
(186, 25),
(187, 25),
(188, 25),
(189, 25),
(190, 25),
(191, 25),
(192, 25),
(193, 25),
(194, 25),
(195, 25),
(196, 25),
(197, 25),
(198, 25),
(199, 25),
(200, 25),
(201, 25),
(202, 25),
(203, 25),
(204, 25),
(205, 25),
(206, 25),
(207, 25),
(208, 25),
(209, 25),
(210, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (211, 25),
(212, 25),
(213, 25),
(214, 25),
(215, 25),
(216, 25),
(217, 25),
(218, 25),
(219, 25),
(220, 25),
(221, 25),
(222, 25),
(223, 25),
(224, 25),
(225, 25),
(226, 25),
(227, 25),
(228, 25),
(229, 25),
(230, 25),
(231, 25),
(232, 25),
(233, 25),
(234, 25),
(235, 25),
(236, 25),
(237, 25),
(238, 25),
(239, 25),
(240, 25),
(241, 25),
(242, 25),
(243, 25),
(244, 25),
(245, 25),
(246, 25),
(247, 25),
(248, 25),
(249, 25),
(250, 25),
(251, 25),
(252, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (253, 25),
(254, 25),
(255, 25),
(256, 25),
(257, 25),
(258, 25),
(259, 25),
(260, 25),
(261, 25),
(262, 25),
(263, 25),
(264, 25),
(265, 25),
(266, 25),
(267, 25),
(268, 25),
(269, 25),
(270, 25),
(271, 25),
(272, 25),
(273, 25),
(274, 25),
(275, 25),
(276, 25),
(277, 25),
(278, 25),
(279, 25),
(280, 25),
(281, 25),
(282, 25),
(283, 25),
(284, 25),
(285, 25),
(286, 25),
(287, 25),
(288, 25),
(289, 25),
(290, 25),
(291, 25),
(292, 25),
(293, 25),
(294, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (295, 25),
(296, 25),
(297, 25),
(298, 25),
(299, 25),
(300, 25),
(301, 25),
(302, 25),
(303, 25),
(304, 25),
(305, 25),
(306, 25),
(307, 25),
(308, 25),
(309, 25),
(310, 25),
(311, 25),
(312, 25),
(313, 25),
(314, 25),
(315, 25),
(316, 25),
(317, 25),
(318, 25),
(319, 25),
(320, 25),
(321, 25),
(322, 25),
(323, 25),
(324, 25),
(325, 25),
(326, 25),
(327, 25),
(328, 25),
(329, 25),
(330, 25),
(331, 25),
(332, 25),
(333, 25),
(334, 25),
(335, 25),
(336, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (337, 25),
(338, 25),
(339, 25),
(340, 25),
(341, 25),
(342, 25),
(343, 25),
(344, 25),
(345, 25),
(346, 25),
(347, 25),
(348, 25),
(349, 25),
(350, 25),
(351, 25),
(352, 25),
(353, 25),
(354, 25),
(355, 25),
(356, 25),
(357, 25),
(358, 25),
(359, 25),
(360, 25),
(361, 25),
(362, 25),
(363, 25),
(364, 25),
(365, 25),
(366, 25),
(367, 25),
(368, 25),
(369, 25),
(370, 25),
(371, 25),
(372, 25),
(373, 25),
(374, 25),
(375, 25),
(376, 25),
(377, 25),
(378, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (379, 25),
(380, 25),
(381, 25),
(382, 25),
(383, 25),
(384, 25),
(385, 25),
(386, 25),
(387, 25),
(388, 25),
(389, 25),
(390, 25),
(391, 25),
(392, 25),
(393, 25),
(394, 25),
(395, 25),
(396, 25),
(397, 25),
(398, 25),
(399, 25),
(400, 25),
(401, 25),
(402, 25),
(403, 25),
(404, 25),
(405, 25),
(406, 25),
(407, 25),
(408, 25),
(409, 25),
(410, 25),
(411, 25),
(412, 25),
(413, 25),
(414, 25),
(415, 25),
(416, 25),
(417, 25),
(418, 25),
(419, 25),
(420, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (421, 25),
(422, 25),
(423, 25),
(424, 25),
(425, 25),
(426, 25),
(427, 25),
(428, 25),
(429, 25),
(430, 25),
(431, 25),
(432, 25),
(433, 25),
(434, 25),
(435, 25),
(436, 25),
(437, 25),
(438, 25),
(439, 25),
(440, 25),
(441, 25),
(442, 25),
(443, 25),
(444, 25),
(445, 25),
(446, 25),
(447, 25),
(448, 25),
(449, 25),
(450, 25),
(451, 25),
(452, 25),
(453, 25),
(454, 25),
(455, 25),
(456, 25),
(457, 25),
(458, 25),
(459, 25),
(460, 25),
(461, 25),
(462, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (463, 25),
(464, 25),
(465, 25),
(466, 25),
(467, 25),
(468, 25),
(469, 25),
(470, 25),
(471, 25),
(472, 25),
(473, 25),
(474, 25),
(475, 25),
(476, 25),
(477, 25),
(478, 25),
(479, 25),
(480, 25),
(481, 25),
(482, 25),
(483, 25),
(484, 25),
(485, 25),
(486, 25),
(487, 25),
(488, 25),
(489, 25),
(490, 25),
(491, 25),
(492, 25),
(493, 25),
(494, 25),
(495, 25),
(496, 25),
(497, 25),
(498, 25),
(499, 25),
(500, 25),
(501, 25),
(502, 25),
(503, 25),
(504, 25);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (505, 25),
(506, 25),
(507, 25),
(508, 25),
(509, 25),
(510, 25),
(511, 25),
(512, 25),
(513, 25),
(514, 25),
(515, 25),
(516, 25),
(517, 25),
(518, 25),
(519, 25),
(520, 25);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260603075816_AddCreatedByToEmployeeAttendance', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Employees] ADD [CardExpiryDate] datetime2 NULL;
GO

ALTER TABLE [Employees] ADD [CardIssueDate] datetime2 NULL;
GO

ALTER TABLE [Employees] ADD [CardPrintedAt] datetime2 NULL;
GO

ALTER TABLE [Employees] ADD [CardVersion] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Employees] ADD [EmployeeCardNumber] nvarchar(50) NULL;
GO

ALTER TABLE [Employees] ADD [QRVerificationCode] nvarchar(100) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260604105856_AddEmployeeIdCardFields', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_AcademicYearId] ON [TeacherSubjectAssignments];
GO

DROP INDEX [IX_TeacherClassAssignments_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments];
GO

DROP INDEX [IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_AcademicYearId] ON [TeacherClassAssignments];
GO

DROP INDEX [IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel] ON [AttendanceNotificationLogs];
GO

ALTER TABLE [AttendanceNotificationLogs] ADD [EmployeeId] int NULL;
GO

CREATE TABLE [AcademicCalendars] (
    [Id] int NOT NULL IDENTITY,
    [AcademicYearId] int NOT NULL,
    [Name] nvarchar(100) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AcademicCalendars] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AcademicCalendars_AcademicYears_AcademicYearId] FOREIGN KEY ([AcademicYearId]) REFERENCES [AcademicYears] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AcademicCalendarEvents] (
    [Id] int NOT NULL IDENTITY,
    [AcademicCalendarId] int NOT NULL,
    [Title] nvarchar(200) NOT NULL,
    [Description] nvarchar(500) NULL,
    [EventType] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsRecurringWeekly] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AcademicCalendarEvents] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AcademicCalendarEvents_AcademicCalendars_AcademicCalendarId] FOREIGN KEY ([AcademicCalendarId]) REFERENCES [AcademicCalendars] ([Id]) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX [IX_TeacherSubjectAssignments_TeacherId_SubjectId_ClassId_SectionId_GroupId_AcademicYearId] ON [TeacherSubjectAssignments] ([TeacherId], [SubjectId], [ClassId], [SectionId], [GroupId], [AcademicYearId]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_TeacherClassAssignments_ClassId_SectionId_GroupId_AcademicYearId] ON [TeacherClassAssignments] ([ClassId], [SectionId], [GroupId], [AcademicYearId]) WHERE [IsActive] = 1 AND [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_TeacherClassAssignments_TeacherId_ClassId_SectionId_GroupId_AcademicYearId] ON [TeacherClassAssignments] ([TeacherId], [ClassId], [SectionId], [GroupId], [AcademicYearId]) WHERE [IsDeleted] = 0;
GO

CREATE UNIQUE INDEX [IX_AttendanceNotificationLogs_EmployeeId_AttendanceDate_NotificationType_NotificationChannel] ON [AttendanceNotificationLogs] ([EmployeeId], [AttendanceDate], [NotificationType], [NotificationChannel]) WHERE [IsDeleted] = 0 AND [EmployeeId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_AttendanceNotificationLogs_StudentId_AttendanceDate_NotificationType_NotificationChannel] ON [AttendanceNotificationLogs] ([StudentId], [AttendanceDate], [NotificationType], [NotificationChannel]) WHERE [IsDeleted] = 0 AND [EmployeeId] IS NULL;
GO

CREATE INDEX [IX_AcademicCalendarEvents_AcademicCalendarId] ON [AcademicCalendarEvents] ([AcademicCalendarId]);
GO

CREATE INDEX [IX_AcademicCalendars_AcademicYearId] ON [AcademicCalendars] ([AcademicYearId]);
GO

ALTER TABLE [AttendanceNotificationLogs] ADD CONSTRAINT [FK_AttendanceNotificationLogs_Employees_EmployeeId] FOREIGN KEY ([EmployeeId]) REFERENCES [Employees] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260606113851_AddAcademicCalendarModule', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[AcademicCalendars]') AND [c].[name] = N'Name');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [AcademicCalendars] DROP CONSTRAINT [' + @var74 + '];');
ALTER TABLE [AcademicCalendars] DROP COLUMN [Name];
GO

ALTER TABLE [AttendanceSettings] ADD [AutoAbsentTime] time NOT NULL DEFAULT '00:00:00';
GO

ALTER TABLE [AttendanceSettings] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [AttendanceSettings] ADD [CreatedBy] nvarchar(64) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AttendanceSettings] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AttendanceSettings] ADD [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AttendanceSettings] ADD [UpdatedAt] datetime2 NULL;
GO

ALTER TABLE [AttendanceSettings] ADD [UpdatedBy] nvarchar(64) NULL;
GO

ALTER TABLE [AcademicCalendars] ADD [Date] date NOT NULL DEFAULT '0001-01-01';
GO

ALTER TABLE [AcademicCalendars] ADD [Description] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [AcademicCalendars] ADD [HolidayType] nvarchar(100) NULL;
GO

ALTER TABLE [AcademicCalendars] ADD [IsEventDay] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AcademicCalendars] ADD [IsExamDay] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AcademicCalendars] ADD [IsHoliday] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AcademicCalendars] ADD [IsWorkingDay] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [AcademicCalendars] ADD [Remarks] nvarchar(500) NULL;
GO

ALTER TABLE [AcademicCalendars] ADD [Title] nvarchar(200) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260606131111_UpdateSettingsAndCalendarEntities', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [AttendanceNotificationLogs] ADD [NextRetryAt] datetime2 NULL;
GO

ALTER TABLE [AttendanceNotificationLogs] ADD [RetryCount] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Admissions] ADD [GuardianAddress] nvarchar(500) NULL;
GO

ALTER TABLE [Admissions] ADD [GuardianEmail] nvarchar(160) NULL;
GO

ALTER TABLE [Admissions] ADD [GuardianMobileNumber] nvarchar(30) NULL;
GO

ALTER TABLE [Admissions] ADD [GuardianNationalId] nvarchar(50) NULL;
GO

ALTER TABLE [Admissions] ADD [GuardianPhoto] nvarchar(260) NULL;
GO

ALTER TABLE [Admissions] ADD [GuardianRelationship] nvarchar(30) NULL;
GO

ALTER TABLE [Admissions] ADD [GuardianRemarks] nvarchar(500) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianAddress] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianEmail] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianMobileNumber] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianNationalId] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianPhoto] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianRelationship] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [GuardianRemarks] nvarchar(max) NULL;
GO

ALTER TABLE [AdmissionListResults] ADD [LinkedGuardianId] int NULL;
GO

CREATE TABLE [AutoAbsentExecutionLogs] (
    [Id] int NOT NULL IDENTITY,
    [ExecutionDate] datetime2 NOT NULL,
    [TargetDate] datetime2 NOT NULL,
    [StudentsProcessed] int NOT NULL,
    [StudentsMarkedAbsent] int NOT NULL,
    [EmployeesProcessed] int NOT NULL,
    [EmployeesMarkedAbsent] int NOT NULL,
    [HolidaysSkipped] int NOT NULL,
    [WeeklyOffsSkipped] int NOT NULL,
    [WorkingDaysEvaluated] int NOT NULL,
    [Status] nvarchar(40) NOT NULL,
    [Message] nvarchar(2000) NULL,
    [DurationMs] int NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AutoAbsentExecutionLogs] PRIMARY KEY ([Id])
);
GO

UPDATE [Admissions] SET [GuardianAddress] = NULL, [GuardianEmail] = NULL, [GuardianMobileNumber] = NULL, [GuardianNationalId] = NULL, [GuardianPhoto] = NULL, [GuardianRelationship] = NULL, [GuardianRemarks] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

CREATE UNIQUE INDEX [IX_AttendanceSettings_IsActive] ON [AttendanceSettings] ([IsActive]) WHERE [IsActive] = 1;
GO

CREATE UNIQUE INDEX [IX_AcademicCalendars_Date] ON [AcademicCalendars] ([Date]) WHERE [IsDeleted] = 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607142417_AddGuardianOnboardingFields', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 2 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 3 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 4 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 5 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 6 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 7 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 8 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 9 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 10 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 11 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 12 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 13 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 14 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 15 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 16 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 17 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 18 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 19 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 20 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 21 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 22 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 23 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 24 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 25 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 26 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 27 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 28 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 29 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 30 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 31 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 32 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 33 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 34 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 35 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 36 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 37 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 38 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 39 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 40 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 41 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 42 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 43 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 44 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 45 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 46 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 47 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 48 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 49 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 50 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 51 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 52 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 53 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 54 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 55 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 56 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 57 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 58 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 59 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 60 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 61 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 62 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 63 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 64 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 65 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 66 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 67 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 68 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 69 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 70 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 71 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 72 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 73 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 74 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 75 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 76 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 77 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 78 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 79 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 80 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 81 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 82 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 83 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 84 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 85 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 86 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 87 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 88 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 89 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 90 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 91 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 92 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 93 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 94 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 95 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 96 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 97 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 98 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 99 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 100 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 101 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 102 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 103 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 104 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 105 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 106 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 107 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 108 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 109 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 110 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 111 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 112 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 113 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 114 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 115 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 116 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 117 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 118 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 119 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 120 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 121 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 122 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 123 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 124 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 125 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 126 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 127 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 128 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 129 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 130 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 132 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 133 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 134 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 135 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 136 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 137 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 138 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 139 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 140 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 141 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 142 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 143 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 144 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 145 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 146 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 147 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 148 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 149 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 150 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 151 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 152 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 153 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 154 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 155 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 156 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 157 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 158 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 159 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 160 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 161 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 162 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 163 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 164 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 165 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 166 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 167 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 168 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 169 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 170 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 171 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 172 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 173 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 174 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 175 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 176 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 177 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 178 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 179 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 180 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 181 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 182 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 184 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 185 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 186 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 187 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 188 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 189 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 190 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 191 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 192 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 193 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 194 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 195 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 196 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 197 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 198 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 199 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 200 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 201 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 202 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 203 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 204 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 205 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 206 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 207 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 208 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 209 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 210 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 211 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 212 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 213 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 214 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 215 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 216 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 217 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 218 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 219 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 220 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 221 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 222 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 223 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 224 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 225 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 226 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 227 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 228 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 229 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 230 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 231 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 232 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 233 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 234 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 235 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 236 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 237 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 238 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 239 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 240 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 241 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 242 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 243 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 244 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 245 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 246 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 247 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 248 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 249 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 250 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 251 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 252 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 253 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 254 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 255 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 256 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 257 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 258 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 259 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 260 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 261 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 262 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 263 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 264 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 265 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 266 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 267 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 268 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 269 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 270 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 271 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 272 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 273 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 274 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 275 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 276 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 277 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 278 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 279 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 280 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 281 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 282 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 283 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 284 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 285 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 286 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 287 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 288 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 289 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 290 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 291 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 292 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 293 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 294 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 295 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 296 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 297 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 298 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 299 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 300 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 301 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 302 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 303 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 304 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 305 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 306 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 307 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 308 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 309 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 310 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 311 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 312 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 313 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 314 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 315 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 316 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 317 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 318 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 319 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 320 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 321 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 322 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 323 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 324 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 325 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 326 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 327 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 328 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 329 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 330 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 331 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 332 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 333 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 334 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 335 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 336 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 337 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 338 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 339 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 340 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 341 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 342 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 343 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 344 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 345 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 346 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 347 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 348 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 349 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 350 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 351 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 352 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 353 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 354 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 355 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 356 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 357 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 358 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 359 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 360 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 361 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 362 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 363 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 364 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 365 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 366 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 367 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 368 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 369 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 370 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 371 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 372 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 373 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 374 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 375 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 376 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 377 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 378 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 379 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 380 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 381 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 382 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 383 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 384 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 385 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 386 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 387 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 388 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 389 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 390 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 391 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 392 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 393 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 394 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 395 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 396 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 397 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 398 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 399 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 400 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 401 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 402 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 403 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 404 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 405 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 406 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 407 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 408 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 409 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 410 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 411 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 412 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 413 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 414 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 415 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 416 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 417 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 418 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 419 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 420 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 421 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 422 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 423 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 424 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 425 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 426 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 427 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 428 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 429 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 430 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 431 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 432 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 433 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 434 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 435 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 436 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 437 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 438 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 439 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 440 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 441 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 442 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 443 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 444 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 445 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 446 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 447 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 448 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 449 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 450 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 451 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 452 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 453 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 454 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 455 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 456 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 457 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 458 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 459 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 460 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 461 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 462 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 463 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 464 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 465 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 466 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 467 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 468 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 469 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 470 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 471 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 472 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 473 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 474 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 475 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 476 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 477 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 478 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 479 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 480 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 481 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 482 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 483 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 484 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 485 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 486 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 487 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 488 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 489 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 490 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 491 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 492 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 493 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 494 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 495 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 496 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 497 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 498 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 499 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 500 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 501 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 502 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 503 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 504 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 505 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 506 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 507 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 508 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 509 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 510 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 511 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 512 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 513 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 514 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 515 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 516 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 517 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 518 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 519 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

DELETE FROM [RolePermissions]
WHERE [PermissionId] = 520 AND [RoleId] = 25;
SELECT @@ROWCOUNT;

GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (521, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Results.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(522, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Results.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(523, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Results.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(524, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Results.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(525, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Results.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(526, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Results.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(527, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Results.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(528, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Results.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(529, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Results.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(530, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Results.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(531, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Results.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(532, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Results.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(533, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Results.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Results', N'Results', NULL, NULL),
(534, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Leave.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(535, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Leave.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(536, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Leave.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(537, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Leave.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(538, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Leave.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(539, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Leave.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(540, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Leave.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(541, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Leave.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(542, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Leave.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(543, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Leave.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(544, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Leave.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(545, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Leave.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(546, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Leave.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Leave', N'Leave', NULL, NULL),
(547, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notice.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(548, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notice.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(549, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Notice.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(550, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notice.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(551, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notice.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(552, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Notice.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(553, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notice.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(554, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notice.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(555, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notice.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(556, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notice.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(557, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notice.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(558, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notice.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(559, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Notice.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notice', N'Notice', NULL, NULL),
(560, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Calendar.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(561, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Calendar.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(562, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Calendar.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL);
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (563, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Calendar.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(564, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Calendar.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(565, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Calendar.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(566, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Calendar.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(567, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Calendar.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(568, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Calendar.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(569, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Calendar.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(570, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Calendar.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(571, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Calendar.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(572, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Calendar.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Calendar', N'Calendar', NULL, NULL),
(573, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Profile.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(574, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Profile.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(575, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Profile.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(576, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Profile.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(577, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Profile.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(578, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Profile.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(579, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Profile.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(580, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Profile.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(581, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Profile.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(582, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Profile.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(583, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Profile.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(584, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Profile.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(585, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Profile.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Profile', N'Profile', NULL, NULL),
(586, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notification.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(587, N'Read', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notification.Read', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(588, N'Create', CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Notification.Create', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(589, N'Edit', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notification.Edit', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(590, N'Update', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notification.Update', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(591, N'Delete', CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), CAST(0 AS bit), N'Notification.Delete', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(592, N'Approve', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notification.Approve', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(593, N'Assign', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notification.Assign', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(594, N'Publish', CAST(0 AS bit), CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), N'Notification.Publish', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(595, N'Export', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notification.Export', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(596, N'Print', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notification.Print', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(597, N'Generate', CAST(1 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Notification.Generate', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL),
(598, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Notification.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Notification', N'Notification', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (521, 1),
(522, 1),
(523, 1),
(524, 1),
(525, 1),
(526, 1),
(527, 1),
(528, 1),
(529, 1),
(530, 1),
(531, 1),
(532, 1),
(533, 1),
(534, 1),
(535, 1),
(536, 1),
(537, 1),
(538, 1),
(539, 1),
(540, 1),
(541, 1),
(542, 1),
(543, 1),
(544, 1),
(545, 1),
(546, 1),
(547, 1),
(548, 1),
(549, 1),
(550, 1),
(551, 1),
(552, 1),
(553, 1),
(554, 1),
(555, 1),
(556, 1),
(557, 1),
(558, 1),
(559, 1),
(560, 1),
(561, 1),
(562, 1);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (563, 1),
(564, 1),
(565, 1),
(566, 1),
(567, 1),
(568, 1),
(569, 1),
(570, 1),
(571, 1),
(572, 1),
(573, 1),
(574, 1),
(575, 1),
(576, 1),
(577, 1),
(578, 1),
(579, 1),
(580, 1),
(581, 1),
(582, 1),
(583, 1),
(584, 1),
(585, 1),
(586, 1),
(587, 1),
(588, 1),
(589, 1),
(590, 1),
(591, 1),
(592, 1),
(593, 1),
(594, 1),
(595, 1),
(596, 1),
(597, 1),
(598, 1),
(521, 2),
(522, 2),
(523, 2),
(524, 2),
(525, 2),
(526, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (527, 2),
(528, 2),
(529, 2),
(530, 2),
(531, 2),
(532, 2),
(533, 2),
(534, 2),
(535, 2),
(536, 2),
(537, 2),
(538, 2),
(539, 2),
(540, 2),
(541, 2),
(542, 2),
(543, 2),
(544, 2),
(545, 2),
(546, 2),
(547, 2),
(548, 2),
(549, 2),
(550, 2),
(551, 2),
(552, 2),
(553, 2),
(554, 2),
(555, 2),
(556, 2),
(557, 2),
(558, 2),
(559, 2),
(560, 2),
(561, 2),
(562, 2),
(563, 2),
(564, 2),
(565, 2),
(566, 2),
(567, 2),
(568, 2);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (569, 2),
(570, 2),
(571, 2),
(572, 2),
(573, 2),
(574, 2),
(575, 2),
(576, 2),
(577, 2),
(578, 2),
(579, 2),
(580, 2),
(581, 2),
(582, 2),
(583, 2),
(584, 2),
(585, 2),
(586, 2),
(587, 2),
(588, 2),
(589, 2),
(590, 2),
(591, 2),
(592, 2),
(593, 2),
(594, 2),
(595, 2),
(596, 2),
(597, 2),
(598, 2),
(521, 25),
(534, 25),
(547, 25),
(560, 25),
(573, 25),
(586, 25),
(521, 26),
(522, 26),
(523, 26),
(524, 26),
(525, 26),
(526, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (527, 26),
(528, 26),
(529, 26),
(530, 26),
(531, 26),
(532, 26),
(533, 26),
(534, 26),
(535, 26),
(536, 26),
(537, 26),
(538, 26),
(539, 26),
(540, 26),
(541, 26),
(542, 26),
(543, 26),
(544, 26),
(545, 26),
(546, 26),
(547, 26),
(548, 26),
(549, 26),
(550, 26),
(551, 26),
(552, 26),
(553, 26),
(554, 26),
(555, 26),
(556, 26),
(557, 26),
(558, 26),
(559, 26),
(560, 26),
(561, 26),
(562, 26),
(563, 26),
(564, 26),
(565, 26),
(566, 26),
(567, 26),
(568, 26);
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (569, 26),
(570, 26),
(571, 26),
(572, 26),
(573, 26),
(574, 26),
(575, 26),
(576, 26),
(577, 26),
(578, 26),
(579, 26),
(580, 26),
(581, 26),
(582, 26),
(583, 26),
(584, 26),
(585, 26),
(586, 26),
(587, 26),
(588, 26),
(589, 26),
(590, 26),
(591, 26),
(592, 26),
(593, 26),
(594, 26),
(595, 26),
(596, 26),
(597, 26),
(598, 26);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607162232_SeedGuardianPortalPermissions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] ON;
INSERT INTO [Permissions] ([Id], [Action], [CanCreate], [CanDelete], [CanRead], [CanUpdate], [Code], [CreatedAt], [CreatedBy], [IsDeleted], [Module], [ModuleName], [UpdatedAt], [UpdatedBy])
VALUES (599, N'Issue', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Library.Issue', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(600, N'Return', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Library.Return', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Library', N'Library', NULL, NULL),
(601, N'View', CAST(0 AS bit), CAST(0 AS bit), CAST(1 AS bit), CAST(0 AS bit), N'Laboratory.View', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Laboratory', N'Laboratory', NULL, NULL),
(602, N'Manage', CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), CAST(1 AS bit), N'Laboratory.Manage', '2026-01-01T00:00:00.0000000Z', N'system', CAST(0 AS bit), N'Laboratory', N'Laboratory', NULL, NULL);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Action', N'CanCreate', N'CanDelete', N'CanRead', N'CanUpdate', N'Code', N'CreatedAt', N'CreatedBy', N'IsDeleted', N'Module', N'ModuleName', N'UpdatedAt', N'UpdatedBy') AND [object_id] = OBJECT_ID(N'[Permissions]'))
    SET IDENTITY_INSERT [Permissions] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] ON;
INSERT INTO [RolePermissions] ([PermissionId], [RoleId])
VALUES (1, 4),
(131, 4),
(133, 4),
(134, 4),
(144, 4),
(157, 4),
(159, 4),
(160, 4),
(170, 4),
(172, 4),
(261, 4),
(326, 4),
(339, 4),
(521, 4),
(209, 21),
(211, 21),
(212, 21),
(214, 21),
(261, 21),
(222, 23),
(225, 23),
(1, 24),
(599, 1),
(600, 1),
(601, 1),
(602, 1),
(599, 2),
(600, 2),
(601, 2),
(602, 2),
(599, 21),
(600, 21),
(601, 22),
(602, 22),
(599, 26),
(600, 26),
(601, 26),
(602, 26);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'PermissionId', N'RoleId') AND [object_id] = OBJECT_ID(N'[RolePermissions]'))
    SET IDENTITY_INSERT [RolePermissions] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607170918_SeedMissingRolePermissions', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentSubjectResults] ADD [IsOptionalSubject] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [StudentSubjectResults] ADD [IsReligionSubject] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Exams] ADD [StudentGroupId] int NULL;
GO

ALTER TABLE [ExamConfigurations] ADD [StudentGroupId] int NULL;
GO

UPDATE [Exams] SET [StudentGroupId] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Marks] SET [Status] = 5
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Marks] SET [Status] = 5
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_Exams_StudentGroupId] ON [Exams] ([StudentGroupId]);
GO

ALTER TABLE [Exams] ADD CONSTRAINT [FK_Exams_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260607184702_CompleteExamResultEnhancements', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [StudentSubjectResults] ADD [AcademicYearId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [StudentSubjectResults] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [StudentSubjectResults] ADD [SectionId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [StudentExamResults] ADD [AcademicYearId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [StudentExamResults] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [StudentExamResults] ADD [SectionId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [StudentExamResults] ADD [StudentGroupId] int NULL;
GO

ALTER TABLE [ResultPublications] ADD [AcademicYearId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MeritResults] ADD [AcademicYearId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MeritResults] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MeritResults] ADD [StudentGroupId] int NULL;
GO

ALTER TABLE [Marks] ADD [AcademicYearId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Marks] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Marks] ADD [SectionId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MarkEntryDrafts] ADD [AcademicYearId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MarkEntryDrafts] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [MarkEntryDrafts] ADD [SectionId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [FinalResults] ADD [SectionId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [FinalResults] ADD [StudentGroupId] int NULL;
GO

UPDATE [Marks] SET [AcademicYearId] = 0, [ClassId] = 0, [SectionId] = 0
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Marks] SET [AcademicYearId] = 0, [ClassId] = 0, [SectionId] = 0
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260608090926_AddDenormalizedFieldsToResultEntities', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Marks] ADD [ComponentValues] nvarchar(max) NULL;
GO

CREATE TABLE [ExamComponents] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(100) NOT NULL,
    [Code] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NULL,
    [DisplayOrder] int NOT NULL,
    [DefaultFullMarks] decimal(18,2) NOT NULL,
    [DefaultPassMarks] decimal(18,2) NOT NULL,
    [IsPractical] bit NOT NULL,
    [IsOptional] bit NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ExamComponents] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SubjectMarkStructures] (
    [Id] int NOT NULL IDENTITY,
    [ComponentId] int NOT NULL,
    [ExamId] int NULL,
    [ClassId] int NULL,
    [SubjectId] int NULL,
    [StudentGroupId] int NULL,
    [FullMarks] decimal(18,2) NOT NULL,
    [PassMarks] decimal(18,2) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_SubjectMarkStructures] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SubjectMarkStructures_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SubjectMarkStructures_ExamComponents_ComponentId] FOREIGN KEY ([ComponentId]) REFERENCES [ExamComponents] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SubjectMarkStructures_Exams_ExamId] FOREIGN KEY ([ExamId]) REFERENCES [Exams] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SubjectMarkStructures_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SubjectMarkStructures_Subjects_SubjectId] FOREIGN KEY ([SubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [Marks] SET [ComponentValues] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Marks] SET [ComponentValues] = NULL
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

CREATE UNIQUE INDEX [IX_ExamComponents_Code] ON [ExamComponents] ([Code]);
GO

CREATE INDEX [IX_SubjectMarkStructures_ClassId] ON [SubjectMarkStructures] ([ClassId]);
GO

CREATE UNIQUE INDEX [IX_SubjectMarkStructures_ComponentId_ExamId_SubjectId_StudentGroupId] ON [SubjectMarkStructures] ([ComponentId], [ExamId], [SubjectId], [StudentGroupId]) WHERE [ExamId] IS NOT NULL AND [SubjectId] IS NOT NULL AND [StudentGroupId] IS NOT NULL;
GO

CREATE INDEX [IX_SubjectMarkStructures_ExamId] ON [SubjectMarkStructures] ([ExamId]);
GO

CREATE INDEX [IX_SubjectMarkStructures_StudentGroupId] ON [SubjectMarkStructures] ([StudentGroupId]);
GO

CREATE INDEX [IX_SubjectMarkStructures_SubjectId] ON [SubjectMarkStructures] ([SubjectId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260608130001_AddMarkEntryComponentValues', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Classes] ADD [IsGroupBased] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(0 AS bit)
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(1 AS bit)
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [IsGroupBased] = CAST(1 AS bit)
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260608173957_AddIsGroupBasedToSchoolClass', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SubjectMarkStructures] DROP CONSTRAINT [FK_SubjectMarkStructures_Exams_ExamId];
GO

DROP TABLE [SubjectComponents];
GO

DROP INDEX [IX_SubjectMarkStructures_ComponentId_ExamId_SubjectId_StudentGroupId] ON [SubjectMarkStructures];
GO

DROP INDEX [IX_SubjectMarkStructures_ExamId] ON [SubjectMarkStructures];
GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SubjectMarkStructures]') AND [c].[name] = N'ExamId');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [SubjectMarkStructures] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [SubjectMarkStructures] DROP COLUMN [ExamId];
GO

                IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID(N'[Students]') AND name = 'OptionalSubjectId')
                BEGIN
                    ALTER TABLE [Students] ADD [OptionalSubjectId] int NULL;
                END
GO

UPDATE [Students] SET [AssignedReligionSubjectId] = 30, [OptionalSubjectId] = NULL
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Students] SET [AssignedReligionSubjectId] = 30, [OptionalSubjectId] = NULL
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [IsReligionSubject] = CAST(1 AS bit), [ReligionType] = N'Islam', [SubjectGroup] = N'Religion'
WHERE [Id] = 30;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [IsReligionSubject] = CAST(1 AS bit), [ReligionType] = N'Hindu', [SubjectGroup] = N'Religion'
WHERE [Id] = 31;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [IsReligionSubject] = CAST(1 AS bit), [ReligionType] = N'Buddhist', [SubjectGroup] = N'Religion'
WHERE [Id] = 32;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [IsReligionSubject] = CAST(1 AS bit), [ReligionType] = N'Christian', [SubjectGroup] = N'Religion'
WHERE [Id] = 33;
SELECT @@ROWCOUNT;

GO

                IF NOT EXISTS (SELECT 1 FROM [Subjects] WHERE [Id] = 34)
                BEGIN
                    INSERT INTO [Subjects] ([Id], [Code], [CreatedAt], [CreatedBy], [DefaultFullMarks], [DefaultPassMarks], [DisplayOrder], [HasAssignment], [HasCQ], [HasContinuousAssessment], [HasLab], [HasMCQ], [HasOral], [HasPractical], [HasViva], [HasWritten], [IsActive], [IsDeleted], [IsMandatory], [IsOptional], [IsPractical], [IsReligionSubject], [Name], [NameBn], [ReligionType], [SubjectGroup], [UpdatedAt], [UpdatedBy])
                    VALUES (34, N'MUS', '2026-01-01T00:00:00.0000000Z', N'system', 100, 33, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 1, 0, 0, 0, N'Music', N'সঙ্গীত', NULL, N'Common', NULL, NULL);
                END
GO

CREATE UNIQUE INDEX [IX_SubjectMarkStructures_ComponentId_SubjectId_StudentGroupId] ON [SubjectMarkStructures] ([ComponentId], [SubjectId], [StudentGroupId]) WHERE [SubjectId] IS NOT NULL AND [StudentGroupId] IS NOT NULL;
GO

CREATE INDEX [IX_Students_OptionalSubjectId] ON [Students] ([OptionalSubjectId]);
GO

ALTER TABLE [Students] ADD CONSTRAINT [FK_Students_Subjects_OptionalSubjectId] FOREIGN KEY ([OptionalSubjectId]) REFERENCES [Subjects] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609102234_RemoveSubjectComponentAndExamId', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_ClassSubjects_SchoolClassId] ON [ClassSubjects];
GO

DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasAssignment');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var76 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasAssignment];
GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasCQ');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasCQ];
GO

DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasContinuousAssessment');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var78 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasContinuousAssessment];
GO

DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasLab');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var79 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasLab];
GO

DECLARE @var80 sysname;
SELECT @var80 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasMCQ');
IF @var80 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var80 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasMCQ];
GO

DECLARE @var81 sysname;
SELECT @var81 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasOral');
IF @var81 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var81 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasOral];
GO

DECLARE @var82 sysname;
SELECT @var82 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasPractical');
IF @var82 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var82 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasPractical];
GO

DECLARE @var83 sysname;
SELECT @var83 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasViva');
IF @var83 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var83 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasViva];
GO

DECLARE @var84 sysname;
SELECT @var84 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Subjects]') AND [c].[name] = N'HasWritten');
IF @var84 IS NOT NULL EXEC(N'ALTER TABLE [Subjects] DROP CONSTRAINT [' + @var84 + '];');
ALTER TABLE [Subjects] DROP COLUMN [HasWritten];
GO

DECLARE @var85 sysname;
SELECT @var85 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'AssignmentMarks');
IF @var85 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var85 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [AssignmentMarks];
GO

DECLARE @var86 sysname;
SELECT @var86 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'ContinuousAssessmentMarks');
IF @var86 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var86 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [ContinuousAssessmentMarks];
GO

DECLARE @var87 sysname;
SELECT @var87 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'LabMarks');
IF @var87 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var87 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [LabMarks];
GO

DECLARE @var88 sysname;
SELECT @var88 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'MCQMarks');
IF @var88 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var88 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [MCQMarks];
GO

DECLARE @var89 sysname;
SELECT @var89 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'OralMarks');
IF @var89 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var89 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [OralMarks];
GO

DECLARE @var90 sysname;
SELECT @var90 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'PracticalMarks');
IF @var90 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var90 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [PracticalMarks];
GO

DECLARE @var91 sysname;
SELECT @var91 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'VivaMarks');
IF @var91 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var91 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [VivaMarks];
GO

DECLARE @var92 sysname;
SELECT @var92 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ExamSubjects]') AND [c].[name] = N'WrittenMarks');
IF @var92 IS NOT NULL EXEC(N'ALTER TABLE [ExamSubjects] DROP CONSTRAINT [' + @var92 + '];');
ALTER TABLE [ExamSubjects] DROP COLUMN [WrittenMarks];
GO

DECLARE @var93 sysname;
SELECT @var93 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'AssignmentMarks');
IF @var93 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var93 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [AssignmentMarks];
GO

DECLARE @var94 sysname;
SELECT @var94 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'BehaviourMarks');
IF @var94 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var94 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [BehaviourMarks];
GO

DECLARE @var95 sysname;
SELECT @var95 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'CQMarks');
IF @var95 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var95 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [CQMarks];
GO

DECLARE @var96 sysname;
SELECT @var96 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'CompetencyMarks');
IF @var96 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var96 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [CompetencyMarks];
GO

DECLARE @var97 sysname;
SELECT @var97 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'ContinuousAssessmentMarks');
IF @var97 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var97 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [ContinuousAssessmentMarks];
GO

DECLARE @var98 sysname;
SELECT @var98 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'LabMarks');
IF @var98 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var98 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [LabMarks];
GO

DECLARE @var99 sysname;
SELECT @var99 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'MCQMarks');
IF @var99 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var99 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [MCQMarks];
GO

DECLARE @var100 sysname;
SELECT @var100 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'OralMarks');
IF @var100 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var100 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [OralMarks];
GO

DECLARE @var101 sysname;
SELECT @var101 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'ParticipationMarks');
IF @var101 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var101 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [ParticipationMarks];
GO

DECLARE @var102 sysname;
SELECT @var102 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'PracticalMarks');
IF @var102 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var102 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [PracticalMarks];
GO

DECLARE @var103 sysname;
SELECT @var103 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'VivaMarks');
IF @var103 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var103 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [VivaMarks];
GO

DECLARE @var104 sysname;
SELECT @var104 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'WrittenMarks');
IF @var104 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var104 + '];');
ALTER TABLE [ClassSubjects] DROP COLUMN [WrittenMarks];
GO

ALTER TABLE [Subjects] ADD [Category] nvarchar(50) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Subjects] ADD [ShortName] nvarchar(30) NOT NULL DEFAULT N'';
GO

ALTER TABLE [SchoolSettings] ADD [BanglaName] nvarchar(160) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [CopyrightText] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [EstablishedYear] int NULL;
GO

ALTER TABLE [SchoolSettings] ADD [FooterLogoPath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [InstagramUrl] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [LinkedInUrl] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [LoginLogoPath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [MetaDescription] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [MetaKeywords] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [MetaTitle] nvarchar(160) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [Mobile] nvarchar(30) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [OgDescription] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [OgImagePath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [OgTitle] nvarchar(160) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [PrincipalDesignation] nvarchar(160) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [PrincipalQualification] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [PrincipalSignaturePath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [SchoolCode] nvarchar(30) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [SchoolDescription] nvarchar(2000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [SchoolMotto] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [ShowAdmissionCTA] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowEvents] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowGallery] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowNotices] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowPrincipalMessage] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowSlider] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowStatistics] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowWelcomeSection] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [TwitterUrl] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [WebsiteBannerPath] nvarchar(260) NULL;
GO

ALTER TABLE [Classes] ADD [ArchivedAt] datetime2 NULL;
GO

ALTER TABLE [Classes] ADD [Capacity] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Classes] ADD [Code] nvarchar(20) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Classes] ADD [Description] nvarchar(500) NULL;
GO

ALTER TABLE [Classes] ADD [IsActive] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Classes] ADD [NameBn] nvarchar(60) NOT NULL DEFAULT N'';
GO

CREATE TABLE [ContactMessages] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(160) NOT NULL,
    [Email] nvarchar(160) NOT NULL,
    [Phone] nvarchar(30) NULL,
    [Subject] nvarchar(260) NOT NULL,
    [Message] nvarchar(4000) NOT NULL,
    [Status] nvarchar(30) NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_ContactMessages] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EmailTemplates] (
    [Id] int NOT NULL IDENTITY,
    [TemplateName] nvarchar(160) NOT NULL,
    [Subject] nvarchar(260) NOT NULL,
    [Body] nvarchar(max) NOT NULL,
    [Placeholders] nvarchar(500) NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_EmailTemplates] PRIMARY KEY ([Id])
);
GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Classes] SET [ArchivedAt] = NULL, [Capacity] = 0, [Code] = N'', [Description] = NULL, [IsActive] = CAST(1 AS bit), [NameBn] = N''
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 11;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 12;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 13;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 14;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 15;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 16;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 17;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 18;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 19;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 20;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 21;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 22;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 23;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 24;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 25;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 26;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 30;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 31;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 32;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 33;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [Category] = N'', [ShortName] = N''
WHERE [Id] = 34;
SELECT @@ROWCOUNT;

GO

CREATE UNIQUE INDEX [IX_ClassSubjects_SchoolClassId_SubjectId_GroupName] ON [ClassSubjects] ([SchoolClassId], [SubjectId], [GroupName]) WHERE [IsDeleted] = 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609125751_AddWebsiteCmsEnhancements', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionCircularPath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionCloseDate] datetime2 NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionCtaText] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionCtaTitle] nvarchar(200) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionEligibility] nvarchar(4000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionFeeNote] nvarchar(2000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionFormPath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionGuidelines] nvarchar(4000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionMetaDescription] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionMetaKeywords] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionMetaTitle] nvarchar(160) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionOgDescription] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionOgImagePath] nvarchar(260) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionOgTitle] nvarchar(160) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionOpenDate] datetime2 NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionProcess] nvarchar(4000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionRequirements] nvarchar(4000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionSubtitle] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [AdmissionTitle] nvarchar(200) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [OnlineAdmissionEnabled] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowAdmissionDownloads] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowAdmissionFees] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowAdmissionGuidelines] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowAdmissionPage] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [SchoolSettings] ADD [ShowAdmissionRequirements] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [AdmissionFeeStructures] (
    [Id] int NOT NULL IDENTITY,
    [SchoolClassId] int NOT NULL,
    [ClassName] nvarchar(100) NOT NULL,
    [AdmissionFee] decimal(18,2) NOT NULL,
    [MonthlyFee] decimal(18,2) NOT NULL,
    [SessionFee] decimal(18,2) NOT NULL,
    [ExamFee] decimal(18,2) NOT NULL,
    [OtherFee] decimal(18,2) NOT NULL,
    [DisplayOrder] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedBy] nvarchar(64) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] nvarchar(64) NULL,
    [UpdatedAt] datetime2 NULL,
    [IsDeleted] bit NOT NULL,
    CONSTRAINT [PK_AdmissionFeeStructures] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609171235_AddAdmissionCmsSettings', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE UNIQUE INDEX [IX_AdmissionFeeStructures_SchoolClassId] ON [AdmissionFeeStructures] ([SchoolClassId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609171853_AddAdmissionFeeUniqueIndex', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [SchoolSettings] ADD [ClassLabel] nvarchar(100) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [EmployeeLabel] nvarchar(100) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [OfficeHours] nvarchar(200) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [SchoolHistory] nvarchar(4000) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [StudentLabel] nvarchar(100) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [TeacherLabel] nvarchar(100) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [WelcomeHeading] nvarchar(200) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [WelcomeTagline] nvarchar(500) NULL;
GO

ALTER TABLE [SchoolSettings] ADD [WelcomeText] nvarchar(4000) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260609183332_AddSchoolSettingCMSFields', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ExamSchedules] ADD [ClassId] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [ExamSchedules] ADD [SectionId] int NULL;
GO

ALTER TABLE [ExamSchedules] ADD [StudentGroupId] int NULL;
GO

DROP INDEX [IX_ClassSubjects_SchoolClassId_SubjectId_GroupName] ON [ClassSubjects];
DECLARE @var105 sysname;
SELECT @var105 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ClassSubjects]') AND [c].[name] = N'GroupName');
IF @var105 IS NOT NULL EXEC(N'ALTER TABLE [ClassSubjects] DROP CONSTRAINT [' + @var105 + '];');
UPDATE [ClassSubjects] SET [GroupName] = N'' WHERE [GroupName] IS NULL;
ALTER TABLE [ClassSubjects] ALTER COLUMN [GroupName] nvarchar(50) NOT NULL;
ALTER TABLE [ClassSubjects] ADD DEFAULT N'' FOR [GroupName];
CREATE UNIQUE INDEX [IX_ClassSubjects_SchoolClassId_SubjectId_GroupName] ON [ClassSubjects] ([SchoolClassId], [SubjectId], [GroupName]) WHERE [IsDeleted] = 0;
GO

UPDATE [StudentGroups] SET [Name] = N'BusinessStudies'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 5;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 6;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 7;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 8;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 9;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 10;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 11;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 12;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 13;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 14;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 15;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'BusinessStudies'
WHERE [Id] = 20;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'BusinessStudies'
WHERE [Id] = 21;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'BusinessStudies'
WHERE [Id] = 22;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 27;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 28;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 29;
SELECT @@ROWCOUNT;

GO

UPDATE [Subjects] SET [SubjectGroup] = N'General'
WHERE [Id] = 34;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_ExamSchedules_ClassId] ON [ExamSchedules] ([ClassId]);
GO

CREATE INDEX [IX_ExamSchedules_SectionId] ON [ExamSchedules] ([SectionId]);
GO

CREATE INDEX [IX_ExamSchedules_StudentGroupId] ON [ExamSchedules] ([StudentGroupId]);
GO

ALTER TABLE [ExamSchedules] ADD CONSTRAINT [FK_ExamSchedules_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [ExamSchedules] ADD CONSTRAINT [FK_ExamSchedules_Sections_SectionId] FOREIGN KEY ([SectionId]) REFERENCES [Sections] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [ExamSchedules] ADD CONSTRAINT [FK_ExamSchedules_StudentGroups_StudentGroupId] FOREIGN KEY ([StudentGroupId]) REFERENCES [StudentGroups] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260611160144_AddExamScheduleClassGroupSection', N'8.0.0');
GO

COMMIT;
GO

