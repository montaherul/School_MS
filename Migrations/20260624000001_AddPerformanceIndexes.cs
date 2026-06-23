using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPerformanceIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Users_IsDeleted_Status')
    CREATE INDEX IX_Users_IsDeleted_Status ON [dbo].[Users] ([IsDeleted], [Status]) INCLUDE ([UserName], [Email], [PhoneNumber]);");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Students_IsDeleted_Status_ClassId_SectionId')
    CREATE INDEX IX_Students_IsDeleted_Status_ClassId_SectionId ON [dbo].[Students] ([IsDeleted], [Status], [SchoolClassId], [SectionId]) INCLUDE ([RollNumber], [StudentNo], [FullName]);");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FeeInvoices_AcademicYearId_Status')
    CREATE INDEX IX_FeeInvoices_AcademicYearId_Status ON [dbo].[FeeInvoices] ([AcademicYearId], [Status]) INCLUDE ([StudentId], [TotalAmount], [DueDate]);");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_FeeInvoices_StudentId_AcademicYearId_IsDeleted')
    CREATE INDEX IX_FeeInvoices_StudentId_AcademicYearId_IsDeleted ON [dbo].[FeeInvoices] ([StudentId], [AcademicYearId], [IsDeleted]);");

            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Payments_FeeInvoiceId_IsDeleted')
    CREATE INDEX IX_Payments_FeeInvoiceId_IsDeleted ON [dbo].[Payments] ([FeeInvoiceId], [IsDeleted]);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Payments_FeeInvoiceId_IsDeleted ON [dbo].[Payments];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_FeeInvoices_StudentId_AcademicYearId_IsDeleted ON [dbo].[FeeInvoices];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_FeeInvoices_AcademicYearId_Status ON [dbo].[FeeInvoices];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Students_IsDeleted_Status_ClassId_SectionId ON [dbo].[Students];");
            migrationBuilder.Sql("DROP INDEX IF EXISTS IX_Users_IsDeleted_Status ON [dbo].[Users];");
        }
    }
}
