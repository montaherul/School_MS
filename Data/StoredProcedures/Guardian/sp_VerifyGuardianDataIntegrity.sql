/*
   Stored Procedure: sp_VerifyGuardianDataIntegrity
   Description: Verifies data integrity across all guardian-related tables.
   Reports any orphan records, duplicates, or inconsistencies.
*/
CREATE OR ALTER PROCEDURE [dbo].[sp_VerifyGuardianDataIntegrity]
AS
BEGIN
    SET NOCOUNT ON;

    -- 1. Duplicate Guardian Users (same UserId)
    SELECT 'Duplicate Guardian Users' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('GuardianId=', Id, ', UserId=', UserId), '; ') AS Details
    FROM Guardians
    WHERE UserId IS NOT NULL AND UserId > 0
    GROUP BY UserId
    HAVING COUNT(*) > 1;

    -- 2. Duplicate Guardians by Email
    SELECT 'Duplicate Guardians by Email' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('GuardianId=', Id), ', ') AS Details
    FROM Guardians
    WHERE Email IS NOT NULL AND Email != ''
    GROUP BY LOWER(TRIM(Email))
    HAVING COUNT(*) > 1;

    -- 3. Duplicate Guardians by Mobile
    SELECT 'Duplicate Guardians by Mobile' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('GuardianId=', Id), ', ') AS Details
    FROM Guardians
    WHERE MobileNumber IS NOT NULL AND MobileNumber != ''
    GROUP BY MobileNumber
    HAVING COUNT(*) > 1;

    -- 4. Orphan StudentGuardian (no guardian)
    SELECT 'Orphan StudentGuardian - Missing Guardian' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('StudentGuardianId=', sg.Id, ', StudentId=', sg.StudentId, ', GuardianId=', sg.GuardianId), '; ') AS Details
    FROM StudentGuardians sg
    LEFT JOIN Guardians g ON sg.GuardianId = g.Id
    WHERE g.Id IS NULL;

    -- 5. Orphan StudentGuardian (no student)
    SELECT 'Orphan StudentGuardian - Missing Student' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('StudentGuardianId=', sg.Id, ', StudentId=', sg.StudentId), '; ') AS Details
    FROM StudentGuardians sg
    LEFT JOIN Students s ON sg.StudentId = s.Id
    WHERE s.Id IS NULL;

    -- 6. Orphan Identity Users (guardian users with no guardian record)
    SELECT 'Orphan Identity Users - Guardian without Profile' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('UserId=', u.Id, ', UserName=', u.UserName), '; ') AS Details
    FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    LEFT JOIN Guardians g ON u.Id = g.UserId
    WHERE r.Name = 'Guardian' AND g.Id IS NULL AND u.IsDeleted = 0;

    -- 7. Orphan Guardian Notifications
    SELECT 'Orphan GuardianNotifications' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('NotificationId=', gn.Id, ', GuardianId=', gn.GuardianId), '; ') AS Details
    FROM GuardianNotifications gn
    LEFT JOIN Guardians g ON gn.GuardianId = g.Id
    WHERE g.Id IS NULL;

    -- 8. Guardians with no children (inactive ones excluded)
    SELECT 'Guardians with No Children' AS CheckName,
           COUNT(*) AS IssueCount,
           STRING_AGG(CONCAT('GuardianId=', g.Id, ', Code=', g.GuardianCode, ', Name=', g.FullName), '; ') AS Details
    FROM Guardians g
    LEFT JOIN StudentGuardians sg ON g.Id = sg.GuardianId AND sg.IsDeleted = 0
    WHERE sg.Id IS NULL AND g.IsDeleted = 0;

    -- Summary
    SELECT 'Data Integrity Check Complete' AS Status;
END
GO