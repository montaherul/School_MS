USE SchoolManagementSystemDb;
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
-- Class 1 (Primary Level - Lower Fees)
(1, 1, 'Class 1', 5000.00, 1500.00, 3000.00, 500.00, 0.00, 1, 1, 'system', GETDATE(), 0),
(2, 17, 'Class 1', 5000.00, 1500.00, 3000.00, 500.00, 0.00, 2, 1, 'system', GETDATE(), 0),

-- Class 2 (Primary Level - Lower Fees)
(3, 2, 'Class 2', 5500.00, 1600.00, 3200.00, 600.00, 0.00, 3, 1, 'system', GETDATE(), 0),
(4, 18, 'Class 2', 5500.00, 1600.00, 3200.00, 600.00, 0.00, 4, 1, 'system', GETDATE(), 0),

-- Class 3 (Primary Level - Lower Fees)
(5, 3, 'Class 3', 6000.00, 1800.00, 3500.00, 700.00, 0.00, 5, 1, 'system', GETDATE(), 0),
(6, 19, 'Class 3', 6000.00, 1800.00, 3500.00, 700.00, 0.00, 6, 1, 'system', GETDATE(), 0),

-- Class 4 (Primary Level - Lower Fees)
(7, 4, 'Class 4', 6500.00, 2000.00, 3800.00, 800.00, 0.00, 7, 1, 'system', GETDATE(), 0),
(8, 20, 'Class 4', 6500.00, 2000.00, 3800.00, 800.00, 0.00, 8, 1, 'system', GETDATE(), 0),

-- Class 5 (Primary Level - Lower Fees)
(9, 5, 'Class 5', 7000.00, 2200.00, 4000.00, 900.00, 0.00, 9, 1, 'system', GETDATE(), 0),
(10, 21, 'Class 5', 7000.00, 2200.00, 4000.00, 900.00, 0.00, 10, 1, 'system', GETDATE(), 0),

-- Class 6 (Primary Level - Lower Fees)
(11, 6, 'Class 6', 7500.00, 2400.00, 4200.00, 1000.00, 0.00, 11, 1, 'system', GETDATE(), 0),
(12, 22, 'Class 6', 7500.00, 2400.00, 4200.00, 1000.00, 0.00, 12, 1, 'system', GETDATE(), 0),

-- Class 7 (Primary Level - Lower Fees)
(13, 7, 'Class 7', 8000.00, 2600.00, 4500.00, 1100.00, 0.00, 13, 1, 'system', GETDATE(), 0),
(14, 23, 'Class 7', 8000.00, 2600.00, 4500.00, 1100.00, 0.00, 14, 1, 'system', GETDATE(), 0),

-- Class 8 (Primary Level - Lower Fees)
(15, 8, 'Class 8', 8500.00, 2800.00, 4800.00, 1200.00, 0.00, 15, 1, 'system', GETDATE(), 0),
(16, 24, 'Class 8', 8500.00, 2800.00, 4800.00, 1200.00, 0.00, 16, 1, 'system', GETDATE(), 0),

-- Class 9 (Secondary Level - Medium Fees)
(17, 9, 'Class 9', 9000.00, 3000.00, 5000.00, 1300.00, 0.00, 17, 1, 'system', GETDATE(), 0),
(18, 25, 'Class 9', 9000.00, 3000.00, 5000.00, 1300.00, 0.00, 18, 1, 'system', GETDATE(), 0),

-- Class 10 (Secondary Level - Medium Fees)
(19, 10, 'Class 10', 10000.00, 3200.00, 5500.00, 1500.00, 0.00, 19, 1, 'system', GETDATE(), 0),
(20, 26, 'Class 10', 10000.00, 3200.00, 5500.00, 1500.00, 0.00, 20, 1, 'system', GETDATE(), 0),

-- Class 11 (Higher Secondary - Higher Fees)
(21, 27, 'Class 11', 12000.00, 4000.00, 6500.00, 1800.00, 0.00, 21, 1, 'system', GETDATE(), 0),

-- Class 12 (Higher Secondary - Higher Fees)
(22, 28, 'Class 12', 13000.00, 4500.00, 7000.00, 2000.00, 0.00, 22, 1, 'system', GETDATE(), 0);

SET IDENTITY_INSERT AdmissionFeeStructures OFF;
GO

PRINT 'Admission fee structures seeded successfully!';
GO

SELECT Id, SchoolClassId, ClassName, AdmissionFee, DisplayOrder, IsActive 
FROM AdmissionFeeStructures 
WHERE IsDeleted = 0 
ORDER BY DisplayOrder;
GO
