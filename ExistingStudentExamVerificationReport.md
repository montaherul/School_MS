# Phase 34A — Existing Student Exam Seed Verification Report

**Generated:** Phase 34A Implementation
**Database:** SchoolManagementSystemDb
**Server:** MONTAHERUL\SQLEXPRESS

---

## Students Used

| ID | Name | StudentNo | Roll | Class | Section | Religion |
|----|------|-----------|------|-------|---------|----------|
| 1 | Sample Student One | STU-2026-0001 | 1 | Class One | A | IRE (Id=30) |
| 2 | Sample Student Two | STU-2026-0002 | 2 | Class One | A | IRE (Id=30) |
| 3 | Pending Applicant | STU-2026003 | 3 | Class One | A | IRE (Id=30) |

**Total Students:** 3 (all existing, no new students created)

---

## Exam Used

| Field | Value |
|-------|-------|
| Exam Id | 1 |
| Name | Half Yearly Examination 2026 - Class One |
| Term | Half Yearly |
| Class | Class One (Id=1) |
| Academic Year | 2026 (Id=1) |
| Status | **Published** |
| StartsOn | 2026-07-01 |
| EndsOn | 2026-07-15 |

**14 Half Yearly exams** already existed for all classes. Reused Exam Id=1.

---

## Subjects Used (11 per student)

| # | Subject | Code | Components | Religion? |
|---|---------|------|------------|-----------|
| 1 | Bangla | BAN | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |
| 2 | English | ENG | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |
| 3 | Mathematics | MAT | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |
| 4 | General Science | GSCI | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |
| 5 | Bangladesh & Global Studies | SOC | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |
| 6 | ICT | ICT | WRITTEN(70)+MCQ(30)+LAB(25)+CT(10)+ASSIGNMENT(20) | No |
| 7 | Islam and Moral Education | IRE | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | **Yes (assigned)** |
| 8 | Physical Education | PE | WRITTEN(70)+MCQ(30)+PRACTICAL(50)+CT(10)+ASSIGNMENT(20) | No |
| 9 | Arts and Crafts | ART | WRITTEN(70)+MCQ(30)+PRACTICAL(50)+CT(10)+ASSIGNMENT(20) | No |
| 10 | Music | MUS | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |
| 11 | Physical Education, Health & Sports | HEALTH | WRITTEN(70)+MCQ(30)+CT(10)+ASSIGNMENT(20) | No |

**Religion filtering:** BRE, CRE, HRE correctly excluded (students assigned IRE only).

---

## Marks Generated

### Student 1: Sample Student One (Top Student)

| Subject | Written | MCQ | Lab | Practical | Assignment | Total | Grade | GP |
|---------|---------|-----|-----|-----------|------------|-------|-------|----|
| Bangla | 62 | 27 | — | — | 17 | 106 | A+ | 5.00 |
| English | 65 | 28 | — | — | 18 | 111 | A+ | 5.00 |
| Mathematics | 68 | 29 | — | — | 19 | 116 | A+ | 5.00 |
| General Science | 60 | 25 | — | — | 16 | 101 | A+ | 5.00 |
| Bangladesh & Global Studies | 58 | 24 | — | — | 15 | 97 | A+ | 5.00 |
| ICT | 63 | 26 | 20 | — | 17 | 126 | A+ | 5.00 |
| IRE (Religion) | 66 | 28 | — | — | 18 | 112 | A+ | 5.00 |
| Physical Education | 52 | 20 | — | 40 | 13 | 125 | A+ | 5.00 |
| Arts and Crafts | 55 | 22 | — | 38 | 14 | 129 | A+ | 5.00 |
| Music | 50 | 20 | — | — | 13 | 83 | A+ | 5.00 |
| Health | 54 | 22 | — | — | 14 | 90 | A+ | 5.00 |

**Total:** 1196 | **GPA:** 5.00 (A+) | **Position:** 1 | **Passed:** 11/11

### Student 2: Sample Student Two (Average Student)

