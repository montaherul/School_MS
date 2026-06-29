using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAdmissionPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Filtered index on Status for admission list/dashboard queries
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_Status' AND object_id = OBJECT_ID('Admissions'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_Status
                        ON Admissions([Status])
                        INCLUDE (Id, ApplicationNo, ApplicantName, AppliedClassId, CreatedAt)
                        WHERE IsDeleted = 0;
                END
            ");

            // 2. Filtered index on AppliedClassId for class-filtered admission lists
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_AppliedClassId' AND object_id = OBJECT_ID('Admissions'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_AppliedClassId
                        ON Admissions(AppliedClassId)
                        INCLUDE (Id, ApplicationNo, ApplicantName, [Status], CreatedAt)
                        WHERE IsDeleted = 0;
                END
            ");

            // 3. Unique index on ApplicationNo for fast lookup + duplicate prevention
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_ApplicationNo' AND object_id = OBJECT_ID('Admissions'))
                BEGIN
                    CREATE UNIQUE NONCLUSTERED INDEX IX_AdmissionApplications_ApplicationNo
                        ON Admissions(ApplicationNo)
                        INCLUDE (Id, ApplicantName, [Status], AppliedClassId, CreatedAt)
                        WHERE IsDeleted = 0;
                END
            ");

            // 4. Filtered index on CreatedAt for dashboard/trend queries
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_CreatedAt' AND object_id = OBJECT_ID('Admissions'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_CreatedAt
                        ON Admissions(CreatedAt DESC)
                        INCLUDE (Id, ApplicationNo, [Status])
                        WHERE IsDeleted = 0;
                END
            ");

            // 5. Filtered index on ReviewedByUserId for reviewer-based queries
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionApplications_ReviewedBy' AND object_id = OBJECT_ID('Admissions'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_AdmissionApplications_ReviewedBy
                        ON Admissions(ReviewedByUserId)
                        INCLUDE (Id, ApplicationNo, [Status])
                        WHERE IsDeleted = 0 AND ReviewedByUserId IS NOT NULL;
                END
            ");

            // 6. Index on AdmissionApplicationId for document queries
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_AdmissionDocuments_ApplicationId' AND object_id = OBJECT_ID('AdmissionDocuments'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_AdmissionDocuments_ApplicationId
                        ON AdmissionDocuments(AdmissionApplicationId)
                        INCLUDE (Id, DocumentType, FilePath, VerificationStatus)
                        WHERE IsDeleted = 0;
                END
            ");

            // 7. Index for GuardianCode lookups
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Guardians_GuardianCode' AND object_id = OBJECT_ID('Guardians'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Guardians_GuardianCode
                        ON Guardians(GuardianCode)
                        INCLUDE (Id, FullName, Email, MobileNumber)
                        WHERE IsDeleted = 0;
                END
            ");

            // 8. Index on Email for guardian lookup during conversion
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Guardians_Email' AND object_id = OBJECT_ID('Guardians'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Guardians_Email
                        ON Guardians(Email)
                        INCLUDE (Id, FullName, GuardianCode)
                        WHERE IsDeleted = 0 AND Email IS NOT NULL;
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_AdmissionApplications_Status ON Admissions;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_AdmissionApplications_AppliedClassId ON Admissions;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_AdmissionApplications_ApplicationNo ON Admissions;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_AdmissionApplications_CreatedAt ON Admissions;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_AdmissionApplications_ReviewedBy ON Admissions;");
migrationBuilder.Sql("DROP INDEX IF EXISTS IX_AdmissionDocuments_ApplicationId ON AdmissionDocuments;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Guardians_GuardianCode ON Guardians;");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Guardians_Email ON Guardians;");
        }
    }
}
