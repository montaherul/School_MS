USE SchoolManagementSystemDb;
GO

-- Seed Admission CMS Settings
-- This script populates the SchoolSettings table with admission-related content

-- Insert SchoolSettings with essential fields only
INSERT INTO SchoolSettings (
    SchoolName, ShortName, EIIN, Address, Phone, Email, Website,
    FacebookUrl, YouTubeUrl, LogoPath, FaviconPath, PrincipalName,
    PrincipalMessage, PrincipalImagePath, Mission, Vision,
    FooterText, GoogleMapEmbed, CreatedBy, CreatedAt, IsDeleted,
    ShowAdmissionCTA, ShowEvents, ShowGallery, ShowNotices,
    ShowPrincipalMessage, ShowSlider, ShowStatistics, ShowWelcomeSection,
    AdmissionEnabled, OnlineAdmissionEnabled, ShowAdmissionPage,
    ShowAdmissionFees, ShowAdmissionGuidelines, ShowAdmissionRequirements,
    ShowAdmissionDownloads, AdmissionTitle, AdmissionSubtitle,
    AdmissionGuidelines, AdmissionEligibility, AdmissionProcess,
    AdmissionRequirements, AdmissionFeeNote, AdmissionCtaTitle,
    AdmissionCtaText, AdmissionOpenDate, AdmissionCloseDate,
    AdmissionCircularPath, AdmissionFormPath, AdmissionMetaTitle,
    AdmissionMetaDescription, AdmissionMetaKeywords, AdmissionOgTitle,
    AdmissionOgDescription, AdmissionOgImagePath,
    ClassLabel, EmployeeLabel, OfficeHours, StudentLabel, TeacherLabel
)
VALUES (
    'Chattogram Collegiate School & College', -- SchoolName
    'CCSC', -- ShortName
    '104298', -- EIIN
    'Ice Factory Road, Double Mooring, Chattogram, Bangladesh', -- Address
    '+880 31 610429', -- Phone
    'info@collegiate-school.edu.bd', -- Email
    'https://school-ms-7l3e.onrender.com/', -- Website
    'https://facebook.com/collegiate.school', -- FacebookUrl
    'https://youtube.com/collegiate.school', -- YouTubeUrl
    NULL, -- LogoPath
    NULL, -- FaviconPath
    'Prof. Muhammad Ashraful Islam', -- PrincipalName
    'Welcome to CCSC. For over a century, our institution has stood as a beacon of standard education, building enlightened citizens of tomorrow through quality education and character construction in Bangladesh.', -- PrincipalMessage
    'https://images.unsplash.com/photo-1544717305-2782549b5136?auto=format&fit=crop&q=80&w=400', -- PrincipalImagePath
    'To provide balanced, modern, and value-based education that equips students with critical thinking skills, high moral standards, and patriotic feelings.', -- Mission
    'To remain a premier academic center of secondary and higher education in Bangladesh, forming leaders of tomorrow through innovation and comprehensive extracurricular excellence.', -- Vision
    '© 2026 Chattogram Collegiate School & College. All rights reserved. Managed by Ministry of Education, Bangladesh.', -- FooterText
    '<iframe src="https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d3690.3129598285514!2d91.82390297592472!3d22.341819341499596!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x30acd89686036329%3A0xe100c5c56c2d1b09!2sChattogram%20Collegiate%20School!5e0!3m2!1sen!2sbd!4v1700000000000!5m2!1sen!2sbd" width="100%" height="350" style="border:0;" allowfullscreen="" loading="lazy"></iframe>', -- GoogleMapEmbed
    'system', -- CreatedBy
    GETDATE(), -- CreatedAt
    0, -- IsDeleted
    1, -- ShowAdmissionCTA
    1, -- ShowEvents
    1, -- ShowGallery
    1, -- ShowNotices
    1, -- ShowPrincipalMessage
    1, -- ShowSlider
    1, -- ShowStatistics
    1, -- ShowWelcomeSection
    1, -- AdmissionEnabled
    1, -- OnlineAdmissionEnabled
    1, -- ShowAdmissionPage
    1, -- ShowAdmissionFees
    1, -- ShowAdmissionGuidelines
    1, -- ShowAdmissionRequirements
    1, -- ShowAdmissionDownloads
    'Admissions Open 2026', -- AdmissionTitle
    'Join our academic excellence journey', -- AdmissionSubtitle
    'Admissions are merit based. Students must meet eligibility requirements. Original documents required. Admission subject to seat availability.', -- AdmissionGuidelines
    'Class VI: Minimum age 11 years
Class VII: Successful completion of Class VI
Class VIII: Successful completion of Class VII
Class IX: Successful completion of Class VIII', -- AdmissionEligibility
    '1. Submit application
2. Document verification
3. Admission test/interview
4. Merit list publication
5. Fee payment
6. Final enrollment', -- AdmissionProcess
    'Bring original birth certificate, previous school transcript, passport size photographs, parent NID copy, and transfer certificate (if applicable).', -- AdmissionRequirements
    'Admission fees are non-refundable after enrollment confirmation.', -- AdmissionFeeNote
    'Apply for Admission', -- AdmissionCtaTitle
    'Quick and seamless online registration. Connects directly to our centralized admission processing system. Review the requirements and get enrolled.', -- AdmissionCtaText
    CAST(GETDATE() AS datetime), -- AdmissionOpenDate
    DATEADD(day, 90, CAST(GETDATE() AS datetime)), -- AdmissionCloseDate
    NULL, -- AdmissionCircularPath
    NULL, -- AdmissionFormPath
    NULL, -- AdmissionMetaTitle
    NULL, -- AdmissionMetaDescription
    NULL, -- AdmissionMetaKeywords
    NULL, -- AdmissionOgTitle
    NULL, -- AdmissionOgDescription
    NULL, -- AdmissionOgImagePath
    'Classrooms', -- ClassLabel
    'Staff Members', -- EmployeeLabel
    'Sat - Thu (8:00 AM - 2:00 PM)', -- OfficeHours
    'Active Students', -- StudentLabel
    'Honorable Teachers' -- TeacherLabel
);
GO

