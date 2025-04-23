using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LearnifyD1.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_Batches_CourseId",
                table: "Batches",
                column: "CourseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Batches");

            migrationBuilder.DropTable(
                name: "Courses");
        }
    }
}
