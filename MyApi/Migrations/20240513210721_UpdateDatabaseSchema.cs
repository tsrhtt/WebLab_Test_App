using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApi.Migrations
{
    public partial class UpdateDatabaseSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directions_AnalysisTypes_AnalysisTypeId",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Departments_DepartmentId",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Laboratories_LaboratoryId",
                table: "Directions");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Directions",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_AnalysisTypes_AnalysisTypeId",
                table: "Directions",
                column: "AnalysisTypeId",
                principalTable: "AnalysisTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Departments_DepartmentId",
                table: "Directions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Laboratories_LaboratoryId",
                table: "Directions",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directions_AnalysisTypes_AnalysisTypeId",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Departments_DepartmentId",
                table: "Directions");

            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Laboratories_LaboratoryId",
                table: "Directions");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Directions",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_AnalysisTypes_AnalysisTypeId",
                table: "Directions",
                column: "AnalysisTypeId",
                principalTable: "AnalysisTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Departments_DepartmentId",
                table: "Directions",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Laboratories_LaboratoryId",
                table: "Directions",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