-- Seed Admission Fee Structures
-- This script populates the AdmissionFeeStructures table with fee data for all classes

SET IDENTITY_INSERT AdmissionFeeStructures ON;
GO

INSERT INTO AdmissionFeeStructures (
    Id, SchoolClassId, ClassName, AdmissionFee, MonthlyFee, SessionFee, 
    ExamFee, OtherFee, DisplayOrder, IsActive, CreatedBy, CreatedAt, IsDeleted
)
VALUES
(1, 1, 'Class 1', 5000.00, 1500.00, 3000.00, 500.00, 0.00, 1, 1, 'system', GETDATE(), 0),
(2, 2, 'Class 2', 5500.00, 1600.00, 3200.00, 600.00, 0.00, 2, 1, 'system', GETDATE(), 0),
(3, 3, 'Class 3', 6000.00, 1800.00, 3500.00, 700.00, 0.00, 3, 1, 'system', GETDATE(), 0),
(4, 4, 'Class 4', 6500.00, 2000.00, 3800.00, 800.00, 0.00, 4, 1, 'system', GETDATE(), 0),
(5, 5, 'Class 5', 7000.00, 2200.00, 4000.00, 900.00, 0.00, 5, 1, 'system', GETDATE(), 0),
(6, 6, 'Class 6', 7500.00, 2400.00, 4200.00, 1000.00, 0.00, 6, 1, 'system', GETDATE(), 0),
(7, 7, 'Class 7', 8000.00, 2600.00, 4500.00, 1100.00, 0.00, 7, 1, 'system', GETDATE(), 0),
(8, 8, 'Class 8', 8500.00, 2800.00, 4800.00, 1200.00, 0.00, 8, 1, 'system', GETDATE(), 0),
(9, 9, 'Class 9', 9000.00, 3000.00, 5000.00, 1300.00, 0.00, 9, 1, 'system', GETDATE(), 0),
(10, 10, 'Class 10', 10000.00, 3200.00, 5500.00, 1500.00, 0.00, 10, 1, 'system', GETDATE(), 0),
(11, 27, 'Class 11', 12000.00, 4000.00, 6500.00, 1800.00, 0.00, 11, 1, 'system', GETDATE(), 0),
(12, 28, 'Class 12', 13000.00, 4500.00, 7000.00, 2000.00, 0.00, 12, 1, 'system', GETDATE(), 0);

SET IDENTITY_INSERT AdmissionFeeStructures OFF;
GO

PRINT 'Admission CMS data seeded successfully!';
GO

SELECT Id, SchoolClassId, ClassName, AdmissionFee, DisplayOrder, IsActive 
FROM AdmissionFeeStructures 
WHERE IsDeleted = 0 
ORDER BY DisplayOrder;
GO