| Subject | Written | MCQ | Lab | Practical | Assignment | Total | Grade | GP |
|---------|---------|-----|-----|-----------|------------|-------|-------|----|
| Bangla | 48 | 20 | — | — | 12 | 80 | A+ | 5.00 |
| English | 45 | 18 | — | — | 11 | 74 | A | 4.00 |
| Mathematics | 50 | 22 | — | — | 13 | 85 | A+ | 5.00 |
| General Science | 42 | 18 | — | — | 10 | 70 | A | 4.00 |
| Bangladesh & Global Studies | 40 | 17 | — | — | 10 | 67 | A- | 3.50 |
| ICT | 44 | 19 | 15 | — | 12 | 90 | A+ | 5.00 |
| IRE (Religion) | 47 | 20 | — | — | 12 | 79 | A | 4.00 |
| Physical Education | 35 | 15 | — | 32 | 9 | 91 | A+ | 5.00 |
| Arts and Crafts | 38 | 16 | — | 30 | 10 | 94 | A+ | 5.00 |
| Music | 36 | 15 | — | — | 9 | 60 | A- | 3.50 |
| Health | 37 | 16 | — | — | 10 | 63 | A- | 3.50 |

**Total:** 853 | **GPA:** 4.32 (A) | **Position:** 2 | **Passed:** 11/11

### Student 3: Pending Applicant (Mixed — Some Fail)

| Subject | Written | MCQ | Lab | Practical | Assignment | Total | Grade | GP | Status |
|---------|---------|-----|-----|-----------|------------|-------|-------|----|--------|
| Bangla | 42 | 16 | — | — | 10 | 68 | A- | 3.50 | PASS |
| English | 55 | 14 | — | — | 10 | 79 | A | 4.00 | PASS |
| Mathematics | 18 | 6 | — | — | 4 | **28** | **F** | **0.00** | **FAIL** |
| General Science | 20 | 5 | — | — | 5 | **30** | **F** | **0.00** | **FAIL** |
| Bangladesh & Global Studies | 40 | 12 | — | — | 10 | 62 | A- | 3.50 | PASS |
| ICT | 35 | 12 | 8 | — | 10 | 65 | A- | 3.50 | PASS |
| IRE (Religion) | 38 | 10 | — | — | 10 | 58 | B | 3.00 | PASS |
| Physical Education | 28 | 10 | — | 28 | 8 | 74 | A | 4.00 | PASS |
| Arts and Crafts | 30 | 12 | — | 25 | 8 | 75 | A | 4.00 | PASS |
| Music | 15 | 5 | — | — | 5 | **25** | **F** | **0.00** | **FAIL** |
| Health | 30 | 10 | — | — | 8 | 48 | C | 2.00 | PASS |

**Total:** 612 | **GPA:** 3.44 (B) | **Position:** 3 | **Passed:** 8/11 | **Failed:** 3

---

## Results Generated

| Metric | Count |
|--------|-------|
| MarkEntry records | 33 |
| StudentSubjectResult records | 33 |
| StudentExamResult records | 3 |
| ResultPublication records | 1 |

---

## Top 10 Merit List

| Position | Student | GPA | Grade | Total Marks | Status |
|----------|---------|-----|-------|-------------|--------|
| 1 | Sample Student One | 5.00 | A+ | 1196 | Passed |
| 2 | Sample Student Two | 4.32 | A | 853 | Passed |
| 3 | Pending Applicant | 3.44 | B | 612 | Failed (3 subjects) |

---

## Failed Students

| Student | Failed Subjects | Failed Count |
|---------|-----------------|--------------|
| Pending Applicant | Mathematics (28), General Science (30), Music (25) | 3 |

---

## Validation Errors

| Check | Status |
|-------|--------|
| No new students created | ✅ PASS |
| Existing students used | ✅ PASS (3 students) |
| Existing subjects used | ✅ PASS (11 subjects) |
| Existing SubjectMarkStructure used | ✅ PASS |
| Existing exam reused | ✅ PASS (Exam Id=1) |
| Religion filtering correct | ✅ PASS (only IRE for assigned students) |
| Optional subject handling | ✅ PASS (no optional subjects in Class 1-5) |
| Results processed | ✅ PASS (33 SubjectResults + 3 ExamResults) |
| Results published | ✅ PASS (Status=Published) |
| Merit positions calculated | ✅ PASS (1, 2, 3) |
| Pass/Fail logic correct | ✅ PASS (33 marks ≥33 = PASS, <33 = FAIL) |

---

## Summary

| Item | Value |
|------|-------|
| Students Used | 3 (existing) |
| New Students Created | 0 |
| Exam Used | Id=1 (existing) |
| New Exams Created | 0 |
| Subjects Used | 11 per student |
| Marks Generated | 33 |
| SubjectResults Generated | 33 |
| ExamResults Generated | 3 |
| Publications Created | 1 |
| Status | **Published** |
| Ready for Live UI Testing | **YES** |
