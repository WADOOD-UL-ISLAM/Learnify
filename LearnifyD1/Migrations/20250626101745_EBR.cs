using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnifyD1.Migrations
{
    /// <inheritdoc />
    public partial class EBR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InstructorId",
                table: "Batches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Batches_InstructorId",
                table: "Batches",
                column: "InstructorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Employees_InstructorId",
                table: "Batches",
                column: "InstructorId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Employees_InstructorId",
                table: "Batches");

            migrationBuilder.DropIndex(
                name: "IX_Batches_InstructorId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "InstructorId",
                table: "Batches");
        }
    }
}
