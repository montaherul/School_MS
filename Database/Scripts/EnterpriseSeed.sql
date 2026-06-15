-- ============================================================
-- Enterprise Seed Script
-- School Management System
-- Populates 10+ users, employees, teachers, students, 
-- guardians, attendance, ID card data, and supporting entities.
-- Password for all seeded users: 12345
-- ============================================================
SET NOCOUNT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
SET ARITHABORT ON;
SET XACT_ABORT ON;

-- ============================================================
-- DECLARE BASE TIMESTAMPS
-- ============================================================
DECLARE @Now DATETIME2 = SYSUTCDATETIME();
DECLARE @SYSTEM NVARCHAR(64) = N'system';

-- ============================================================
-- 1. DEPARTMENTS
-- ============================================================
PRINT 'Seeding Departments...';
SET IDENTITY_INSERT Departments ON;
INSERT INTO Departments (Id, Name, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, Name, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (1, N'Administration'),
    (2, N'Academic'),
    (3, N'Finance'),
    (4, N'Transport'),
    (5, N'Library'),
    (6, N'Science Lab')
) D(Id, Name)
WHERE NOT EXISTS (SELECT 1 FROM Departments WHERE Id = D.Id);
SET IDENTITY_INSERT Departments OFF;

-- ============================================================
-- 2. DESIGNATIONS
-- ============================================================
PRINT 'Seeding Designations...';
SET IDENTITY_INSERT Designations ON;
INSERT INTO Designations (Id, Name, RoleLevel, IsTeachingRole, IsAdministrativeRole, RequiresLogin, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, Name, RoleLevel, IsTeachingRole, IsAdministrativeRole, RequiresLogin, IsActive, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (1, N'Principal',              10, 0, 1, 1, 1),
    (2, N'Vice Principal',          9, 0, 1, 1, 1),
    (3, N'Senior Lecturer',         8, 1, 0, 1, 1),
    (4, N'Lecturer',                7, 1, 0, 1, 1),
    (5, N'Assistant Teacher',       6, 1, 0, 1, 1),
    (6, N'Office Staff',            4, 0, 1, 1, 1),
    (7, N'Accountant',              5, 0, 1, 1, 1),
    (8, N'Librarian',               4, 0, 0, 1, 1),
    (9, N'Lab Assistant',           3, 0, 0, 1, 1),
    (10, N'Driver',                 2, 0, 0, 1, 1),
    (11, N'Support Staff',          1, 0, 0, 0, 1)
) D(Id, Name, RoleLevel, IsTeachingRole, IsAdministrativeRole, RequiresLogin, IsActive)
WHERE NOT EXISTS (SELECT 1 FROM Designations WHERE Id = D.Id);
SET IDENTITY_INSERT Designations OFF;

-- ============================================================
-- 3. DESIGNATION-ROLE MAPPINGS
-- ============================================================
PRINT 'Seeding DesignationRoleMappings...';
SET IDENTITY_INSERT DesignationRoleMappings ON;
INSERT INTO DesignationRoleMappings (Id, DesignationId, RoleId, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, DesignationId, RoleId, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Principal -> Principal role (Id=2)
    (1,  1, 2),
    -- Vice Principal -> AssistantHead (Id=3)
    (2,  2, 3),
    -- Senior Lecturer -> SeniorLecturer (Id=4)
    (3,  3, 4),
    -- Lecturer -> Teacher (Id=5)
    (4,  4, 5),
    -- Assistant Teacher -> Teacher (Id=5)
    (5,  5, 5),
    -- Office Staff -> Office (Id=6)
    (6,  6, 6),
    -- Accountant -> Accountant (Id=20)
    (7,  7, 20),
    -- Librarian -> Librarian (Id=21)
    (8,  8, 21),
    -- Lab Assistant -> LabAssistant (Id=22)
    (9,  9, 22),
    -- Driver -> TransportStaff (Id=23)
    (10, 10, 23),
    -- Support Staff -> SupportStaff (Id=24)
    (11, 11, 24)
) M(Id, DesignationId, RoleId)
WHERE NOT EXISTS (SELECT 1 FROM DesignationRoleMappings WHERE Id = M.Id);
SET IDENTITY_INSERT DesignationRoleMappings OFF;

-- ============================================================
-- 4. USERS (10 users, IDs 3-12, since 1=admin, 2=superadmin)
--    All passwords: 12345 (PBKDF2-SHA256:100000:...)
-- ============================================================
PRINT 'Seeding Users...';
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (Id, UserName, Email, PhoneNumber, PasswordHash, Status, IsEmailConfirmed, LastLoginAt, FailedLoginAttempts, MustChangePassword, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, UserName, Email, PhoneNumber, PasswordHash, 1 /*Active*/, 1, NULL, 0, 0, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (3,  N'principal',      N'principal@school.local',      N'01700000001', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (4,  N'viceprincipal',  N'viceprincipal@school.local',  N'01700000002', N'PBKDF2-SHA256:100000:EeWsd3Qa9IyG4fJac57OYQ==:WBaKV4Xh9aAJ6zEtQKaK10/q8IdXYXUDGbuRPQQPcTQ='),
    (5,  N'lecturer1',      N'lecturer1@school.local',      N'01700000003', N'PBKDF2-SHA256:100000:Nj6dVn2b6uS4u97pRuNgag==:HXrQD7F/9oCGv6FA5HUpX2sr2BMTV3wP1kYtpJYMqKA='),
    (6,  N'lecturer2',      N'lecturer2@school.local',      N'01700000004', N'PBKDF2-SHA256:100000:pT5OOpOIPBGjqkpl5uk5ag==:WmsRDmFtLIx3QR2gmr1l3+LyW2DVtFYZK5/F2YPuHzE='),
    (7,  N'teacher1',       N'teacher1@school.local',       N'01700000005', N'PBKDF2-SHA256:100000:srHEDKBYxhQnyOxsU9Doqg==:mLyhVVl5angB5brpz0STPyKUuXbWFHJ24kwLbGE/Tns='),
    (8,  N'officer',        N'officer@school.local',        N'01700000006', N'PBKDF2-SHA256:100000:4cJSQB762OK6uzu7aVENfA==:EmHPQjUqP+xecTeWHsry8rWP+d6AR09iZUd1w8r4DjE='),
    (9,  N'accountant',     N'accountant@school.local',     N'01700000007', N'PBKDF2-SHA256:100000:dF7cXESwMpFAeiss+tu5EQ==:kzg1mMDFNox/MR+KcF0LbyZBRWSE0g38dQlqcD9NzrM='),
    (10, N'librarian',      N'librarian@school.local',      N'01700000008', N'PBKDF2-SHA256:100000:dl8k2ZHatMqW6bymiwr5Fg==:TC1dI5aqS4Qsd0SuQA/ufRzZ4h1x65B7XtEpOBOCB70='),
    (11, N'labassistant',   N'labassistant@school.local',   N'01700000009', N'PBKDF2-SHA256:100000:ZexsmGs2d8NOThvGjGDH0Q==:d+PEWR+mrbtjqn/A/qlv59SKbQdR5HCSSyDURwkPGgI='),
    (12, N'driver',         N'driver@school.local',         N'01700000010', N'PBKDF2-SHA256:100000:jipxcMvWf26m0eQ5496wtQ==:GrW83NGxxkGq4VIfjwZZQB+afJfhNziqtq1uy2YrsCU=')
) U(Id, UserName, Email, PhoneNumber, PasswordHash)
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Id = U.Id);
SET IDENTITY_INSERT Users OFF;

-- Assign roles for new users
PRINT 'Seeding UserRoles...';
INSERT INTO UserRoles (UserId, RoleId)
SELECT UserId, RoleId
FROM (VALUES
    (3,  2),   -- principal -> Principal
    (4,  3),   -- viceprincipal -> AssistantHead
    (5,  5),   -- lecturer1 -> Teacher
    (6,  5),   -- lecturer2 -> Teacher
    (7,  5),   -- teacher1 -> Teacher
    (8,  6),   -- officer -> Office
    (9,  20),  -- accountant -> Accountant
    (10, 21),  -- librarian -> Librarian
    (11, 22),  -- labassistant -> LabAssistant
    (12, 23)   -- driver -> TransportStaff
) UR(UserId, RoleId)
WHERE NOT EXISTS (SELECT 1 FROM UserRoles WHERE UserId = UR.UserId AND RoleId = UR.RoleId);

