==========================================================
PHASE 34A — EXISTING STUDENT EXAM SEED DATA
Generated: Phase 34A Implementation
Database: SchoolManagementSystemDb
Server: MONTAHERUL\SQLEXPRESS

STUDENTS USED: 3 existing (Class One, Section A)
EXAM USED: Id=1 (Half Yearly Examination 2026 - Class One)
SUBJECTS: 14 ExamSubjects already configured
RELIGION: All 3 students assigned IRE (Id=30)
==========================================================
DO NOT EXECUTE BLINDLY — Review before running.
==========================================================

-- ===========================================================
-- STEP 1: CLEAR EXISTING MARKS (clean slate for Class One)
-- ===========================================================
-- Soft-delete existing marks for Exam 1
UPDATE Marks SET IsDeleted=1, UpdatedBy='Phase34A_Cleanup', UpdatedAt=GETUTCDATE()
WHERE ExamId=1 AND IsDeleted=0;

-- ===========================================================
-- STEP 2: GENERATE MARKS FOR ALL 3 STUDENTS
-- ===========================================================
-- Students: Id=1 (Roll 1), Id=2 (Roll 2), Id=3 (Roll 3)
-- All Class One, Section A, IRE assigned
-- Religion filtering: Only IRE for religion subjects

-- Student 1: Sample Student One — TOP STUDENT (high marks)
-- Student 2: Sample Student Two — AVERAGE STUDENT (medium marks)
-- Student 3: Pending Applicant — MIXED (some pass, some fail)

-- ===========================================================
-- MARKS GENERATION
-- ===========================================================

-- -----------------------------------------------
-- STUDENT 1: Sample Student One (Top Student)
-- -----------------------------------------------

-- Bangla (BAN): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20 → Total component max=130
-- Realistic: Written 62, MCQ 27, CT 8, Assignment 17 → Total = 114 → scaled to 100 = 88
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 1, 1, 1, 1, NULL,
    62, 27, NULL, NULL, NULL, NULL, NULL,
    17, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- English (ENG): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 65, MCQ 28, CT 9, Assignment 18 → Total=120 → 92
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 2, 1, 1, 1, NULL,
    65, 28, NULL, NULL, NULL, NULL, NULL,
    18, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Mathematics (MAT): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 68, MCQ 29, CT 9, Assignment 19 → Total=125 → 96
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 3, 1, 1, 1, NULL,
    68, 29, NULL, NULL, NULL, NULL, NULL,
    19, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- General Science (GSCI): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 60, MCQ 25, CT 8, Assignment 16 → Total=109 → 84
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 4, 1, 1, 1, NULL,
    60, 25, NULL, NULL, NULL, NULL, NULL,
    16, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Bangladesh and Global Studies (SOC): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 58, MCQ 24, CT 7, Assignment 15 → Total=104 → 80
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 5, 1, 1, 1, NULL,
    58, 24, NULL, NULL, NULL, NULL, NULL,
    15, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- ICT: WRITTEN=70, MCQ=30, LAB=25, CT=10, ASSIGNMENT=20
-- Written 63, MCQ 26, Lab 20, CT 8, Assignment 17 → Total=154 → scaled to 100=77
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 6, 1, 1, 1, NULL,
    63, 26, NULL, NULL, NULL, 20, NULL,
    17, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Islam and Moral Education (IRE) — ASSIGNED RELIGION: WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 66, MCQ 28, CT 9, Assignment 18 → Total=121 → 93
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 9, 1, 1, 1, NULL,
    66, 28, NULL, NULL, NULL, NULL, NULL,
    18, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Physical Education (PE): WRITTEN=70, MCQ=30, PRACTICAL=50, CT=10, ASSIGNMENT=20
-- Written 55, MCQ 22, Practical 38, CT 7, Assignment 14 → Total=186 → scaled=74
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 7, 1, 1, 1, NULL,
    55, 22, NULL, 38, NULL, NULL, NULL,
    14, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Arts and Crafts (ART): WRITTEN=70, MCQ=30, PRACTICAL=50, CT=10, ASSIGNMENT=20
