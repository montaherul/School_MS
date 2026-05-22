using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddGroupIdToAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "TeacherTimetables",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "TeacherSubjectAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GroupId",
                table: "TeacherClassAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherTimetables_GroupId",
                table: "TeacherTimetables",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjectAssignments_GroupId",
                table: "TeacherSubjectAssignments",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherClassAssignments_GroupId",
                table: "TeacherClassAssignments",
                column: "GroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherClassAssignments_StudentGroups_GroupId",
                table: "TeacherClassAssignments",
                column: "GroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherSubjectAssignments_StudentGroups_GroupId",
                table: "TeacherSubjectAssignments",
                column: "GroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TeacherTimetables_StudentGroups_GroupId",
                table: "TeacherTimetables",
                column: "GroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeacherClassAssignments_StudentGroups_GroupId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherSubjectAssignments_StudentGroups_GroupId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_TeacherTimetables_StudentGroups_GroupId",
                table: "TeacherTimetables");

            migrationBuilder.DropIndex(
                name: "IX_TeacherTimetables_GroupId",
                table: "TeacherTimetables");

            migrationBuilder.DropIndex(
                name: "IX_TeacherSubjectAssignments_GroupId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropIndex(
                name: "IX_TeacherClassAssignments_GroupId",
                table: "TeacherClassAssignments");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "TeacherTimetables");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "TeacherSubjectAssignments");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "TeacherClassAssignments");
        }
    }
}
