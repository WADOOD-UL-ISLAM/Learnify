using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnifyD1.Migrations
{
    /// <inheritdoc />
    public partial class initialcrete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fees = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseId);
                });

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
                name: "Batches",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Slots = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Batches", x => x.BatchId);
                    table.ForeignKey(
                        name: "FK_Batches_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "CourseId",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "CourseId", "CourseName", "Fees" },
                values: new object[,]
                {
                    { 1, "Web Development", 0 },
                    { 2, "Mobile App Development", 0 },
                    { 3, "Data Science", 0 },
                    { 4, "Graphic Design", 0 }
                });

            migrationBuilder.InsertData(
                table: "Students",
                columns: new[] { "StudentId", "StudentEmail", "StudentName" },
                values: new object[,]
                {
                    { 1, "www.wadoodnawaz@gmail.com", "Jane" },
                    { 2, "www.wadoodnawaz@gmail.com", "Jane" },
                    { 3, "www.wadoodnawaz@gmail.com", "Jane" },
                    { 4, "www.wadoodnawaz@gmail.com", "Jane" }
                });

            migrationBuilder.InsertData(
                table: "Batches",
                columns: new[] { "BatchId", "BatchName", "CourseId", "Slots", "time" },
                values: new object[,]
                {
                    { 1, "WD-2023-01", 1, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "WD-2023-02", 1, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "MAD-2023-01", 2, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "DS-2023-01", 3, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "GD-2023-01", 4, 0, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "StudentBatches",
                columns: new[] { "BatchId", "StudentId", "EnrollmentId", "dateTime" },
                values: new object[,]
                {
                    { 1, 1, 0, new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 1, 0, new DateTime(2023, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 1, 2, 0, new DateTime(2023, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, 0, new DateTime(2023, 1, 3, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 4, 0, new DateTime(2023, 1, 4, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CourseId",
                table: "Batches",
                column: "CourseId");

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
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