-- Written 52, MCQ 20, Practical 40, CT 7, Assignment 13 → Total=182 → 73
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 8, 1, 1, 1, NULL,
    52, 20, NULL, 40, NULL, NULL, NULL,
    13, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Music (MUS): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 50, MCQ 20, CT 7, Assignment 13 → Total=110 → 69
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 21, 1, 1, 1, NULL,
    50, 20, NULL, NULL, NULL, NULL, NULL,
    13, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Health (HEALTH): WRITTEN=70, MCQ=30, CT=10, ASSIGNMENT=20
-- Written 54, MCQ 22, CT 7, Assignment 14 → Total=117 → 70
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 1, 22, 1, 1, 1, NULL,
    54, 22, NULL, NULL, NULL, NULL, NULL,
    14, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);


-- -----------------------------------------------
-- STUDENT 2: Sample Student Two (Average Student)
-- -----------------------------------------------

-- Bangla
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 1, 1, 1, 1, NULL,
    48, 20, NULL, NULL, NULL, NULL, NULL,
    12, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- English
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 2, 1, 1, 1, NULL,
    45, 18, NULL, NULL, NULL, NULL, NULL,
    11, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Mathematics
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 3, 1, 1, 1, NULL,
    50, 22, NULL, NULL, NULL, NULL, NULL,
    13, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- General Science
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 4, 1, 1, 1, NULL,
    42, 18, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Bangladesh and Global Studies
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 5, 1, 1, 1, NULL,
    40, 17, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- ICT
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 6, 1, 1, 1, NULL,
    44, 19, NULL, NULL, NULL, 15, NULL,
    12, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- IRE (Assigned Religion)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 9, 1, 1, 1, NULL,
    47, 20, NULL, NULL, NULL, NULL, NULL,
    12, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Physical Education
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 7, 1, 1, 1, NULL,
    38, 16, NULL, 30, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Arts and Crafts
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 8, 1, 1, 1, NULL,
    35, 15, NULL, 32, NULL, NULL, NULL,
    9, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Music
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 21, 1, 1, 1, NULL,
    36, 15, NULL, NULL, NULL, NULL, NULL,
    9, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Health
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 2, 22, 1, 1, 1, NULL,
    37, 16, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);


-- -----------------------------------------------
-- STUDENT 3: Pending Applicant (Mixed — some fail)
-- -----------------------------------------------

-- Bangla — PASS (65)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 1, 1, 1, 1, NULL,
    42, 16, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- English — PASS (76) — already had 76
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 2, 1, 1, 1, NULL,
    55, 14, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Mathematics — FAIL (28)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 3, 1, 1, 1, NULL,
    18, 6, NULL, NULL, NULL, NULL, NULL,
    4, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- General Science — FAIL (30)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 4, 1, 1, 1, NULL,
    20, 5, NULL, NULL, NULL, NULL, NULL,
    5, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Bangladesh and Global Studies — PASS (60)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 5, 1, 1, 1, NULL,
    40, 12, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- ICT — PASS (62)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 6, 1, 1, 1, NULL,
    35, 12, NULL, NULL, NULL, 8, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- IRE (Assigned Religion) — PASS (58)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 9, 1, 1, 1, NULL,
    38, 10, NULL, NULL, NULL, NULL, NULL,
    10, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Physical Education — PASS (55)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 7, 1, 1, 1, NULL,
    30, 12, NULL, 25, NULL, NULL, NULL,
    8, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Arts and Crafts — PASS (50)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 8, 1, 1, 1, NULL,
    28, 10, NULL, 28, NULL, NULL, NULL,
    8, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Music — FAIL (25)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 21, 1, 1, 1, NULL,
    15, 5, NULL, NULL, NULL, NULL, NULL,
    5, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);

-- Health — PASS (48)
INSERT INTO Marks (ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks,
    AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks,
    MarksObtained, Grade, GradePoint, EnteredByTeacherId, Status, IsLocked, ComponentValues,
    CreatedBy, CreatedAt, IsDeleted)