-- ============================================================
-- 5. EMPLOYEES (10 employees, IDs 1-10)
-- ============================================================
PRINT 'Seeding Employees...';
SET IDENTITY_INSERT Employees ON;
INSERT INTO Employees (
    Id, EmployeeCode, FullName, FatherName, MotherName, Gender, DateOfBirth, BloodGroup,
    Religion, Nationality, NIDNumber, Phone, Email, PresentAddress, PermanentAddress,
    JoiningDate, DepartmentId, DesignationId, EmployeeType, IsTeachingStaff, Status,
    UserId, EmergencyContactName, EmergencyContactPhone, Remarks,
    EmployeeCardNumber, CardIssueDate, CardExpiryDate, CardPrintedAt, CardVersion, QRVerificationCode,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT Id, EmployeeCode, FullName, FatherName, MotherName, Gender, DateOfBirth, BloodGroup,
    Religion, Nationality, NIDNumber, Phone, Email, PresentAddress, PermanentAddress,
    JoiningDate, DepartmentId, DesignationId, EmployeeType, IsTeachingStaff, Status,
    UserId, EmergencyContactName, EmergencyContactPhone, Remarks,
    EmployeeCardNumber, CardIssueDate, CardExpiryDate, CardPrintedAt, CardVersion, QRVerificationCode,
    @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
-- 1: Principal
(1, N'EMP-00001', N'Abdur Rahman',     N'Mohammad Ali',  N'Fatima Begum',  N'Male',   '1975-03-15', N'A+', N'Islam', N'Bangladeshi', N'1234567890', N'01711110001', N'principal@school.local', N'12, Gulshan Avenue, Dhaka', N'Village: Charghat, Rajshahi', '2024-01-01', 1, 1, N'Full-Time', 0, N'Active', 3, N'Mrs. Rahman', N'01711119001', N'School principal', N'EMP-CARD-00001', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-PRINCIPAL-00001'),
-- 2: Vice Principal
(2, N'EMP-00002', N'Sultana Parvin',   N'Abdul Jalil',   N'Rahima Khatun', N'Female', '1980-07-22', N'B+', N'Islam', N'Bangladeshi', N'2345678901', N'01711110002', N'viceprincipal@school.local', N'45, Banani, Dhaka', N'Village: Bagha, Rajshahi', '2024-01-01', 2, 2, N'Full-Time', 0, N'Active', 4, N'Mr. Parvin', N'01711119002', N'Vice principal', N'EMP-CARD-00002', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-VP-00002'),
-- 3: Senior Lecturer (Bengali)
(3, N'EMP-00003', N'Shahidul Islam',   N'Abdul Mannan',  N'Ayesha Begum',  N'Male',   '1985-11-10', N'O+', N'Islam', N'Bangladeshi', N'3456789012', N'01711110003', N'lecturer1@school.local', N'78, Mirpur, Dhaka', N'Village: Tanore, Rajshahi', '2024-06-01', 2, 3, N'Full-Time', 1, N'Active', 5, N'Mrs. Islam', N'01711119003', N'Senior lecturer Bengali', N'EMP-CARD-00003', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-LECT-00003'),
-- 4: Lecturer (English)
(4, N'EMP-00004', N'Nafisa Jahan',     N'Kabir Hossain', N'Shahida Begum', N'Female', '1990-02-28', N'AB+',N'Islam', N'Bangladeshi', N'4567890123', N'01711110004', N'lecturer2@school.local', N'22, Uttara, Dhaka', N'Village: Godagari, Rajshahi', '2024-06-01', 2, 4, N'Full-Time', 1, N'Active', 6, N'Mr. Jahan', N'01711119004', N'Lecturer English', N'EMP-CARD-00004', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-LECT-00004'),
-- 5: Assistant Teacher (Mathematics)
(5, N'EMP-00005', N'Rafiqul Hasan',    N'Delwar Hossain', N'Jennatun Nessa', N'Male', '1992-09-05', N'B-', N'Islam', N'Bangladeshi', N'5678901234', N'01711110005', N'teacher1@school.local', N'55, Mohammadpur, Dhaka', N'Village: Paba, Rajshahi', '2025-01-01', 2, 5, N'Full-Time', 1, N'Active', 7, N'Mrs. Hasan', N'01711119005', N'Asst teacher Mathematics', N'EMP-CARD-00005', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-TCHR-00005'),
-- 6: Office Staff
(6, N'EMP-00006', N'Kamrul Hasan',     N'Abul Kalam',    N'Jahanara Begum', N'Male', '1988-04-18', N'A-', N'Islam', N'Bangladeshi', N'6789012345', N'01711110006', N'officer@school.local', N'10, Dhanmondi, Dhaka', N'Village: Boalia, Rajshahi', '2024-01-01', 1, 6, N'Full-Time', 0, N'Active', 8, N'Mrs. Hasan', N'01711119006', N'Office staff', N'EMP-CARD-00006', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-OFF-00006'),
-- 7: Accountant
(7, N'EMP-00007', N'Shamim Ara',       N'Abdur Rahim',   N'Hosneara Begum', N'Female','1987-12-01', N'O-', N'Islam', N'Bangladeshi', N'7890123456', N'01711110007', N'accountant@school.local', N'88, Bashundhara, Dhaka', N'Village: Charghat, Rajshahi', '2024-03-01', 3, 7, N'Full-Time', 0, N'Active', 9, N'Mr. Ara', N'01711119007', N'Senior accountant', N'EMP-CARD-00007', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-ACC-00007'),
-- 8: Librarian
(8, N'EMP-00008', N'Farida Yasmin',    N'Mohammad Hossain', N'Shahin Ara', N'Female', '1993-06-14', N'AB-', N'Islam', N'Bangladeshi', N'8901234567', N'01711110008', N'librarian@school.local', N'33, Shyamoli, Dhaka', N'Village: Mohanpur, Rajshahi', '2024-06-01', 5, 8, N'Full-Time', 0, N'Active', 10, N'Mr. Yasmin', N'01711119008', N'Librarian', N'EMP-CARD-00008', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-LIB-00008'),
-- 9: Lab Assistant
(9, N'EMP-00009', N'Hasan Mahmud',     N'Nurul Islam',    N'Rokeya Begum',  N'Male',   '1995-08-20', N'B+', N'Islam', N'Bangladeshi', N'9012345678', N'01711110009', N'labassistant@school.local', N'99, Rampura, Dhaka', N'Village: Bagmara, Rajshahi', '2025-01-01', 6, 9, N'Full-Time', 0, N'Active', 11, N'Mrs. Mahmud', N'01711119009', N'Lab assistant', N'EMP-CARD-00009', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-LAB-00009'),
-- 10: Driver
(10, N'EMP-00010', N'Ali Hossain',     N'Monir Uddin',   N'Aleya Begum',   N'Male',   '1990-01-10', N'A+', N'Islam', N'Bangladeshi', N'0123456789', N'01711110010', N'driver@school.local', N'44, Malibagh, Dhaka', N'Village: Durgapur, Rajshahi', '2025-06-01', 4, 10, N'Full-Time', 0, N'Active', 12, N'Mrs. Hossain', N'01711119010', N'School bus driver', N'EMP-CARD-00010', '2026-01-01', '2027-12-31', '2026-01-15', 1, N'QR-DRV-00010')
) E(Id, EmployeeCode, FullName, FatherName, MotherName, Gender, DateOfBirth, BloodGroup,
    Religion, Nationality, NIDNumber, Phone, Email, PresentAddress, PermanentAddress,
    JoiningDate, DepartmentId, DesignationId, EmployeeType, IsTeachingStaff, Status,
    UserId, EmergencyContactName, EmergencyContactPhone, Remarks,
    EmployeeCardNumber, CardIssueDate, CardExpiryDate, CardPrintedAt, CardVersion, QRVerificationCode)
WHERE NOT EXISTS (SELECT 1 FROM Employees WHERE Id = E.Id);
SET IDENTITY_INSERT Employees OFF;

-- ============================================================
-- 6. TEACHERS (5 teachers for employees 1-5 which are teaching staff)
-- ============================================================
PRINT 'Seeding Teachers...';
SET IDENTITY_INSERT Teachers ON;
INSERT INTO Teachers (Id, EmployeeId, TeacherCode, SubjectSpecialization, TeachingLevel, IsClassTeacher, IsExamController, IsRoutineCoordinator, TeachingExperienceYears, Remarks, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, EmployeeId, TeacherCode, SubjectSpecialization, TeachingLevel, IsClassTeacher, IsExamController, IsRoutineCoordinator, TeachingExperienceYears, Remarks, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Principal is not in Teachers; Teacher 1 = Senior Lecturer (Bengali)
    (1, 3, N'TCH-00001', N'Bengali, History',               N'Secondary',  0, 1, 0, 11, N'Senior lecturer in Bengali'),
    -- Teacher 2 = Lecturer (English)
    (2, 4, N'TCH-00002', N'English, English Literature',    N'Secondary',  0, 0, 0, 8,  N'Lecturer in English'),
    -- Teacher 3 = Assistant Teacher (Mathematics)
    (3, 5, N'TCH-00003', N'Mathematics, Higher Math',       N'Secondary',  1, 0, 0, 4,  N'Class teacher and mathematics instructor'),
    -- Teacher 4 = also need one more from existing DbInitializer teacher reference (TeacherId=1,2 were used in MarkEntry legacy seed)
    -- The existing DbInitializer references Teachers with Id=1,2 in MarkEntry (EnteredByTeacherId = 1). So we have a conflict.
    -- Since the DbInitializer already seeds Teacher records? Let me check... 
    -- DbInitializer only seeds MarkEntry with EnteredByTeacherId=1,2 -- but does NOT seed Teachers table.
    -- This means MarkEntry(1).EnteredByTeacherId=1 references a Teachers row that doesn't exist!
    -- We must create Teachers(1,2) for the legacy MarkEntry rows to work, plus not break the new ones.
    -- Teacher ID 4 and 5 will be additional teachers for remaining teaching employees
    (4, 1, N'TCH-00004', N'School Administration, Management', N'Secondary', 0, 0, 1, 12, N'Principal (teaching admin)'),
    (5, 2, N'TCH-00005', N'Education, Curriculum',           N'Secondary', 0, 0, 0, 10, N'Vice principal')
) T(Id, EmployeeId, TeacherCode, SubjectSpecialization, TeachingLevel, IsClassTeacher, IsExamController, IsRoutineCoordinator, TeachingExperienceYears, Remarks)
WHERE NOT EXISTS (SELECT 1 FROM Teachers WHERE Id = T.Id);
SET IDENTITY_INSERT Teachers OFF;

-- ============================================================
-- 7. TEACHER CLASS ASSIGNMENTS
-- ============================================================
PRINT 'Seeding TeacherClassAssignments...';
SET IDENTITY_INSERT TeacherClassAssignments ON;
INSERT INTO TeacherClassAssignments (Id, TeacherId, ClassId, GroupId, SectionId, AcademicYearId, IsClassTeacher, IsActive, AssignedAt, AssignedBy, Remarks, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, TeacherId, ClassId, GroupId, SectionId, 1 /*2026*/, IsClassTeacher, 1, @Now, @SYSTEM, Remarks, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Teacher 4 (Principal) -> removed (unique index conflicts with Teacher 1 for Class 9/10 Science A)
    -- Teacher 5 (VP) -> Class 6-8 A
    (1, 5, 6,  NULL, 11, 0, N'VP oversight Class 6-A'),
    (2, 5, 7,  NULL, 13, 0, N'VP oversight Class 7-A'),
    (3, 5, 8,  NULL, 15, 0, N'VP oversight Class 8-A'),
    -- Teacher 1 (Senior Lecturer Bengali) -> Class 9 Science A, Class 10 Science A
    (4, 1, 9,  1, 18, 0, N'Bengali instructor Class 9 Science A'),
    (5, 1, 10, 1, 27, 0, N'Bengali instructor Class 10 Science A'),
    -- Teacher 2 (English Lecturer) -> Class 9 Business A, Class 10 Business A
    (6, 2, 9,  2, 21, 0, N'English instructor Class 9 Business A'),
    (7, 2, 10, 2, 30, 0, N'English instructor Class 10 Business A'),
    -- Teacher 3 (Math Asst Teacher) -> Class 1-3
    (8, 3, 1, NULL, 1,  1, N'Class teacher Class 1-A'),
    (9, 3, 2, NULL, 3,  0, N'Math instructor Class 2-A'),
    (10, 3, 3, NULL, 5,  0, N'Math instructor Class 3-A')
) AS T(Id, TeacherId, ClassId, GroupId, SectionId, IsClassTeacher, Remarks)
WHERE NOT EXISTS (SELECT 1 FROM TeacherClassAssignments WHERE Id = T.Id);
SET IDENTITY_INSERT TeacherClassAssignments OFF;

-- ============================================================
-- 8. TEACHER SUBJECT ASSIGNMENTS
-- ============================================================
PRINT 'Seeding TeacherSubjectAssignments...';
SET IDENTITY_INSERT TeacherSubjectAssignments ON;
INSERT INTO TeacherSubjectAssignments (Id, TeacherId, SubjectId, ClassId, GroupId, SectionId, AcademicYearId, IsActive, AssignedAt, AssignedBy, Remarks, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, TeacherId, SubjectId, ClassId, GroupId, SectionId, 1, 1, @Now, @SYSTEM, Remarks, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Teacher 1 (Sr Lecturer Bengali) -> Bangla 1st/2nd Paper Class 9-10 Science A
    (1,  1, 9,  9,  1, 18, N'Bangla 1st Paper Class 9 Science A'),
    (2,  1, 10, 9,  1, 18, N'Bangla 2nd Paper Class 9 Science A'),
    (3,  1, 9,  10, 1, 27, N'Bangla 1st Paper Class 10 Science A'),
    (4,  1, 10, 10, 1, 27, N'Bangla 2nd Paper Class 10 Science A'),
    -- Teacher 2 (Lecturer English) -> English 1st/2nd Paper Class 9-10 Business A
    (5,  2, 11, 9,  2, 21, N'English 1st Paper Class 9 Business A'),
    (6,  2, 12, 9,  2, 21, N'English 2nd Paper Class 9 Business A'),
    (7,  2, 11, 10, 2, 30, N'English 1st Paper Class 10 Business A'),
    (8,  2, 12, 10, 2, 30, N'English 2nd Paper Class 10 Business A'),
    -- Teacher 3 (Asst Teacher Math) -> Mathematics Class 1-3
    (9,  3, 3,  1, NULL, 1,  N'Mathematics Class 1-A'),
    (10, 3, 3,  2, NULL, 3,  N'Mathematics Class 2-A'),
    (11, 3, 3,  3, NULL, 5,  N'Mathematics Class 3-A'),
    -- Teacher 4 (Principal) -> General Science, ICT for Class 9 Science A
    (12, 4, 13, 9,  1, 18, N'General Science Class 9 Science A'),
    (13, 4, 14, 9,  1, 18, N'ICT Class 9 Science A'),
    -- Teacher 5 (VP) -> Bangladesh & Global Studies, Agriculture Class 6-A
    (14, 5, 5,  6, NULL, 11, N'BGS Class 6-A'),
    (15, 5, 15, 6, NULL, 11, N'Agriculture Class 6-A')
) AS T(Id, TeacherId, SubjectId, ClassId, GroupId, SectionId, Remarks)
WHERE NOT EXISTS (SELECT 1 FROM TeacherSubjectAssignments WHERE Id = T.Id);
SET IDENTITY_INSERT TeacherSubjectAssignments OFF;

-- ============================================================
-- 9. STUDENTS (10 additional students, IDs 3-12, since 1-2 exist)
-- ============================================================
PRINT 'Seeding Students...';
SET IDENTITY_INSERT Students ON;
INSERT INTO Students (
    Id, StudentNo, FullName, DateOfBirth, Gender, FatherName, MotherName,
    MobileNumber, Nationality, Country, MaritalStatus, Religion, AssignedReligionSubjectId,
    ClassId, SectionId, RollNumber, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT Id, StudentNo, FullName, DateOfBirth, Gender, FatherName, MotherName,
    MobileNumber, Nationality, Country, MaritalStatus, Religion, AssignedReligionSubjectId,
    ClassId, SectionId, RollNumber, 1 /*Active*/, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Class 1 Section A (Id=1), rolls 3-4
    (3,  N'STU-2026-0003', N'Ayesha Khatun',         '2018-03-10', N'Female', N'Abdur Rahim',  N'Hosneara Begum',  N'01720000003', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 1, 1, 3),
    (4,  N'STU-2026-0004', N'Rafiq Hasan',           '2018-06-22', N'Male',   N'Jalil Ahmed',   N'Shahida Parvin', N'01720000004', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 1, 1, 4),
    -- Class 2 Section A (Id=3), rolls 1-2
    (5,  N'STU-2026-0005', N'Sadia Islam',           '2017-04-15', N'Female', N'Delwar Hossain', N'Rokeya Begum',  N'01720000005', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 2, 3, 1),
    (6,  N'STU-2026-0006', N'Kamal Hossain',         '2017-08-30', N'Male',   N'Abul Kashem',   N'Jahanara Begum', N'01720000006', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 2, 3, 2),
    -- Class 3 Section A (Id=5), rolls 1-2
    (7,  N'STU-2026-0007', N'Shahnaz Akhter',       '2016-01-20', N'Female', N'Mohammad Ali',  N'Fatima Begum',   N'01720000007', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 3, 5, 1),
    (8,  N'STU-2026-0008', N'Mizanur Rahman',        '2016-05-14', N'Male',   N'Abdul Mannan',  N'Ayesha Begum',   N'01720000008', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 3, 5, 2),
    -- Class 9 Science A (SectionId=18), rolls 1-2
    (9,  N'STU-2026-0009', N'Taslima Sultana',       '2009-02-10', N'Female', N'Nurul Islam',   N'Rahima Khatun',  N'01720000009', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 9, 18, 1),
    (10, N'STU-2026-0010', N'Hasanuzzaman',          '2009-07-25', N'Male',   N'Shahidul Islam', N'Jennatun Nessa', N'01720000010', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 9, 18, 2),
    -- Class 10 Business Studies A (SectionId=30), rolls 1-2
    (11, N'STU-2026-0011', N'Nazma Begum',           '2008-04-05', N'Female', N'Kabir Hossain', N'Shahin Ara',     N'01720000011', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 10, 30, 1),
    (12, N'STU-2026-0012', N'Rashidul Islam',        '2008-09-18', N'Male',   N'Abdur Rahman',  N'Aleya Begum',    N'01720000012', N'Bangladeshi', N'Bangladesh', N'Single', N'Islam', 30, 10, 30, 2)
) S(Id, StudentNo, FullName, DateOfBirth, Gender, FatherName, MotherName,
    MobileNumber, Nationality, Country, MaritalStatus, Religion, AssignedReligionSubjectId,
    ClassId, SectionId, RollNumber)
WHERE NOT EXISTS (SELECT 1 FROM Students WHERE Id = S.Id);
SET IDENTITY_INSERT Students OFF;

-- ============================================================
-- 10. GUARDIANS (10 guardians, IDs 3-12, since 1-2 exist)
-- ============================================================
PRINT 'Seeding Guardians...';
SET IDENTITY_INSERT Guardians ON;
INSERT INTO Guardians (
    Id, GuardianCode, FirstName, LastName, FullName, Gender, RelationType,
    MobileNumber, Occupation, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT Id, GuardianCode, FirstName, LastName, FullName, Gender, RelationType,
    MobileNumber, Occupation, 1 /*Active*/, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (3,  N'GRD-00003', N'Abdur',   N'Rahim',    N'Abdur Rahim',    N'Male',   1, N'01730000003', N'Teacher'),
    (4,  N'GRD-00004', N'Jalil',   N'Ahmed',    N'Jalil Ahmed',    N'Male',   1, N'01730000004', N'Businessman'),
    (5,  N'GRD-00005', N'Delwar',  N'Hossain',  N'Delwar Hossain', N'Male',   1, N'01730000005', N'Farmer'),
    (6,  N'GRD-00006', N'Abul',    N'Kashem',   N'Abul Kashem',    N'Male',   1, N'01730000006', N'Govt. Employee'),
    (7,  N'GRD-00007', N'Mohammad',N'Ali',       N'Mohammad Ali',   N'Male',   1, N'01730000007', N'Lawyer'),
    (8,  N'GRD-00008', N'Abdul',   N'Mannan',   N'Abdul Mannan',   N'Male',   1, N'01730000008', N'Doctor'),
    (9,  N'GRD-00009', N'Nurul',   N'Islam',    N'Nurul Islam',    N'Male',   1, N'01730000009', N'Engineer'),
    (10, N'GRD-00010', N'Shahidul',N'Islam',    N'Shahidul Islam', N'Male',   1, N'01730000010', N'Teacher'),
    (11, N'GRD-00011', N'Kabir',   N'Hossain',  N'Kabir Hossain',  N'Male',   1, N'01730000011', N'Businessman'),
    (12, N'GRD-00012', N'Abdur',   N'Rahman',   N'Abdur Rahman',   N'Male',   1, N'01730000012', N'Accountant')
) G(Id, GuardianCode, FirstName, LastName, FullName, Gender, RelationType,
    MobileNumber, Occupation)
WHERE NOT EXISTS (SELECT 1 FROM Guardians WHERE Id = G.Id);
SET IDENTITY_INSERT Guardians OFF;

-- ============================================================
-- 11. STUDENT-GUARDIAN JUNCTION (IDs 3-12)
-- ============================================================
PRINT 'Seeding StudentGuardians...';
SET IDENTITY_INSERT StudentGuardians ON;
INSERT INTO StudentGuardians (Id, StudentId, GuardianId, Relationship, IsPrimaryGuardian, ReceivesAttendanceNotifications, ReceivesResultNotifications, ReceivesFeeNotifications, ReceivesSMS, ReceivesEmail, ReceivesWhatsApp, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, StudentId, GuardianId, 1 /*Father*/, 1, 1, 1, 1, 1, 1, 0, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (3,  3,  3),
    (4,  4,  4),
    (5,  5,  5),
    (6,  6,  6),
    (7,  7,  7),
    (8,  8,  8),
    (9,  9,  9),
    (10, 10, 10),
    (11, 11, 11),
    (12, 12, 12)
) SG(Id, StudentId, GuardianId)
WHERE NOT EXISTS (SELECT 1 FROM StudentGuardians WHERE Id = SG.Id);
SET IDENTITY_INSERT StudentGuardians OFF;

-- ============================================================
-- 12. ATTENDANCE RECORDS (5 days for April 2026)
-- ============================================================
PRINT 'Seeding Attendance...';
DECLARE @Date DATE = '2026-04-01';
WHILE @Date <= '2026-04-05'
BEGIN
    INSERT INTO Attendance (StudentId, SchoolClassId, SectionId, AttendanceDate, Status, CreatedBy, CreatedAt, IsDeleted)
    SELECT 1, 1, 1, @Date,
        CASE WHEN @Date = '2026-04-02' THEN 2 ELSE 1 END,
        @SYSTEM, @Now, 0
    WHERE NOT EXISTS (SELECT 1 FROM Attendance WHERE StudentId = 1 AND AttendanceDate = @Date);

    INSERT INTO Attendance (StudentId, SchoolClassId, SectionId, AttendanceDate, Status, CreatedBy, CreatedAt, IsDeleted)
    SELECT 2, 1, 1, @Date,
        CASE WHEN @Date = '2026-04-03' THEN 2 ELSE 1 END,
        @SYSTEM, @Now, 0
    WHERE NOT EXISTS (SELECT 1 FROM Attendance WHERE StudentId = 2 AND AttendanceDate = @Date);

    INSERT INTO Attendance (StudentId, SchoolClassId, SectionId, AttendanceDate, Status, CreatedBy, CreatedAt, IsDeleted)
    SELECT 3, 1, 1, @Date,
        CASE WHEN @Date = '2026-04-04' THEN 3 ELSE 1 END,
        @SYSTEM, @Now, 0
    WHERE NOT EXISTS (SELECT 1 FROM Attendance WHERE StudentId = 3 AND AttendanceDate = @Date);

    INSERT INTO Attendance (StudentId, SchoolClassId, SectionId, AttendanceDate, Status, CreatedBy, CreatedAt, IsDeleted)
    SELECT 4, 1, 1, @Date, 1, @SYSTEM, @Now, 0
    WHERE NOT EXISTS (SELECT 1 FROM Attendance WHERE StudentId = 4 AND AttendanceDate = @Date);

    SET @Date = DATEADD(DAY, 1, @Date);
END;

-- Employee attendance for April 2026 (5 days, all teaching employees 1-5)
SET @Date = '2026-04-01';
WHILE @Date <= '2026-04-05'
BEGIN
    INSERT INTO EmployeeAttendances (EmployeeId, AttendanceDate, CheckInTime, CheckOutTime, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
    SELECT EmployeeId, @Date, CAST('08:00' AS TIME), CAST('16:00' AS TIME), 1 /*Present*/, @Now, @SYSTEM, NULL, NULL, 0
    FROM (VALUES (1),(2),(3),(4),(5)) E(EmployeeId)
    WHERE NOT EXISTS (SELECT 1 FROM EmployeeAttendances WHERE EmployeeId = E.EmployeeId AND AttendanceDate = @Date);
    SET @Date = DATEADD(DAY, 1, @Date);
END;

-- ============================================================
-- 13. CLASS-SUBJECT MAPPINGS (for classes 1, 2, 3, 9, 10)
-- ============================================================
PRINT 'Seeding ClassSubjects...';
SET IDENTITY_INSERT ClassSubjects ON;
INSERT INTO ClassSubjects (Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Class 1 (Id=1): Bangla(1), English(2), Math(3), General Science(4), BGS(5), Religion(6), Arts(7), PE(8), Music(34)
    (1,  1, 1,  NULL, NULL, 100, 33, 1, 1),
    (2,  1, 2,  NULL, NULL, 100, 33, 2, 1),
    (3,  1, 3,  NULL, NULL, 100, 33, 3, 1),
    (4,  1, 4,  NULL, NULL, 100, 33, 4, 1),
    (5,  1, 5,  NULL, NULL, 100, 33, 5, 1),
    (6,  1, 6,  NULL, NULL, 100, 33, 6, 1),
    (7,  1, 7,  NULL, NULL, 100, 33, 7, 1),
    (8,  1, 8,  NULL, NULL, 100, 33, 8, 1),
    (9,  1, 34, NULL, NULL, 100, 33, 9, 1),
    -- Class 2 (Id=2): same subjects
    (10, 2, 1,  NULL, NULL, 100, 33, 1, 1),
    (11, 2, 2,  NULL, NULL, 100, 33, 2, 1),
    (12, 2, 3,  NULL, NULL, 100, 33, 3, 1),
    (13, 2, 4,  NULL, NULL, 100, 33, 4, 1),
    (14, 2, 5,  NULL, NULL, 100, 33, 5, 1),
    (15, 2, 6,  NULL, NULL, 100, 33, 6, 1),
    (16, 2, 7,  NULL, NULL, 100, 33, 7, 1),
    (17, 2, 8,  NULL, NULL, 100, 33, 8, 1),
    (18, 2, 34, NULL, NULL, 100, 33, 9, 1),
    -- Class 3 (Id=3): same subjects
    (19, 3, 1,  NULL, NULL, 100, 33, 1, 1),
    (20, 3, 2,  NULL, NULL, 100, 33, 2, 1),
    (21, 3, 3,  NULL, NULL, 100, 33, 3, 1),
    (22, 3, 4,  NULL, NULL, 100, 33, 4, 1),
    (23, 3, 5,  NULL, NULL, 100, 33, 5, 1),
    (24, 3, 6,  NULL, NULL, 100, 33, 6, 1),
    (25, 3, 7,  NULL, NULL, 100, 33, 7, 1),
    (26, 3, 8,  NULL, NULL, 100, 33, 8, 1),
    (27, 3, 34, NULL, NULL, 100, 33, 9, 1),
    -- Class 9 Science (GroupId=1): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12), Science(13), ICT(14), Agriculture(15), Physics(16), Chemistry(17), Biology(18), Higher Math(19)
    (28, 9, 9,  1, NULL, 100, 33, 1, 1),
    (29, 9, 10, 1, NULL, 100, 33, 2, 1),
    (30, 9, 11, 1, NULL, 100, 33, 3, 1),
    (31, 9, 12, 1, NULL, 100, 33, 4, 1),
    (32, 9, 13, 1, NULL, 100, 33, 5, 1),
    (33, 9, 14, 1, NULL, 100, 33, 6, 1),
    (34, 9, 15, 1, NULL, 100, 33, 7, 1),
    (35, 9, 16, 1, NULL, 100, 33, 8, 1),
    (36, 9, 17, 1, NULL, 100, 33, 9, 1),
    (37, 9, 18, 1, NULL, 100, 33, 10, 1),
    (38, 9, 19, 1, NULL, 100, 33, 11, 1),
    -- Class 10 Business Studies (GroupId=2): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12), Science(13), ICT(14), Agriculture(15), Accounting(20), Finance(21), Business Ent(22)
    (39, 10, 9,  2, NULL, 100, 33, 1, 1),
    (40, 10, 10, 2, NULL, 100, 33, 2, 1),
    (41, 10, 11, 2, NULL, 100, 33, 3, 1),
    (42, 10, 12, 2, NULL, 100, 33, 4, 1),
    (43, 10, 13, 2, NULL, 100, 33, 5, 1),
    (44, 10, 14, 2, NULL, 100, 33, 6, 1),
    (45, 10, 15, 2, NULL, 100, 33, 7, 1),
    (46, 10, 20, 2, NULL, 100, 33, 8, 1),
    (47, 10, 21, 2, NULL, 100, 33, 9, 1),
    (48, 10, 22, 2, NULL, 100, 33, 10, 1)
) CS(Id, SchoolClassId, SubjectId, StudentGroupId, SectionId, FullMarks, PassMarks, DisplayOrder, IsMandatory)
WHERE NOT EXISTS (SELECT 1 FROM ClassSubjects WHERE Id = CS.Id);
SET IDENTITY_INSERT ClassSubjects OFF;

-- ============================================================
-- 14. EXAM TYPES (First Terminal, Half Yearly, etc.)
-- ============================================================
PRINT 'Seeding ExamTypes...';
SET IDENTITY_INSERT ExamTypes ON;
INSERT INTO ExamTypes (Id, Name, Code, Description, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, Name, Code, Description, DisplayOrder, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (1, N'First Terminal',  N'1',     N'First terminal exam',  1),
    (2, N'Half Yearly',     N'2',     N'Half yearly exam',     2),
    (3, N'Second Terminal', N'3',     N'Second terminal exam', 3),
    (4, N'Annual',          N'4',     N'Annual exam',          4),
    (5, N'Final',           N'5',     N'Final exam',           5),
    (6, N'Pre-Test',        N'6',     N'Pre-test exam',        6),
    (7, N'Test',            N'7',     N'Test exam',            7)
) ET(Id, Name, Code, Description, DisplayOrder)
WHERE NOT EXISTS (SELECT 1 FROM ExamTypes WHERE Id = ET.Id);
SET IDENTITY_INSERT ExamTypes OFF;

-- ============================================================
-- 15. EXAM COMPONENTS (Written, MCQ, Practical, Viva, etc.)
-- ============================================================
PRINT 'Seeding ExamComponents...';
SET IDENTITY_INSERT ExamComponents ON;
INSERT INTO ExamComponents (Id, Name, Code, Description, DisplayOrder, DefaultFullMarks, DefaultPassMarks, IsPractical, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, Name, Code, Description, DisplayOrder, DefaultFullMarks, DefaultPassMarks, IsPractical, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (1, N'Written',    N'WRITTEN',   N'Written/Theory exam',          1, 70, 23, 0),
    (2, N'MCQ',        N'MCQ',       N'Multiple choice questions',    2, 30, 10, 0),
    (3, N'Practical',  N'PRACTICAL', N'Practical exam',               3, 50, 17, 1),
    (4, N'Viva',       N'VIVA',      N'Viva voce',                    4, 20, 7,  0),
    (5, N'CQ',         N'CQ',        N'Creative question',            5, 50, 17, 0),
    (6, N'Lab',        N'LAB',       N'Laboratory work',              6, 25, 8,  1),
    (7, N'Oral',       N'ORAL',      N'Oral exam',                    7, 15, 5,  0),
    (8, N'Assignment', N'ASSIGNMENT', N'Subject assignment',          8, 20, 7,  0)
) EC(Id, Name, Code, Description, DisplayOrder, DefaultFullMarks, DefaultPassMarks, IsPractical)
WHERE NOT EXISTS (SELECT 1 FROM ExamComponents WHERE Id = EC.Id);
SET IDENTITY_INSERT ExamComponents OFF;

-- ============================================================
-- 16. EXAM CONFIGURATIONS (ExamType + Class + Group)
-- ============================================================
PRINT 'Seeding ExamConfigurations...';
SET IDENTITY_INSERT ExamConfigurations ON;
INSERT INTO ExamConfigurations (Id, ExamTypeId, ClassId, StudentGroupId, DisplayName, ExamWeightage, DisplayOrder, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, ExamTypeId, ClassId, StudentGroupId, DisplayName, 100.0, DisplayOrder, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Class 1-3: Half Yearly (ExamTypeId=2)
    (1,  2, 1, NULL, N'Class 1 Half Yearly 2026',   1),
    (2,  2, 2, NULL, N'Class 2 Half Yearly 2026',   2),
    (3,  2, 3, NULL, N'Class 3 Half Yearly 2026',   3),
    -- Class 9 Science: Half Yearly
    (4,  2, 9, 1,    N'Class 9 Science Half Yearly', 4),
    -- Class 10 Business: Annual (ExamTypeId=4)
    (5,  4, 10, 2,   N'Class 10 Business Annual',    5)
) EC(Id, ExamTypeId, ClassId, StudentGroupId, DisplayName, DisplayOrder)
WHERE NOT EXISTS (SELECT 1 FROM ExamConfigurations WHERE Id = EC.Id);
SET IDENTITY_INSERT ExamConfigurations OFF;

-- ============================================================
-- 17. EXAMS
-- ============================================================
PRINT 'Seeding Exams...';
SET IDENTITY_INSERT Exams ON;
INSERT INTO Exams (Id, Name, Term, Status, AcademicYearId, ClassId, SectionId, StudentGroupId, StartsOn, EndsOn, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, Name, Term, 1 /*Draft*/, 1, ClassId, SectionId, StudentGroupId, StartsOn, EndsOn, 0, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (2, N'Class 1 Half Yearly 2026',  2 /*HalfYearly*/, 1, NULL, NULL, '2026-05-01', '2026-05-15'),
    (3, N'Class 2 Half Yearly 2026',  2,                 2, NULL, NULL, '2026-05-01', '2026-05-15'),
    (4, N'Class 3 Half Yearly 2026',  2,                 3, NULL, NULL, '2026-05-01', '2026-05-15'),
    (5, N'Class 9 Science Half Yearly',2,                9, NULL, 1,    '2026-05-10', '2026-05-25'),
    (6, N'Class 10 Business Annual',  4 /*Annual*/,      10, NULL, 2,   '2026-11-01', '2026-11-20')
) E(Id, Name, Term, ClassId, SectionId, StudentGroupId, StartsOn, EndsOn)
WHERE NOT EXISTS (SELECT 1 FROM Exams WHERE Id = E.Id);
SET IDENTITY_INSERT Exams OFF;

-- ============================================================
-- 18. EXAM SUBJECTS (subjects linked to exams)
-- ============================================================
PRINT 'Seeding ExamSubjects...';
SET IDENTITY_INSERT ExamSubjects ON;
INSERT INTO ExamSubjects (Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, IsOptional, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks, 0, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Exam 2 (Class 1 Half Yearly): Bangla(1), English(2), Math(3), GenSci(4), BGS(5), Religion(6)
    (1,  2, 1,  1, NULL, 100, 33),
    (2,  2, 2,  1, NULL, 100, 33),
    (3,  2, 3,  1, NULL, 100, 33),
    (4,  2, 4,  1, NULL, 100, 33),
    (5,  2, 5,  1, NULL, 100, 33),
    (6,  2, 6,  1, NULL, 100, 33),
    -- Exam 5 (Class 9 Science): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12), Science(13), ICT(14), Physics(16), Chemistry(17), Biology(18), Higher Math(19)
    (7,  5, 9,  9, 1, 100, 33),
    (8,  5, 10, 9, 1, 100, 33),
    (9,  5, 11, 9, 1, 100, 33),
    (10, 5, 12, 9, 1, 100, 33),
    (11, 5, 13, 9, 1, 100, 33),
    (12, 5, 14, 9, 1, 100, 33),
    (13, 5, 16, 9, 1, 100, 33),
    (14, 5, 17, 9, 1, 100, 33),
    (15, 5, 18, 9, 1, 100, 33),
    (16, 5, 19, 9, 1, 100, 33),
    -- Exam 6 (Class 10 Business Annual): Bangla 1st(9), Bangla 2nd(10), English 1st(11), English 2nd(12), Science(13), ICT(14), Accounting(20), Finance(21), Business Ent(22)
    (17, 6, 9,  10, 2, 100, 33),
    (18, 6, 10, 10, 2, 100, 33),
    (19, 6, 11, 10, 2, 100, 33),
    (20, 6, 12, 10, 2, 100, 33),
    (21, 6, 13, 10, 2, 100, 33),
    (22, 6, 14, 10, 2, 100, 33),
    (23, 6, 20, 10, 2, 100, 33),
    (24, 6, 21, 10, 2, 100, 33),
    (25, 6, 22, 10, 2, 100, 33)
) ES(Id, ExamId, SubjectId, ClassId, StudentGroupId, FullMarks, PassMarks)
WHERE NOT EXISTS (SELECT 1 FROM ExamSubjects WHERE Id = ES.Id);
SET IDENTITY_INSERT ExamSubjects OFF;

-- ============================================================
-- 19. RESULT SETTINGS (first seed)
-- ============================================================
PRINT 'Seeding ResultSettings...';
SET IDENTITY_INSERT ResultSettings ON;
INSERT INTO ResultSettings (Id, AcademicYearId, OptionalSubjectMode, FailSubjectMode, OptionalBonusMaxGPA, BestOfCount, RequirePassedOptionalOnly, MaxFailedCompulsoryAllowed, MinimumPromotionGPA, IncludeReligionInGPA, AutoCalculateComponentTotal, GpaRoundingPrecision, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT 1, 1, 1, 0, 0.50, 1, 1, 0, 1.00, 1, 1, 2, 1, @Now, @SYSTEM, NULL, NULL, 0
WHERE NOT EXISTS (SELECT 1 FROM ResultSettings WHERE Id = 1);
SET IDENTITY_INSERT ResultSettings OFF;

-- ============================================================
-- 20. SUBJECT MARK STRUCTURES (component-level mark allocation)
-- ============================================================
PRINT 'Seeding SubjectMarkStructures...';
SET IDENTITY_INSERT SubjectMarkStructures ON;
INSERT INTO SubjectMarkStructures (Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, IsActive, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks, 1, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Class 1, Subject 1 (Bangla): Written(1)=70, MCQ(2)=30
    (1,  1, 1, 1,  NULL, 70, 23),
    (2,  2, 1, 1,  NULL, 30, 10),
    -- Class 1, Subject 2 (English): Written(1)=70, MCQ(2)=30
    (3,  1, 1, 2,  NULL, 70, 23),
    (4,  2, 1, 2,  NULL, 30, 10),
    -- Class 1, Subject 3 (Math): Written(1)=70, MCQ(2)=30
    (5,  1, 1, 3,  NULL, 70, 23),
    (6,  2, 1, 3,  NULL, 30, 10),
    -- Class 9, Subject 16 (Physics, Science group): Written(1)=50, CQ(5)=25, Practical(3)=25
    (7,  1, 9, 16, 1, 50, 17),
    (8,  5, 9, 16, 1, 25, 8),
    (9,  3, 9, 16, 1, 25, 8),
    -- Class 9, Subject 18 (Biology, Science group): Written(1)=50, CQ(5)=25, Practical(3)=25
    (10, 1, 9, 18, 1, 50, 17),
    (11, 5, 9, 18, 1, 25, 8),
    (12, 3, 9, 18, 1, 25, 8),
    -- Class 10, Subject 20 (Accounting, Business): Written(1)=70, MCQ(2)=30
    (13, 1, 10, 20, 2, 70, 23),
    (14, 2, 10, 20, 2, 30, 10)
) SMS(Id, ComponentId, ClassId, SubjectId, StudentGroupId, FullMarks, PassMarks)
WHERE NOT EXISTS (SELECT 1 FROM SubjectMarkStructures WHERE Id = SMS.Id);
SET IDENTITY_INSERT SubjectMarkStructures OFF;

-- ============================================================
-- 21. MARKS (MarkEntry) for new exams with students
-- ============================================================
PRINT 'Seeding MarkEntries...';
SET IDENTITY_INSERT Marks ON;
INSERT INTO Marks (Id, ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId, WrittenMarks, MCQMarks, MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, ExamId, StudentId, SubjectId, 1, ClassId, SectionId, StudentGroupId, WrittenMarks, MCQMarks, MarksObtained, Grade, GradePoint, EnteredByTeacherId, 5 /*Published*/, 0, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Exam 2 (Class 1 Half Yearly) -> Student 1-4, Subject 1 (Bangla)
    (3,  2, 1, 1, 1, 1, NULL, 56, 24, 80, N'A+', 5.00, 3 /*Teacher 3*/),
    (4,  2, 2, 1, 1, 1, NULL, 50, 20, 70, N'A',  4.00, 3 /*Teacher 3*/),
    (5,  2, 3, 1, 1, 1, NULL, 45, 18, 63, N'A-', 3.50, 3 /*Teacher 3*/),
    (6,  2, 4, 1, 1, 1, NULL, 30, 15, 45, N'C',  2.00, 3 /*Teacher 3*/),
    -- Exam 2, Subject 2 (English)
    (7,  2, 1, 2, 1, 1, NULL, 60, 25, 85, N'A+', 5.00, 3 /*Teacher 3*/),
    (8,  2, 2, 2, 1, 1, NULL, 40, 22, 62, N'A-', 3.50, 3 /*Teacher 3*/),
    (9,  2, 3, 2, 1, 1, NULL, 35, 15, 50, N'B',  3.00, 3 /*Teacher 3*/),
    (10, 2, 4, 2, 1, 1, NULL, 18, 10, 28, N'F',  0.00, 3 /*Teacher 3*/),
    -- Exam 2, Subject 3 (Mathematics)
    (11, 2, 1, 3, 1, 1, NULL, 55, 28, 83, N'A+', 5.00, 3 /*Teacher 3*/),
    (12, 2, 2, 3, 1, 1, NULL, 42, 20, 62, N'A-', 3.50, 3 /*Teacher 3*/),
    (13, 2, 3, 3, 1, 1, NULL, 60, 26, 86, N'A+', 5.00, 3 /*Teacher 3*/),
    (14, 2, 4, 3, 1, 1, NULL, 33, 15, 48, N'C',  2.00, 3 /*Teacher 3*/)
) M(Id, ExamId, StudentId, SubjectId, ClassId, SectionId, StudentGroupId, WrittenMarks, MCQMarks, MarksObtained, Grade, GradePoint, EnteredByTeacherId)
WHERE NOT EXISTS (SELECT 1 FROM Marks WHERE Id = M.Id);
SET IDENTITY_INSERT Marks OFF;

-- ============================================================
-- 22. EMPLOYEE QUALIFICATIONS (for teaching employees)
-- ============================================================
PRINT 'Seeding EmployeeQualifications...';
SET IDENTITY_INSERT EmployeeQualifications ON;
INSERT INTO EmployeeQualifications (Id, EmployeeId, ExamName, BoardOrUniversity, InstituteName, GroupOrSubject, PassingYear, Result, CGPAOrDivision, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, EmployeeId, ExamName, BoardOrUniversity, InstituteName, GroupOrSubject, PassingYear, Result, CGPAOrDivision, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Employee 1 (Principal): BA, MA
    (1, 1, N'SSC',      N'Rajshahi',     N'Rajshahi High School',   N'Science',           N'1990', N'First Division', N'4.50'),
    (2, 1, N'HSC',      N'Rajshahi',     N'Rajshahi College',       N'Humanities',        N'1992', N'First Division', N'4.30'),
    (3, 1, N'B.Ed',     N'National University', N'Rajshahi University', N'Education',     N'1996', N'First Class',    N'3.75'),
    -- Employee 3 (Sr Lecturer Bangla): BA (Hons), MA
    (4, 3, N'SSC',      N'Dhaka',        N'Dhaka Collegiate School', N'Science',           N'2001', N'First Division', N'4.80'),
    (5, 3, N'HSC',      N'Dhaka',        N'Dhaka College',           N'Humanities',        N'2003', N'First Division', N'4.60'),
    (6, 3, N'BA (Hons)', N'University of Dhaka', N'University of Dhaka', N'Bengali',       N'2007', N'First Class',    N'3.85'),
    (7, 3, N'MA',        N'University of Dhaka', N'University of Dhaka', N'Bengali',       N'2009', N'First Class',    N'3.92'),
    -- Employee 4 (Lecturer English): BA (Hons), MA
    (8, 4, N'SSC',       N'Comilla',      N'Comilla Zilla School',    N'Science',           N'2006', N'First Division', N'5.00'),
    (9, 4, N'HSC',       N'Comilla',      N'Comilla College',         N'Humanities',        N'2008', N'First Division', N'4.85'),
    (10,4, N'BA (Hons)', N'University of Rajshahi', N'University of Rajshahi', N'English', N'2012', N'First Class',   N'3.78'),
    -- Employee 5 (Asst Teacher Math): BSc (Hons), MSc
    (11,5, N'SSC',       N'Jessore',      N'Jessore Zilla School',    N'Science',           N'2007', N'First Division', N'5.00'),
    (12,5, N'HSC',       N'Jessore',      N'Jessore College',         N'Science',           N'2009', N'First Division', N'4.90'),
    (13,5, N'BSc (Hons)', N'University of Dhaka', N'University of Dhaka', N'Mathematics',   N'2013', N'First Class',    N'3.65')
) EQ(Id, EmployeeId, ExamName, BoardOrUniversity, InstituteName, GroupOrSubject, PassingYear, Result, CGPAOrDivision)
WHERE NOT EXISTS (SELECT 1 FROM EmployeeQualifications WHERE Id = EQ.Id);
SET IDENTITY_INSERT EmployeeQualifications OFF;

-- ============================================================
-- 23. LEAVE APPLICATIONS (student + teacher)
-- ============================================================
PRINT 'Seeding LeaveApplications...';
SET IDENTITY_INSERT LeaveApplications ON;
INSERT INTO LeaveApplications (Id, EmployeeId, LeaveTypeId, FromDate, ToDate, TotalDays, Reason, ApprovalStatus, CreatedAt)
SELECT Id, EmployeeId, LeaveTypeId, FromDate, ToDate, TotalDays, Reason, 1 /*Pending*/, @Now
FROM (VALUES
    (1, 3, 1 /*SickLeave*/, '2026-04-10', '2026-04-12', 3, N'Fever and cough'),
    (2, 4, 2 /*CasualLeave*/, '2026-04-15', '2026-04-15', 1, N'Family event')
) LA(Id, EmployeeId, LeaveTypeId, FromDate, ToDate, TotalDays, Reason)
WHERE NOT EXISTS (SELECT 1 FROM LeaveApplications WHERE Id = LA.Id);
SET IDENTITY_INSERT LeaveApplications OFF;

-- ============================================================
-- 24. FEE INVOICES for new students
-- ============================================================
PRINT 'Seeding FeeInvoices...';
SET IDENTITY_INSERT FeeInvoices ON;
INSERT INTO FeeInvoices (Id, InvoiceNo, StudentId, DueDate, TotalAmount, PaidAmount, Status, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, InvoiceNo, StudentId, DueDate, TotalAmount, PaidAmount, Status, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (3,  N'INV-2026-0003', 3, '2026-05-10', 2500, 1500, 2 /*Partial*/),
    (4,  N'INV-2026-0004', 4, '2026-05-10', 2500, 2500, 1 /*Paid*/),
    (5,  N'INV-2026-0005', 5, '2026-05-10', 3000, 0,    3 /*Unpaid*/),
    (6,  N'INV-2026-0006', 6, '2026-05-10', 3000, 3000, 1 /*Paid*/),
    (7,  N'INV-2026-0007', 7, '2026-05-10', 3000, 3000, 1 /*Paid*/),
    (8,  N'INV-2026-0008', 8, '2026-05-10', 3000, 2000, 2 /*Partial*/),
    (9,  N'INV-2026-0009', 9, '2026-05-15', 4500, 0,    3 /*Unpaid*/)
) FI(Id, InvoiceNo, StudentId, DueDate, TotalAmount, PaidAmount, Status)
WHERE NOT EXISTS (SELECT 1 FROM FeeInvoices WHERE Id = FI.Id);
SET IDENTITY_INSERT FeeInvoices OFF;

-- ============================================================
-- 25. EMPLOYEE INVITATIONS (Onboarding data)
-- ============================================================
PRINT 'Seeding EmployeeInvitations...';
SET IDENTITY_INSERT EmployeeInvitations ON;
INSERT INTO EmployeeInvitations (
    Id, FullName, Email, Mobile, InvitationCode, InvitationToken,
    DepartmentId, DesignationId, JoiningDate, EmploymentType, Status, IsTeachingStaff,
    Remarks, ExpiresAt, IsUsed, IsApproved, OnboardedAt, CreatedEmployeeId,
    InvitationStatus, SentAt, OpenedAt, CompletedAt,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT
    EI.Id, EI.FullName, EI.Email, EI.Mobile, EI.InvitationCode,
    N'TOKEN-' + CAST(EI.Id AS NVARCHAR(10)) + N'-' + CAST(ABS(CHECKSUM(NEWID())) % 1000000 AS NVARCHAR(10)),
    EI.DepartmentId, EI.DesignationId, EI.JoiningDate, EI.EmploymentType,
    EI.Status, EI.IsTeachingStaff, EI.Remarks, EI.ExpiresAt,
    EI.IsUsed, EI.IsApproved, EI.OnboardedAt, EI.CreatedEmployeeId,
    EI.InvitationStatus, EI.SentAt, EI.OpenedAt, EI.CompletedAt,
    @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (1,  N'Mahbubul Haque',      N'mahbubul.haque@school.local',      N'01740000001', N'INV-00001',
     2, 3,  '2026-07-01', N'Full-Time', N'Active', 1, NULL, DATEADD(DAY, 30, @Now), 0, 0, NULL, NULL,
     N'Sent',   '2026-06-01', NULL, NULL),
    (2,  N'Sharmin Akhter',     N'sharmin.akhter@school.local',      N'01740000002', N'INV-00002',
     2, 4,  '2026-07-01', N'Full-Time', N'Active', 1, NULL, DATEADD(DAY, 30, @Now), 0, 0, NULL, NULL,
     N'Sent',   '2026-06-01', NULL, NULL),
    (3,  N'Jahidul Islam',      N'jahidul.islam@school.local',       N'01740000003', N'INV-00003',
     1, 6,  '2026-07-15', N'Full-Time', N'Active', 0, NULL, DATEADD(DAY, 30, @Now), 0, 0, NULL, NULL,
     N'Started', NULL, NULL, NULL),
    (4,  N'Hasina Begum',       N'hasina.begum@school.local',        N'01740000004', N'INV-00004',
     5, 8,  '2026-08-01', N'Full-Time', N'Active', 0, NULL, DATEADD(DAY, 14, @Now), 0, 0, NULL, NULL,
     N'Started', NULL, NULL, NULL),
    (5,  N'Sohel Rana',         N'sohel.rana@school.local',          N'01740000005', N'INV-00005',
     6, 9,  '2026-08-01', N'Full-Time', N'Active', 0, NULL, DATEADD(DAY, 21, @Now), 0, 0, NULL, NULL,
     N'Started', NULL, NULL, NULL),
    (6,  N'Shahin Alam',        N'shahin.alam@school.local',         N'01740000006', N'INV-00006',
     3, 7,  '2026-07-01', N'Full-Time', N'Active', 0, NULL, DATEADD(DAY, 30, @Now), 0, 1, '2026-06-10', 7,
     N'Completed', '2026-06-01', '2026-06-05', '2026-06-10'),
    (7,  N'Tania Sultana',      N'tania.sultana@school.local',       N'01740000007', N'INV-00007',
     4, 10, '2026-06-01', N'Part-Time', N'Active', 0, N'Weekend driver only', DATEADD(DAY, -5, @Now), 0, 0, NULL, NULL,
     N'Started', NULL, NULL, NULL),
    (8,  N'Abdus Salam',        N'abdus.salam@school.local',         N'01740000008', N'INV-00008',
     2, 5,  '2026-07-15', N'Full-Time', N'Active', 1, N'Primary section', DATEADD(DAY, 60, @Now), 1, 0, '2026-06-12', 7,
     N'Completed', '2026-05-15', '2026-05-20', '2026-06-12'),
    (9,  N'Parvin Sultana',     N'parvin.sultana@school.local',      N'01740000009', N'INV-00009',
     1, 6,  '2026-05-01', N'Full-Time', N'Active', 0, NULL, DATEADD(DAY, -10, @Now), 1, 1, '2026-04-20', 8,
     N'Approved',  '2026-03-15', '2026-03-18', '2026-04-20'),
    (10, N'Rokeya Begum',       N'rokeya.begum@school.local',        N'01740000010', N'INV-00010',
     5, 8,  '2026-04-01', N'Full-Time', N'Expired', 0, NULL, DATEADD(DAY, -90, @Now), 0, 0, NULL, NULL,
     N'Expired',  NULL, NULL, NULL)
) EI(Id, FullName, Email, Mobile, InvitationCode,
    DepartmentId, DesignationId, JoiningDate, EmploymentType, Status, IsTeachingStaff,
    Remarks, ExpiresAt, IsUsed, IsApproved, OnboardedAt, CreatedEmployeeId,
    InvitationStatus, SentAt, OpenedAt, CompletedAt)
WHERE NOT EXISTS (SELECT 1 FROM EmployeeInvitations WHERE Id = EI.Id);
SET IDENTITY_INSERT EmployeeInvitations OFF;

-- ============================================================
-- 26. ADMISSIONS (50 applications)
-- ============================================================
PRINT 'Seeding Admissions...';
SET IDENTITY_INSERT Admissions ON;
INSERT INTO Admissions (
    Id, ApplicationNo, ApplicantName, DateOfBirth, Gender, FatherName, MotherName,
    GuardianName, FatherOrGuardianMobileNo, ApplicantMobileNumber, Nationality, Country,
    MaritalStatus, Religion, AppliedClassId, Status, AdmissionFee, AdmissionFeePaid,
    CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted
)
SELECT Id, ApplicationNo, ApplicantName, DateOfBirth, Gender, FatherName, MotherName,
    GuardianName, GuardianMobile, ApplicantMobile, N'Bangladeshi', N'Bangladesh',
    N'Single', Religion, ClassId, Status, Fee, 0,
    @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    -- Class 1 (5 applicants)
    (2,  N'APP-2026-0002', N'Arif Hossain',       '2019-01-15', N'Male',   N'Jamil Hossain',        N'Akter Banu',             N'Jamil Hossain',        N'01810000002', N'01710000002', N'Islam', 1, 1, 1500),
    (3,  N'APP-2026-0003', N'Mim Akhter',          '2019-03-22', N'Female', N'Abdul Kuddus',         N'Sahida Begum',           N'Abdul Kuddus',          N'01810000003', N'01710000003', N'Islam', 1, 1, 1500),
    (4,  N'APP-2026-0004', N'Tanim Islam',         '2019-05-10', N'Male',   N'Nurul Islam',          N'Fatima Khatun',          N'Nurul Islam',           N'01810000004', N'01710000004', N'Islam', 1, 1, 1500),
    (5,  N'APP-2026-0005', N'Jannatul Ferdous',    '2018-11-08', N'Female', N'Abul Kalam Azad',      N'Rahima Akhter',          N'Abul Kalam Azad',       N'01810000005', N'01710000005', N'Islam', 1, 2, 1500),
    (6,  N'APP-2026-0006', N'Fahim Hasan',         '2019-07-30', N'Male',   N'Shahidul Hasan',       N'Shamima Yeasmin',        N'Shahidul Hasan',        N'01810000006', N'01710000006', N'Islam', 1, 3, 1500),
    -- Class 2 (5 applicants)
    (7,  N'APP-2026-0007', N'Sadia Afrin',         '2018-02-14', N'Female', N'Abdur Rahman',         N'Hosneara Begum',         N'Abdur Rahman',          N'01820000001', N'01720000011', N'Islam', 2, 1, 2000),
    (8,  N'APP-2026-0008', N'Rakibul Islam',       '2017-09-20', N'Male',   N'Delwar Hossain',       N'Shahida Parvin',         N'Delwar Hossain',        N'01820000002', N'01720000012', N'Islam', 2, 1, 2000),
    (9,  N'APP-2026-0009', N'Tasnim Jahan',        '2018-04-05', N'Female', N'Mohammad Ali',         N'Farida Yasmin',          N'Mohammad Ali',          N'01820000003', N'01720000013', N'Islam', 2, 1, 2000),
    (10, N'APP-2026-0010', N'Arman Hossain',       '2017-12-18', N'Male',   N'Jahangir Alam',        N'Rokeya Begum',           N'Jahangir Alam',         N'01820000004', N'01720000014', N'Islam', 2, 2, 2000),
    (11, N'APP-2026-0011', N'Nusrat Jahan',        '2018-06-25', N'Female', N'Kabir Hossain',        N'Shamim Ara',             N'Kabir Hossain',         N'01820000005', N'01720000015', N'Islam', 2, 3, 2000),
    -- Class 3 (5 applicants)
    (12, N'APP-2026-0012', N'Al Amin',             '2017-03-11', N'Male',   N'Abdul Mannan',         N'Ayesha Begum',           N'Abdul Mannan',          N'01830000001', N'01730000010', N'Islam', 3, 1, 2000),
    (13, N'APP-2026-0013', N'Sabina Yasmin',       '2016-08-22', N'Female', N'Shahjahan Ali',        N'Jahanara Begum',         N'Shahjahan Ali',         N'01830000002', N'01730000011', N'Islam', 3, 1, 2000),
    (14, N'APP-2026-0014', N'Rashed Hasan',        '2017-01-30', N'Male',   N'Abul Hashem',          N'Nargis Begum',           N'Abul Hashem',           N'01830000003', N'01730000012', N'Islam', 3, 1, 2000),
    (15, N'APP-2026-0015', N'Nasrin Akhter',       '2016-10-15', N'Female', N'Abdur Rahim',          N'Rahima Khatun',          N'Abdur Rahim',           N'01830000004', N'01730000013', N'Islam', 3, 2, 2000),
    (16, N'APP-2026-0016', N'Sohel Rana',          '2017-05-08', N'Male',   N'Shahin Alam',          N'Shahnaj Begum',          N'Shahin Alam',           N'01830000005', N'01730000014', N'Islam', 3, 3, 2000),
    -- Class 4 (5 applicants)
    (17, N'APP-2026-0017', N'Ruma Akhter',         '2016-02-28', N'Female', N'Jalil Ahmed',          N'Hosneara Parvin',        N'Jalil Ahmed',           N'01840000001', N'01740000015', N'Islam', 4, 1, 2500),
    (18, N'APP-2026-0018', N'Shamim Hossain',      '2015-11-12', N'Male',   N'Shahidul Islam',        N'Shahina Akhter',         N'Shahidul Islam',        N'01840000002', N'01740000016', N'Islam', 4, 1, 2500),
    (19, N'APP-2026-0019', N'Sultana Razia',       '2016-07-05', N'Female', N'Abul Basher',          N'Kohinoor Begum',         N'Abul Basher',           N'01840000003', N'01740000017', N'Islam', 4, 2, 2500),
    (20, N'APP-2026-0020', N'Farhad Hossain',      '2015-09-18', N'Male',   N'Abdur Razzak',         N'Amina Khatun',           N'Abdur Razzak',          N'01840000004', N'01740000018', N'Islam', 4, 1, 2500),
    (21, N'APP-2026-0021', N'Shahnaz Begum',       '2016-04-20', N'Female', N'Khalilur Rahman',      N'Marjina Begum',          N'Khalilur Rahman',       N'01840000005', N'01740000019', N'Islam', 4, 3, 2500),
    -- Class 5 (5 applicants)
    (22, N'APP-2026-0022', N'Imran Hossain',       '2015-01-10', N'Male',   N'Jahir Uddin',          N'Shahida Khatun',         N'Jahir Uddin',           N'01850000001', N'01750000001', N'Islam', 5, 1, 2500),
    (23, N'APP-2026-0023', N'Tania Akhter',        '2014-06-15', N'Female', N'Abul Hossain',         N'Romena Begum',           N'Abul Hossain',          N'01850000002', N'01750000002', N'Islam', 5, 1, 2500),
    (24, N'APP-2026-0024', N'Rafiqul Islam',       '2015-03-22', N'Male',   N'Shahjahan',            N'Jenatun Nesa',           N'Shahjahan',             N'01850000003', N'01750000003', N'Islam', 5, 2, 2500),
    (25, N'APP-2026-0025', N'Shamima Sultana',     '2014-08-30', N'Female', N'Abdur Rahman',         N'Fatima Begum',           N'Abdur Rahman',           N'01850000004', N'01750000004', N'Islam', 5, 1, 2500),
    (26, N'APP-2026-0026', N'Shahin Alam',         '2015-05-14', N'Male',   N'Shah Alam',            N'Rashida Begum',          N'Shah Alam',             N'01850000005', N'01750000005', N'Islam', 5, 3, 2500),
    -- Class 6 (4 applicants)
    (27, N'APP-2026-0027', N'Mahfuzur Rahman',     '2014-01-20', N'Male',   N'Abdur Rashid',         N'Hafiza Begum',           N'Abdur Rashid',          N'01860000001', N'01760000001', N'Islam', 6, 1, 3000),
    (28, N'APP-2026-0028', N'Jannatun Nesa',       '2013-07-11', N'Female', N'Delwar Hossain',       N'Shahnaz Begum',          N'Delwar Hossain',        N'01860000002', N'01760000002', N'Islam', 6, 1, 3000),
    (29, N'APP-2026-0029', N'Sakib Hasan',         '2014-04-05', N'Male',   N'Nurul Islam',          N'Parvin Sultana',         N'Nurul Islam',           N'01860000003', N'01760000003', N'Islam', 6, 2, 3000),
    (30, N'APP-2026-0030', N'Sharmin Akhter',      '2013-10-28', N'Female', N'Shahidul Islam',       N'Sabina Yasmin',          N'Shahidul Islam',        N'01860000004', N'01760000004', N'Islam', 6, 3, 3000),
    -- Class 7 (4 applicants)
    (31, N'APP-2026-0031', N'Rashedul Islam',      '2013-02-18', N'Male',   N'Abdul Mannan',         N'Shamim Ara',             N'Abdul Mannan',          N'01870000001', N'01770000001', N'Islam', 7, 1, 3000),
    (32, N'APP-2026-0032', N'Shahnaj Parvin',      '2012-09-05', N'Female', N'Jahangir Alam',        N'Rokeya Sultana',         N'Jahangir Alam',         N'01870000002', N'01770000002', N'Islam', 7, 1, 3000),
    (33, N'APP-2026-0033', N'Jahid Hasan',         '2013-05-22', N'Male',   N'Abul Hossain',         N'Shahina Khatun',         N'Abul Hossain',          N'01870000003', N'01770000003', N'Islam', 7, 2, 3000),
    (34, N'APP-2026-0034', N'Nazma Begum',         '2012-12-15', N'Female', N'Shahjahan Ali',        N'Jennatun Nesa',          N'Shahjahan Ali',         N'01870000004', N'01770000004', N'Islam', 7, 1, 3000),
    -- Class 8 (4 applicants)
    (35, N'APP-2026-0035', N'Shafiqul Islam',      '2012-03-10', N'Male',   N'Abdur Rahim',          N'Rowshan Ara',            N'Abdur Rahim',           N'01880000001', N'01780000001', N'Islam', 8, 1, 3500),
    (36, N'APP-2026-0036', N'Shamima Sultana',     '2011-08-25', N'Female', N'Shahidul Hasan',       N'Shamima Yeasmin',        N'Shahidul Hasan',        N'01880000002', N'01780000002', N'Islam', 8, 1, 3500),
    (37, N'APP-2026-0037', N'Abul Kalam',          '2012-06-14', N'Male',   N'Abul Kashem',          N'Romena Begum',           N'Abul Kashem',           N'01880000003', N'01780000003', N'Islam', 8, 2, 3500),
    (38, N'APP-2026-0038', N'Taslima Khatun',      '2011-11-30', N'Female', N'Nurul Islam',          N'Hosneara Begum',         N'Nurul Islam',           N'01880000004', N'01780000004', N'Islam', 8, 3, 3500),
    -- Class 9 (7 applicants)
    (39, N'APP-2026-0039', N'Shahidul Alam',       '2010-01-15', N'Male',   N'Abdul Mannan',         N'Sahida Begum',           N'Abdul Mannan',          N'01890000001', N'01790000001', N'Islam', 9, 1, 4000),
    (40, N'APP-2026-0040', N'Tanjina Akhter',      '2010-04-22', N'Female', N'Delwar Hossain',       N'Shahida Parvin',         N'Delwar Hossain',        N'01890000002', N'01790000002', N'Islam', 9, 1, 4000),
    (41, N'APP-2026-0041', N'Hasan Mahmud',        '2009-09-10', N'Male',   N'Shahidul Islam',       N'Rashida Begum',          N'Shahidul Islam',        N'01890000003', N'01790000003', N'Islam', 9, 2, 4000),
    (42, N'APP-2026-0042', N'Sadia Jahan',         '2010-02-28', N'Female', N'Kabir Hossain',        N'Shamim Ara',             N'Kabir Hossain',         N'01890000004', N'01790000004', N'Islam', 9, 1, 4000),
    (43, N'APP-2026-0043', N'Rana Mia',            '2009-11-05', N'Male',   N'Abdur Rahman',         N'Fatima Khatun',          N'Abdur Rahman',          N'01890000005', N'01790000005', N'Islam', 9, 3, 4000),
    (44, N'APP-2026-0044', N'Salma Begum',         '2010-06-18', N'Female', N'Mohammad Ali',         N'Shahina Begum',          N'Mohammad Ali',          N'01890000006', N'01790000006', N'Islam', 9, 1, 4000),
    (45, N'APP-2026-0045', N'Jahangir Alam',       '2009-08-12', N'Male',   N'Abul Kalam',           N'Rowshan Ara',            N'Abul Kalam',            N'01890000007', N'01790000007', N'Hindu', 9, 2, 4000),
    -- Class 10 (6 applicants)
    (46, N'APP-2026-0046', N'Mahbub Hasan',        '2008-03-20', N'Male',   N'Jalil Ahmed',          N'Hosneara Begum',         N'Jalil Ahmed',           N'01900000001', N'01700000040', N'Islam', 10, 1, 4500),
    (47, N'APP-2026-0047', N'Rokeya Begum',        '2008-07-15', N'Female', N'Abdur Rahim',          N'Amina Khatun',           N'Abdur Rahim',           N'01900000002', N'01700000041', N'Islam', 10, 1, 4500),
    (48, N'APP-2026-0048', N'Shahin Alam',         '2007-12-05', N'Male',   N'Shahidul Islam',       N'Shahnaj Begum',          N'Shahidul Islam',        N'01900000003', N'01700000042', N'Islam', 10, 2, 4500),
    (49, N'APP-2026-0049', N'Sharmin Jahan',       '2008-05-22', N'Female', N'Nurul Islam',          N'Jennatun Nesa',          N'Nurul Islam',           N'01900000004', N'01700000043', N'Islam', 10, 1, 4500),
    (50, N'APP-2026-0050', N'Rafiqul Hasan',       '2007-09-10', N'Male',   N'Abdul Mannan',         N'Ayesha Begum',           N'Abdul Mannan',          N'01900000005', N'01700000044', N'Hindu', 10, 3, 4500),
    (51, N'APP-2026-0051', N'Taslima Akhter',      '2008-01-28', N'Female', N'Shahjahan Ali',        N'Rahima Khatun',          N'Shahjahan Ali',         N'01900000006', N'01700000045', N'Islam', 10, 1, 4500)
) A(Id, ApplicationNo, ApplicantName, DateOfBirth, Gender, FatherName, MotherName,
    GuardianName, GuardianMobile, ApplicantMobile, Religion, ClassId, Status, Fee)
WHERE NOT EXISTS (SELECT 1 FROM Admissions WHERE Id = A.Id);
SET IDENTITY_INSERT Admissions OFF;

-- ============================================================
-- 27. GUARDIAN USER ACCOUNTS (for existing guardians 1-12)
-- ============================================================
PRINT 'Seeding guardian user accounts...';
SET IDENTITY_INSERT Users ON;
INSERT INTO Users (Id, UserName, Email, PhoneNumber, PasswordHash, Status, IsEmailConfirmed, LastLoginAt, FailedLoginAttempts, MustChangePassword, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, IsDeleted)
SELECT Id, UserName, Email, PhoneNumber, PasswordHash, 1 /*Active*/, 1, NULL, 0, 0, @Now, @SYSTEM, NULL, NULL, 0
FROM (VALUES
    (13, N'gdn-GRD00001', N'guardian1@school.local',  N'01700000001', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (14, N'gdn-GRD00002', N'guardian2@school.local',  N'01700000002', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (15, N'gdn-GRD00003', N'guardian3@school.local',  N'01800000001', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (16, N'gdn-GRD00004', N'guardian4@school.local',  N'01730000004', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (17, N'gdn-GRD00005', N'guardian5@school.local',  N'01730000005', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (18, N'gdn-GRD00006', N'guardian6@school.local',  N'01730000006', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (19, N'gdn-GRD00007', N'guardian7@school.local',  N'01730000007', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (20, N'gdn-GRD00008', N'guardian8@school.local',  N'01730000008', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (21, N'gdn-GRD00009', N'guardian9@school.local',  N'01730000009', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (22, N'gdn-GRD00010', N'guardian10@school.local', N'01730000010', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (23, N'gdn-GRD00011', N'guardian11@school.local', N'01730000011', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM='),
    (24, N'gdn-GRD00012', N'guardian12@school.local', N'01730000012', N'PBKDF2-SHA256:100000:7U7d3in6upclYrcegdMbeg==:OSGS7hCa80mCzsQDuB0A00RtOV+NZ82sPhuba3tlfxM=')
) U(Id, UserName, Email, PhoneNumber, PasswordHash)
WHERE NOT EXISTS (SELECT 1 FROM Users WHERE Id = U.Id);
SET IDENTITY_INSERT Users OFF;

-- ============================================================
-- 28. GUARDIAN USER-ROLE ASSIGNMENTS (Role ID 25 = Guardian)
-- ============================================================
PRINT 'Seeding guardian user-role assignments...';
INSERT INTO UserRoles (UserId, RoleId)
SELECT U.Id, 25
FROM (VALUES (13),(14),(15),(16),(17),(18),(19),(20),(21),(22),(23),(24)) U(Id)
WHERE NOT EXISTS (SELECT 1 FROM UserRoles UR WHERE UR.UserId = U.Id AND UR.RoleId = 25);

-- ============================================================
-- 29. LINK GUARDIAN RECORDS TO USER ACCOUNTS
-- ============================================================
PRINT 'Linking guardians to user accounts...';
UPDATE Guardians
SET UserId = G.UserId,
    Email = G.Email,
    PortalAccessEnabled = 1,
    UpdatedAt = @Now,
    UpdatedBy = @SYSTEM
FROM (VALUES
    (1, 13, N'guardian1@school.local'),
    (2, 14, N'guardian2@school.local'),
    (3, 15, N'guardian3@school.local'),
    (4, 16, N'guardian4@school.local'),
    (5, 17, N'guardian5@school.local'),
    (6, 18, N'guardian6@school.local'),
    (7, 19, N'guardian7@school.local'),
    (8, 20, N'guardian8@school.local'),
    (9, 21, N'guardian9@school.local'),
    (10,22, N'guardian10@school.local'),
    (11,23, N'guardian11@school.local'),
    (12,24, N'guardian12@school.local')
) G(GuardianId, UserId, Email)
WHERE Guardians.Id = G.GuardianId;

PRINT 'Enterprise seed data insertion complete.';
GO
