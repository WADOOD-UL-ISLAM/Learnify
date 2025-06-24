using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnifyD1.Migrations
{
    /// <inheritdoc />
    public partial class ImageWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Students",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1,
                columns: new[] { "ImagePath", "StudentEmail" },
                values: new object[] { "/images/students/student1.jpg", "wadoodnawaz@gmail.com" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2,
                columns: new[] { "ImagePath", "StudentEmail", "StudentName" },
                values: new object[] { "/images/students/student2.jpg", "john.doe@gmail.com", "John" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 3,
                columns: new[] { "ImagePath", "StudentEmail", "StudentName" },
                values: new object[] { "/images/students/student3.jpg", "alice.smith@gmail.com", "Alice" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 4,
                columns: new[] { "ImagePath", "StudentEmail", "StudentName" },
                values: new object[] { "/images/students/student4.jpg", "bob.brown@gmail.com", "Bob" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Students");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 1,
                column: "StudentEmail",
                value: "www.wadoodnawaz@gmail.com");

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 2,
                columns: new[] { "StudentEmail", "StudentName" },
                values: new object[] { "www.wadoodnawaz@gmail.com", "Jane" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 3,
                columns: new[] { "StudentEmail", "StudentName" },
                values: new object[] { "www.wadoodnawaz@gmail.com", "Jane" });

            migrationBuilder.UpdateData(
                table: "Students",
                keyColumn: "StudentId",
                keyValue: 4,
                columns: new[] { "StudentEmail", "StudentName" },
                values: new object[] { "www.wadoodnawaz@gmail.com", "Jane" });
        }
    }
}
