-- ============================================================
-- SCHOOL MANAGEMENT SYSTEM - COMPREHENSIVE SEED SCRIPT
-- Database: SchoolManagementSystemDbdemo
-- School: Green Valley International School
-- Academic Year: 2027
-- ============================================================

USE [SchoolManagementSystemDbdemo]
GO

EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'
GO

-- ============================================================
-- 1. ACADEMIC YEAR
-- ============================================================
SET IDENTITY_INSERT [AcademicYears] ON
GO
INSERT INTO [AcademicYears] ([Id], [Name], [Code], [StartsOn], [EndsOn], [IsActive], [IsCurrent], [IsLocked], [Status], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES (1, 'Academic Year 2027', 'AY2027', '2027-01-01', '2027-12-31', 1, 1, 0, 'Active', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [AcademicYears] OFF
GO

-- ============================================================
-- 2. CLASSES (Play, Nursery, KG, Class 1-10)
-- ============================================================
SET IDENTITY_INSERT [Classes] ON
GO
INSERT INTO [Classes] ([Id], [Name], [NameBn], [Code], [SortOrder], [Capacity], [IsGroupBased], [IsHigherSecondary], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'Play', N'প্লে', 'PL', 1, 45, 0, 0, 1, 'system', GETUTCDATE(), 0),
(2, 'Nursery', N'নার্সারি', 'NS', 2, 45, 0, 0, 1, 'system', GETUTCDATE(), 0),
(3, 'KG', N'কেজি', 'KG', 3, 45, 0, 0, 1, 'system', GETUTCDATE(), 0),
(4, 'Class 1', N'প্রথম শ্রেণী', 'C1', 4, 60, 0, 0, 1, 'system', GETUTCDATE(), 0),
(5, 'Class 2', N'দ্বিতীয় শ্রেণী', 'C2', 5, 60, 0, 0, 1, 'system', GETUTCDATE(), 0),
(6, 'Class 3', N'তৃতীয় শ্রেণী', 'C3', 6, 60, 0, 0, 1, 'system', GETUTCDATE(), 0),
(7, 'Class 4', N'চতুর্থ শ্রেণী', 'C4', 7, 60, 0, 0, 1, 'system', GETUTCDATE(), 0),
(8, 'Class 5', N'পঞ্চম শ্রেণী', 'C5', 8, 60, 0, 0, 1, 'system', GETUTCDATE(), 0),
(9, 'Class 6', N'ষষ্ঠ শ্রেণী', 'C6', 9, 80, 0, 0, 1, 'system', GETUTCDATE(), 0),
(10, 'Class 7', N'সপ্তম শ্রেণী', 'C7', 10, 80, 0, 0, 1, 'system', GETUTCDATE(), 0),
(11, 'Class 8', N'অষ্টম শ্রেণী', 'C8', 11, 80, 0, 0, 1, 'system', GETUTCDATE(), 0),
(12, 'Class 9', N'নবম শ্রেণী', 'C9', 12, 100, 1, 0, 1, 'system', GETUTCDATE(), 0),
(13, 'Class 10', N'দশম শ্রেণী', 'C10', 13, 100, 1, 0, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Classes] OFF
GO

-- ============================================================
-- 3. SECTIONS
-- ============================================================
SET IDENTITY_INSERT [Sections] ON
GO
INSERT INTO [Sections] ([Id], [SchoolClassId], [Name], [Capacity], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 1, 'A', 45, 'system', GETUTCDATE(), 0),
(2, 2, 'A', 45, 'system', GETUTCDATE(), 0),
(3, 3, 'A', 45, 'system', GETUTCDATE(), 0),
(4, 4, 'A', 30, 'system', GETUTCDATE(), 0), (5, 4, 'B', 30, 'system', GETUTCDATE(), 0),
(6, 5, 'A', 30, 'system', GETUTCDATE(), 0), (7, 5, 'B', 30, 'system', GETUTCDATE(), 0),
(8, 6, 'A', 30, 'system', GETUTCDATE(), 0), (9, 6, 'B', 30, 'system', GETUTCDATE(), 0),
(10, 7, 'A', 30, 'system', GETUTCDATE(), 0), (11, 7, 'B', 30, 'system', GETUTCDATE(), 0),
(12, 8, 'A', 30, 'system', GETUTCDATE(), 0), (13, 8, 'B', 30, 'system', GETUTCDATE(), 0),
(14, 9, 'A', 40, 'system', GETUTCDATE(), 0), (15, 9, 'B', 40, 'system', GETUTCDATE(), 0),
(16, 10, 'A', 40, 'system', GETUTCDATE(), 0), (17, 10, 'B', 40, 'system', GETUTCDATE(), 0),
(18, 11, 'A', 40, 'system', GETUTCDATE(), 0), (19, 11, 'B', 40, 'system', GETUTCDATE(), 0),
(20, 12, 'A', 50, 'system', GETUTCDATE(), 0), (21, 12, 'B', 50, 'system', GETUTCDATE(), 0),
(22, 13, 'A', 50, 'system', GETUTCDATE(), 0), (23, 13, 'B', 50, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Sections] OFF
GO

-- ============================================================
-- 4. STUDENT GROUPS
-- ============================================================
SET IDENTITY_INSERT [StudentGroups] ON
GO
INSERT INTO [StudentGroups] ([Id], [Name], [Code], [Description], [MinClass], [MaxClass], [DisplayOrder], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'Science', 'SCI', 'Science Group', 9, 10, 1, 1, 'system', GETUTCDATE(), 0),
(2, 'Humanities', 'HUM', 'Humanities Group', 9, 10, 2, 1, 'system', GETUTCDATE(), 0),
(3, 'Business Studies', 'BUS', 'Business Studies Group', 9, 10, 3, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [StudentGroups] OFF
GO

-- ============================================================
-- 5. ROLES
-- ============================================================
SET IDENTITY_INSERT [Roles] ON
GO
INSERT INTO [Roles] ([Id], [Name], [Description], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'Super Admin', 'System Super Administrator', 'system', GETUTCDATE(), 0),
(2, 'Admin', 'School Administrator', 'system', GETUTCDATE(), 0),
(3, 'Principal', 'School Principal', 'system', GETUTCDATE(), 0),
(4, 'Vice Principal', 'Vice Principal', 'system', GETUTCDATE(), 0),
(5, 'Accountant', 'Accountant', 'system', GETUTCDATE(), 0),
(6, 'Teacher', 'Teacher', 'system', GETUTCDATE(), 0),
(7, 'Student', 'Student', 'system', GETUTCDATE(), 0),
(8, 'Guardian', 'Guardian/Parent', 'system', GETUTCDATE(), 0),
(9, 'Librarian', 'Librarian', 'system', GETUTCDATE(), 0),
(10, 'Transport Manager', 'Transport Manager', 'system', GETUTCDATE(), 0),
(11, 'Exam Controller', 'Exam Controller', 'system', GETUTCDATE(), 0),
(12, 'Admission Officer', 'Admission Officer', 'system', GETUTCDATE(), 0),
(13, 'Academic Coordinator', 'Academic Coordinator', 'system', GETUTCDATE(), 0),
(14, 'IT Administrator', 'IT Administrator', 'system', GETUTCDATE(), 0),
(15, 'Office Staff', 'Office Staff', 'system', GETUTCDATE(), 0),
(16, 'Finance Manager', 'Finance Manager', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Roles] OFF
GO

-- ============================================================
-- 6. DEPARTMENTS
-- ============================================================
SET IDENTITY_INSERT [Departments] ON
GO
INSERT INTO [Departments] ([Id], [Name], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'Academic', 'system', GETUTCDATE(), 0),
(2, 'Administration', 'system', GETUTCDATE(), 0),
(3, 'Finance & Accounts', 'system', GETUTCDATE(), 0),
(4, 'Library', 'system', GETUTCDATE(), 0),
(5, 'Transport', 'system', GETUTCDATE(), 0),
(6, 'Security', 'system', GETUTCDATE(), 0),
(7, 'Housekeeping', 'system', GETUTCDATE(), 0),
(8, 'IT', 'system', GETUTCDATE(), 0),
(9, 'Admission', 'system', GETUTCDATE(), 0),
(10, 'Examination', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Departments] OFF
GO

-- ============================================================
-- 7. DESIGNATIONS
-- ============================================================
SET IDENTITY_INSERT [Designations] ON
GO
INSERT INTO [Designations] ([Id], [Name], [RoleLevel], [IsTeachingRole], [IsAdministrativeRole], [RequiresLogin], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'Super Admin', 1, 0, 1, 1, 1, 'system', GETUTCDATE(), 0),
(2, 'Principal', 1, 0, 1, 1, 1, 'system', GETUTCDATE(), 0),
(3, 'Vice Principal', 2, 1, 1, 1, 1, 'system', GETUTCDATE(), 0),
(4, 'Finance Manager', 2, 0, 1, 1, 1, 'system', GETUTCDATE(), 0),
(5, 'Accountant', 3, 0, 0, 1, 1, 'system', GETUTCDATE(), 0),
(6, 'Senior Teacher', 3, 1, 0, 1, 1, 'system', GETUTCDATE(), 0),
(7, 'Teacher', 4, 1, 0, 1, 1, 'system', GETUTCDATE(), 0),
(8, 'Admission Officer', 3, 0, 1, 1, 1, 'system', GETUTCDATE(), 0),
(9, 'Librarian', 3, 0, 0, 1, 1, 'system', GETUTCDATE(), 0),
(10, 'Transport Manager', 3, 0, 0, 1, 1, 'system', GETUTCDATE(), 0),
(11, 'Exam Controller', 2, 0, 1, 1, 1, 'system', GETUTCDATE(), 0),
(12, 'Academic Coordinator', 2, 1, 1, 1, 1, 'system', GETUTCDATE(), 0),
(13, 'IT Administrator', 3, 0, 0, 1, 1, 'system', GETUTCDATE(), 0),
(14, 'Office Staff', 4, 0, 0, 1, 1, 'system', GETUTCDATE(), 0),
(15, 'Security Staff', 5, 0, 0, 0, 1, 'system', GETUTCDATE(), 0),
(16, 'Cleaner', 5, 0, 0, 0, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Designations] OFF
GO

-- ============================================================
-- 8. SUBJECTS (NCTB Bangladesh Curriculum)
-- ============================================================
SET IDENTITY_INSERT [Subjects] ON
GO
INSERT INTO [Subjects] ([Id], [Code], [Name], [NameBn], [ShortName], [Category], [IsMandatory], [IsOptional], [IsReligionSubject], [IsPractical], [TheoryMarks], [PracticalMarks], [PassMarks], [Credit], [DisplayOrder], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'BAN', 'Bangla', N'বাংলা', 'BAN', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 1, 1, 'system', GETUTCDATE(), 0),
(2, 'ENG', 'English', 'English', 'ENG', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 2, 1, 'system', GETUTCDATE(), 0),
(3, 'MATH', 'Mathematics', N'গণিত', 'MATH', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 3, 1, 'system', GETUTCDATE(), 0),
(4, 'SCI', 'Science', N'বিজ্ঞান', 'SCI', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 4, 1, 'system', GETUTCDATE(), 0),
(5, 'SS', 'Bangladesh & Global Studies', N'বাংলাদেশ ও বিশ্বপরিচয়', 'SS', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 5, 1, 'system', GETUTCDATE(), 0),
(6, 'REL', 'Islam & Moral Education', N'ইসলাম ও নৈতিক শিক্ষা', 'REL', 'Religion', 1, 0, 1, 0, 100, 0, 33, 1, 6, 1, 'system', GETUTCDATE(), 0),
(7, 'HINDU', 'Hindu Religion & Moral Edu', N'হিন্দু ধর্ম ও নৈতিক শিক্ষা', 'HINDU', 'Religion', 1, 0, 1, 0, 100, 0, 33, 1, 7, 1, 'system', GETUTCDATE(), 0),
(8, 'PHY', 'Physics', N'পদার্থ বিজ্ঞান', 'PHY', 'Core', 0, 0, 0, 1, 75, 25, 33, 1, 8, 1, 'system', GETUTCDATE(), 0),
(9, 'CHEM', 'Chemistry', N'রসায়ন', 'CHEM', 'Core', 0, 0, 0, 1, 75, 25, 33, 1, 9, 1, 'system', GETUTCDATE(), 0),
(10, 'BIO', 'Biology', N'জীব বিজ্ঞান', 'BIO', 'Core', 0, 0, 0, 1, 75, 25, 33, 1, 10, 1, 'system', GETUTCDATE(), 0),
(11, 'HMATH', 'Higher Mathematics', N'উচ্চতর গণিত', 'HMATH', 'Elective', 0, 1, 0, 0, 100, 0, 33, 1, 11, 1, 'system', GETUTCDATE(), 0),
(12, 'HIST', 'History', N'ইতিহাস', 'HIST', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 12, 1, 'system', GETUTCDATE(), 0),
(13, 'GEO', 'Geography', N'ভূগোল', 'GEO', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 13, 1, 'system', GETUTCDATE(), 0),
(14, 'CIV', 'Civics & Citizenship', N'পৌরনীতি ও নাগরিকতা', 'CIV', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 14, 1, 'system', GETUTCDATE(), 0),
(15, 'ECO', 'Economics', N'অর্থনীতি', 'ECO', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 15, 1, 'system', GETUTCDATE(), 0),
(16, 'ACC', 'Accounting', N'হিসাব বিজ্ঞান', 'ACC', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 16, 1, 'system', GETUTCDATE(), 0),
(17, 'MGT', 'Management', N'ব্যবস্থাপনা', 'MGT', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 17, 1, 'system', GETUTCDATE(), 0),
(18, 'FIN', 'Finance & Banking', N'অর্থ ও ব্যাংকিং', 'FIN', 'Core', 0, 0, 0, 0, 100, 0, 33, 1, 18, 1, 'system', GETUTCDATE(), 0),
(19, 'AGRI', 'Agriculture', N'কৃষি শিক্ষা', 'AGRI', 'Elective', 0, 1, 0, 0, 100, 0, 33, 1, 19, 1, 'system', GETUTCDATE(), 0),
(20, 'ICT', 'Information & Communication Technology', N'তথ্য ও যোগাযোগ প্রযুক্তি', 'ICT', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 20, 1, 'system', GETUTCDATE(), 0),
(21, 'PE', 'Physical Education', N'শারীরিক শিক্ষা', 'PE', 'Core', 1, 0, 0, 0, 50, 0, 17, 1, 21, 1, 'system', GETUTCDATE(), 0),
(22, 'ARTS', 'Arts & Crafts', N'চারু ও কারুকলা', 'ARTS', 'Core', 1, 0, 0, 0, 50, 0, 17, 1, 22, 1, 'system', GETUTCDATE(), 0),
(23, 'WKM', 'Work & Life Oriented Education', N'কাজ ও জীবনমুখী শিক্ষা', 'WKM', 'Core', 1, 0, 0, 0, 50, 0, 17, 1, 23, 1, 'system', GETUTCDATE(), 0),
(24, 'ENG2', 'English 2nd Paper', 'English 2nd Paper', 'ENG2', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 2, 1, 'system', GETUTCDATE(), 0),
(25, 'BAN2', 'Bangla 2nd Paper', N'বাংলা দ্বিতীয় পত্র', 'BAN2', 'Core', 1, 0, 0, 0, 100, 0, 33, 1, 1, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Subjects] OFF
GO

-- ============================================================
-- 9. USERS
-- ============================================================
SET IDENTITY_INSERT [Users] ON
GO
DECLARE @demoHash NVARCHAR(512) = N'AQAAAAIAAYagAAAAEJ6KqZ8sL0Rn3Qn0wF0v1T5z2X4y8p5rIs7f3t9u2j1k6l0m3n4o5p6q7r8s9t0u1v2w3x4y5z6A=='
DECLARE @now DATETIME = GETUTCDATE()

INSERT INTO [Users] ([Id], [UserName], [Email], [PhoneNumber], [PasswordHash], [Status], [IsEmailConfirmed], [LastLoginAt], [FailedLoginAttempts], [MustChangePassword], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'superadmin', 'superadmin@gvi.edu.bd', '01700000001', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(2, 'principal', 'principal@gvi.edu.bd', '01700000002', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(3, 'vp_academic', 'vp.academic@gvi.edu.bd', '01700000003', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(4, 'vp_admin', 'vp.admin@gvi.edu.bd', '01700000004', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(5, 'finance_mgr', 'finance.mgr@gvi.edu.bd', '01700000005', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(6, 'accountant1', 'acc1@gvi.edu.bd', '01700000006', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(7, 'accountant2', 'acc2@gvi.edu.bd', '01700000007', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(8, 'accountant3', 'acc3@gvi.edu.bd', '01700000008', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(9, 'admission_officer', 'admission@gvi.edu.bd', '01700000009', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(10, 'librarian', 'librarian@gvi.edu.bd', '01700000010', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(11, 'transport_mgr', 'transport@gvi.edu.bd', '01700000011', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(12, 'exam_controller', 'exam.ctrl@gvi.edu.bd', '01700000012', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(13, 'academic_coord', 'academic.coord@gvi.edu.bd', '01700000013', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(14, 'it_admin', 'it@gvi.edu.bd', '01700000014', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(15, 'staff1', 'staff1@gvi.edu.bd', '01700000015', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(16, 'staff2', 'staff2@gvi.edu.bd', '01700000016', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(17, 'staff3', 'staff3@gvi.edu.bd', '01700000017', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(18, 'staff4', 'staff4@gvi.edu.bd', '01700000018', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0),
(19, 'staff5', 'staff5@gvi.edu.bd', '01700000019', @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0)

INSERT INTO [Users] ([Id], [UserName], [Email], [PhoneNumber], [PasswordHash], [Status], [IsEmailConfirmed], [LastLoginAt], [FailedLoginAttempts], [MustChangePassword], [CreatedBy], [CreatedAt], [IsDeleted])
SELECT
    ROW_NUMBER() OVER (ORDER BY n) + 19,
    'teacher' + CAST(ROW_NUMBER() OVER (ORDER BY n) AS NVARCHAR),
    'teacher' + CAST(ROW_NUMBER() OVER (ORDER BY n) AS NVARCHAR) + '@gvi.edu.bd',
    '01700000' + RIGHT('0' + CAST(ROW_NUMBER() OVER (ORDER BY n) + 19 AS NVARCHAR), 2),
    @demoHash, 1, 1, @now, 0, 0, 'system', @now, 0
FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10),(11),(12),(13),(14),(15)) AS Nums(n)

INSERT INTO [Users] ([Id], [UserName], [Email], [PhoneNumber], [PasswordHash], [Status], [IsEmailConfirmed], [LastLoginAt], [FailedLoginAttempts], [MustChangePassword], [CreatedBy], [CreatedAt], [IsDeleted])
SELECT
    ROW_NUMBER() OVER (ORDER BY n) + 34,
    CASE WHEN n <= 5 THEN 'security' + CAST(n AS NVARCHAR) ELSE 'cleaner' + CAST(n-5 AS NVARCHAR) END,
    CASE WHEN n <= 5 THEN 'security' + CAST(n AS NVARCHAR) + '@gvi.edu.bd' ELSE 'cleaner' + CAST(n-5 AS NVARCHAR) + '@gvi.edu.bd' END,
    '01700000' + RIGHT('0' + CAST(ROW_NUMBER() OVER (ORDER BY n) + 34 AS NVARCHAR), 2),
    @demoHash, 1, 1, NULL, 0, 0, 'system', @now, 0
FROM (VALUES (1),(2),(3),(4),(5),(6),(7),(8),(9),(10)) AS Nums(n)
GO
SET IDENTITY_INSERT [Users] OFF
GO

-- User Roles
INSERT INTO [UserRoles] ([UserId], [RoleId])
VALUES
(1,1),(2,3),(3,4),(4,4),(5,16),(6,5),(7,5),(8,5),(9,12),(10,9),(11,10),(12,11),(13,13),(14,14),
(15,15),(16,15),(17,15),(18,15),(19,15),
(20,6),(21,6),(22,6),(23,6),(24,6),(25,6),(26,6),(27,6),(28,6),(29,6),(30,6),(31,6),(32,6),(33,6),(34,6),
(35,15),(36,15),(37,15),(38,15),(39,15),
(40,15),(41,15),(42,15),(43,15),(44,15)
GO

-- Permissions
SET IDENTITY_INSERT [Permissions] ON
GO
INSERT INTO [Permissions] ([Id], [Module], [ModuleName], [Action], [Code], [CanCreate], [CanRead], [CanUpdate], [CanDelete], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 'Students', 'Student Management', 'Full Access', 'Students.FullAccess', 1,1,1,1, 'system', @now, 0),
(2, 'Students', 'Student Management', 'Read', 'Students.Read', 0,1,0,0, 'system', @now, 0),
(3, 'Fees', 'Fee Management', 'Full Access', 'Fees.FullAccess', 1,1,1,1, 'system', @now, 0),
(4, 'Fees', 'Fee Management', 'Read', 'Fees.Read', 0,1,0,0, 'system', @now, 0),
(5, 'Accounting', 'Accounting', 'Full Access', 'Accounting.FullAccess', 1,1,1,1, 'system', @now, 0),
(6, 'Exam', 'Exam Management', 'Full Access', 'Exam.FullAccess', 1,1,1,1, 'system', @now, 0),
(7, 'Result', 'Result Management', 'Full Access', 'Result.FullAccess', 1,1,1,1, 'system', @now, 0),
(8, 'Attendance', 'Attendance Management', 'Full Access', 'Attendance.FullAccess', 1,1,1,1, 'system', @now, 0),
(9, 'Settings', 'Settings', 'Manage', 'Settings.Manage', 1,1,1,1, 'system', @now, 0)
GO
SET IDENTITY_INSERT [Permissions] OFF
GO

INSERT INTO [RolePermissions] ([RoleId], [PermissionId])
VALUES
(1,1),(1,2),(1,3),(1,4),(1,5),(1,6),(1,7),(1,8),(1,9),
(2,1),(2,2),(2,3),(2,4),(2,5),(2,6),(2,7),(2,8),(2,9),
(3,2),(3,4),(3,6),(3,7),(3,8),
(4,2),(4,4),
(16,3),(16,4),(16,5),
(5,3),(5,4),
(6,2),(6,8),
(11,6),(11,7),
(12,1)
GO

-- ============================================================
-- 10. EMPLOYEES
-- ============================================================
SET IDENTITY_INSERT [Employees] ON
GO
DECLARE @empNow DATETIME = GETUTCDATE()

INSERT INTO [Employees] ([Id],[EmployeeCode],[FullName],[Gender],[DateOfBirth],[BloodGroup],[Religion],[Nationality],[Phone],[Email],[PresentAddress],[PermanentAddress],[JoiningDate],[DepartmentId],[DesignationId],[EmployeeType],[IsTeachingStaff],[Status],[UserId],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'EMP001','Md. Kamal Hossain','Male','1980-05-15','A+','Islam','Bangladeshi','01711111101','kamal.hossain@gvi.edu.bd','12 Gulshan Ave, Dhaka','Bogura','2018-01-01',2,1,'Full-Time',0,'Active',1,'system',@empNow,0),
(2,'EMP002','Prof. Dr. Ayesha Begum','Female','1975-03-20','B+','Islam','Bangladeshi','01711111102','ayesha@gvi.edu.bd','45 Banani, Dhaka','Tangail','2019-01-01',1,2,'Full-Time',0,'Active',2,'system',@empNow,0),
(3,'EMP003','Md. Shahidul Islam','Male','1982-07-10','O+','Islam','Bangladeshi','01711111103','shahidul@gvi.edu.bd','78 Dhanmondi, Dhaka','Narsingdi','2020-01-01',1,3,'Full-Time',1,'Active',3,'system',@empNow,0),
(4,'EMP004','Farida Yasmin','Female','1984-11-25','AB+','Islam','Bangladeshi','01711111104','farida@gvi.edu.bd','34 Uttara, Dhaka','Chandpur','2020-06-01',2,3,'Full-Time',0,'Active',4,'system',@empNow,0),
(5,'EMP005','Md. Rafiqul Islam','Male','1978-09-12','A-','Islam','Bangladeshi','01711111105','rafiqul@gvi.edu.bd','56 Mirpur, Dhaka','Sirajganj','2019-06-01',3,4,'Full-Time',0,'Active',5,'system',@empNow,0),
(6,'EMP006','Md. Nurul Absar','Male','1985-01-30','B-','Islam','Bangladeshi','01711111106','nurul@gvi.edu.bd','89 Mohammadpur, Dhaka','Natore','2021-01-01',3,5,'Full-Time',0,'Active',6,'system',@empNow,0),
(7,'EMP007','Sultana Razia','Female','1987-04-18','O-','Islam','Bangladeshi','01711111107','sultana@gvi.edu.bd','23 Shyamoli, Dhaka','Faridpur','2021-06-01',3,5,'Full-Time',0,'Active',7,'system',@empNow,0),
(8,'EMP008','Md. Moniruzzaman','Male','1986-08-05','A+','Islam','Bangladeshi','01711111108','moniruzzaman@gvi.edu.bd','67 Lalmatia, Dhaka','Jhenaidah','2022-01-01',3,5,'Full-Time',0,'Active',8,'system',@empNow,0),
(9,'EMP009','Abdul Karim','Male','1983-12-22','B+','Islam','Bangladeshi','01711111109','abdul.karim@gvi.edu.bd','90 Kakrail, Dhaka','Pirojpur','2020-03-01',9,8,'Full-Time',0,'Active',9,'system',@empNow,0),
(10,'EMP010','Khadiza Akhter','Female','1988-06-14','AB-','Islam','Bangladeshi','01711111110','khadiza@gvi.edu.bd','15 Rampura, Dhaka','Faridpur','2021-11-01',4,9,'Full-Time',0,'Active',10,'system',@empNow,0),
(11,'EMP011','Md. Jashim Uddin','Male','1980-10-08','O+','Islam','Bangladeshi','01711111111','jashim@gvi.edu.bd','22 Motijheel, Dhaka','Barisal','2019-09-01',5,10,'Full-Time',0,'Active',11,'system',@empNow,0),
(12,'EMP012','Md. Abul Kalam','Male','1979-02-28','A+','Islam','Bangladeshi','01711111112','abul.kalam@gvi.edu.bd','44 New Market, Dhaka','Rajshahi','2020-07-01',10,11,'Full-Time',0,'Active',12,'system',@empNow,0),
(13,'EMP013','Nasrin Sultana','Female','1985-05-05','B+','Islam','Bangladeshi','01711111113','nasrin@gvi.edu.bd','55 Bashundhara, Dhaka','Comilla','2021-04-01',1,12,'Full-Time',1,'Active',13,'system',@empNow,0),
(14,'EMP014','Md. Shafiqul Islam','Male','1987-09-15','O-','Islam','Bangladeshi','01711111114','shafiqul.it@gvi.edu.bd','88 Badda, Dhaka','Rangpur','2022-02-01',8,13,'Full-Time',0,'Active',14,'system',@empNow,0),
(15,'EMP015','Shamima Akhter','Female','1990-03-08','A-','Islam','Bangladeshi','01711111115','shamima@gvi.edu.bd','12 Malibagh, Dhaka','Dhaka','2022-06-01',2,14,'Full-Time',0,'Active',15,'system',@empNow,0),
(16,'EMP016','Md. Rubel Hossain','Male','1991-07-12','B+','Islam','Bangladeshi','01711111116','rubel@gvi.edu.bd','34 Jatrabari, Dhaka','Feni','2023-01-01',2,14,'Full-Time',0,'Active',16,'system',@empNow,0),
(17,'EMP017','Fatema Akhter','Female','1992-11-20','AB+','Islam','Bangladeshi','01711111117','fatema@gvi.edu.bd','56 Kallyanpur, Dhaka','Gopalganj','2023-03-01',2,14,'Full-Time',0,'Active',17,'system',@empNow,0),
(18,'EMP018','Md. Sohel Rana','Male','1989-12-01','O+','Islam','Bangladeshi','01711111118','sohel@gvi.edu.bd','78 Khilkhet, Dhaka','Jamalpur','2022-09-01',2,14,'Full-Time',0,'Active',18,'system',@empNow,0),
(19,'EMP019','Parvin Sultana','Female','1990-08-15','B-','Islam','Bangladeshi','01711111119','parvin@gvi.edu.bd','90 Basabo, Dhaka','Kishoreganj','2023-06-01',2,14,'Full-Time',0,'Active',19,'system',@empNow,0)

INSERT INTO [Employees] ([Id],[EmployeeCode],[FullName],[Gender],[DateOfBirth],[BloodGroup],[Religion],[Nationality],[Phone],[Email],[PresentAddress],[PermanentAddress],[JoiningDate],[DepartmentId],[DesignationId],[EmployeeType],[IsTeachingStaff],[Status],[UserId],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(20,'EMP020','Md. Mizanur Rahman','Male','1985-03-15','A+','Islam','Bangladeshi','01711111120','mizan.rahman@gvi.edu.bd','12 Banasree, Dhaka','Mymensingh','2020-01-01',1,7,'Full-Time',1,'Active',20,'system',@empNow,0),
(21,'EMP021','Shahina Akhter','Female','1988-06-22','B+','Islam','Bangladeshi','01711111121','shahina.akhter@gvi.edu.bd','45 Basundhara, Dhaka','Mymensingh','2020-06-01',1,7,'Full-Time',1,'Active',21,'system',@empNow,0),
(22,'EMP022','Md. Abdul Gaffar','Male','1983-09-10','O+','Islam','Bangladeshi','01711111122','abdul.gaffar@gvi.edu.bd','78 Mirpur 12, Dhaka','Mymensingh','2019-01-01',1,6,'Full-Time',1,'Active',22,'system',@empNow,0),
(23,'EMP023','Rebeka Sultana','Female','1990-12-05','AB-','Islam','Bangladeshi','01711111123','rebeka.sultana@gvi.edu.bd','34 Motijheel, Dhaka','Mymensingh','2021-01-01',1,7,'Full-Time',1,'Active',23,'system',@empNow,0),
(24,'EMP024','Md. Anwar Hossain','Male','1986-04-18','B-','Islam','Bangladeshi','01711111124','anwar.hossain@gvi.edu.bd','56 Uttara 5, Dhaka','Mymensingh','2020-03-01',1,7,'Full-Time',1,'Active',24,'system',@empNow,0),
(25,'EMP025','Moushumi Akhter','Female','1992-07-25','A-','Islam','Bangladeshi','01711111125','moushumi@gvi.edu.bd','89 Dhanmondi 27, Dhaka','Mymensingh','2021-06-01',1,7,'Full-Time',1,'Active',25,'system',@empNow,0),
(26,'EMP026','Md. Rafiqul Hasan','Male','1984-01-30','A+','Islam','Bangladeshi','01711111126','rafiqul.hasan@gvi.edu.bd','22 Mohammadpur, Dhaka','Mymensingh','2019-06-01',1,6,'Full-Time',1,'Active',26,'system',@empNow,0),
(27,'EMP027','Hasina Begum','Female','1989-08-12','O+','Islam','Bangladeshi','01711111127','hasina@gvi.edu.bd','44 Shyamoli, Dhaka','Mymensingh','2020-09-01',1,7,'Full-Time',1,'Active',27,'system',@empNow,0),
(28,'EMP028','Md. Mozammel Haque','Male','1987-05-20','B+','Islam','Bangladeshi','01711111128','mozammel@gvi.edu.bd','66 Kallyanpur, Dhaka','Mymensingh','2021-03-01',1,7,'Full-Time',1,'Active',28,'system',@empNow,0),
(29,'EMP029','Salma Parvin','Female','1991-10-08','AB+','Islam','Bangladeshi','01711111129','salma.parvin@gvi.edu.bd','77 Badda, Dhaka','Mymensingh','2022-01-01',1,7,'Full-Time',1,'Active',29,'system',@empNow,0),
(30,'EMP030','Md. Delwar Hossain','Male','1982-11-15','O-','Islam','Bangladeshi','01711111130','delwar@gvi.edu.bd','88 Rampura, Dhaka','Mymensingh','2018-06-01',1,6,'Full-Time',1,'Active',30,'system',@empNow,0),
(31,'EMP031','Nasima Khatun','Female','1993-02-28','A+','Islam','Bangladeshi','01711111131','nasima.khatun@gvi.edu.bd','12 Banani, Dhaka','Mymensingh','2022-06-01',1,7,'Full-Time',1,'Active',31,'system',@empNow,0),
(32,'EMP032','Md. Shamsul Alam','Male','1985-09-05','B-','Islam','Bangladeshi','01711111132','shamsul.alam@gvi.edu.bd','34 Gulshan 2, Dhaka','Mymensingh','2020-06-01',1,6,'Full-Time',1,'Active',32,'system',@empNow,0),
(33,'EMP033','Tahmina Akhter','Female','1990-04-12','O+','Islam','Bangladeshi','01711111133','tahmina@gvi.edu.bd','56 Khilkhet, Dhaka','Netrokona','2021-09-01',1,7,'Full-Time',1,'Active',33,'system',@empNow,0),
(34,'EMP034','Md. Shahjahan Ali','Male','1983-07-22','AB-','Islam','Bangladeshi','01711111134','shahjahan.ali@gvi.edu.bd','78 Mirpur 1, Dhaka','Netrokona','2019-09-01',1,6,'Full-Time',1,'Active',34,'system',@empNow,0)

INSERT INTO [Employees] ([Id],[EmployeeCode],[FullName],[Gender],[DateOfBirth],[BloodGroup],[Religion],[Nationality],[Phone],[Email],[PresentAddress],[PermanentAddress],[JoiningDate],[DepartmentId],[DesignationId],[EmployeeType],[IsTeachingStaff],[Status],[UserId],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(35,'EMP035','Md. Abdul Hamid','Male','1975-01-10','A+','Islam','Bangladeshi','01711111135',NULL,'Staff Qtr, Dhaka','Madaripur','2020-01-01',6,15,'Full-Time',0,'Active',35,'system',@empNow,0),
(36,'EMP036','Md. Joynal Abedin','Male','1978-03-22','B+','Islam','Bangladeshi','01711111136',NULL,'Staff Qtr, Dhaka','Shariatpur','2020-03-01',6,15,'Full-Time',0,'Active',36,'system',@empNow,0),
(37,'EMP037','Md. Sirajul Islam','Male','1976-07-15','O+','Islam','Bangladeshi','01711111137',NULL,'Staff Qtr, Dhaka','Shariatpur','2020-06-01',6,15,'Full-Time',0,'Active',37,'system',@empNow,0),
(38,'EMP038','Md. Kuddus Ali','Male','1979-11-30','AB+','Islam','Bangladeshi','01711111138',NULL,'Staff Qtr, Dhaka','Shariatpur','2021-01-01',6,15,'Full-Time',0,'Active',38,'system',@empNow,0),
(39,'EMP039','Md. Hanif Sheikh','Male','1977-05-18','B-','Islam','Bangladeshi','01711111139',NULL,'Staff Qtr, Dhaka','Shariatpur','2021-03-01',6,15,'Full-Time',0,'Active',39,'system',@empNow,0),
(40,'EMP040','Mst. Jahanara Begum','Female','1980-02-15','A-','Islam','Bangladeshi','01711111140',NULL,'Staff Qtr, Dhaka','Shariatpur','2021-06-01',7,16,'Full-Time',0,'Active',40,'system',@empNow,0),
(41,'EMP041','Mst. Saleha Khatun','Female','1982-08-20','O+','Islam','Bangladeshi','01711111141',NULL,'Staff Qtr, Dhaka','Shariatpur','2021-09-01',7,16,'Full-Time',0,'Active',41,'system',@empNow,0),
(42,'EMP042','Mst. Rahima Akhter','Female','1985-04-10','B+','Islam','Bangladeshi','01711111142',NULL,'Staff Qtr, Dhaka','Shariatpur','2022-01-01',7,16,'Full-Time',0,'Active',42,'system',@empNow,0),
(43,'EMP043','Mst. Amina Begum','Female','1983-10-25','AB-','Islam','Bangladeshi','01711111143',NULL,'Staff Qtr, Dhaka','Shariatpur','2022-03-01',7,16,'Full-Time',0,'Active',43,'system',@empNow,0),
(44,'EMP044','Mst. Shahnaj Parvin','Female','1986-12-05','A+','Islam','Bangladeshi','01711111144',NULL,'Staff Qtr, Dhaka','Shariatpur','2022-06-01',7,16,'Full-Time',0,'Active',44,'system',@empNow,0)
GO
SET IDENTITY_INSERT [Employees] OFF
GO

-- Teachers
SET IDENTITY_INSERT [Teachers] ON
GO
INSERT INTO [Teachers] ([Id],[EmployeeId],[TeacherCode],[SubjectSpecialization],[TeachingLevel],[IsClassTeacher],[IsExamController],[IsRoutineCoordinator],[TeachingExperienceYears],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,20,'TCH001','Bangla','Secondary',0,0,0,8,'system',@empNow,0),
(2,21,'TCH002','English','Secondary',0,0,0,6,'system',@empNow,0),
(3,22,'TCH003','Mathematics','Secondary',0,0,0,10,'system',@empNow,0),
(4,23,'TCH004','Science','Secondary',0,0,0,5,'system',@empNow,0),
(5,24,'TCH005','Social Science','Secondary',0,0,0,7,'system',@empNow,0),
(6,25,'TCH006','ICT','Secondary',0,0,0,4,'system',@empNow,0),
(7,26,'TCH007','Physics','Higher Secondary',1,0,0,9,'system',@empNow,0),
(8,27,'TCH008','Chemistry','Secondary',0,0,0,5,'system',@empNow,0),
(9,28,'TCH009','Biology','Secondary',0,0,0,6,'system',@empNow,0),
(10,29,'TCH010','English','Secondary',0,0,0,4,'system',@empNow,0),
(11,30,'TCH011','Mathematics','Higher Secondary',1,0,0,11,'system',@empNow,0),
(12,31,'TCH012','Bangla','Primary',0,0,0,3,'system',@empNow,0),
(13,32,'TCH013','Science','Higher Secondary',0,1,0,8,'system',@empNow,0),
(14,33,'TCH014','Social Science','Primary',0,0,0,4,'system',@empNow,0),
(15,34,'TCH015','Accounting','Higher Secondary',1,0,0,10,'system',@empNow,0)
GO
SET IDENTITY_INSERT [Teachers] OFF
GO

-- Designation-Role Mappings
INSERT INTO [DesignationRoleMappings] ([DesignationId],[RoleId],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,1,1,'system',@empNow,0),(2,3,1,'system',@empNow,0),(3,4,1,'system',@empNow,0),
(4,16,1,'system',@empNow,0),(5,5,1,'system',@empNow,0),(6,6,1,'system',@empNow,0),
(7,6,1,'system',@empNow,0),(8,12,1,'system',@empNow,0),(9,9,1,'system',@empNow,0),
(10,10,1,'system',@empNow,0),(11,11,1,'system',@empNow,0),(12,13,1,'system',@empNow,0),
(13,14,1,'system',@empNow,0),(14,15,1,'system',@empNow,0),(15,15,1,'system',@empNow,0),
(16,15,1,'system',@empNow,0)
GO

-- Employee Salaries
INSERT INTO [EmployeeSalaries] ([EmployeeId],[BasicSalary],[HouseRent],[MedicalAllowance],[TransportAllowance],[OtherAllowance],[Deduction],[TotalSalary],[EffectiveFrom],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT e.Id,
    CASE WHEN e.DesignationId <= 2 THEN 80000 WHEN e.DesignationId <= 4 THEN 50000 WHEN e.DesignationId <= 5 THEN 35000 WHEN e.DesignationId <= 7 THEN 25000 WHEN e.DesignationId <= 8 THEN 20000 WHEN e.DesignationId <= 13 THEN 15000 ELSE 8000 END,
    CASE WHEN e.DesignationId <= 2 THEN 40000 WHEN e.DesignationId <= 4 THEN 25000 WHEN e.DesignationId <= 5 THEN 15000 WHEN e.DesignationId <= 7 THEN 10000 WHEN e.DesignationId <= 8 THEN 8000 WHEN e.DesignationId <= 13 THEN 5000 ELSE 3000 END,
    CASE WHEN e.DesignationId <= 2 THEN 5000 WHEN e.DesignationId <= 4 THEN 3000 WHEN e.DesignationId <= 7 THEN 2000 ELSE 1500 END,
    CASE WHEN e.DesignationId <= 7 THEN 3000 ELSE 1500 END,
    2000, 500,
    CASE WHEN e.DesignationId <= 2 THEN 127000 WHEN e.DesignationId <= 4 THEN 77300 WHEN e.DesignationId <= 5 THEN 51700 WHEN e.DesignationId <= 7 THEN 36700 WHEN e.DesignationId <= 8 THEN 29700 WHEN e.DesignationId <= 13 THEN 21700 ELSE 11800 END,
    '2027-01-01', 'system', @empNow, 0
FROM [Employees] e
GO

-- ============================================================
-- 11. GUARDIANS (300 — one per student)
-- ============================================================
SET IDENTITY_INSERT [Guardians] ON
GO
INSERT INTO [Guardians] ([Id],[FullName],[Relation],[Gender],[Phone],[Email],[Address],[Occupation],[NidNumber],[IsPrimary],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'Md. Anowar Hossain','Father','Male','01730000001','anowar.hossain@gmail.com','12/B, Mirpur 10, Dhaka','Businessman','1234567890',1,'system',GETUTCDATE(),0),
(2,'Mst. Rebeka Begum','Mother','Female','01730000002','rebeka.begum@gmail.com','45/C, Banasree, Dhaka','Housewife','2234567890',0,'system',GETUTCDATE(),0),
(3,'Md. Rafiqul Islam','Father','Male','01730000003','rafiqul.islam@gmail.com','78/D, Bashundhara, Dhaka','Government Service','3234567890',1,'system',GETUTCDATE(),0),
(4,'Mst. Hasina Parvin','Mother','Female','01730000004','hasina.parvin@gmail.com','34/A, Kallyanpur, Dhaka','Teacher','4234567890',0,'system',GETUTCDATE(),0),
(5,'Md. Abdul Jalil','Father','Male','01730000005','abdul.jalil@gmail.com','56/E, Rampura, Dhaka','Businessman','5234567890',1,'system',GETUTCDATE(),0),
(6,'Mst. Saleha Khatun','Mother','Female','01730000006','saleha.khatun@gmail.com','89/B, Lalmatia, Dhaka','Housewife','6234567890',0,'system',GETUTCDATE(),0),
(7,'Md. Shahidul Islam','Father','Male','01730000007','shahidul.islam@gmail.com','12/C, Dhanmondi 15, Dhaka','Businessman','7234567890',1,'system',GETUTCDATE(),0),
(8,'Mst. Nasima Begum','Mother','Female','01730000008','nasima.begum@gmail.com','45/D, Gulshan 1, Dhaka','Housewife','8234567890',0,'system',GETUTCDATE(),0),
(9,'Md. Khairul Alam','Father','Male','01730000009','khairul.alam@gmail.com','78/F, Uttara 3, Dhaka','Service Holder','9234567890',1,'system',GETUTCDATE(),0),
(10,'Mst. Josna Begum','Mother','Female','01730000010','josna.begum@gmail.com','22/E, Banani, Dhaka','Housewife','1034567890',0,'system',GETUTCDATE(),0),
(11,'Md. Jasim Uddin','Father','Male','01730000011','jasim.uddin@gmail.com','34/G, Motijheel, Dhaka','Businessman','1134567890',1,'system',GETUTCDATE(),0),
(12,'Mst. Momena Khatun','Mother','Female','01730000012','momena.khatun@gmail.com','56/H, Malibagh, Dhaka','Housewife','1234567891',0,'system',GETUTCDATE(),0),
(13,'Md. Abul Hossain','Father','Male','01730000013','abul.hossain@gmail.com','78/I, Badda, Dhaka','Service Holder','1334567890',1,'system',GETUTCDATE(),0),
(14,'Mst. Nurun Nahar','Mother','Female','01730000014','nurun.nahar@gmail.com','90/J, Wari, Dhaka','Housewife','1434567890',0,'system',GETUTCDATE(),0),
(15,'Md. Mizanur Rahman','Father','Male','01730000015','mizan.rahman@gmail.com','12/K, Shyamoli, Dhaka','Businessman','1534567890',1,'system',GETUTCDATE(),0),
(16,'Mst. Parvin Sultana','Mother','Female','01730000016','parvin.sultana@gmail.com','45/L, Mohammadpur, Dhaka','Housewife','1634567890',0,'system',GETUTCDATE(),0),
(17,'Md. Nuruzzaman','Father','Male','01730000017','nuruzzaman@gmail.com','78/M, Jatrabari, Dhaka','Service Holder','1734567890',1,'system',GETUTCDATE(),0),
(18,'Mst. Khaleda Begum','Mother','Female','01730000018','khaleda.begum@gmail.com','34/N, Khilgaon, Dhaka','Teacher','1834567890',0,'system',GETUTCDATE(),0),
(19,'Md. Hamidur Rahman','Father','Male','01730000019','hamidur.rahman@gmail.com','56/O, Basabo, Dhaka','Businessman','1934567890',1,'system',GETUTCDATE(),0),
(20,'Mst. Shahida Akhter','Mother','Female','01730000020','shahida.akhter@gmail.com','89/P, Shantinagar, Dhaka','Housewife','2034567890',0,'system',GETUTCDATE(),0)

GO
DECLARE @i INT = 21
WHILE @i <= 300
BEGIN
    INSERT INTO [Guardians] ([Id],[FullName],[Relation],[Gender],[Phone],[Email],[Address],[Occupation],[NidNumber],[IsPrimary],[CreatedBy],[CreatedAt],[IsDeleted])
    VALUES (
        @i,
        CASE WHEN @i % 2 = 1 THEN 'Md. Guardian ' + CAST(@i AS NVARCHAR) + ' Rahman' ELSE 'Mst. Guardian ' + CAST(@i AS NVARCHAR) + ' Begum' END,
        CASE WHEN @i % 2 = 1 THEN 'Father' ELSE 'Mother' END,
        CASE WHEN @i % 2 = 1 THEN 'Male' ELSE 'Female' END,
        '0173' + RIGHT('00000' + CAST(@i AS NVARCHAR), 6),
        'guardian' + CAST(@i AS NVARCHAR) + '@gmail.com',
        CASE @i % 5 WHEN 0 THEN 'Mirpur, Dhaka' WHEN 1 THEN 'Uttara, Dhaka' WHEN 2 THEN 'Dhanmondi, Dhaka' WHEN 3 THEN 'Bashundhara, Dhaka' ELSE 'Motijheel, Dhaka' END,
        CASE @i % 4 WHEN 0 THEN 'Businessman' WHEN 1 THEN 'Service Holder' WHEN 2 THEN 'Teacher' ELSE 'Grocer' END,
        CAST(30000000 + @i AS NVARCHAR),
        CASE WHEN @i % 2 = 1 THEN 1 ELSE 0 END,
        'system', GETUTCDATE(), 0
    )
    SET @i = @i + 1
END
GO
SET IDENTITY_INSERT [Guardians] OFF
GO

PRINT '>>> Guardians done'
GO

-- ============================================================
-- 12. STUDENTS (300 — Play(1-3) through Class 10)
-- ============================================================
SET IDENTITY_INSERT [Students] ON
GO

-- First name pool (Bangladeshi)
DECLARE @firstNamePool TABLE (id INT, name_en NVARCHAR(50), name_bn NVARCHAR(50), gender NVARCHAR(10))
INSERT INTO @firstNamePool VALUES
(1,'Md. Rahim','রহিম','Male'),(2,'Md. Karim','করিম','Male'),(3,'Md. Hasan','হাসান','Male'),
(4,'Md. Hossain','হোসেন','Male'),(5,'Md. Kamal','কামাল','Male'),(6,'Md. Jamal','জামাল','Male'),
(7,'Md. Shahid','শহীদ','Male'),(8,'Md. Anwar','আনোয়ার','Male'),(9,'Md. Farid','ফরিদ','Male'),
(10,'Md. Shafiq','শফিক','Male'),(11,'Md. Sohel','সোহেল','Male'),(12,'Md. Rakib','রাকিব','Male'),
(13,'Md. Rubel','রুবেল','Male'),(14,'Md. Tuhin','তুহিন','Male'),(15,'Md. Shohan','শহন','Male'),
(16,'Md. Arif','আরিফ','Male'),(17,'Md. Shohag','সোহাগ','Male'),(18,'Md. Russell','রাসেল','Male'),
(19,'Md. Jahid','জাহিদ','Male'),(20,'Md. Masud','মাসুদ','Male'),(21,'Md. Mizan','মিজান','Male'),
(22,'Md. Jahangir','জাহাঙ্গীর','Male'),(23,'Md. Monir','মনির','Male'),(24,'Md. Azad','আজাদ','Male'),
(25,'Md. Mostafa','মোস্তফা','Male'),(26,'Mst. Rina','রীনা','Female'),(27,'Mst. Mina','মিনা','Female'),
(28,'Mst. Tania','তানিয়া','Female'),(29,'Mst. Nahar','নাহার','Female'),(30,'Mst. Parvin','পারভীন','Female'),
(31,'Mst. Salma','সালমা','Female'),(32,'Mst. Nasrin','নাসরিন','Female'),(33,'Mst. Sabina','সাবিনা','Female'),
(34,'Mst. Shahida','শাহিদা','Female'),(35,'Mst. Roksana','রোকসানা','Female'),(36,'Mst. Kohinoor','কোহিনুর','Female'),
(37,'Mst. Jahanara','জাহানারা','Female'),(38,'Mst. Shahnaj','শাহনাজ','Female'),(39,'Mst. Fahmida','ফাহমিদা','Female'),
(40,'Mst. Taslima','তসলিমা','Female'),(41,'Mst. Rabeya','রাবেয়া','Female'),(42,'Mst. Sharmin','শারমিন','Female'),
(43,'Mst. Nasima','নাসিমা','Female'),(44,'Mst. Khadiza','খাদিজা','Female'),(45,'Mst. Ayesha','আয়েশা','Female'),
(46,'Mst. Farhana','ফারহানা','Female'),(47,'Mst. Shimul','শিমুল','Female'),(48,'Mst. Beauty','বিউটি','Female'),
(49,'Mst. Moushumi','মৌসুমী','Female'),(50,'Mst. Lucky','লাকী','Female')

-- Last name pool
DECLARE @lastNamePool TABLE (id INT, name_en NVARCHAR(50), name_bn NVARCHAR(50))
INSERT INTO @lastNamePool VALUES
(1,'Islam','ইসলাম'),(2,'Hossain','হোসেন'),(3,'Rahman','রহমান'),(4,'Mia','মিয়া'),
(5,'Sarder','সরদার'),(6,'Sarker','সরকার'),(7,'Biswas','বিশ্বাস'),(8,'Chowdhury','চৌধুরী'),
(9,'Sheikh','শেখ'),(10,'Haque','হক'),(11,'Talukder','তালুকদার'),(12,'Khan','খান'),
(13,'Mollah','মোল্লা'),(14,'Howlader','হাওলাদার'),(15,'Gazi','গাজী'),(16,'Shah','শাহ')

DECLARE @studentId INT = 1
DECLARE @classId INT, @section NVARCHAR(2), @shift NVARCHAR(20), @studentGender NVARCHAR(10)
DECLARE @fname NVARCHAR(50), @fbn NVARCHAR(50), @lname NVARCHAR(50), @lbn NVARCHAR(50)
DECLARE @dob DATE, @admissionDate DATE = '2027-01-01'
DECLARE @fnR INT, @lnR INT, @cId INT, @sR INT
DECLARE @studentEmail NVARCHAR(100)
DECLARE @roll INT

WHILE @studentId <= 300
BEGIN
    -- Class distribution: Play(1-3)=15, Nursery(2)=15, KG(3)=15, C1=30, C2=30, C3=30, C4=30, C5=30, C6=25, C7=25, C8=20, C9=20, C10=15
    SET @classId = CASE
        WHEN @studentId <= 15 THEN 1  -- Play
        WHEN @studentId <= 30 THEN 2  -- Nursery
        WHEN @studentId <= 45 THEN 3  -- KG
        WHEN @studentId <= 75 THEN 4  -- Class 1
        WHEN @studentId <= 105 THEN 5 -- Class 2
        WHEN @studentId <= 135 THEN 6 -- Class 3
        WHEN @studentId <= 165 THEN 7 -- Class 4
        WHEN @studentId <= 195 THEN 8 -- Class 5
        WHEN @studentId <= 220 THEN 9 -- Class 6
        WHEN @studentId <= 245 THEN 10 -- Class 7
        WHEN @studentId <= 265 THEN 11 -- Class 8
        WHEN @studentId <= 285 THEN 12 -- Class 9
        ELSE 13 -- Class 10
    END

    SET @section = CASE @classId
        WHEN 1 THEN 'A' WHEN 2 THEN 'A' WHEN 3 THEN 'A'
        WHEN 4 THEN CASE WHEN @studentId <= 60 THEN 'A' ELSE 'B' END
        WHEN 5 THEN CASE WHEN @studentId <= 90 THEN 'A' ELSE 'B' END
        WHEN 6 THEN CASE WHEN @studentId <= 120 THEN 'A' ELSE 'B' END
        WHEN 7 THEN CASE WHEN @studentId <= 150 THEN 'A' ELSE 'B' END
        WHEN 8 THEN CASE WHEN @studentId <= 180 THEN 'A' ELSE 'B' END
        WHEN 9 THEN 'A' WHEN 10 THEN 'A' WHEN 11 THEN 'A' WHEN 12 THEN 'A' ELSE 'A'
    END

    SET @shift = CASE WHen @classId <= 8 THEN 'Morning' ELSE 'Day' END

    -- Gender: mix
    SET @sR = (@studentId * 7) % 2
    SET @studentGender = CASE WHEN @sR = 0 THEN 'Male' ELSE 'Female' END

    -- Pick random names
    SET @fnR = ((@studentId * 13 + 7) % 50) + 1
    SET @lnR = ((@studentId * 17 + 3) % 16) + 1
    SELECT @fname = name_en, @fbn = name_bn FROM @firstNamePool WHERE id = @fnR
    SELECT @lname = name_en, @lbn = name_bn FROM @lastNamePool WHERE id = @lnR

    -- DOB based on class
    SET @dob = DATEADD(YEAR, -(@classId + 4 + (@studentId % 3)), @admissionDate)

    -- Roll number within section
    SET @roll = (@studentId - 1) % 30 + 1

    SET @studentEmail = LOWER(REPLACE(@fname, 'Md. ', '') + REPLACE(@lname, ' ', '') + CAST(@studentId AS NVARCHAR)) + '@student.gvi.edu.bd'

    INSERT INTO [Students] ([Id],[StudentId],[FullName],[FullNameBn],[Gender],[DateOfBirth],[BloodGroup],[Religion],[Nationality],[Phone],[Email],[PresentAddress],[PermanentAddress],[FatherName],[MotherName],[FatherOccupation],[MotherOccupation],[ClassId],[Section],[Shift],[RollNumber],[AcademicYear],[SessionYear],[AdmissionDate],[GuardianId],[IsActive],[IsTransferOut],[CreatedBy],[CreatedAt],[IsDeleted])
    VALUES (
        @studentId,
        'GVI-' + RIGHT('0000' + CAST(@studentId AS NVARCHAR), 4) + '-2027',
        @fname + ' ' + @lname,
        @fbn + ' ' + @lbn,
        @studentGender,
        @dob,
        CASE (@studentId * 11) % 8 WHEN 0 THEN 'A+' WHEN 1 THEN 'A-' WHEN 2 THEN 'B+' WHEN 3 THEN 'B-' WHEN 4 THEN 'O+' WHEN 5 THEN 'O-' WHEN 6 THEN 'AB+' ELSE 'AB-' END,
        CASE @fnR % 4 WHEN 0 THEN 'Islam' WHEN 1 THEN 'Islam' WHEN 2 THEN 'Hindu' ELSE 'Islam' END,
        'Bangladeshi',
        '0174' + RIGHT('00000' + CAST(@studentId AS NVARCHAR), 6),
        @studentEmail,
        CASE @classId WHEN 1 THEN '12 Mirpur, Dhaka' WHEN 2 THEN '45 Uttara, Dhaka' WHEN 3 THEN '78 Dhanmondi, Dhaka' ELSE 'House ' + CAST(@studentId AS NVARCHAR) + ', Block ' + CHAR(65 + (@studentId % 26)) + ', Dhaka' END,
        'Village: ' + CAST(@studentId AS NVARCHAR) + ', Dist: Dhaka',
        CASE WHEN @sR = 0 THEN 'Md. Guardian ' + CAST(@studentId AS NVARCHAR) + ' Rahman' ELSE 'Md. Guardian ' + CAST(@studentId AS NVARCHAR) + ' Hossain' END,
        CASE WHEN @sR = 0 THEN 'Mst. Guardian ' + CAST(@studentId AS NVARCHAR) + ' Begum' ELSE 'Mst. Guardian ' + CAST(@studentId AS NVARCHAR) + ' Khatun' END,
        CASE @fnR % 5 WHEN 0 THEN 'Businessman' WHEN 1 THEN 'Service Holder' WHEN 2 THEN 'Teacher' WHEN 3 THEN 'Farmer' ELSE 'Grocer' END,
        'Housewife',
        @classId, @section, @shift, @roll,
        '2027', '2027', @admissionDate,
        @studentId, 1, 0, 'system', GETUTCDATE(), 0
    )

    SET @studentId = @studentId + 1
END
GO
SET IDENTITY_INSERT [Students] OFF
GO

PRINT '>>> Students done (300)'
GO

-- ============================================================
-- 13. STUDENT-GUARDIAN MAPPING + STUDENT ATTENDANCE
-- ============================================================
INSERT INTO [StudentGuardians] ([StudentId],[GuardianId],[Relationship],[IsPrimary],[IsEmergencyContact],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT Id, Id, 'Father', 1, 1, 'system', GETUTCDATE(), 0 FROM [Students]
GO

-- Student Attendance (Jan-Jun 2027, real distribution: Present 90%, Absent 4%, Late 3%, Leave 2%, Half Day 1%)
DECLARE @atdStudentId INT = 1, @atdDate DATE, @atdStatus INT, @atdCount INT
WHILE @atdStudentId <= 300
BEGIN
    SET @atdCount = 0
    SET @atdDate = '2027-01-01'
    WHILE @atdDate <= '2027-06-30'
    BEGIN
        -- Every weekday (Sat-Thu) generates attendance, Fri off
        IF DATEPART(WEEKDAY, @atdDate) != 6 -- Friday check
        BEGIN
            SET @atdStatus = CASE ABS(CHECKSUM(NEWID())) % 100
                WHEN 0 THEN 4 -- Leave (1%)
                WHEN 1 THEN 4
                WHEN 2 THEN 5 -- Half Day (1%)
                WHEN 3 THEN 3 -- Late (3%)
                WHEN 4 THEN 3
                WHEN 5 THEN 3
                WHEN 6 THEN 2 -- Absent (4%)
                WHEN 7 THEN 2
                WHEN 8 THEN 2
                WHEN 9 THEN 2
                ELSE 1 -- Present (90%)
            END
            INSERT INTO [StudentAttendances] ([StudentId],[ClassId],[SectionId],[AttendanceDate],[Status],[Remarks],[RecordedBy],[CreatedAt])
            VALUES (@atdStudentId,
                (SELECT ClassId FROM [Students] WHERE Id = @atdStudentId),
                1, @atdDate, @atdStatus,
                CASE @atdStatus WHEN 2 THEN 'Absent' WHEN 3 THEN 'Late' WHEN 4 THEN 'Leave' WHEN 5 THEN 'Half Day' ELSE NULL END,
                'system', GETUTCDATE())
        END
        SET @atdDate = DATEADD(DAY, 1, @atdDate)
        SET @atdCount = @atdCount + 1
    END
    SET @atdStudentId = @atdStudentId + 1
END
GO

PRINT '>>> Attendance done'
GO

-- ============================================================
-- 14. FEE STRUCTURES (Play→Class 10)
-- ============================================================
SET IDENTITY_INSERT [FeeStructures] ON
GO
INSERT INTO [FeeStructures] ([Id],[Name],[ClassId],[AcademicYear],[MonthlyFee],[AdmissionFee],[AnnualFee],[ExamFee],[LibraryFee],[SportsFee],[LabFee],[TransportFee],[OtherFee],[TotalAmount],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'Play Session Fee',1,'2027',1500,3000,2000,500,200,200,0,0,500,7900,1,'system',GETUTCDATE(),0),
(2,'Nursery Session Fee',2,'2027',1800,3500,2500,600,250,250,0,0,500,9650,1,'system',GETUTCDATE(),0),
(3,'KG Session Fee',3,'2027',2000,4000,3000,700,300,300,0,0,500,10800,1,'system',GETUTCDATE(),0),
(4,'Class 1 Session Fee',4,'2027',2500,5000,3500,800,300,300,500,0,600,13500,1,'system',GETUTCDATE(),0),
(5,'Class 2 Session Fee',5,'2027',2500,5000,3500,800,300,300,500,0,600,13500,1,'system',GETUTCDATE(),0),
(6,'Class 3 Session Fee',6,'2027',2800,5000,4000,1000,400,400,600,0,700,14900,1,'system',GETUTCDATE(),0),
(7,'Class 4 Session Fee',7,'2027',2800,5000,4000,1000,400,400,600,0,700,14900,1,'system',GETUTCDATE(),0),
(8,'Class 5 Session Fee',8,'2027',3000,5500,4500,1200,500,500,700,0,800,16700,1,'system',GETUTCDATE(),0),
(9,'Class 6 Session Fee',9,'2027',3500,6000,5000,1500,500,500,800,1000,1000,19800,1,'system',GETUTCDATE(),0),
(10,'Class 7 Session Fee',10,'2027',3500,6000,5000,1500,500,500,800,1000,1000,19800,1,'system',GETUTCDATE(),0),
(11,'Class 8 Session Fee',11,'2027',4000,6000,5500,2000,600,600,1000,1200,1200,22100,1,'system',GETUTCDATE(),0),
(12,'Class 9 Session Fee',12,'2027',4500,7000,6000,2500,700,700,1200,1500,1500,25600,1,'system',GETUTCDATE(),0),
(13,'Class 10 Session Fee',13,'2027',5000,8000,7000,3000,800,800,1500,1800,1800,29700,1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [FeeStructures] OFF
GO

-- Assign fee structures to students
INSERT INTO [StudentFeeAssignments] ([StudentId],[FeeStructureId],[AcademicYearId],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT s.Id, fs.Id, 1, 1, 'system', GETUTCDATE(), 0
FROM [Students] s
JOIN [FeeStructures] fs ON s.ClassId = fs.ClassId AND fs.AcademicYear = '2027'
GO

PRINT '>>> Fee Structures done'
GO

-- ============================================================
-- 15. FEE INVOICES (Monthly: Jan-Jun 2027)
-- ============================================================
SET IDENTITY_INSERT [FeeInvoices] ON
GO
DECLARE @invStudentId INT = 1, @invMonth INT = 1, @invId INT = 1
WHILE @invId <= 1800
BEGIN
    SET @invStudentId = ((@invId - 1) / 6) + 1
    IF @invStudentId > 300 BREAK
    SET @invMonth = ((@invId - 1) % 6) + 1

    INSERT INTO [FeeInvoices] ([Id],[StudentId],[InvoiceNo],[DueDate],[TotalAmount],[PaidAmount],[DiscountAmount],[LateFee],[Status],[Remarks],[CreatedBy],[CreatedAt],[IsDeleted])
    SELECT
        @invId,
        s.Id,
        'INV-' + RIGHT('0000' + CAST(@invId AS NVARCHAR), 6) + '/2027',
        DATEADD(MONTH, @invMonth - 1, '2027-02-10'),
        fs.TotalAmount,
        0, 0, 0,
        CASE WHEN @invMonth <= 4 THEN 3 ELSE 1 END, -- 3=Paid, 1=Unpaid
        NULL,
        'system', GETUTCDATE(), 0
    FROM [Students] s
    JOIN [StudentFeeAssignments] sfa ON s.Id = sfa.StudentId AND sfa.AcademicYearId = 1
    JOIN [FeeStructures] fs ON sfa.FeeStructureId = fs.Id
    WHERE s.Id = @invStudentId

    SET @invId = @invId + 1
END
GO
SET IDENTITY_INSERT [FeeInvoices] OFF
GO

PRINT '>>> Fee Invoices done (1800)'
GO

-- ============================================================
-- 16. PAYMENTS (1000)
-- ============================================================
SET IDENTITY_INSERT [Payments] ON
GO
DECLARE @payId INT = 1
WHILE @payId <= 1000
BEGIN
    DECLARE @payInvoiceId INT = ((@payId - 1) * 6) % 1800 + 1
    DECLARE @payAmount DECIMAL(18,2), @payMethod INT, @payDate DATE, @payRef NVARCHAR(50)

    SET @payAmount = CASE @payId % 4
        WHEN 0 THEN ROUND(CAST(5000 + (@payId * 37) % 15000 AS DECIMAL), 2)
        WHEN 1 THEN ROUND(CAST(10000 + (@payId * 23) % 20000 AS DECIMAL), 2)
        WHEN 2 THEN ROUND(CAST(8000 + (@payId * 53) % 12000 AS DECIMAL), 2)
        ELSE ROUND(CAST(3000 + (@payId * 7) % 5000 AS DECIMAL), 2)
    END
    SET @payMethod = CASE @payId % 3 WHEN 0 THEN 1 WHEN 1 THEN 2 ELSE 3 END -- 1=Cash, 2=Bank, 3=SSLCommerz
    SET @payDate = DATEADD(DAY, -((@payId * 17) % 180), '2027-07-01')
    SET @payRef = CASE @payMethod
        WHEN 1 THEN 'CR-' + RIGHT('0000' + CAST(@payId AS NVARCHAR), 6)
        WHEN 2 THEN 'CHQ-' + RIGHT('0000' + CAST(@payId AS NVARCHAR), 6)
        ELSE 'SSL-' + RIGHT('0000' + CAST(@payId AS NVARCHAR), 6)
    END

    INSERT INTO [Payments] ([Id],[FeeInvoiceId],[PaidAt],[Amount],[Method],[ReferenceNo],[LateFee],[DiscountAmount],[Remarks],[CreatedBy],[CreatedAt],[IsDeleted])
    VALUES (@payId, @payInvoiceId, @payDate, @payAmount, @payMethod, @payRef,
        0, 0,
        CASE @payMethod WHEN 1 THEN 'Cash Payment' WHEN 2 THEN 'Bank Cheque' ELSE 'SSLCommerz Payment' END,
        'system', GETUTCDATE(), 0)

    SET @payId = @payId + 1
END
GO
SET IDENTITY_INSERT [Payments] OFF
GO

-- Update invoice paid amounts
UPDATE fi SET
    PaidAmount = ISNULL((SELECT SUM(Amount) FROM Payments WHERE FeeInvoiceId = fi.Id), 0)
FROM [FeeInvoices] fi
GO

PRINT '>>> Payments done (1000)'
GO

-- ============================================================
-- 17. EXAMS (All Classes × 3 Terms = 39 exams)
-- ============================================================
SET IDENTITY_INSERT [Exams] ON
GO
INSERT INTO [Exams] ([Id],[ExamName],[ExamType],[ClassId],[AcademicYear],[StartDate],[EndDate],[IsResultPublished],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
-- Play (Class 1)
(1,'1st Term Exam 2027','1st Term',1,'2027','2027-03-15','2027-03-20',1,1,'system',GETUTCDATE(),0),
(2,'2nd Term Exam 2027','2nd Term',1,'2027','2027-06-15','2027-06-20',1,1,'system',GETUTCDATE(),0),
(3,'Final Exam 2027','Final',1,'2027','2027-11-15','2027-11-22',0,1,'system',GETUTCDATE(),0),
-- Nursery (Class 2)
(4,'1st Term Exam 2027','1st Term',2,'2027','2027-03-17','2027-03-22',1,1,'system',GETUTCDATE(),0),
(5,'2nd Term Exam 2027','2nd Term',2,'2027','2027-06-17','2027-06-22',1,1,'system',GETUTCDATE(),0),
(6,'Final Exam 2027','Final',2,'2027','2027-11-17','2027-11-24',0,1,'system',GETUTCDATE(),0),
-- KG (Class 3)
(7,'1st Term Exam 2027','1st Term',3,'2027','2027-03-19','2027-03-24',1,1,'system',GETUTCDATE(),0),
(8,'2nd Term Exam 2027','2nd Term',3,'2027','2027-06-19','2027-06-24',1,1,'system',GETUTCDATE(),0),
(9,'Final Exam 2027','Final',3,'2027','2027-11-19','2027-11-26',0,1,'system',GETUTCDATE(),0),
-- Class 1 (Class 4)
(10,'1st Term Exam 2027','1st Term',4,'2027','2027-03-10','2027-03-18',1,1,'system',GETUTCDATE(),0),
(11,'2nd Term Exam 2027','2nd Term',4,'2027','2027-06-10','2027-06-18',1,1,'system',GETUTCDATE(),0),
(12,'Final Exam 2027','Final',4,'2027','2027-11-10','2027-11-20',0,1,'system',GETUTCDATE(),0),
-- Class 2 (Class 5)
(13,'1st Term Exam 2027','1st Term',5,'2027','2027-03-11','2027-03-19',1,1,'system',GETUTCDATE(),0),
(14,'2nd Term Exam 2027','2nd Term',5,'2027','2027-06-11','2027-06-19',1,1,'system',GETUTCDATE(),0),
(15,'Final Exam 2027','Final',5,'2027','2027-11-11','2027-11-21',0,1,'system',GETUTCDATE(),0),
-- Class 3 (Class 6)
(16,'1st Term Exam 2027','1st Term',6,'2027','2027-03-12','2027-03-20',1,1,'system',GETUTCDATE(),0),
(17,'2nd Term Exam 2027','2nd Term',6,'2027','2027-06-12','2027-06-20',1,1,'system',GETUTCDATE(),0),
(18,'Final Exam 2027','Final',6,'2027','2027-11-12','2027-11-22',0,1,'system',GETUTCDATE(),0),
-- Class 4 (Class 7)
(19,'1st Term Exam 2027','1st Term',7,'2027','2027-03-13','2027-03-21',1,1,'system',GETUTCDATE(),0),
(20,'2nd Term Exam 2027','2nd Term',7,'2027','2027-06-13','2027-06-21',1,1,'system',GETUTCDATE(),0),
(21,'Final Exam 2027','Final',7,'2027','2027-11-13','2027-11-23',0,1,'system',GETUTCDATE(),0),
-- Class 5 (Class 8)
(22,'1st Term Exam 2027','1st Term',8,'2027','2027-03-14','2027-03-22',1,1,'system',GETUTCDATE(),0),
(23,'2nd Term Exam 2027','2nd Term',8,'2027','2027-06-14','2027-06-22',1,1,'system',GETUTCDATE(),0),
(24,'Final Exam 2027','Final',8,'2027','2027-11-14','2027-11-24',0,1,'system',GETUTCDATE(),0),
-- Class 6 (Class 9)
(25,'1st Term Exam 2027','1st Term',9,'2027','2027-03-05','2027-03-15',1,1,'system',GETUTCDATE(),0),
(26,'2nd Term Exam 2027','2nd Term',9,'2027','2027-06-05','2027-06-15',1,1,'system',GETUTCDATE(),0),
(27,'Final Exam 2027','Final',9,'2027','2027-11-05','2027-11-18',0,1,'system',GETUTCDATE(),0),
-- Class 7 (Class 10)
(28,'1st Term Exam 2027','1st Term',10,'2027','2027-03-06','2027-03-16',1,1,'system',GETUTCDATE(),0),
(29,'2nd Term Exam 2027','2nd Term',10,'2027','2027-06-06','2027-06-16',1,1,'system',GETUTCDATE(),0),
(30,'Final Exam 2027','Final',10,'2027','2027-11-06','2027-11-19',0,1,'system',GETUTCDATE(),0),
-- Class 8 (Class 11)
(31,'1st Term Exam 2027','1st Term',11,'2027','2027-03-07','2027-03-17',1,1,'system',GETUTCDATE(),0),
(32,'2nd Term Exam 2027','2nd Term',11,'2027','2027-06-07','2027-06-17',1,1,'system',GETUTCDATE(),0),
(33,'Final Exam 2027','Final',11,'2027','2027-11-07','2027-11-20',0,1,'system',GETUTCDATE(),0),
-- Class 9 (Class 12)
(34,'1st Term Exam 2027','1st Term',12,'2027','2027-03-08','2027-03-18',1,1,'system',GETUTCDATE(),0),
(35,'2nd Term Exam 2027','2nd Term',12,'2027','2027-06-08','2027-06-18',1,1,'system',GETUTCDATE(),0),
(36,'Final Exam 2027','Final',12,'2027','2027-11-08','2027-11-22',0,1,'system',GETUTCDATE(),0),
-- Class 10 (Class 13)
(37,'1st Term Exam 2027','1st Term',13,'2027','2027-03-09','2027-03-19',1,1,'system',GETUTCDATE(),0),
(38,'2nd Term Exam 2027','2nd Term',13,'2027','2027-06-09','2027-06-19',1,1,'system',GETUTCDATE(),0),
(39,'Final Exam 2027','Final',13,'2027','2027-11-09','2027-11-25',0,1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [Exams] OFF
GO

PRINT '>>> Exams done (39)'
GO

-- ============================================================
-- 18. EXAM SUBJECTS (link exams × subjects per class)
-- ============================================================
DECLARE @exId INT, @subjId INT
SELECT @exId = MIN(Id) FROM [Exams]
WHILE @exId IS NOT NULL
BEGIN
    DECLARE @exClassId INT = (SELECT ClassId FROM [Exams] WHERE Id = @exId)
    -- Subjects for this class level
    DECLARE @subjCursor CURSOR FOR
    SELECT Id FROM [Subjects] WHERE IsMandatory = 1 AND IsDeleted = 0
    UNION ALL
    SELECT Id FROM [Subjects] WHERE Id IN (8,9,10) AND @exClassId >= 9
    UNION ALL
    SELECT Id FROM [Subjects] WHERE Id IN (19) AND @exClassId >= 9
    ORDER BY Id

    OPEN @subjCursor
    FETCH NEXT FROM @subjCursor INTO @subjId
    WHILE @@FETCH_STATUS = 0
    BEGIN
        IF NOT EXISTS (SELECT 1 FROM [ExamSubjects] WHERE ExamId = @exId AND SubjectId = @subjId)
            INSERT INTO [ExamSubjects] ([ExamId],[SubjectId],[TotalMarks],[PassMarks],[IsCompulsory],[Order],[CreatedBy],[CreatedAt],[IsDeleted])
            VALUES (@exId, @subjId,
                CASE WHEN @subjId IN (21,22,23) THEN 50 ELSE 100 END,
                CASE WHEN @subjId IN (21,22,23) THEN 17 ELSE 33 END,
                1,
                @subjId,
                'system', GETUTCDATE(), 0)
        FETCH NEXT FROM @subjCursor INTO @subjId
    END
    CLOSE @subjCursor; DEALLOCATE @subjCursor

    SELECT @exId = MIN(Id) FROM [Exams] WHERE Id > @exId
END
GO

PRINT '>>> Exam Subjects done'
GO

-- ============================================================
-- 19. EXAM SCHEDULES & ADMIT CARDS
-- ============================================================
INSERT INTO [ExamSchedules] ([ExamId],[ClassId],[SubjectId],[ExamDate],[StartsAt],[EndsAt],[RoomName],[RoomNo],[BuildingName],[ShiftName],[Instructions],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT e.Id, e.ClassId, es.SubjectId,
    CAST(e.StartDate AS DATE),
    '09:00', '12:00',
    'Room-' + CAST((es.SubjectId % 10) + 1 AS NVARCHAR),
    CAST((es.SubjectId % 10) + 1 AS NVARCHAR),
    'Main Building', 'Morning',
    'Follow exam rules strictly',
    'system', GETUTCDATE(), 0
FROM [Exams] e
JOIN [ExamSubjects] es ON e.Id = es.ExamId
GO

PRINT '>>> Exam Schedules done'
GO

-- ============================================================
-- 20. ACADEMIC CALENDAR (2027)
-- ============================================================
SET IDENTITY_INSERT [AcademicCalendars] ON
GO
INSERT INTO [AcademicCalendars] ([Id],[AcademicYearId],[Date],[Title],[Description],[IsHoliday],[IsEventDay],[IsWorkingDay],[Remarks],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,1,'2027-01-01','New Year Day','New Year 2027',1,0,0,'Public Holiday',1,'system',GETUTCDATE(),0),
(2,1,'2027-02-21','International Mother Language Day','Shaheed Dibosh',1,1,0,'National Holiday',1,'system',GETUTCDATE(),0),
(3,1,'2027-03-17','National Childrens Day','Bangabandhu Birth Anniversary',1,0,0,'National Holiday',1,'system',GETUTCDATE(),0),
(4,1,'2027-03-26','Independence Day','Independence Day 1971',1,0,0,'National Holiday',1,'system',GETUTCDATE(),0),
(5,1,'2027-04-14','Bengali New Year','Pohela Boishakh 1434',1,1,0,'Festival Holiday',1,'system',GETUTCDATE(),0),
(6,1,'2027-05-01','May Day','International Workers Day',1,0,0,'National Holiday',1,'system',GETUTCDATE(),0),
(7,1,'2027-06-04','Shab-e-Barat','Shab-e-Barat',1,0,0,'Religious Holiday',1,'system',GETUTCDATE(),0),
(8,1,'2027-07-07','Eid-ul-Adha','Eid-ul-Adha',1,0,0,'Religious Holiday',1,'system',GETUTCDATE(),0),
(9,1,'2027-08-15','National Mourning Day','Bangabandhu Death Anniversary',1,0,0,'National Holiday',1,'system',GETUTCDATE(),0),
(10,1,'2027-09-28','Eid-e-Miladunnabi','Eid-e-Miladunnabi',1,0,0,'Religious Holiday',1,'system',GETUTCDATE(),0),
(11,1,'2027-10-06','Durga Puja','Bijoya Dashami',1,0,0,'Religious Holiday',1,'system',GETUTCDATE(),0),
(12,1,'2027-12-16','Victory Day','Victory Day 1971',1,1,0,'National Holiday',1,'system',GETUTCDATE(),0),
(13,1,'2027-12-25','Christmas Day','Boro Din',1,0,0,'Religious Holiday',1,'system',GETUTCDATE(),0),
(14,1,'2027-01-12','School Reopening','Academic Year 2027 Opening Ceremony',0,1,1,'School Event',1,'system',GETUTCDATE(),0),
(15,1,'2027-01-15','Classes Begin','Regular classes start for 2027',0,0,1,'Academic',1,'system',GETUTCDATE(),0),
(16,1,'2027-02-16','Annual Sports Day','Sports competition and athletics',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(17,1,'2027-03-10','1st Term Exams Begin','First Term Examination starts',0,0,1,'Exam Day',1,'system',GETUTCDATE(),0),
(18,1,'2027-03-25','Annual Cultural Program','Cultural competition and drama',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(19,1,'2027-05-15','Parents Day','Guardian-teacher meeting',0,1,0,'PTA Event',1,'system',GETUTCDATE(),0),
(20,1,'2027-05-30','Science Fair','Annual science and tech exhibition',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(21,1,'2027-06-10','2nd Term Exams Begin','Second Term Examination starts',0,0,1,'Exam Day',1,'system',GETUTCDATE(),0),
(22,1,'2027-06-25','Summer Vacation Begins','Summer break starts',1,0,0,'Vacation',1,'system',GETUTCDATE(),0),
(23,1,'2027-07-15','School Reopens','Summer vacation ends',0,0,1,'Academic',1,'system',GETUTCDATE(),0),
(24,1,'2027-09-05','Teachers Day','Teacher appreciation day',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(25,1,'2027-11-10','Final Exams Begin','Annual Examination starts',0,0,1,'Exam Day',1,'system',GETUTCDATE(),0),
(26,1,'2027-11-30','Result Preparation','Teachers submit final results',0,0,1,'Academic',1,'system',GETUTCDATE(),0),
(27,1,'2027-12-10','Annual Result Published','Year-end result publication',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(28,1,'2027-12-12','Prize Giving Ceremony','Annual prize distribution',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(29,1,'2027-12-20','Annual Picnic','School annual picnic',0,1,0,'School Event',1,'system',GETUTCDATE(),0),
(30,1,'2027-12-31','Year End','Academic Year 2027 ends',0,0,1,'Academic',1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [AcademicCalendars] OFF
GO

PRINT '>>> Academic Calendar done'
GO

-- ============================================================
-- 21. NOTICES / COMMUNICATIONS
-- ============================================================
SET IDENTITY_INSERT [Notices] ON
GO
INSERT INTO [Notices] ([Id],[Title],[Content],[NoticeType],[TargetGroup],[PublishDate],[ExpiryDate],[IsPublic],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,N'School Reopening Notice',N'Academic Year 2027 begins 12 Jan. Collect new calendar from office.',N'General',N'All','2027-01-05','2027-01-15',1,1,'system',GETUTCDATE(),0),
(2,N'1st Term Exam Schedule',N'First Term Exam starts 10 Mar 2027. Routine on notice board.',N'Exam',N'Students','2027-02-20','2027-03-25',0,1,'system',GETUTCDATE(),0),
(3,N'Guardian-Teacher Meeting',N'PTA meeting 15 May 2027 at 9 AM. All guardians must attend.',N'Event',N'Guardians','2027-05-01','2027-05-16',1,1,'system',GETUTCDATE(),0),
(4,N'Annual Sports Day 2027',N'Sports Day 16 Feb. Register by 10 Feb.',N'Event',N'Students','2027-02-01','2027-02-17',1,1,'system',GETUTCDATE(),0),
(5,N'Fee Payment Reminder',N'Jan fees due 10 Feb. Late fine BDT 100.',N'Finance',N'Guardians','2027-01-15','2027-02-11',0,1,'system',GETUTCDATE(),0),
(6,N'Summer Vacation Notice',N'School closed 25 Jun–14 Jul. Reopens 15 Jul.',N'General',N'All','2027-06-10','2027-07-16',1,1,'system',GETUTCDATE(),0),
(7,N'Final Exam 2027',N'Annual Exam starts 10 Nov. Routine by 25 Oct.',N'Exam',N'Students','2027-10-01','2027-12-20',0,1,'system',GETUTCDATE(),0),
(8,N'Prize Giving Ceremony',N'Annual prize ceremony 12 Dec at school auditorium.',N'Event',N'All','2027-11-15','2027-12-13',1,1,'system',GETUTCDATE(),0),
(9,N'New Academic Year Registration',N'Registration for 2027 open now at admission office.',N'General',N'Public','2026-12-01','2027-02-01',1,1,'system',GETUTCDATE(),0),
(10,N'Science Fair 2027',N'Science Fair 30 May. Class 6-10 students can participate.',N'Event',N'Students','2027-04-15','2027-06-01',1,1,'system',GETUTCDATE(),0),
(11,N'SSC Preparation Workshop',N'SSC prep workshop for Class 10 from 5-10 Feb.',N'Exam',N'Students','2027-01-20','2027-02-11',0,1,'system',GETUTCDATE(),0),
(12,N'2nd Term Exam Schedule',N'2nd Term Exam starts 10 Jun. Prepare accordingly.',N'Exam',N'Students','2027-05-15','2027-06-20',0,1,'system',GETUTCDATE(),0),
(13,N'Monthly Fee Due',N'Feb fees due 10 Mar. Pay online via SSLCommerz.',N'Finance',N'Guardians','2027-02-15','2027-03-11',0,1,'system',GETUTCDATE(),0),
(14,N'Cultural Program 2027',N'Annual cultural program 25 Mar. Students register by 15 Mar.',N'Event',N'Students','2027-03-01','2027-03-26',1,1,'system',GETUTCDATE(),0),
(15,N'Eid-ul-Adha Holiday',N'School closed 7-11 Jul for Eid-ul-Adha.',N'Holiday',N'All','2027-06-20','2027-07-15',1,1,'system',GETUTCDATE(),0),
(16,N'Independence Day Program',N'26 Mar celebration. All students attend in uniform.',N'Event',N'Students','2027-03-15','2027-03-27',1,1,'system',GETUTCDATE(),0),
(17,N'PTA Meeting Notice',N'Class 5-8 PTA meeting 20 Apr at 10 AM.',N'Event',N'Guardians','2027-04-05','2027-04-21',0,1,'system',GETUTCDATE(),0),
(18,N'Admission Open 2028',N'Early admission for 2028 starts 1 Dec 2027.',N'General',N'Public','2027-11-20','2028-01-15',1,1,'system',GETUTCDATE(),0),
(19,N'Winter Vacation Notice',N'Winter break 25 Dec–5 Jan. School reopens 6 Jan 2028.',N'General',N'All','2027-12-15','2028-01-07',1,1,'system',GETUTCDATE(),0),
(20,N'Blood Donation Camp',N'Blood donation camp 21 Feb at school auditorium.',N'Event',N'All','2027-02-10','2027-02-22',1,1,'system',GETUTCDATE(),0),
(21,N'Online Fee Payment',N'Pay fees via SSLCommerz. No cash after 10th.',N'Finance',N'Guardians','2027-03-01','2027-12-31',0,1,'system',GETUTCDATE(),0),
(22,N'Annual Picnic 2027',N'School picnic 20 Dec. Consent form by 10 Dec.',N'Event',N'Students','2027-12-01','2027-12-21',1,1,'system',GETUTCDATE(),0),
(23,N'Uniform Order Notice',N'Winter uniform orders by 15 Oct. Contact supplier.',N'General',N'Students','2027-09-20','2027-10-16',1,1,'system',GETUTCDATE(),0),
(24,N'Month of Ramadan Schedule',N'School timing changes during Ramadan. Details to follow.',N'General',N'All','2027-05-10','2027-05-20',1,1,'system',GETUTCDATE(),0),
(25,N'Result Publication Date',N'1st Term results published 5 Apr. Check student portal.',N'Exam',N'Students','2027-03-28','2027-04-10',0,1,'system',GETUTCDATE(),0),
(26,N'National Mourning Day',N'15 Aug holiday. School flag at half-mast.',N'Holiday',N'All','2027-08-10','2027-08-16',1,1,'system',GETUTCDATE(),0),
(27,N'Science Olympiad',N'Inter-school Science Olympiad 15 Jul. Register by 1 Jul.',N'Event',N'Students','2027-06-15','2027-07-05',1,1,'system',GETUTCDATE(),0),
(28,N'Class 10 Board Exam Prep',N'Board exam prep classes start 1 Sep. Mandatory attendance.',N'Exam',N'Students','2027-08-15','2027-09-15',0,1,'system',GETUTCDATE(),0),
(29,N'Parent Portal Activation',N'Check student progress via parent portal. Login details sent.',N'General',N'Guardians','2027-02-01','2027-12-31',0,1,'system',GETUTCDATE(),0),
(30,N'Annual Result & Prize Day',N'Annual result 10 Dec. Prize ceremony 12 Dec.',N'Event',N'All','2027-11-25','2027-12-15',1,1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [Notices] OFF
GO

-- ============================================================
-- 22. LIBRARY BOOKS
-- ============================================================
SET IDENTITY_INSERT [Books] ON
GO
INSERT INTO [Books] ([Id],[Title],[Author],[Publisher],[Isbn],[Category],[Quantity],[AvailableQuantity],[ShelfLocation],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,N'Bangla Byakoron O Rachona',N'Dr. Enamul Haque',N'NCTB','978-984-34-001-1',N'Textbook',20,18,'A1',1,'system',GETUTCDATE(),0),
(2,N'English Grammar & Composition',N'Wren & Martin',N'S Chand','978-812-190-009-3',N'Textbook',25,22,'A2',1,'system',GETUTCDATE(),0),
(3,N'High School Mathematics',N'S L Loney',N'Cambridge','978-521-800-019-4',N'Textbook',15,13,'B1',1,'system',GETUTCDATE(),0),
(4,N'General Science for Class 6-10',N'Prof. M A Mannan',N'NCTB','978-984-34-002-5',N'Textbook',30,28,'B2',1,'system',GETUTCDATE(),0),
(5,N'Bangladesh O Bishwa Porichoy',N'Dr. Maniruzzaman',N'NCTB','978-984-34-003-2',N'Textbook',20,19,'C1',1,'system',GETUTCDATE(),0),
(6,N'Islamic Studies',N'Maulana Abdul Gaffar',N'Islamic Foundation','978-984-34-004-9',N'Religion',15,14,'C2',1,'system',GETUTCDATE(),0),
(7,N'Physics for Secondary',N'Dr. S M Imdadul Hoque',N'NCTB','978-984-34-005-6',N'Textbook',12,10,'D1',1,'system',GETUTCDATE(),0),
(8,N'Chemistry for Secondary',N'Prof. A H Bhuiyan',N'NCTB','978-984-34-006-3',N'Textbook',12,11,'D2',1,'system',GETUTCDATE(),0),
(9,N'Biology for Secondary',N'Dr. M H Khan',N'NCTB','978-984-34-007-0',N'Textbook',12,9,'E1',1,'system',GETUTCDATE(),0),
(10,N'ICT (Information & Communication Tech)',N'Prof. M S Islam',N'NCTB','978-984-34-008-7',N'Textbook',20,18,'E2',1,'system',GETUTCDATE(),0),
(11,N'Business Studies',N'Prof. A B M Siddique',N'NCTB','978-984-34-009-4',N'Textbook',10,9,'F1',1,'system',GETUTCDATE(),0),
(12,N'Accounting Theory',N'Prof. M A Hakim',N'NCTB','978-984-34-010-0',N'Textbook',10,8,'F2',1,'system',GETUTCDATE(),0),
(13,N'Sheba O Samaj',N'Md. Ahsan Habib',N'Anonna Prokash','978-984-80-001-0',N'Story',5,4,'G1',1,'system',GETUTCDATE(),0),
(14,N'Lal Nil Dipaboli',N'Rashid Karim',N'Ananya Prokash','978-984-80-002-7',N'Novel',5,3,'G2',1,'system',GETUTCDATE(),0),
(15,N'National Encyclopedia',N'Bangladesh Asiatic Society',N'Asiatic Society','978-984-30-000-1',N'Reference',2,2,'H1',1,'system',GETUTCDATE(),0),
(16,N'English-Bengali Dictionary',N'S M Lutfar Rahman',N'Ideal Library','978-984-89-001-0',N'Dictionary',4,4,'H2',1,'system',GETUTCDATE(),0),
(17,N'Short Stories of Tagore',N'Rabindranath Tagore',N'Vishwabharati','978-812-910-000-3',N'Literature',6,5,'I1',1,'system',GETUTCDATE(),0),
(18,N'Childrens Science Encyclopedia',N'Dr. Qazi A H M Mahtab',N'Anonna Prokash','978-984-80-003-4',N'Reference',4,3,'I2',1,'system',GETUTCDATE(),0),
(19,N'Himu Somogro',N'Humayun Ahmed',N'Shomoy Prokashana','978-984-46-000-1',N'Novel',8,7,'J1',1,'system',GETUTCDATE(),0),
(20,N'Misir Ali Somogro',N'Humayun Ahmed',N'Shomoy Prokashana','978-984-46-000-2',N'Novel',6,5,'J2',1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [Books] OFF
GO

-- Book Issues (30 sample)
DECLARE @issueId INT = 1, @issueStudent INT, @issueBook INT, @issueDate DATE, @dueDate DATE
WHILE @issueId <= 30
BEGIN
    SET @issueStudent = ((@issueId * 17) % 300) + 1
    SET @issueBook = (@issueId % 20) + 1
    SET @issueDate = DATEADD(DAY, -((@issueId * 7) % 60 + 10), GETUTCDATE())
    SET @dueDate = DATEADD(DAY, 14, @issueDate)

    INSERT INTO [BookIssues] ([StudentId],[BookId],[IssueDate],[DueDate],[ReturnDate],[Status],[CreatedBy],[CreatedAt],[IsDeleted])
    VALUES (@issueStudent, @issueBook, @issueDate, @dueDate,
        CASE WHEN @issueId <= 15 THEN DATEADD(DAY, 10, @issueDate) ELSE NULL END,
        CASE WHEN @issueId <= 15 THEN 'Returned' ELSE 'Issued' END,
        'librarian', GETUTCDATE(), 0)

    SET @issueId = @issueId + 1
END
GO

PRINT '>>> Library done'
GO

-- ============================================================
-- 23. TRANSPORT ROUTES & ASSIGNMENTS
-- ============================================================
SET IDENTITY_INSERT [TransportRoutes] ON
GO
INSERT INTO [TransportRoutes] ([Id],[Name],[PickupDropSchedule],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,N'Mirpur Route','Mirpur 12→Mirpur 11→Mirpur 10→Mirpur 2→Mirpur 1→School','system',GETUTCDATE(),0),
(2,N'Uttara Route','Uttara Sector 4→Sector 5→Sector 3→Sector 2→Sector 1→School','system',GETUTCDATE(),0),
(3,N'Bashundhara Route','Bashundhara R/A→Badda Link Road→Madani Ave→School','system',GETUTCDATE(),0),
(4,N'Dhanmondi Route','Dhanmondi 32→Dhanmondi 27→Dhanmondi 15→Dhanmondi 5→Dhanmondi 1→School','system',GETUTCDATE(),0),
(5,N'Mohammadpur Route','Lalmatia→Mohammadpur Town Hall→Shyamoli→School','system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [TransportRoutes] OFF
GO

-- Vehicles
SET IDENTITY_INSERT [Vehicles] ON
GO
INSERT INTO [Vehicles] ([Id],[RegistrationNo],[Capacity],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'DHAKA-METRO-BUS-101',50,'system',GETUTCDATE(),0),
(2,'DHAKA-METRO-BUS-102',40,'system',GETUTCDATE(),0),
(3,'DHAKA-METRO-MICRO-301',25,'system',GETUTCDATE(),0),
(4,'DHAKA-METRO-BUS-103',50,'system',GETUTCDATE(),0),
(5,'DHAKA-METRO-MICRO-302',25,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [Vehicles] OFF
GO

-- Assign ~50 students to transport routes (with VehicleId)
INSERT INTO [StudentRouteAssignments] ([StudentId],[TransportRouteId],[VehicleId],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT Id, (Id % 5) + 1, (Id % 5) + 1, 'system', GETUTCDATE(), 0
FROM [Students]
WHERE Id <= 50
GO

PRINT '>>> Transport done'
GO

-- ============================================================
-- 24. FINANCIAL ACCOUNTING (Chart of Accounts + Financial Period)
-- ============================================================
SET IDENTITY_INSERT [ChartOfAccounts] ON
GO
INSERT INTO [ChartOfAccounts] ([Id],[AccountCode],[AccountName],[AccountType],[ParentAccountId],[IsCashAccount],[IsBankAccount],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'1-000','Assets','Asset',NULL,0,0,1,'system',GETUTCDATE(),0),
(2,'1-001','Cash in Hand','CurrentAsset',1,1,0,1,'system',GETUTCDATE(),0),
(3,'1-002','Bank Accounts','CurrentAsset',1,0,1,1,'system',GETUTCDATE(),0),
(4,'1-003','Accounts Receivable','CurrentAsset',1,0,0,1,'system',GETUTCDATE(),0),
(5,'1-004','Fixed Assets','FixedAsset',1,0,0,1,'system',GETUTCDATE(),0),
(6,'2-000','Liabilities','Liability',NULL,0,0,1,'system',GETUTCDATE(),0),
(7,'2-001','Accounts Payable','CurrentLiability',6,0,0,1,'system',GETUTCDATE(),0),
(8,'2-002','Salary Payable','CurrentLiability',6,0,0,1,'system',GETUTCDATE(),0),
(9,'2-003','Student Deposits','CurrentLiability',6,0,0,1,'system',GETUTCDATE(),0),
(10,'3-000','Income','Income',NULL,0,0,1,'system',GETUTCDATE(),0),
(11,'3-001','Tuition Fees','Revenue',10,0,0,1,'system',GETUTCDATE(),0),
(12,'3-002','Admission Fees','Revenue',10,0,0,1,'system',GETUTCDATE(),0),
(13,'3-003','Transport Fees','Revenue',10,0,0,1,'system',GETUTCDATE(),0),
(14,'3-004','Library Fees','Revenue',10,0,0,1,'system',GETUTCDATE(),0),
(15,'3-005','Other Income','Revenue',10,0,0,1,'system',GETUTCDATE(),0),
(16,'4-000','Expenses','Expense',NULL,0,0,1,'system',GETUTCDATE(),0),
(17,'4-001','Salary Expense','OperatingExpense',16,0,0,1,'system',GETUTCDATE(),0),
(18,'4-002','Utility Expense','OperatingExpense',16,0,0,1,'system',GETUTCDATE(),0),
(19,'4-003','Rent Expense','OperatingExpense',16,0,0,1,'system',GETUTCDATE(),0),
(20,'4-004','Maintenance Expense','OperatingExpense',16,0,0,1,'system',GETUTCDATE(),0),
(21,'4-005','Office Supplies','OperatingExpense',16,0,0,1,'system',GETUTCDATE(),0),
(22,'4-006','Exam Expense','OperatingExpense',16,0,0,1,'system',GETUTCDATE(),0),
(23,'5-000','Equity','Equity',NULL,0,0,1,'system',GETUTCDATE(),0),
(24,'5-001','Retained Earnings','RetainedEarnings',23,0,0,1,'system',GETUTCDATE(),0),
(25,'5-002','Opening Balance Equity','Equity',23,0,0,1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [ChartOfAccounts] OFF
GO

-- Financial Periods
SET IDENTITY_INSERT [FinancialPeriods] ON
GO
INSERT INTO [FinancialPeriods] ([Id],[PeriodName],[StartDate],[EndDate],[Status],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'January 2027','2027-01-01','2027-01-31','Open',1,'system',GETUTCDATE(),0),
(2,'February 2027','2027-02-01','2027-02-28','Open',1,'system',GETUTCDATE(),0),
(3,'March 2027','2027-03-01','2027-03-31','Open',1,'system',GETUTCDATE(),0),
(4,'April 2027','2027-04-01','2027-04-30','Open',1,'system',GETUTCDATE(),0),
(5,'May 2027','2027-05-01','2027-05-31','Open',1,'system',GETUTCDATE(),0),
(6,'June 2027','2027-06-01','2027-06-30','Open',1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [FinancialPeriods] OFF
GO

PRINT '>>> Accounting done'
GO

-- ============================================================
-- 25. SAMPLE EXAM RESULTS (Class 5 students for Final Exam)
-- ============================================================
-- Results for Class 5 first 30 students, 1st Term Exam (realistic distribution)
DECLARE @resultExamId INT = (SELECT Id FROM [Exams] WHERE ClassId = 8 AND ExamType = '1st Term')
IF @resultExamId IS NOT NULL
BEGIN
    INSERT INTO [StudentSubjectResults] ([ExamId],[StudentId],[SubjectId],[ExamSubjectId],[ClassId],[SectionId],[AcademicYearId],[MarksObtained],[FullMarks],[PassMarks],[Grade],[GradePoint],[IsPassed],[CalculatedAt],[IsOptionalSubject],[IsReligionSubject],[CreatedBy],[CreatedAt],[IsDeleted])
    SELECT
        @resultExamId, s.Id, es.SubjectId, es.Id, 8, 1, 1,
        CAST(m AS DECIMAL(10,2)),
        CASE WHEN es.SubjectId IN (21,22,23) THEN 50 ELSE 100 END,
        CASE WHEN es.SubjectId IN (21,22,23) THEN 17 ELSE 33 END,
        CASE WHEN m >= 85 THEN 'A+' WHEN m >= 75 THEN 'A' WHEN m >= 65 THEN 'A-' WHEN m >= 55 THEN 'B' WHEN m >= 45 THEN 'C' WHEN m >= 35 THEN 'D' ELSE 'F' END,
        CASE WHEN m >= 85 THEN 5.0 WHEN m >= 75 THEN 4.0 WHEN m >= 65 THEN 3.5 WHEN m >= 55 THEN 3.0 WHEN m >= 45 THEN 2.0 WHEN m >= 35 THEN 1.0 ELSE 0.0 END,
        CASE WHEN m >= 35 THEN 1 ELSE 0 END,
        GETUTCDATE(), 0, 0,
        'system', GETUTCDATE(), 0
    FROM (SELECT TOP 30 Id FROM [Students] WHERE ClassId = 8 ORDER BY Id) s
    CROSS JOIN (SELECT Id, SubjectId FROM [ExamSubjects] WHERE ExamId = @resultExamId) es
    CROSS APPLY (SELECT CAST(
        CASE ABS(CHECKSUM(NEWID())) % 100
            WHEN 0 THEN 30 WHEN 1 THEN 32 WHEN 2 THEN 34 WHEN 3 THEN 35 WHEN 4 THEN 36 -- F 5%
            WHEN 5 THEN 38 WHEN 6 THEN 40 WHEN 7 THEN 33 WHEN 8 THEN 35 WHEN 9 THEN 37 -- D 10%
            WHEN 10 THEN 39 WHEN 11 THEN 42 WHEN 12 THEN 44 WHEN 13 THEN 46 WHEN 14 THEN 48 -- C 15%
            WHEN 15 THEN 50 WHEN 16 THEN 52 WHEN 17 THEN 54 WHEN 18 THEN 56 WHEN 19 THEN 58 -- B 20%
            WHEN 20 THEN 60 WHEN 21 THEN 62 WHEN 22 THEN 64 WHEN 23 THEN 66 WHEN 24 THEN 68 -- A- 20%
            WHEN 25 THEN 70 WHEN 26 THEN 72 WHEN 27 THEN 74 WHEN 28 THEN 76 WHEN 29 THEN 78 -- A 20%
            WHEN 30 THEN 67 WHEN 31 THEN 71 WHEN 32 THEN 75 WHEN 33 THEN 79 WHEN 34 THEN 82
            WHEN 35 THEN 65 WHEN 36 THEN 69 WHEN 37 THEN 73 WHEN 38 THEN 77 WHEN 39 THEN 81
            ELSE 86 + (ABS(CHECKSUM(NEWID())) % 14) -- A+ 10%
        END AS INT) AS m) calc
    WHERE NOT EXISTS (SELECT 1 FROM [StudentSubjectResults] r WHERE r.ExamId = @resultExamId AND r.StudentId = s.Id AND r.SubjectId = es.SubjectId)
END
GO

PRINT '>>> Sample Results done'
GO

-- ============================================================
-- 26. TEACHER SUBJECT ASSIGNMENTS
-- ============================================================
DECLARE @tchId INT, @tchSubjId INT, @tchClassId INT
-- Teachers 1-15 assigned to subjects based on specialization
INSERT INTO [ClassSubjectTeacher] ([TeacherId],[ClassSubjectId],[AcademicYearId],[IsDeleted],[CreatedBy],[CreatedAt])
SELECT t.Id, cs.Id, 1, 0, 'system', GETUTCDATE()
FROM [Teachers] t
JOIN [ClassSubject] cs ON cs.ClassId = 
    CASE t.Id
        WHEN 1 THEN 4 WHEN 2 THEN 5 WHEN 3 THEN 6 WHEN 4 THEN 7 WHEN 5 THEN 8
        WHEN 6 THEN 9 WHEN 7 THEN 10 WHEN 8 THEN 11 WHEN 9 THEN 12 WHEN 10 THEN 13
        WHEN 11 THEN 4 WHEN 12 THEN 5 WHEN 13 THEN 6 WHEN 14 THEN 7 WHEN 15 THEN 8
    END
WHERE t.Id <= 15
GO

-- ============================================================
-- 26b. CLASS SUBJECTS (link subjects to classes)
-- ============================================================
INSERT INTO [ClassSubjects] ([SchoolClassId],[SubjectId],[GroupName],[FullMarks],[PassMarks],[IsMandatory],[IsOptional],[IsReligionSubject],[DisplayOrder],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT c.Id, s.Id, 'General',
    CASE WHEN s.Id IN (21,22,23) THEN 50 ELSE 100 END,
    CASE WHEN s.Id IN (21,22,23) THEN 17 ELSE 33 END,
    CASE WHEN s.IsMandatory = 1 THEN 1 ELSE 0 END,
    CASE WHEN s.IsOptional = 1 THEN 1 ELSE 0 END,
    CASE WHEN s.IsReligionSubject = 1 THEN 1 ELSE 0 END,
    s.DisplayOrder, 1, 'system', GETUTCDATE(), 0
FROM [Classes] c
CROSS JOIN [Subjects] s
WHERE (s.IsMandatory = 1
    OR (s.Id IN (8,9,10) AND c.Id >= 9)
    OR (s.Id = 19 AND c.Id >= 9)
    OR (s.Id IN (16,17,18) AND c.Id >= 12))
  AND NOT EXISTS (SELECT 1 FROM [ClassSubjects] cs WHERE cs.SchoolClassId = c.Id AND cs.SubjectId = s.Id)
GO

PRINT '>>> Class Subjects done'
GO

PRINT '>>> Teacher Subject Assignments done'
GO

-- ============================================================
-- 27. CLASS TEACHER ASSIGNMENT
-- ============================================================
UPDATE [Classes] SET ClassTeacherId = 
    CASE Id WHEN 1 THEN 12 WHEN 2 THEN 14 WHEN 3 THEN 12 WHEN 4 THEN 1
            WHEN 5 THEN 2 WHEN 6 THEN 3 WHEN 7 THEN 4 WHEN 8 THEN 5
            WHEN 9 THEN 6 WHEN 10 THEN 7 WHEN 11 THEN 8 WHEN 12 THEN 9 WHEN 13 THEN 15
    END
WHERE Id BETWEEN 1 AND 13
GO

PRINT '>>> Class Teacher Assignment done'
GO

-- ============================================================
-- 28. WORKING DAYS & PERIODS (for Timetable)
-- ============================================================
SET IDENTITY_INSERT [WorkingDays] ON
GO
INSERT INTO [WorkingDays] ([Id],[Name],[DayNumber],[IsWorkingDay],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES (1,'Saturday',1,1,1,'system',GETUTCDATE(),0),
       (2,'Sunday',2,1,1,'system',GETUTCDATE(),0),
       (3,'Monday',3,1,1,'system',GETUTCDATE(),0),
       (4,'Tuesday',4,1,1,'system',GETUTCDATE(),0),
       (5,'Wednesday',5,1,1,'system',GETUTCDATE(),0),
       (6,'Thursday',6,1,1,'system',GETUTCDATE(),0),
       (7,'Friday',7,0,1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [WorkingDays] OFF
GO

SET IDENTITY_INSERT [RoutinePeriods] ON
GO
INSERT INTO [RoutinePeriods] ([Id],[Name],[PeriodNumber],[StartTime],[EndTime],[IsBreak],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,N'Period 1',1,'09:00','09:45',0,1,'system',GETUTCDATE(),0),
(2,N'Period 2',2,'09:45','10:30',0,1,'system',GETUTCDATE(),0),
(3,N'Period 3',3,'10:30','11:15',0,1,'system',GETUTCDATE(),0),
(4,N'Tiffin Break',4,'11:15','11:30',1,1,'system',GETUTCDATE(),0),
(5,N'Period 4',5,'11:30','12:15',0,1,'system',GETUTCDATE(),0),
(6,N'Period 5',6,'12:15','13:00',0,1,'system',GETUTCDATE(),0),
(7,N'Period 6',7,'13:00','13:45',0,1,'system',GETUTCDATE(),0),
(8,N'Period 7',8,'13:45','14:30',0,1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [RoutinePeriods] OFF
GO

SET IDENTITY_INSERT [Rooms] ON
GO
INSERT INTO [Rooms] ([Id],[RoomCode],[RoomName],[Capacity],[RoomType],[Building],[Floor],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,'R101','Room 101',40,'Classroom','Main','1',1,'system',GETUTCDATE(),0),
(2,'R102','Room 102',40,'Classroom','Main','1',1,'system',GETUTCDATE(),0),
(3,'R103','Room 103',40,'Classroom','Main','1',1,'system',GETUTCDATE(),0),
(4,'R104','Room 104',35,'Classroom','Main','1',1,'system',GETUTCDATE(),0),
(5,'R105','Room 105',35,'Classroom','Main','1',1,'system',GETUTCDATE(),0),
(6,'R201','Room 201',40,'Classroom','Main','2',1,'system',GETUTCDATE(),0),
(7,'R202','Room 202',40,'Classroom','Main','2',1,'system',GETUTCDATE(),0),
(8,'R203','Room 203',40,'Classroom','Main','2',1,'system',GETUTCDATE(),0),
(9,'R204','Room 204',35,'Classroom','Main','2',1,'system',GETUTCDATE(),0),
(10,'R205','Room 205',35,'Classroom','Main','2',1,'system',GETUTCDATE(),0),
(11,'SL301','Science Lab',30,'Laboratory','Science','3',1,'system',GETUTCDATE(),0),
(12,'CL301','Computer Lab',25,'ComputerLab','Science','3',1,'system',GETUTCDATE(),0),
(13,'LIB','Central Library',60,'Library','Main','G',1,'system',GETUTCDATE(),0),
(14,'AUD','Auditorium',200,'Auditorium','Main','G',1,'system',GETUTCDATE(),0),
(15,'PL301','Physics Lab',25,'Laboratory','Science','3',1,'system',GETUTCDATE(),0),
(16,'CHL302','Chemistry Lab',25,'Laboratory','Science','3',1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [Rooms] OFF
GO

PRINT '>>> Working Days, Periods & Rooms done'
GO

-- ============================================================
-- 29. DAILY CLASS ROUTINE (Mon-Thu: 7 periods, all 13 classes)
-- ============================================================
-- Day 1=Saturday...Day6=Thursday, Friday=off
-- Periods 1,2,3,5,6,7 (4 is break)
-- Assign using deterministic hash for consistent results
INSERT INTO [RoutineEntries] ([AcademicYearId],[DayNumber],[RoutinePeriodId],[ClassId],[SectionId],[SubjectId],[TeacherId],[RoomId],[IsLab],[IsDeleted],[CreatedBy],[CreatedAt])
SELECT 
    1 AS AcademicYearId,
    dayNum AS DayNumber,
    periodId AS RoutinePeriodId,
    c.Id AS ClassId,
    NULL AS SectionId,
    -- Subject: deterministic from class+day+period
    CASE (c.Id * dayNum + periodId * 7) % 25 + 1
        WHEN 0 THEN 1 WHEN 1 THEN 2 WHEN 2 THEN 3 WHEN 3 THEN 4 WHEN 4 THEN 5
        WHEN 5 THEN 6 WHEN 6 THEN 20 WHEN 7 THEN 21 WHEN 8 THEN 22 WHEN 9 THEN 23
        ELSE ((c.Id + dayNum + periodId) % 10) + 1
    END AS SubjectId,
    -- Teacher: deterministic from class+day
    CASE (c.Id + dayNum) % 15 + 1
        WHEN 1 THEN 1 WHEN 2 THEN 2 WHEN 3 THEN 3 WHEN 4 THEN 4 WHEN 5 THEN 5
        WHEN 6 THEN 6 WHEN 7 THEN 7 WHEN 8 THEN 8 WHEN 9 THEN 9 WHEN 10 THEN 10
        WHEN 11 THEN 11 WHEN 12 THEN 12 WHEN 13 THEN 13 WHEN 14 THEN 14 ELSE 15
    END AS TeacherId,
    -- Room: deterministic
    (c.Id % 10) + 1 AS RoomId,
    0 AS IsLab, 0 AS IsDeleted, 'system', GETUTCDATE()
FROM [Classes] c
CROSS JOIN (VALUES (1),(2),(3),(4),(5),(6)) AS Days(dayNum)
CROSS JOIN (VALUES (1),(2),(3),(5),(6),(7)) AS Periods(periodId)
WHERE c.Id BETWEEN 1 AND 13
GO

PRINT '>>> Class Routine done'
GO

-- ============================================================
-- 30. ADMISSION APPLICATIONS (300 students)
-- ============================================================
SET IDENTITY_INSERT [Admissions] ON
GO
INSERT INTO [Admissions] ([Id],[ApplicationNo],[ApplicantName],[FatherName],[MotherName],[Gender],[DateOfBirth],[Religion],[Nationality],[MaritalStatus],[Country],[ApplicantMobileNumber],[AppliedClassId],[Status],[AdmissionFee],[AdmissionFeePaid],[AllDocumentsVerified],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT 
    s.Id,
    'ADM-' + RIGHT('0000' + CAST(s.Id AS NVARCHAR), 5),
    s.FullName,
    s.FatherName,
    s.MotherName,
    s.Gender,
    s.DateOfBirth,
    s.Religion,
    'Bangladeshi',
    'Unmarried',
    'Bangladesh',
    s.Phone,
    s.ClassId,
    1, -- Approved
    0, 0, 1,
    'system', GETUTCDATE(), 0
FROM [Students] s
GO
SET IDENTITY_INSERT [Admissions] OFF
GO

PRINT '>>> Admission Applications done'
GO

-- ============================================================
-- 31. STUDENT DOCUMENTS
-- ============================================================
SET IDENTITY_INSERT [StudentDocuments] ON
GO
INSERT INTO [StudentDocuments] ([Id],[StudentId],[DocumentType],[FilePath],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT 
    s.Id,
    s.Id,
    CASE ((s.Id + 1) % 5)
        WHEN 0 THEN 'Birth Certificate'
        WHEN 1 THEN 'Transfer Certificate'
        WHEN 2 THEN 'Previous Report Card'
        WHEN 3 THEN 'Guardian NID'
        ELSE 'Passport Photo'
    END,
    '/documents/student_' + CAST(s.Id AS NVARCHAR) + '/', 'system', GETUTCDATE(), 0
FROM [Students] s
GO
SET IDENTITY_INSERT [StudentDocuments] OFF
GO

-- Transfer Certificates for 10 students
SET IDENTITY_INSERT [TransferCertificates] ON
GO
INSERT INTO [TransferCertificates] ([Id],[StudentId],[CertificateNo],[OldClassId],[OldSectionId],[Reason],[NewSchoolName],[IssueDate],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT TOP 10
    Id,
    Id,
    'TC-' + RIGHT('0000' + CAST(Id AS NVARCHAR), 5) + '/2027',
    ClassId,
    NULL,
    CASE (Id % 3) WHEN 0 THEN 'Family moved to another city' WHEN 1 THEN 'Parent job transfer' ELSE 'Seeking better academic environment' END,
    CASE (Id % 4) WHEN 0 THEN 'Dhaka Residential Model College' WHEN 1 THEN 'St. Joseph Higher Secondary School' WHEN 2 THEN 'Notre Dame College' ELSE 'Viqarunnisa Noon School & College' END,
    '2027-06-15', 1, 'system', GETUTCDATE(), 0
FROM [Students]
WHERE Id <= 10
GO
SET IDENTITY_INSERT [TransferCertificates] OFF
GO

PRINT '>>> Student Documents & Certificates done'
GO

-- ============================================================
-- 32. COMPREHENSIVE EXAM RESULTS (all 39 exams)
-- ============================================================
-- Generate marks for all published exams (realistic grade distribution)
INSERT INTO [StudentSubjectResults] ([ExamId],[StudentId],[SubjectId],[ExamSubjectId],[ClassId],[SectionId],[AcademicYearId],[MarksObtained],[FullMarks],[PassMarks],[Grade],[GradePoint],[IsPassed],[CalculatedAt],[IsOptionalSubject],[IsReligionSubject],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT
    e.Id, s.Id, es.SubjectId, es.Id, s.ClassId, 1, 1,
    CAST(m AS DECIMAL(10,2)),
    CASE WHEN es.SubjectId IN (21,22,23) THEN 50 ELSE 100 END,
    CASE WHEN es.SubjectId IN (21,22,23) THEN 17 ELSE 33 END,
    CASE WHEN m >= 85 THEN 'A+' WHEN m >= 75 THEN 'A' WHEN m >= 65 THEN 'A-' WHEN m >= 55 THEN 'B' WHEN m >= 45 THEN 'C' WHEN m >= 35 THEN 'D' ELSE 'F' END,
    CASE WHEN m >= 85 THEN 5.0 WHEN m >= 75 THEN 4.0 WHEN m >= 65 THEN 3.5 WHEN m >= 55 THEN 3.0 WHEN m >= 45 THEN 2.0 WHEN m >= 35 THEN 1.0 ELSE 0.0 END,
    CASE WHEN m >= 35 THEN 1 ELSE 0 END,
    GETUTCDATE(), 0, 0,
    'system', GETUTCDATE(), 0
FROM [Exams] e
JOIN [ExamSubjects] es ON e.Id = es.ExamId
JOIN [Students] s ON s.ClassId = e.ClassId
CROSS APPLY (SELECT CAST(
    CASE ABS(CHECKSUM(NEWID())) % 100
        WHEN 0 THEN 30 WHEN 1 THEN 32 WHEN 2 THEN 34 WHEN 3 THEN 35 WHEN 4 THEN 36 -- F 5%
        WHEN 5 THEN 38 WHEN 6 THEN 40 WHEN 7 THEN 33 WHEN 8 THEN 35 WHEN 9 THEN 37 -- D 10%
        WHEN 10 THEN 39 WHEN 11 THEN 42 WHEN 12 THEN 44 WHEN 13 THEN 46 WHEN 14 THEN 48 -- C 15%
        WHEN 15 THEN 50 WHEN 16 THEN 52 WHEN 17 THEN 54 WHEN 18 THEN 56 WHEN 19 THEN 58 -- B 20%
        WHEN 20 THEN 60 WHEN 21 THEN 62 WHEN 22 THEN 64 WHEN 23 THEN 66 WHEN 24 THEN 68 -- A- 20%
        WHEN 25 THEN 70 WHEN 26 THEN 72 WHEN 27 THEN 74 WHEN 28 THEN 76 WHEN 29 THEN 78 -- A 20%
        WHEN 30 THEN 67 WHEN 31 THEN 71 WHEN 32 THEN 75 WHEN 33 THEN 79 WHEN 34 THEN 82
        WHEN 35 THEN 65 WHEN 36 THEN 69 WHEN 37 THEN 73 WHEN 38 THEN 77 WHEN 39 THEN 81
        ELSE 86 + (ABS(CHECKSUM(NEWID())) % 14) -- A+ 10%
    END AS INT) AS m) calc
WHERE e.IsResultPublished = 1
  AND NOT EXISTS (SELECT 1 FROM [StudentSubjectResults] r WHERE r.ExamId = e.Id AND r.StudentId = s.Id AND r.SubjectId = es.SubjectId)
GO

PRINT '>>> Comprehensive StudentSubjectResults done'
GO

PRINT '>>> Comprehensive Exam Results done'
GO

-- ============================================================
-- 33. FEE DISCOUNTS, LATE FINES, SCHOLARSHIPS
-- ============================================================
SET IDENTITY_INSERT [FeeDiscounts] ON
GO
INSERT INTO [FeeDiscounts] ([Id],[Name],[DiscountType],[Value],[SchoolClassId],[Description],[IsActive],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,N'Merit Scholarship (Top 5%)',1,5000,8,N'For students scoring A+ in all subjects',1,'system',GETUTCDATE(),0),
(2,N'Sibling Discount',2,1500,NULL,N'Discount for siblings studying in same school',1,'system',GETUTCDATE(),0),
(3,N'Need Based Scholarship',3,3000,NULL,N'Financial need based scholarship',1,'system',GETUTCDATE(),0),
(4,N'Science Fair Winner',1,2000,9,N'Award for science fair winners',1,'system',GETUTCDATE(),0),
(5,N'Sports Excellence',1,1500,6,N'Outstanding performance in sports',1,'system',GETUTCDATE(),0),
(6,N'Early Bird Discount',4,1000,NULL,N'Fee paid before 10th of the month',1,'system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [FeeDiscounts] OFF
GO

-- Apply some discounts to individual students (via FeeLedger)
INSERT INTO [FeeLedgers] ([StudentId],[FeeDiscountId],[TransactionType],[Debit],[Credit],[Balance],[Description],[TransactionDate],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT TOP 20
    s.Id,
    1, 6, 0, 5000, 5000, 'Merit Scholarship applied',
    '2027-01-15', 'system', GETUTCDATE(), 0
FROM [Students] s
WHERE s.ClassId = 8
ORDER BY s.Id
GO

-- Late fines on some invoices (Status 3 = Paid)
UPDATE [FeeInvoices] SET 
    LateFee = CASE WHEN DueDate < '2027-02-15' AND Status = 3 THEN 100 ELSE 0 END
WHERE Id % 15 = 0
GO

-- Refund records for 5 students (TransactionType: 5=Refund)
SET IDENTITY_INSERT [FeeLedgers] ON
GO
INSERT INTO [FeeLedgers] ([Id],[StudentId],[TransactionType],[Debit],[Credit],[Balance],[Description],[TransactionDate],[CreatedBy],[CreatedAt],[IsDeleted])
VALUES
(1,1,5,0,3000,3000,'Fee refund due to overpayment','2027-04-15','system',GETUTCDATE(),0),
(2,5,5,0,2500,2500,'Tuition fee refund','2027-05-10','system',GETUTCDATE(),0),
(3,10,5,0,1500,1500,'Library fee refund','2027-06-01','system',GETUTCDATE(),0),
(4,20,5,0,5000,5000,'Transport fee refund','2027-06-15','system',GETUTCDATE(),0),
(5,50,5,0,2000,2000,'Excess fee refund','2027-05-20','system',GETUTCDATE(),0)
GO
SET IDENTITY_INSERT [FeeLedgers] OFF
GO

PRINT '>>> Discounts, Fines & Refunds done'
GO

-- ============================================================
-- 34. PARENT PORTAL DATA
-- ============================================================
-- Login history for students
INSERT INTO [UserSessions] ([UserId],[LoginAt],[LogoutAt],[IpAddress],[UserAgent],[CreatedBy],[CreatedAt],[IsDeleted])
SELECT 
    u.Id,
    DATEADD(DAY, -((s.Id * 7) % 90), GETUTCDATE()),
    DATEADD(HOUR, 1, DATEADD(DAY, -((s.Id * 7) % 90), GETUTCDATE())),
    '192.168.' + CAST((s.Id % 254) + 1 AS NVARCHAR) + '.' + CAST((s.Id * 3) % 254 + 1 AS NVARCHAR),
    'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    'system', GETUTCDATE(), 0
FROM [Students] s
JOIN [Users] u ON u.Id = s.Id + 19 -- Student users start at ID 20 (teacher1)
WHERE s.Id <= 100
  AND NOT EXISTS (SELECT 1 FROM [UserSessions] us WHERE us.UserId = u.Id)
GO

PRINT '>>> Parent Portal data done'
GO

-- ============================================================
-- 35. DASHBOARD DATA AUGMENTATION
-- ============================================================
-- Extra payments for dashboard chart demo
DECLARE @extraPay INT = 1001
WHILE @extraPay <= 1200
BEGIN
    DECLARE @epStudent INT = ((@extraPay * 13) % 300) + 1
    DECLARE @epAmount DECIMAL(18,2) = 500 + ((@extraPay * 37) % 4500)
    DECLARE @epMonth INT = ((@extraPay - 1001) % 6) + 1

    INSERT INTO [Payments] ([FeeInvoiceId],[PaidAt],[Amount],[Method],[ReferenceNo],[LateFee],[DiscountAmount],[CreatedBy],[CreatedAt],[IsDeleted])
    SELECT Id, 
        DATEADD(MONTH, @epMonth - 1, '2027-01-15'),
        @epAmount,
        CASE @extraPay % 3 WHEN 0 THEN 1 WHEN 1 THEN 3 ELSE 2 END, -- 1=Cash, 3=SSLCommerz, 2=Bank
        'DASH-' + RIGHT('0000' + CAST(@extraPay AS NVARCHAR), 6),
        0, 0, 'system', GETUTCDATE(), 0
    FROM [FeeInvoices]
    WHERE StudentId = @epStudent
      AND NOT EXISTS (SELECT 1 FROM [Payments] p JOIN [FeeInvoices] fi ON p.FeeInvoiceId = fi.Id WHERE fi.StudentId = @epStudent AND p.CreatedAt > '2027-06-01')

    SET @extraPay = @extraPay + 1
END
GO

PRINT '>>> Dashboard data augmentation done'
GO

-- ============================================================
-- 36. FINAL SUMMARY
-- ============================================================
PRINT '╔══════════════════════════════════════════════════════════╗'
PRINT '║      SEED COMPLETE - SchoolManagementSystemDbdemo      ║'
PRINT '╠══════════════════════════════════════════════════════════╣'
PRINT '║  Green Valley International School                     ║'
PRINT '║  Academic Year: 2027                                   ║'
PRINT '║                                                        ║'
PRINT '║  DATA COUNTS:                                          ║'
DECLARE @cnt INT
SET @cnt = (SELECT COUNT(*) FROM [Classes]);          PRINT '║  [Classes]          = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Sections]);         PRINT '║  [Sections]         = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Shifts]);           PRINT '║  [Shifts]           = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Subjects]);         PRINT '║  [Subjects]         = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Users]);            PRINT '║  [Users]            = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Employees]);        PRINT '║  [Employees]        = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Teachers]);         PRINT '║  [Teachers]         = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Guardians]);        PRINT '║  [Guardians]        = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Students]);         PRINT '║  [Students]         = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [StudentAttendances]); PRINT '║  [Attendances]      = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [FeeInvoices]);      PRINT '║  [FeeInvoices]      = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Payments]);         PRINT '║  [Payments]         = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Exams]);            PRINT '║  [Exams]            = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Books]);            PRINT '║  [Books]            = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [BookIssues]);       PRINT '║  [BookIssues]       = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [TransportRoutes]);  PRINT '║  [TransportRoutes]  = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Notices]);          PRINT '║  [Notices]          = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [RoutineEntries]);  PRINT '║  [RoutineEntries]   = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [Admissions]);       PRINT '║  [Admissions]       = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [StudentDocuments]); PRINT '║  [StudentDocuments] = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [TransferCertificates]); PRINT '║  [Certificates]     = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [StudentSubjectResults]); PRINT '║  [StudentSubjectResults] = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                  ║'
SET @cnt = (SELECT COUNT(*) FROM [FeeDiscounts]);     PRINT '║  [FeeDiscounts]     = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
SET @cnt = (SELECT COUNT(*) FROM [FeeLedgers]);       PRINT '║  [FeeLedgers]       = ' + RIGHT('     ' + CAST(@cnt AS NVARCHAR), 5) + '                                         ║'
PRINT '╚══════════════════════════════════════════════════════════╝'
PRINT ' '
 

GO
GO

-- ============================================================
-- 37. WEBSITE SLIDERS (Hero Banners)
-- ============================================================
SET IDENTITY_INSERT [Sliders] ON
GO
INSERT INTO [Sliders] ([Id], [Title], [Subtitle], [ButtonText], [ButtonUrl], [ImagePath], [DisplayOrder], [IsActive], [StartDate], [EndDate], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Welcome to Green Valley International School', N'Nurturing Future Leaders Since 1998', N'Learn More', '/about', '/images/sliders/slide1.jpg', 1, 1, NULL, NULL, 'system', GETUTCDATE(), 0),
(2, N'Academic Excellence', N'Comprehensive education from Play to Class 10 following NCTB curriculum', N'Our Academics', '/academics', '/images/sliders/slide2.jpg', 2, 1, NULL, NULL, 'system', GETUTCDATE(), 0),
(3, N'State-of-the-Art Facilities', N'Modern labs, library, sports ground & digital classrooms', N'Explore Facilities', '/facilities', '/images/sliders/slide3.jpg', 3, 1, NULL, NULL, 'system', GETUTCDATE(), 0),
(4, N'Admission Open 2027', N'Enroll now for the academic year 2027. Limited seats available.', N'Apply Now', '/admission', '/images/sliders/slide4.jpg', 4, 1, '2026-12-01', '2027-03-31', 'system', GETUTCDATE(), 0),
(5, N'Holistic Development', N'Sports, culture, science fairs & leadership programs for overall growth', N'Our Activities', '/activities', '/images/sliders/slide5.jpg', 5, 1, NULL, NULL, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Sliders] OFF
GO

PRINT '>>> Sliders done (5)'
GO

-- ============================================================
-- 38. SCHOOL SETTINGS (Singleton — 1 row)
-- ============================================================
SET IDENTITY_INSERT [SchoolSettings] ON
GO
INSERT INTO [SchoolSettings] (
    [Id], [SchoolName], [ShortName], [BanglaName], [EIIN], [SchoolCode], [EstablishedYear],
    [SchoolMotto], [SchoolDescription], [Address], [Phone], [Mobile], [Email], [Website],
    [FacebookUrl], [YouTubeUrl], [LogoPath], [FaviconPath], [FooterLogoPath], [WebsiteBannerPath],
    [PrincipalName], [PrincipalDesignation], [PrincipalMessage], [PrincipalImagePath], [PrincipalQualification],
    [Mission], [Vision], [FooterText], [CopyrightText],
    [ShowSlider], [ShowPrincipalMessage], [ShowNotices], [ShowEvents], [ShowGallery],
    [ShowAdmissionCTA], [ShowStatistics], [ShowWelcomeSection],
    [AdmissionEnabled], [OnlineAdmissionEnabled], [ShowAdmissionPage],
    [ShowAdmissionFees], [ShowAdmissionGuidelines], [ShowAdmissionRequirements], [ShowAdmissionDownloads],
    [AdmissionTitle], [AdmissionSubtitle], [AdmissionGuidelines], [AdmissionEligibility],
    [AdmissionRequirements], [AdmissionProcess], [AdmissionFeeNote],
    [AdmissionCtaTitle], [AdmissionCtaText], [AdmissionOpenDate], [AdmissionCloseDate],
    [EnableStudentPortal], [EnableGuardianPortal], [EnableGuardianActivation],
    [RequireGuardianForAdmission], [EnableGuardianNotifications], [AllowResultWithDue],
    [CreatedBy], [CreatedAt], [IsDeleted]
)
VALUES (
    1,
    N'Green Valley International School', N'Green Valley', N'গ্রীন ভ্যালি ইন্টারন্যাশনাল স্কুল',
    N'123456', N'GVI', 1998,
    N'Knowledge is Light',
    N'Green Valley International School is a renowned educational institution in Dhaka, Bangladesh, committed to providing quality education from Play to Class 10 following the NCTB curriculum. We nurture students to become responsible, creative, and ethical global citizens.',
    N'12 Gulshan Ave, Gulshan-1, Dhaka-1212, Bangladesh',
    N'01711111101', N'01711111101', N'info@gvi.edu.bd', N'https://www.gvi.edu.bd',
    N'https://facebook.com/greenvalleyschool', N'https://youtube.com/@greenvalleyschool',
    N'/images/logo.png', N'/images/favicon.png', N'/images/footer-logo.png', N'/images/website-banner.jpg',
    N'Prof. Dr. Ayesha Begum', N'Principal',
    N'Welcome to Green Valley International School. We are dedicated to nurturing young minds with quality education, moral values, and holistic development.',
    N'/images/principal.jpg', N'PhD in Education, University of Dhaka',
    N'To provide inclusive, equitable, and quality education that empowers every student to reach their full potential.',
    N'To be a leading educational institution in Bangladesh that produces globally competitive, ethical, and innovative leaders.',
    N'Green Valley International School — Shaping Tomorrow''s Leaders Today',
    N'© 2027 Green Valley International School. All rights reserved.',
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1,
    N'Admission', N'Begin your journey at Green Valley',
    N'Admission is open for the academic year 2027. Please follow the guidelines below.',
    N'Students must meet the age requirement for their desired class level. Previous academic records will be reviewed.',
    N'Birth certificate, Previous report card, Passport-size photos, Guardian NID, Transfer certificate (if applicable)',
    N'1. Submit online or offline application. 2. Document verification. 3. Admission test/interview. 4. Fee payment. 5. Orientation.',
    N'Admission fees are one-time and non-refundable. Monthly fees are payable by the 10th of each month.',
    N'Admissions Open', N'Enroll your child today for academic year 2027. Limited seats available.',
    '2026-12-01', '2027-03-31',
    1, 1, 1, 1, 1, 0,
    'system', GETUTCDATE(), 0
)
GO
SET IDENTITY_INSERT [SchoolSettings] OFF
GO

PRINT '>>> SchoolSettings done (1)'
GO

-- ============================================================
-- 39. WEBSITE PAGES
-- ============================================================
SET IDENTITY_INSERT [WebsitePages] ON
GO
INSERT INTO [WebsitePages] ([Id], [Title], [Slug], [Content], [MetaTitle], [MetaDescription], [IsPublished], [DisplayOrder], [PublishAt], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Home', N'home',
 N'<h2>Welcome to Green Valley International School</h2><p>Green Valley International School (GVIS) is a premier educational institution in Dhaka, Bangladesh. Established in 1998, we have been providing quality education for over 25 years. Our dedicated faculty and modern facilities ensure that every student receives the best possible education.</p>',
 N'Green Valley International School - Home', N'Welcome to Green Valley International School, a premier educational institution in Dhaka offering quality education from Play to Class 10.', 1, 0, NULL, 'system', GETUTCDATE(), 0),

(2, N'About Us', N'about',
 N'<h2>About Green Valley International School</h2><p>Green Valley International School was established in 1998 with a vision to provide quality education to the children of Bangladesh. Located in the heart of Dhaka, our school offers a conducive learning environment with state-of-the-art facilities.</p><h3>Our Mission</h3><p>To provide inclusive, equitable, and quality education that empowers every student to reach their full potential.</p><h3>Our Vision</h3><p>To be a leading educational institution in Bangladesh that produces globally competitive, ethical, and innovative leaders.</p>',
 N'About Green Valley International School', N'Learn about Green Valley International School - our mission, vision, history, and commitment to quality education in Dhaka, Bangladesh.', 1, 1, NULL, 'system', GETUTCDATE(), 0),

(3, N'Academics', N'academics',
 N'<h2>Academic Programs</h2><p>We offer comprehensive education from Play to Class 10 following the NCTB curriculum. Our academic program is designed to develop critical thinking, creativity, and a love for learning.</p><h3>Curriculum</h3><p>We follow the National Curriculum and Textbook Board (NCTB) curriculum with enhanced focus on English, ICT, and practical learning.</p>',
 N'Academics - Green Valley International School', N'Explore academic programs at Green Valley International School. NCTB curriculum from Play to Class 10 with focus on holistic development.', 1, 2, NULL, 'system', GETUTCDATE(), 0),

(4, N'Admission', N'admission',
 N'<h2>Admission 2027</h2><p>Admissions are now open for the academic year 2027. We welcome students from Play to Class 10. Limited seats available.</p><h3>Admission Process</h3><ol><li>Submit application form (online or offline)</li><li>Document verification</li><li>Admission test/interview</li><li>Fee payment</li><li>Orientation and class commencement</li></ol>',
 N'Admission - Green Valley International School 2027', N'Admission open for 2027 at Green Valley International School. Apply now for Play to Class 10. Limited seats available.', 1, 3, NULL, 'system', GETUTCDATE(), 0),

(5, N'Facilities', N'facilities',
 N'<h2>School Facilities</h2><p>Green Valley International School offers modern facilities to ensure a holistic learning experience for all students.</p><ul><li>Spacious classrooms with smart boards</li><li>Science laboratories (Physics, Chemistry, Biology)</li><li>Computer lab with 25 workstations</li><li>Central library with 5000+ books</li><li>Auditorium for events and assemblies</li><li>Sports ground for outdoor activities</li><li>School bus service (5 routes across Dhaka)</li></ul>',
 N'Facilities - Green Valley International School', N'Explore modern facilities at Green Valley International School - smart classrooms, science labs, library, computer lab, sports ground, and transport.', 1, 4, NULL, 'system', GETUTCDATE(), 0),

(6, N'Contact Us', N'contact',
 N'<h2>Contact Us</h2><p>We would love to hear from you. Reach out to us for any inquiries about admissions, academics, or school activities.</p><h3>Address</h3><p>12 Gulshan Ave, Gulshan-1, Dhaka-1212, Bangladesh</p><h3>Phone</h3><p>+880 1711 111101</p><h3>Email</h3><p>info@gvi.edu.bd</p><h3>Office Hours</h3><p>Saturday - Thursday: 9:00 AM - 4:00 PM<br>Friday: Closed</p>',
 N'Contact - Green Valley International School', N'Contact Green Valley International School in Dhaka, Bangladesh. Phone, email, and office hours information.', 1, 5, NULL, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [WebsitePages] OFF
GO

PRINT '>>> WebsitePages done (6)'
GO

-- ============================================================
-- 40. EVENT CATEGORIES
-- ============================================================
SET IDENTITY_INSERT [EventCategories] ON
GO
INSERT INTO [EventCategories] ([Id], [Name], [Slug], [Description], [ColorCode], [DisplayOrder], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Academic', N'academic', N'Academic events including exams, workshops, seminars, and educational programs', N'#2196F3', 1, 1, 'system', GETUTCDATE(), 0),
(2, N'Cultural', N'cultural', N'Cultural programs, celebrations, talent shows, and competitions', N'#9C27B0', 2, 1, 'system', GETUTCDATE(), 0),
(3, N'Sports', N'sports', N'Sports competitions, athletics, games, and physical activities', N'#4CAF50', 3, 1, 'system', GETUTCDATE(), 0),
(4, N'Religious', N'religious', N'Religious festivals, observances, and spiritual programs', N'#FF9800', 4, 1, 'system', GETUTCDATE(), 0),
(5, N'Holiday', N'holiday', N'National and public holidays observed by the school', N'#F44336', 5, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [EventCategories] OFF
GO

PRINT '>>> EventCategories done (5)'
GO

-- ============================================================
-- 41. EVENTS (12 tied to categories + linked to AcademicCalendar)
-- ============================================================
SET IDENTITY_INSERT [Events] ON
GO
INSERT INTO [Events] ([Id], [Title], [Description], [EventDate], [EventLocation], [IsUpcoming], [IsPublished], [ApprovalStatus], [EventCategoryId], [CoverImagePath], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Academic Year 2027 Opening Ceremony', N'Annual opening ceremony to welcome students and staff for the new academic year 2027. Chief guest: Prof. Dr. Ayesha Begum, Principal.', '2027-01-12', N'School Auditorium', 0, 1, 2, 1, N'/images/events/opening-ceremony.jpg', 'system', GETUTCDATE(), 0),
(2, N'International Mother Language Day', N'Commemorating Shaheed Dibosh with cultural programs, poetry recitation, and essay competitions.', '2027-02-21', N'School Premises', 0, 1, 2, 2, N'/images/events/mother-language-day.jpg', 'system', GETUTCDATE(), 0),
(3, N'Annual Sports Day 2027', N'Annual sports competition featuring track and field events, relay races, and fun games for all classes.', '2027-02-16', N'School Sports Ground', 0, 1, 2, 3, N'/images/events/sports-day.jpg', 'system', GETUTCDATE(), 0),
(4, N'Independence Day Celebration', N'Celebrating Bangladesh Independence Day with flag hoisting, parade, cultural performances, and discussions.', '2027-03-26', N'School Auditorium', 0, 1, 2, 2, N'/images/events/independence-day.jpg', 'system', GETUTCDATE(), 0),
(5, N'Pohela Boishakh 1434', N'Celebrating Bengali New Year with traditional programs, cultural performances, and festive food.', '2027-04-14', N'School Premises', 0, 1, 2, 2, N'/images/events/pohela-boishakh.jpg', 'system', GETUTCDATE(), 0),
(6, N'1st Term Examination 2027', N'First term examinations for all classes. See exam schedule for details.', '2027-03-10', N'Respective Classrooms', 0, 1, 2, 1, NULL, 'system', GETUTCDATE(), 0),
(7, N'Annual Science Fair 2027', N'Annual science and technology exhibition showcasing student projects, experiments, and innovations.', '2027-05-30', N'Science Building', 0, 1, 2, 1, N'/images/events/science-fair.jpg', 'system', GETUTCDATE(), 0),
(8, N'Guardian-Teacher Meeting (Parents Day)', N'Semester-wise guardian-teacher meeting to discuss student progress and development.', '2027-05-15', N'School Auditorium', 0, 1, 2, 1, NULL, 'system', GETUTCDATE(), 0),
(9, N'Eid-ul-Adha Holiday', N'School closed for Eid-ul-Adha celebrations. Classes resume on 12 July 2027.', '2027-07-07', N'School Closed', 0, 1, 2, 4, NULL, 'system', GETUTCDATE(), 0),
(10, N'Teachers Day 2027', N'Celebrating our dedicated teachers with cultural programs, awards, and appreciation ceremonies.', '2027-09-05', N'School Auditorium', 1, 1, 2, 2, N'/images/events/teachers-day.jpg', 'system', GETUTCDATE(), 0),
(11, N'Annual Result Publication 2027', N'Year-end result publication for all classes. Results available on student portal.', '2027-12-10', N'School Premises', 1, 1, 2, 1, NULL, 'system', GETUTCDATE(), 0),
(12, N'Prize Giving Ceremony 2027', N'Annual prize distribution honoring academic excellence, sports achievements, and extracurricular participation.', '2027-12-12', N'School Auditorium', 1, 1, 2, 2, N'/images/events/prize-giving.jpg', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Events] OFF
GO

PRINT '>>> Events done (12)'
GO

-- ============================================================
-- 42. GALLERIES (5 albums) + GALLERY IMAGES (20 images, 4 each)
-- ============================================================
SET IDENTITY_INSERT [Galleries] ON
GO
INSERT INTO [Galleries] ([Id], [AlbumName], [Description], [CoverImagePath], [DisplayOrder], [IsPublished], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Annual Sports Day 2027', N'Photos from the Annual Sports Day held on 16 February 2027 featuring track events, relay races, and prize distribution.', N'/images/galleries/sports-day/cover.jpg', 1, 1, 'system', GETUTCDATE(), 0),
(2, N'Cultural Program 2027', N'Cultural performances including dance, drama, music, and poetry recitation by students.', N'/images/galleries/cultural/cover.jpg', 2, 1, 'system', GETUTCDATE(), 0),
(3, N'Science Fair 2027', N'Student projects and experiments showcased at the Annual Science Fair 2027.', N'/images/galleries/science-fair/cover.jpg', 3, 1, 'system', GETUTCDATE(), 0),
(4, N'Prize Giving Ceremony 2027', N'Award ceremony honoring student achievements in academics, sports, and co-curricular activities.', N'/images/galleries/prize-giving/cover.jpg', 4, 1, 'system', GETUTCDATE(), 0),
(5, N'School Campus', N'A glimpse of our beautiful campus including classrooms, labs, library, playground, and facilities.', N'/images/galleries/campus/cover.jpg', 5, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Galleries] OFF
GO

SET IDENTITY_INSERT [GalleryImages] ON
GO
INSERT INTO [GalleryImages] ([Id], [ImagePath], [AltText], [Caption], [DisplayOrder], [GalleryId], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'/images/galleries/sports-day/img1.jpg', N'Opening ceremony of Sports Day', N'Students marching during the opening ceremony', 1, 1, 'system', GETUTCDATE(), 0),
(2, N'/images/galleries/sports-day/img2.jpg', N'100 meter race', N'Thrilling 100-meter sprint final', 2, 1, 'system', GETUTCDATE(), 0),
(3, N'/images/galleries/sports-day/img3.jpg', N'Relay race', N'Exciting 4x100 relay race competition', 3, 1, 'system', GETUTCDATE(), 0),
(4, N'/images/galleries/sports-day/img4.jpg', N'Prize distribution Sports Day', N'Winners receiving medals and certificates', 4, 1, 'system', GETUTCDATE(), 0),
(5, N'/images/galleries/cultural/img1.jpg', N'Cultural dance performance', N'Students performing traditional Bangladeshi dance', 1, 2, 'system', GETUTCDATE(), 0),
(6, N'/images/galleries/cultural/img2.jpg', N'Drama performance', N'Students presenting a thought-provoking drama', 2, 2, 'system', GETUTCDATE(), 0),
(7, N'/images/galleries/cultural/img3.jpg', N'Music performance', N'Solo and group music performances by talented students', 3, 2, 'system', GETUTCDATE(), 0),
(8, N'/images/galleries/cultural/img4.jpg', N'Pohela Boishakh celebration', N'Celebrating Bengali New Year with traditional colors', 4, 2, 'system', GETUTCDATE(), 0),
(9, N'/images/galleries/science-fair/img1.jpg', N'Science project display', N'Students presenting their science projects to judges', 1, 3, 'system', GETUTCDATE(), 0),
(10, N'/images/galleries/science-fair/img2.jpg', N'Robotics project', N'Robotics and automation projects showcased by students', 2, 3, 'system', GETUTCDATE(), 0),
(11, N'/images/galleries/science-fair/img3.jpg', N'Chemistry experiments', N'Live chemistry demonstrations by students', 3, 3, 'system', GETUTCDATE(), 0),
(12, N'/images/galleries/science-fair/img4.jpg', N'Science fair winners', N'Winners of the Science Fair with their certificates', 4, 3, 'system', GETUTCDATE(), 0),
(13, N'/images/galleries/prize-giving/img1.jpg', N'Academic excellence award', N'Top performers receiving academic excellence awards', 1, 4, 'system', GETUTCDATE(), 0),
(14, N'/images/galleries/prize-giving/img2.jpg', N'Sports award ceremony', N'Sports achievers receiving their well-deserved awards', 2, 4, 'system', GETUTCDATE(), 0),
(15, N'/images/galleries/prize-giving/img3.jpg', N'Cultural award', N'Cultural program participants receiving certificates', 3, 4, 'system', GETUTCDATE(), 0),
(16, N'/images/galleries/prize-giving/img4.jpg', N'Principal address', N'Principal addressing the gathering at the ceremony', 4, 4, 'system', GETUTCDATE(), 0),
(17, N'/images/galleries/campus/img1.jpg', N'School building exterior', N'Front view of Green Valley International School', 1, 5, 'system', GETUTCDATE(), 0),
(18, N'/images/galleries/campus/img2.jpg', N'Science laboratory', N'Well-equipped science laboratory for practical learning', 2, 5, 'system', GETUTCDATE(), 0),
(19, N'/images/galleries/campus/img3.jpg', N'Library', N'Central library with extensive collection of books', 3, 5, 'system', GETUTCDATE(), 0),
(20, N'/images/galleries/campus/img4.jpg', N'Computer lab', N'Modern computer lab with 25 workstations', 4, 5, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [GalleryImages] OFF
GO

PRINT '>>> Galleries done (5) + GalleryImages done (20)'
GO

-- ============================================================
-- 43. ANNOUNCEMENTS (Public Website Notices)
-- ============================================================
SET IDENTITY_INSERT [Announcements] ON
GO
INSERT INTO [Announcements] ([Id], [Title], [Content], [IsActive], [PublishDate], [ExpiryDate], [Priority], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'School Reopening for Academic Year 2027', N'School reopens on 12 January 2027. All students must report by 9:00 AM in full uniform. Class routines and new books will be distributed on the first day.', 1, '2027-01-05', '2027-01-20', N'High', 'system', GETUTCDATE(), 0),
(2, N'Admission Open for 2028 Academic Year', N'Early registration for academic year 2028 begins from 1 December 2027. Limited seats available in all classes. Visit admission office or apply online.', 1, '2027-11-20', '2028-02-15', N'High', 'system', GETUTCDATE(), 0),
(3, N'Online Fee Payment Now Available', N'Guardians can now pay school fees online via SSLCommerz. Visit the student portal and click on "Pay Fees". No cash payments accepted after the 10th of each month.', 1, '2027-03-01', NULL, N'Normal', 'system', GETUTCDATE(), 0),
(4, N'Summer Vacation Notice', N'School will remain closed from 25 June to 14 July for summer vacation. Classes resume on 15 July 2027. Summer assignments will be provided before vacation.', 1, '2027-06-10', '2027-07-16', N'Normal', 'system', GETUTCDATE(), 0),
(5, N'New Academic Calendar Published', N'The academic calendar for 2027 has been published. All important dates including exams, holidays, and events are listed. Download from the academics section.', 1, '2027-01-02', '2027-02-01', N'Low', 'system', GETUTCDATE(), 0),
(6, N'Parent Portal Activation', N'Guardians can now track student progress, attendance, and fee status through the parent portal. Login credentials have been sent to registered mobile numbers.', 1, '2027-02-01', NULL, N'Normal', 'system', GETUTCDATE(), 0),
(7, N'SSC Preparation Workshop for Class 10', N'A special SSC preparation workshop will be held from 5-10 February 2027. All Class 10 students must attend. Expert teachers will cover key topics and exam strategies.', 1, '2027-01-20', '2027-02-11', N'High', 'system', GETUTCDATE(), 0),
(8, N'Winter Uniform Order Notice', N'Winter uniform orders must be placed by 15 October 2027. Contact the school supplier at the uniform shop. Sizes available from Class 1 to 10.', 1, '2027-09-20', '2027-10-20', N'Normal', 'system', GETUTCDATE(), 0),
(9, N'Annual School Picnic 2027', N'The annual school picnic will be held on 20 December 2027. Consent forms must be submitted by 10 December. Students will visit Fantasy Kingdom, Dhaka.', 1, '2027-12-01', '2027-12-21', N'Low', 'system', GETUTCDATE(), 0),
(10, N'Blood Donation Camp', N'A blood donation camp will be organized on 21 February 2027 at the school auditorium in collaboration with Red Crescent Society. Staff and guardians are encouraged to participate.', 1, '2027-02-10', '2027-02-22', N'Normal', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [Announcements] OFF
GO

PRINT '>>> Announcements done (10)'
GO

-- ============================================================
-- 44. CONTACT MESSAGES (Sample Visitor Submissions)
-- ============================================================
SET IDENTITY_INSERT [ContactMessages] ON
GO
INSERT INTO [ContactMessages] ([Id], [Name], [Email], [Phone], [Subject], [Message], [Status], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Md. Abdul Karim', N'akarim@gmail.com', N'01712345678', N'Admission Inquiry for Class 1', N'Dear Sir/Madam, I would like to inquire about admission for my son into Class 1 for the academic year 2027. Please let me know the admission process, required documents, and fee structure. Thank you.', N'Read', 'system', GETUTCDATE(), 0),
(2, N'Mst. Farzana Akhter', N'farzana.akhter@yahoo.com', N'01723456789', N'Tuition Fee Payment Issue', N'I am trying to pay the tuition fee online through SSLCommerz but the payment is not going through. My son is Md. Rahim Islam, Class 5, Roll 12. Please help resolve this issue.', N'Unread', 'system', GETUTCDATE(), 0),
(3, N'Mohammad Shahidul Islam', N'shahidul.islam@outlook.com', N'01734567890', N'Transfer Certificate Request', N'We are relocating to Chittagong due to job transfer. I request a Transfer Certificate for my daughter Mst. Nasrin Sultana, Class 7, Roll 8. Kindly let me know the procedure and timeline.', N'Unread', 'system', GETUTCDATE(), 0),
(4, N'Md. Mizanur Rahman', N'mizan.rahman@gmail.com', N'01745678901', N'Suggestion for School Library', N'I visited the school library during the last PTA meeting. I suggest adding more English storybooks and reference materials for the secondary level students. Happy to donate some books.', N'Read', 'system', GETUTCDATE(), 0),
(5, N'Mst. Shahina Begum', N'shahina.begum@gmail.com', N'01756789012', N'Complaint About School Transport', N'The school bus (Route 2, Uttara) has been arriving late consistently for the past two weeks. Today it was 25 minutes late. Please look into this matter urgently as students are getting late to school.', N'Unread', 'system', GETUTCDATE(), 0),
(6, N'Md. Jahangir Alam', N'jahangir.alam@email.com', N'01767890123', N'Science Fair Participation', N'I would like to register my son Md. Tuhin Alam, Class 8, for the upcoming Science Fair. He has prepared a working model on renewable energy. Please confirm the registration deadline and guidelines.', N'Unread', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [ContactMessages] OFF
GO

PRINT '>>> ContactMessages done (6)'
GO

-- ============================================================
-- 45. ADMISSION FEE STRUCTURES (Public-facing — one per class)
-- ============================================================
SET IDENTITY_INSERT [AdmissionFeeStructures] ON
GO
INSERT INTO [AdmissionFeeStructures] ([Id], [SchoolClassId], [ClassName], [AdmissionFee], [MonthlyFee], [SessionFee], [ExamFee], [OtherFee], [RegistrationFee], [DevelopmentFee], [LibraryFee], [LaboratoryFee], [DisplayOrder], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 1, N'Play', 3000.00, 1500.00, 2000.00, 500.00, 500.00, 500.00, 1000.00, 200.00, 0.00, 1, 1, 'system', GETUTCDATE(), 0),
(2, 2, N'Nursery', 3500.00, 1800.00, 2500.00, 600.00, 500.00, 500.00, 1000.00, 250.00, 0.00, 2, 1, 'system', GETUTCDATE(), 0),
(3, 3, N'KG', 4000.00, 2000.00, 3000.00, 700.00, 500.00, 500.00, 1000.00, 300.00, 0.00, 3, 1, 'system', GETUTCDATE(), 0),
(4, 4, N'Class 1', 5000.00, 2500.00, 3500.00, 800.00, 600.00, 1000.00, 1500.00, 300.00, 500.00, 4, 1, 'system', GETUTCDATE(), 0),
(5, 5, N'Class 2', 5000.00, 2500.00, 3500.00, 800.00, 600.00, 1000.00, 1500.00, 300.00, 500.00, 5, 1, 'system', GETUTCDATE(), 0),
(6, 6, N'Class 3', 5000.00, 2800.00, 4000.00, 1000.00, 700.00, 1000.00, 1500.00, 400.00, 600.00, 6, 1, 'system', GETUTCDATE(), 0),
(7, 7, N'Class 4', 5000.00, 2800.00, 4000.00, 1000.00, 700.00, 1000.00, 1500.00, 400.00, 600.00, 7, 1, 'system', GETUTCDATE(), 0),
(8, 8, N'Class 5', 5500.00, 3000.00, 4500.00, 1200.00, 800.00, 1000.00, 2000.00, 500.00, 700.00, 8, 1, 'system', GETUTCDATE(), 0),
(9, 9, N'Class 6', 6000.00, 3500.00, 5000.00, 1500.00, 1000.00, 1500.00, 2000.00, 500.00, 800.00, 9, 1, 'system', GETUTCDATE(), 0),
(10, 10, N'Class 7', 6000.00, 3500.00, 5000.00, 1500.00, 1000.00, 1500.00, 2000.00, 500.00, 800.00, 10, 1, 'system', GETUTCDATE(), 0),
(11, 11, N'Class 8', 6000.00, 4000.00, 5500.00, 2000.00, 1200.00, 1500.00, 2500.00, 600.00, 1000.00, 11, 1, 'system', GETUTCDATE(), 0),
(12, 12, N'Class 9', 7000.00, 4500.00, 6000.00, 2500.00, 1500.00, 2000.00, 2500.00, 700.00, 1200.00, 12, 1, 'system', GETUTCDATE(), 0),
(13, 13, N'Class 10', 8000.00, 5000.00, 7000.00, 3000.00, 1800.00, 2000.00, 3000.00, 800.00, 1500.00, 13, 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [AdmissionFeeStructures] OFF
GO

PRINT '>>> AdmissionFeeStructures done (13)'
GO

-- ============================================================
-- 46. EMAIL TEMPLATES
-- ============================================================
SET IDENTITY_INSERT [EmailTemplates] ON
GO
INSERT INTO [EmailTemplates] ([Id], [TemplateName], [Subject], [Body], [Placeholders], [Category], [IsActive], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, N'Welcome Email - Guardian',
 N'Welcome to Green Valley International School - {{StudentName}}',
 N'<h2>Welcome to Green Valley International School!</h2><p>Dear {{GuardianName}},</p><p>We are delighted to welcome your ward <strong>{{StudentName}}</strong> (Class: {{ClassName}}, Roll: {{RollNumber}}) to Green Valley International School for the academic year {{AcademicYear}}.</p><p>Please find below important information:</p><ul><li><strong>School Reopening:</strong> 12 January 2027</li><li><strong>School Timings:</strong> 9:00 AM - 2:30 PM</li><li><strong>Uniform:</strong> Full uniform is mandatory from day one</li></ul><p>Login to the parent portal using your registered mobile number to track academic progress and fee status.</p><p>Warm regards,<br>Principal<br>Green Valley International School</p>',
 N'{{GuardianName}}, {{StudentName}}, {{ClassName}}, {{RollNumber}}, {{AcademicYear}}', N'Registration', 1, 'system', GETUTCDATE(), 0),

(2, N'Fee Payment Reminder',
 N'Fee Payment Reminder - Due Date: {{DueDate}}',
 N'<h2>Fee Payment Reminder</h2><p>Dear {{GuardianName}},</p><p>This is a reminder that the monthly tuition fee for <strong>{{StudentName}}</strong> ({{ClassName}}) amounting to <strong>BDT {{Amount}}</strong> is due on <strong>{{DueDate}}</strong>.</p><p>Please pay the fee before the due date to avoid late fines. You can pay online via SSLCommerz through the parent portal or visit the school accounts office.</p><p>Invoice No: {{InvoiceNo}}<br>Late Fine (after due date): BDT 100 per month</p><p>Thank you,<br>Accounts Department<br>Green Valley International School</p>',
 N'{{GuardianName}}, {{StudentName}}, {{ClassName}}, {{Amount}}, {{DueDate}}, {{InvoiceNo}}', N'Finance', 1, 'system', GETUTCDATE(), 0),

(3, N'Event Notification',
 N'{{EventName}} - {{EventDate}} at Green Valley International School',
 N'<h2>Event Notification</h2><p>Dear {{GuardianName}},</p><p>We are pleased to inform you about the upcoming event at Green Valley International School:</p><p><strong>Event:</strong> {{EventName}}<br><strong>Date:</strong> {{EventDate}}<br><strong>Location:</strong> {{EventLocation}}<br><strong>Description:</strong> {{EventDescription}}</p><p>We look forward to your participation and support.</p><p>Warm regards,<br>Event Management<br>Green Valley International School</p>',
 N'{{GuardianName}}, {{EventName}}, {{EventDate}}, {{EventLocation}}, {{EventDescription}}', N'Event', 1, 'system', GETUTCDATE(), 0),

(4, N'Admission Confirmation',
 N'Admission Confirmation - {{StudentName}} - Green Valley International School',
 N'<h2>Admission Confirmed!</h2><p>Dear {{GuardianName}},</p><p>Congratulations! The admission of <strong>{{StudentName}}</strong> to Class <strong>{{ClassName}}</strong> at Green Valley International School has been confirmed for the academic year {{AcademicYear}}.</p><p><strong>Student ID:</strong> {{StudentId}}<br><strong>Application No:</strong> {{ApplicationNo}}</p><p>Please collect the student ID card and textbooks from the school office on the designated date. Further communication will be sent to your registered email and phone.</p><p>Welcome to the Green Valley family!</p><p>Warm regards,<br>Admission Office<br>Green Valley International School</p>',
 N'{{GuardianName}}, {{StudentName}}, {{ClassName}}, {{AcademicYear}}, {{StudentId}}, {{ApplicationNo}}', N'Registration', 1, 'system', GETUTCDATE(), 0),

(5, N'Password Reset',
 N'Password Reset - Green Valley International School Portal',
 N'<h2>Password Reset</h2><p>Dear {{UserName}},</p><p>We received a request to reset your password for the Green Valley International School portal.</p><p>Your temporary password is: <strong>{{TempPassword}}</strong></p><p>Please log in and change your password immediately for security purposes.</p><p>If you did not request this password reset, please contact the school IT department immediately.</p><p>Thank you,<br>IT Department<br>Green Valley International School</p>',
 N'{{UserName}}, {{TempPassword}}', N'Security', 1, 'system', GETUTCDATE(), 0),

(6, N'Exam Result Published',
 N'Exam Results Published - {{ExamName}} {{AcademicYear}}',
 N'<h2>Exam Results Published</h2><p>Dear {{GuardianName}},</p><p>The results for the <strong>{{ExamName}}</strong> ({{AcademicYear}}) have been published. You can view the results on the parent portal.</p><p><strong>Student:</strong> {{StudentName}}<br><strong>Class:</strong> {{ClassName}}<br><strong>Exam:</strong> {{ExamName}}</p><p>Please log in to the parent portal to view the detailed subject-wise results and grade report.</p><p>If you have any questions, please contact the class teacher or exam controller.</p><p>Warm regards,<br>Examination Department<br>Green Valley International School</p>',
 N'{{GuardianName}}, {{StudentName}}, {{ClassName}}, {{ExamName}}, {{AcademicYear}}', N'Exam', 1, 'system', GETUTCDATE(), 0),

(7, N'General Notification',
 N'{{Subject}} - Green Valley International School',
 N'<h2>{{Subject}}</h2><p>Dear {{RecipientName}},</p><p>{{MessageBody}}</p><p>Thank you,<br>Green Valley International School</p>',
 N'{{Subject}}, {{RecipientName}}, {{MessageBody}}', N'General', 1, 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [EmailTemplates] OFF
GO

PRINT '>>> EmailTemplates done (7)'
GO

-- ============================================================
-- WEBSITE ENTITY SEED SUMMARY
-- ============================================================
PRINT '╔══════════════════════════════════════════════════════════╗'
PRINT '║      WEBSITE ENTITY SEED COMPLETE                       ║'
PRINT '╠══════════════════════════════════════════════════════════╣'
PRINT '║  Green Valley International School - Public Website     ║'
DECLARE @wc INT
SET @wc = (SELECT COUNT(*) FROM [Sliders]);                PRINT '║  [Sliders]          = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [SchoolSettings]);         PRINT '║  [SchoolSettings]   = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [WebsitePages]);           PRINT '║  [WebsitePages]     = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [EventCategories]);        PRINT '║  [EventCategories]  = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [Events]);                 PRINT '║  [Events]           = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [Galleries]);              PRINT '║  [Galleries]        = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [GalleryImages]);          PRINT '║  [GalleryImages]    = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [Announcements]);          PRINT '║  [Announcements]    = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [ContactMessages]);        PRINT '║  [ContactMessages]  = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [AdmissionFeeStructures]); PRINT '║  [AdmissionFeeStr]  = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
SET @wc = (SELECT COUNT(*) FROM [EmailTemplates]);         PRINT '║  [EmailTemplates]   = ' + RIGHT('     ' + CAST(@wc AS NVARCHAR), 5) + '                                         ║'
PRINT '╚══════════════════════════════════════════════════════════╝'
GO

-- ============================================================
-- 47. PREVIOUS ACADEMIC YEAR (2026) FOR PROMOTION CONTEXT
-- ============================================================
SET IDENTITY_INSERT [AcademicYears] ON
GO
INSERT INTO [AcademicYears] ([Id], [Name], [Code], [StartsOn], [EndsOn], [IsActive], [IsCurrent], [IsLocked], [Status], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES (2, 'Academic Year 2026', 'AY2026', '2026-01-01', '2026-12-31', 1, 0, 1, 'Closed', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [AcademicYears] OFF
GO
PRINT '>>> Academic Year 2026 added (Id=2)'
GO

-- ============================================================
-- 48. STUDENT PROMOTION HISTORY
-- 48a. PromotioSession — batch promotion from AY 2026 → 2027
-- ============================================================
SET IDENTITY_INSERT [PromotioSessions] ON
GO
INSERT INTO [PromotioSessions] ([Id], [AcademicYearId], [SessionName], [PromotionDate], [Status], [Remarks], [ExecutedByUserId], [ExecutedAt], [ApprovedByUserId], [ApprovedAt], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES (1, 1, N'AY 2026→2027 Year-End Promotion', '2026-12-31', 'Approved', N'Batch promotion executed for all classes. Most students promoted, some repeaters identified.', 2, '2026-12-31', 1, '2026-12-31', 'system', GETUTCDATE(), 0)
GO
SET IDENTITY_INSERT [PromotioSessions] OFF
GO

-- 48b. PromotionHistory — one per student (Classes 2-13; Play students are new)
PRINT '>>> Generating PromotionHistory for 285 students...'
GO
INSERT INTO [PromotionHistory] ([StudentId], [FromClassId], [ToClassId], [AcademicYearId], [PromotioSessionId], [Status], [PromotedAt], [PromotedByUserId], [NewSectionId], [NewRollNumber], [RollGenerationMethod], [Remarks], [CreatedBy], [CreatedAt], [IsDeleted])
SELECT
    s.Id,
    s.ClassId - 1 AS FromClassId,
    s.ClassId AS ToClassId,
    1 AS AcademicYearId,
    1 AS PromotioSessionId,
    CASE
        WHEN s.Id % 20 = 0 THEN 3  -- Repeat (5%)
        WHEN s.Id % 50 = 0 THEN 4  -- Failed (2%)
        ELSE 2                      -- Promoted (93%)
    END AS Status,
    '2026-12-31' AS PromotedAt,
    2 AS PromotedByUserId,
    (SELECT TOP 1 sec.Id FROM [Sections] sec WHERE sec.SchoolClassId = s.ClassId ORDER BY sec.Name) AS NewSectionId,
    s.RollNumber AS NewRollNumber,
    'MeritBased' AS RollGenerationMethod,
    CASE
        WHEN s.Id % 20 = 0 THEN N'Repeated class — did not meet minimum GPA requirement'
        WHEN s.Id % 50 = 0 THEN N'Failed — critical subjects not passed'
        ELSE N'Promoted based on satisfactory academic performance'
    END AS Remarks,
    'system', GETUTCDATE(), 0
FROM [Students] s
WHERE s.Id >= 16
GO
PRINT '>>> PromotionHistory done'
GO

-- 48c. PromotionExecution — stats per from-class
INSERT INTO [PromotionExecutions] ([AcademicYearId], [SchoolClassId], [TotalStudents], [PromotedCount], [RepeatCount], [FailedCount], [Notes], [ExecutedByUserId], [ExecutedAt], [IsApproved], [ApprovedByUserId], [ApprovedAt], [CreatedBy], [CreatedAt], [IsDeleted])
VALUES
(1, 1, 15, 0, 0, 0, N'Class 1 (Play): New admissions only — no promotions from prior year', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 2, 15, 14, 1, 0, N'Nursery: Promoted from Play; 1 student repeating', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 3, 15, 13, 1, 1, N'KG: Promoted from Nursery; 1 repeat, 1 failed', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 4, 30, 28, 1, 1, N'Class 1: Promoted from KG', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 5, 30, 28, 2, 0, N'Class 2: Promoted from Class 1; 2 repeaters', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 6, 30, 27, 2, 1, N'Class 3: Promoted from Class 2', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 7, 30, 28, 1, 1, N'Class 4: Promoted from Class 3', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 8, 30, 27, 2, 1, N'Class 5: Promoted from Class 4', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 9, 25, 23, 1, 1, N'Class 6: Promoted from Class 5', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 10, 25, 23, 2, 0, N'Class 7: Promoted from Class 6', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 11, 20, 18, 1, 1, N'Class 8: Promoted from Class 7', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 12, 20, 18, 1, 1, N'Class 9: Promoted from Class 8', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0),
(1, 13, 15, 14, 1, 0, N'Class 10: Promoted from Class 9; 1 student repeating for SSC prep', 2, '2026-12-31', 1, 1, '2026-12-31', 'system', GETUTCDATE(), 0)
GO
PRINT '>>> PromotionExecutions done (13)'
GO

-- 48d. StudentPromotion — simplified promotion records
INSERT INTO [StudentPromotions] ([StudentId], [FromClassId], [ToClassId], [AcademicYearId], [PromotedAt], [CreatedBy], [CreatedAt], [IsDeleted])
SELECT s.Id, s.ClassId - 1, s.ClassId, 1, '2026-12-31', 'system', GETUTCDATE(), 0
FROM [Students] s
WHERE s.Id >= 16
GO
PRINT '>>> StudentPromotions done (285)'
GO

-- ============================================================
-- 49. ACTIVITY LOGS (1,000 Records for Audit Module)
-- ============================================================
PRINT '>>> Generating 1,000 ActivityLog records...'
GO
DECLARE @alId INT = 1
WHILE @alId <= 1000
BEGIN
    DECLARE @alUserId INT, @alAction NVARCHAR(100), @alModule NVARCHAR(100)
    DECLARE @alRecordId INT, @alDate DATETIME, @alIp NVARCHAR(64)

    SET @alUserId = ((@alId * 7 + 3) % 34) + 1

    SET @alAction = CASE (@alId % 20)
        WHEN 0 THEN 'User Login'
        WHEN 1 THEN 'User Logout'
        WHEN 2 THEN 'Payment Collected'
        WHEN 3 THEN 'Attendance Marked'
        WHEN 4 THEN 'Student Added'
        WHEN 5 THEN 'Result Published'
        WHEN 6 THEN 'Fee Invoice Generated'
        WHEN 7 THEN 'Exam Created'
        WHEN 8 THEN 'Library Book Issued'
        WHEN 9 THEN 'Book Returned'
        WHEN 10 THEN 'Notice Published'
        WHEN 11 THEN 'Admission Approved'
        WHEN 12 THEN 'Fee Waiver Approved'
        WHEN 13 THEN 'Promotion Executed'
        WHEN 14 THEN 'Student Updated'
        WHEN 15 THEN 'Class Routine Updated'
        WHEN 16 THEN 'Report Generated'
        WHEN 17 THEN 'Settings Changed'
        WHEN 18 THEN 'Email Sent'
        ELSE 'Security Settings Modified'
    END

    SET @alModule = CASE
        WHEN @alAction IN ('User Login','User Logout') THEN 'Auth'
        WHEN @alAction IN ('Payment Collected','Fee Invoice Generated','Fee Waiver Approved') THEN 'Finance'
        WHEN @alAction = 'Attendance Marked' THEN 'Attendance'
        WHEN @alAction IN ('Student Added','Student Updated') THEN 'Students'
        WHEN @alAction IN ('Result Published','Promotion Executed') THEN 'Result'
        WHEN @alAction = 'Exam Created' THEN 'Exam'
        WHEN @alAction IN ('Library Book Issued','Book Returned') THEN 'Library'
        WHEN @alAction IN ('Notice Published','Email Sent') THEN 'Communication'
        WHEN @alAction = 'Admission Approved' THEN 'Admission'
        WHEN @alAction = 'Class Routine Updated' THEN 'Academic'
        WHEN @alAction = 'Report Generated' THEN 'Report'
        WHEN @alAction IN ('Settings Changed','Security Settings Modified') THEN 'Settings'
        ELSE 'General'
    END

    SET @alRecordId = CASE
        WHEN @alAction = 'Payment Collected' THEN (@alId % 1200) + 1
        WHEN @alAction IN ('Student Added','Student Updated') THEN (@alId % 300) + 1
        WHEN @alAction = 'Exam Created' THEN (@alId % 39) + 1
        WHEN @alAction = 'Library Book Issued' THEN (@alId % 20) + 1
        WHEN @alAction = 'Book Returned' THEN (@alId % 20) + 1
        WHEN @alAction = 'Fee Invoice Generated' THEN (@alId % 1800) + 1
        WHEN @alAction = 'Admission Approved' THEN (@alId % 300) + 1
        WHEN @alAction = 'Result Published' THEN (@alId % 39) + 1
        WHEN @alAction = 'Notice Published' THEN (@alId % 30) + 1
        ELSE NULL
    END

    -- Spread dates across Jan-Jun 2027 with weighted recency
    SET @alDate = DATEADD(SECOND, ((@alId * 1337 + 7777) % 15552000), '2027-01-01')

    SET @alIp = CASE (@alId % 8)
        WHEN 0 THEN '192.168.1.100'
        WHEN 1 THEN '192.168.1.101'
        WHEN 2 THEN '10.0.0.50'
        WHEN 3 THEN '10.0.0.51'
        WHEN 4 THEN '172.16.0.10'
        WHEN 5 THEN '192.168.1.200'
        WHEN 6 THEN '192.168.1.50'
        ELSE '203.0.113.' + CAST((@alId % 254) + 1 AS NVARCHAR)
    END

    INSERT INTO [ActivityLogs] ([UserId], [Action], [Module], [RecordId], [OldValues], [NewValues], [IpAddress], [CreatedBy], [CreatedAt], [IsDeleted])
    VALUES (@alUserId, @alAction, @alModule, @alRecordId, NULL, NULL, @alIp, 'system', @alDate, 0)

    SET @alId = @alId + 1
END
GO
PRINT '>>> ActivityLogs done (1,000)'
GO

-- ============================================================
-- 50. DASHBOARD AUGMENTATION (Recent July Data)
-- ============================================================
PRINT '>>> Adding July attendance, payments & sessions for dashboard freshness...'
GO

-- 50a. Attendance for 1-15 July 2027 (30 random students per day)
DECLARE @julDate DATE = '2027-07-01'
WHILE @julDate <= '2027-07-15'
BEGIN
    IF DATEPART(WEEKDAY, @julDate) != 6 -- Skip Friday
    BEGIN
        INSERT INTO [StudentAttendances] ([StudentId], [ClassId], [SectionId], [AttendanceDate], [Status], [Remarks], [RecordedBy], [CreatedAt])
        SELECT TOP 30
            s.Id, s.ClassId, 1, @julDate,
            CASE ABS(CHECKSUM(NEWID())) % 100
                WHEN 0 THEN 4 WHEN 1 THEN 5 WHEN 2 THEN 3 WHEN 3 THEN 3 WHEN 4 THEN 2
                ELSE 1
            END,
            NULL, 'system', GETUTCDATE()
        FROM [Students] s
        ORDER BY NEWID()
    END
    SET @julDate = DATEADD(DAY, 1, @julDate)
END
GO

-- 50b. July payments (100 extra records for recent collections)
DECLARE @julPayId INT = 1201
WHILE @julPayId <= 1300
BEGIN
    DECLARE @jpStudent INT = ((@julPayId * 13) % 300) + 1
    DECLARE @jpAmount DECIMAL(18,2) = 500 + ((@julPayId * 37) % 4500)
    DECLARE @jpDay INT = ((@julPayId - 1201) % 15) + 1
    DECLARE @jpDate DATE = DATEADD(DAY, @jpDay - 1, '2027-07-01')

    INSERT INTO [Payments] ([FeeInvoiceId], [PaidAt], [Amount], [Method], [ReferenceNo], [LateFee], [DiscountAmount], [CreatedBy], [CreatedAt], [IsDeleted])
    SELECT TOP 1 fi.Id, @jpDate, @jpAmount,
        CASE @julPayId % 3 WHEN 0 THEN 1 WHEN 1 THEN 3 ELSE 2 END,
        'JUL-' + RIGHT('0000' + CAST(@julPayId AS NVARCHAR), 6),
        0, 0, 'system', GETUTCDATE(), 0
    FROM [FeeInvoices] fi
    WHERE fi.StudentId = @jpStudent AND fi.Status = 1

    SET @julPayId = @julPayId + 1
END
GO

-- 50c. Recent UserSessions for staff (Jul 2027)
INSERT INTO [UserSessions] ([UserId], [LoginAt], [LogoutAt], [IpAddress], [UserAgent], [CreatedBy], [CreatedAt], [IsDeleted])
SELECT
    u.Id,
    DATEADD(HOUR, 8 + (u.Id % 8), '2027-07-14'),
    DATEADD(HOUR, 10 + (u.Id % 6), '2027-07-14'),
    CASE u.Id % 4 WHEN 0 THEN '192.168.1.100' WHEN 1 THEN '192.168.1.101' WHEN 2 THEN '10.0.0.50' ELSE '172.16.0.10' END,
    'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36',
    'system', GETUTCDATE(), 0
FROM [Users] u
WHERE u.Id <= 14
GO

-- 50d. Update invoice paid amounts for July payments
UPDATE fi SET
    PaidAmount = ISNULL((SELECT SUM(Amount) FROM [Payments] WHERE FeeInvoiceId = fi.Id), 0)
FROM [FeeInvoices] fi
WHERE fi.Id IN (SELECT FeeInvoiceId FROM [Payments] WHERE PaidAt >= '2027-07-01')
GO

PRINT '>>> Dashboard augmentation complete (July 2027 data)'
GO

-- ============================================================
-- FINAL EXTENDED SUMMARY
-- ============================================================
PRINT '╔══════════════════════════════════════════════════════════╗'
PRINT '║      EXTENDED SEED COMPLETE - ALL MODULES               ║'
PRINT '╠══════════════════════════════════════════════════════════╣'
PRINT '║  Green Valley International School - Full Demo Dataset   ║'
DECLARE @xc INT
SET @xc = (SELECT COUNT(*) FROM [PromotioSessions]);     PRINT '║  [PromotioSessions] = ' + RIGHT('     ' + CAST(@xc AS NVARCHAR), 5) + '                                         ║'
SET @xc = (SELECT COUNT(*) FROM [PromotionHistories]);   PRINT '║  [PromotionHistories]= ' + RIGHT('     ' + CAST(@xc AS NVARCHAR), 5) + '                                         ║'
SET @xc = (SELECT COUNT(*) FROM [PromotionExecutions]);  PRINT '║  [PromotionExec]    = ' + RIGHT('     ' + CAST(@xc AS NVARCHAR), 5) + '                                         ║'
SET @xc = (SELECT COUNT(*) FROM [StudentPromotions]);    PRINT '║  [StudentPromotions] = ' + RIGHT('     ' + CAST(@xc AS NVARCHAR), 5) + '                                         ║'
SET @xc = (SELECT COUNT(*) FROM [ActivityLogs]);         PRINT '║  [ActivityLogs]      = ' + RIGHT('     ' + CAST(@xc AS NVARCHAR), 5) + '                                         ║'
PRINT '╚══════════════════════════════════════════════════════════╝'
PRINT ' '
PRINT '>>> ALL SEED DATA LOADED SUCCESSFULLY'
GO