using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnifyD1.Migrations
{
    /// <inheritdoc />
    public partial class StudentUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "BatchId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "BatchId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "BatchId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Batches",
                keyColumn: "BatchId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Courses",
                keyColumn: "CourseId",
                keyValue: 3);

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StudentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.StudentId);
                });

            migrationBuilder.CreateTable(
                name: "StudentBatches",
                columns: table => new
                {
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    EnrollmentId = table.Column<int>(type: "int", nullable: false),
                    dateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentBatches", x => new { x.StudentId, x.BatchId });
                    table.ForeignKey(
                        name: "FK_StudentBatches_Batches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "Batches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentBatches_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentBatches_BatchId",
                table: "StudentBatches",
                column: "BatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentBatches");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "CourseName", "Fees" },
                values: new object[,]
                {
                    { 1, "Computer Science", 50000 },
                    { 2, "Mathematics", 40000 },
                    { 3, "Physics", 45000 }
                });

            migrationBuilder.InsertData(
                table: "Batches",
                columns: new[] { "BatchId", "BatchName", "CourseId", "Slots", "time" },
                values: new object[,]
                {
                    { 1, "CS Morning", 1, 30, new DateTime(2025, 4, 1, 9, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "CS Evening", 1, 25, new DateTime(2025, 4, 1, 18, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "Math Morning", 2, 20, new DateTime(2025, 4, 1, 10, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "Physics Evening", 3, 15, new DateTime(2025, 4, 1, 17, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }
    }
}