VALUES (1, 3, 22, 1, 1, 1, NULL,
    30, 10, NULL, NULL, NULL, NULL, NULL,
    8, NULL, NULL, NULL, NULL,
    0, NULL, NULL, 1, 2, 0, NULL,
    'Phase34A_Seed', GETUTCDATE(), 0);


-- ===========================================================
-- STEP 3: CALCULATE TOTAL MARKS FOR ALL ENTRIES
-- ===========================================================
-- MarksObtained = sum of all non-null component fields

UPDATE Marks SET MarksObtained = 
    ISNULL(WrittenMarks,0) + ISNULL(MCQMarks,0) + ISNULL(CQMarks,0) + 
    ISNULL(PracticalMarks,0) + ISNULL(VivaMarks,0) + ISNULL(LabMarks,0) + 
    ISNULL(OralMarks,0) + ISNULL(AssignmentMarks,0) + ISNULL(ContinuousAssessmentMarks,0) +
    ISNULL(CompetencyMarks,0) + ISNULL(BehaviourMarks,0) + ISNULL(ParticipationMarks,0)
WHERE ExamId=1 AND IsDeleted=0;

-- Verify totals
SELECT m.Id, s.Name as SubjectName, st.FullName, m.WrittenMarks, m.MCQMarks, m.LabMarks,
       m.PracticalMarks, m.AssignmentMarks, m.MarksObtained as TotalMarks
FROM Marks m
JOIN Subjects s ON m.SubjectId = s.Id
JOIN Students st ON m.StudentId = st.Id
WHERE m.ExamId=1 AND m.IsDeleted=0
ORDER BY st.Id, s.Name;


-- ===========================================================
-- STEP 4: CALCULATE GRADES FOR ALL MARKS
-- ===========================================================
-- Grade lookup: A+(>=80)=5.00, A(>=70)=4.00, A-(>=60)=3.50, 
--               B(>=50)=3.00, C(>=40)=2.00, D(>=33)=1.00, F(<33)=0.00

UPDATE m SET
    m.Grade = CASE 
        WHEN m.MarksObtained >= 80 THEN 'A+'
        WHEN m.MarksObtained >= 70 THEN 'A'
        WHEN m.MarksObtained >= 60 THEN 'A-'
        WHEN m.MarksObtained >= 50 THEN 'B'
        WHEN m.MarksObtained >= 40 THEN 'C'
        WHEN m.MarksObtained >= 33 THEN 'D'
        ELSE 'F'
    END,
    m.GradePoint = CASE 
        WHEN m.MarksObtained >= 80 THEN 5.00
        WHEN m.MarksObtained >= 70 THEN 4.00
        WHEN m.MarksObtained >= 60 THEN 3.50
        WHEN m.MarksObtained >= 50 THEN 3.00
        WHEN m.MarksObtained >= 40 THEN 2.00
        WHEN m.MarksObtained >= 33 THEN 1.00
        ELSE 0.00
    END
FROM Marks m
WHERE m.ExamId=1 AND m.IsDeleted=0;


-- ===========================================================
-- STEP 5: GENERATE StudentSubjectResults
-- ===========================================================
-- For each mark entry, create a StudentSubjectResult
-- Only include religion subject if student is assigned to it

DELETE FROM StudentSubjectResults WHERE ExamId=1 AND IsDeleted=0;

INSERT INTO StudentSubjectResults (
    ExamId, StudentId, SubjectId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    IsOptionalSubject, IsReligionSubject,
    MarksObtained, FullMarks, PassMarks,
    Grade, GradePoint, IsPassed,
    CalculatedAt, CreatedBy, CreatedAt, IsDeleted
)
SELECT 
    m.ExamId,
    m.StudentId,
    m.SubjectId,
    m.AcademicYearId,
    m.ClassId,
    m.SectionId,
    m.StudentGroupId,
    0 as IsOptionalSubject,
    CASE WHEN cs.IsReligionSubject = 1 THEN 1 ELSE 0 END as IsReligionSubject,
    m.MarksObtained,
    es.FullMarks,
    es.PassMarks,
    m.Grade,
    m.GradePoint,
    CASE WHEN m.MarksObtained >= es.PassMarks THEN 1 ELSE 0 END as IsPassed,
    GETUTCDATE(),
    'Phase34A_Seed',
    GETUTCDATE(),
    0
