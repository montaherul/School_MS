-- Bulk Provision Employee Users
-- Run this to create user accounts for all employees who don't have one.
-- Uses PBKDF2-SHA256 password hash for "12345".
-- Assigns role based on DesignationRoleMapping (fallback: Office Staff = 6).

BEGIN TRANSACTION;

DECLARE @now DATETIME2 = GETUTCDATE();
DECLARE @password NVARCHAR(20) = '12345';

-- Cursor over employees without users whose designation requires login
DECLARE emp_cursor CURSOR FOR
SELECT e.Id, e.EmployeeCode, e.FullName, e.Email, e.Phone, e.DesignationId
FROM Employees e
WHERE e.UserId IS NULL AND e.IsDeleted = 0
  AND EXISTS (SELECT 1 FROM Designations d WHERE d.Id = e.DesignationId AND d.RequiresLogin = 1);

OPEN emp_cursor;

DECLARE @empId INT, @empCode NVARCHAR(50), @fullName NVARCHAR(200), @email NVARCHAR(200), @phone NVARCHAR(50), @desigId INT;
DECLARE @username NVARCHAR(256), @finalUsername NVARCHAR(256), @passwordHash NVARCHAR(500);
DECLARE @salt VARBINARY(16), @key VARBINARY(32), @userId INT;
DECLARE @roleId INT;
DECLARE @count INT;

FETCH NEXT FROM emp_cursor INTO @empId, @empCode, @fullName, @email, @phone, @desigId;

WHILE @@FETCH_STATUS = 0
BEGIN
    -- Generate username from employee code
    SET @username = LOWER(REPLACE(REPLACE(@empCode, '-', ''), '_', ''));

    -- Ensure uniqueness
    SET @count = 1;
    SET @finalUsername = @username;
    WHILE EXISTS (SELECT 1 FROM Users WHERE UserName = @finalUsername AND IsDeleted = 0)
    BEGIN
        SET @finalUsername = @username + CAST(@count AS NVARCHAR);
        SET @count = @count + 1;
    END

    -- Generate PBKDF2-SHA256 hash (use C# code in production; for dev use pre-computed)
    SET @salt = CRYPT_GEN_RANDOM(16);
    SET @passwordHash = 'PBKDF2-SHA256:100000:' + CAST(N'' AS XML).value('xs:base64Binary(sql:variable("@salt"))', 'NVARCHAR(MAX)') + ':' + 'PLACEHOLDER_KEY';

    -- NOTE: SQL Server cannot compute PBKDF2. Use the C# BulkProvisionUsers tool instead.
    -- This script documents the SQL structure; run the C# tool for actual execution.

    PRINT 'Would create user for Employee ' + CAST(@empId AS NVARCHAR) + ': ' + @fullName + ' -> ' + @finalUsername;

    FETCH NEXT FROM emp_cursor INTO @empId, @empCode, @fullName, @email, @phone, @desigId;
END

CLOSE emp_cursor;
DEALLOCATE emp_cursor;

ROLLBACK;
-- Replace with COMMIT after C# tool generates hashes.
PRINT 'Use BulkProvisionUsers console app or dotnet script for production.';
