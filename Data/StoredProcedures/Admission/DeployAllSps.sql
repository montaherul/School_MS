-- ============================================================================
-- Admission Module Stored Procedures Deployment Script
-- Run this script against your SchoolMS database
-- ============================================================================

-- 1. sp_GetAdmissionList (updated with group fields)
-- 2. sp_AdmissionDashboard
-- 3. sp_AdmissionRegisterReport
-- 4. sp_AdmissionTrendAnalysis
-- 5. sp_AdmissionConversionFunnel
-- 6. sp_AdmissionClassDemand
-- 7. sp_AdmissionRevenueReport

-- Note: These SPs use GO batch separators. Execute in SSMS or use sqlcmd with -b flag.

-- ============================================================================
-- SP 1: sp_GetAdmissionList (Updated with AppliedStudentGroupId, AppliedStudentGroupName)
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_GetAdmissionList.sql
GO

-- ============================================================================
-- SP 2: sp_AdmissionDashboard
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_AdmissionDashboard.sql
GO

-- ============================================================================
-- SP 3: sp_AdmissionRegisterReport
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_AdmissionRegisterReport.sql
GO

-- ============================================================================
-- SP 4: sp_AdmissionTrendAnalysis
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_AdmissionTrendAnalysis.sql
GO

-- ============================================================================
-- SP 5: sp_AdmissionConversionFunnel
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_AdmissionConversionFunnel.sql
GO

-- ============================================================================
-- SP 6: sp_AdmissionClassDemand
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_AdmissionClassDemand.sql
GO

-- ============================================================================
-- SP 7: sp_AdmissionRevenueReport
-- ============================================================================
:r Data\StoredProcedures\Admission\sp_AdmissionRevenueReport.sql
GO

PRINT 'All Admission Stored Procedures deployed successfully.';