FROM Marks m
JOIN ExamSubjects es ON m.ExamId = es.ExamId AND m.SubjectId = es.SubjectId AND es.IsDeleted=0
JOIN ClassSubjects cs ON m.SubjectId = cs.SubjectId AND cs.SchoolClassId = m.ClassId AND cs.IsDeleted=0 AND cs.IsActive=1
WHERE m.ExamId=1 AND m.IsDeleted=0
-- Only include IRE for these students (all assigned IRE)
AND (
    cs.IsReligionSubject = 0 
    OR m.SubjectId = 30  -- IRE is assigned religion
)
ORDER BY m.StudentId, m.SubjectId;


-- ===========================================================
-- STEP 6: GENERATE StudentExamResults
-- ===========================================================
-- Aggregate all subject results per student into exam result

DELETE FROM StudentExamResults WHERE ExamId=1 AND IsDeleted=0;

INSERT INTO StudentExamResults (
    ExamId, StudentId, AcademicYearId, ClassId, SectionId, StudentGroupId,
    TotalMarks, TotalFullMarks, Gpa, Grade,
    Position, ClassPosition, GroupPosition,
    IsPassed, FailedSubjectCount, PassedSubjectCount,
    Status, CalculatedAt, CreatedBy, CreatedAt, IsDeleted
)
SELECT 
    ssr.ExamId,
    ssr.StudentId,
    ssr.AcademicYearId,
    ssr.ClassId,
    ssr.SectionId,
    ssr.StudentGroupId,
    SUM(ssr.MarksObtained) as TotalMarks,
    SUM(ssr.FullMarks) as TotalFullMarks,
    CAST(
        CASE WHEN COUNT(*) > 0 THEN 
            ROUND(SUM(CASE WHEN ssr.IsPassed = 1 THEN ssr.GradePoint ELSE 0 END) * 1.0 / COUNT(CASE WHEN ssr.IsPassed = 1 THEN 1 END), 2)
        ELSE 0 END
    AS DECIMAL(18,2)) as Gpa,
    '' as Grade,  -- will update below
    0 as Position,
    0 as ClassPosition,
    NULL as GroupPosition,
    CASE WHEN SUM(CASE WHEN ssr.IsPassed = 0 THEN 1 ELSE 0 END) = 0 THEN 1 ELSE 0 END as IsPassed,
    SUM(CASE WHEN ssr.IsPassed = 0 THEN 1 ELSE 0 END) as FailedSubjectCount,
    SUM(CASE WHEN ssr.IsPassed = 1 THEN 1 ELSE 0 END) as PassedSubjectCount,
    5 as Status,  -- Published
    GETUTCDATE(),
    'Phase34A_Seed',
    GETUTCDATE(),
    0
FROM StudentSubjectResults ssr
WHERE ssr.ExamId=1 AND ssr.IsDeleted=0
GROUP BY ssr.ExamId, ssr.StudentId, ssr.AcademicYearId, ssr.ClassId, ssr.SectionId, ssr.StudentGroupId
ORDER BY ssr.StudentId;

-- Update Grade from GPA
UPDATE StudentExamResults SET
    Grade = CASE 
        WHEN Gpa >= 5.00 THEN 'A+'
        WHEN Gpa >= 4.00 THEN 'A'
        WHEN Gpa >= 3.50 THEN 'A-'
        WHEN Gpa >= 3.00 THEN 'B'
        WHEN Gpa >= 2.00 THEN 'C'
        WHEN Gpa >= 1.00 THEN 'D'
        ELSE 'F'
    END
WHERE ExamId=1 AND IsDeleted=0;


-- ===========================================================
-- STEP 7: CALCULATE MERIT POSITIONS
-- ===========================================================
-- Rank students by GPA descending

;WITH RankedStudents AS (
    SELECT 
        StudentId,
        Gpa,
        ROW_NUMBER() OVER (ORDER BY Gpa DESC, TotalMarks DESC) as Position
    FROM StudentExamResults
    WHERE ExamId=1 AND IsDeleted=0
)
UPDATE ser SET
    ser.Position = rs.Position,
    ser.ClassPosition = rs.Position
FROM StudentExamResults ser
JOIN RankedStudents rs ON ser.StudentId = rs.StudentId
WHERE ser.ExamId=1 AND ser.IsDeleted=0;


-- ===========================================================
-- STEP 8: PUBLISH RESULTS
-- ===========================================================
-- Create ResultPublication record

IF NOT EXISTS (SELECT 1 FROM ResultPublications WHERE ExamId=1 AND IsDeleted=0)
BEGIN
    INSERT INTO ResultPublications (
        ExamId, AcademicYearId, Status, PublishedAt, IsLocked, 
        PublicationNotes, CreatedBy, CreatedAt, IsDeleted
    )
    VALUES (1, 1, 5, GETUTCDATE(), 0, 
        'Phase 34A - Auto-published seed data', 'Phase34A_Seed', GETUTCDATE(), 0);
END

-- Update Exam status to Published
UPDATE Exams SET Status = 5 WHERE Id = 1 AND IsDeleted=0;

-- Update all marks to Published status
UPDATE Marks SET Status = 5 WHERE ExamId=1 AND IsDeleted=0;

-- Update all subject results status
UPDATE StudentSubjectResults SET CalculatedAt = GETUTCDATE() WHERE ExamId=1 AND IsDeleted=0;


-- ===========================================================
-- STEP 9: VERIFICATION QUERIES
-- ===========================================================

-- 9a. Students used
SELECT 'STUDENTS USED' as Section, Id, FullName, StudentNo, RollNumber 
FROM Students WHERE IsDeleted=0 ORDER BY Id;

-- 9b. Exam used
SELECT 'EXAM USED' as Section, Id, Name, Status, StartsOn, EndsOn 
FROM Exams WHERE Id=1 AND IsDeleted=0;

-- 9c. Marks generated
SELECT 'MARKS GENERATED' as Section, 
       st.FullName, s.Name as Subject, m.WrittenMarks, m.MCQMarks, m.LabMarks,
       m.PracticalMarks, m.AssignmentMarks, m.MarksObtained as Total, m.Grade
FROM Marks m
JOIN Students st ON m.StudentId = st.Id
JOIN Subjects s ON m.SubjectId = s.Id
WHERE m.ExamId=1 AND m.IsDeleted=0
ORDER BY st.Id, s.Name;

-- 9d. Subject Results
SELECT 'SUBJECT RESULTS' as Section,
       st.FullName, s.Name as Subject, ssr.MarksObtained, ssr.FullMarks, 
       ssr.PassMarks, ssr.Grade, ssr.GradePoint, ssr.IsPassed
FROM StudentSubjectResults ssr
JOIN Students st ON ssr.StudentId = st.Id
JOIN Subjects s ON ssr.SubjectId = s.Id
WHERE ssr.ExamId=1 AND ssr.IsDeleted=0
ORDER BY st.Id, s.Name;

-- 9e. Exam Results (GPA + Position)
SELECT 'EXAM RESULTS' as Section,
       st.FullName, ser.TotalMarks, ser.TotalFullMarks, ser.Gpa, ser.Grade,
       ser.Position, ser.IsPassed, ser.FailedSubjectCount, ser.PassedSubjectCount,
       ser.Status
FROM StudentExamResults ser
JOIN Students st ON ser.StudentId = st.Id
WHERE ser.ExamId=1 AND ser.IsDeleted=0
ORDER BY ser.Position;

-- 9f. Publication
SELECT 'PUBLICATION' as Section, ExamId, Status, PublishedAt, IsLocked
FROM ResultPublications WHERE ExamId=1 AND IsDeleted=0;


-- ===========================================================
-- SUMMARY
-- ===========================================================
-- Students: 3 (existing, Class One)
-- Exam: Id=1 (existing Half Yearly)
-- ExamSubjects: 14 (existing)
-- Marks: 33 (11 subjects × 3 students)
-- SubjectResults: 33 (religion filtered)
-- ExamResults: 3
-- Publication: 1
-- Status: Published
