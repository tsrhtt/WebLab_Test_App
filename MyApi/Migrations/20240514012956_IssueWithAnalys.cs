using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    public partial class IssueWithAnalys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directions_AnalysisTypes_AnalysisTypeId",
                table: "Directions");

            migrationBuilder.DropTable(
                name: "AnalysisTypes");

            migrationBuilder.RenameColumn(
                name: "AnalysisTypeName",
                table: "Directions",
                newName: "AnalysTypeName");

            migrationBuilder.RenameColumn(
                name: "AnalysisTypeId",
                table: "Directions",
                newName: "AnalysTypeId");

            migrationBuilder.RenameColumn(
                name: "AnalysisTypeFormat",
                table: "Directions",
                newName: "AnalysTypeFormat");

            migrationBuilder.RenameIndex(
                name: "IX_Directions_AnalysisTypeId",
                table: "Directions",
                newName: "IX_Directions_AnalysTypeId");

            migrationBuilder.CreateTable(
                name: "AnalysTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Format = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_AnalysTypes_AnalysTypeId",
                table: "Directions",
                column: "AnalysTypeId",
                principalTable: "AnalysTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directions_AnalysTypes_AnalysTypeId",
                table: "Directions");

            migrationBuilder.DropTable(
                name: "AnalysTypes");

            migrationBuilder.RenameColumn(
                name: "AnalysTypeName",
                table: "Directions",
                newName: "AnalysisTypeName");

            migrationBuilder.RenameColumn(
                name: "AnalysTypeId",
                table: "Directions",
                newName: "AnalysisTypeId");

            migrationBuilder.RenameColumn(
                name: "AnalysTypeFormat",
                table: "Directions",
                newName: "AnalysisTypeFormat");

            migrationBuilder.RenameIndex(
                name: "IX_Directions_AnalysTypeId",
                table: "Directions",
                newName: "IX_Directions_AnalysisTypeId");

            migrationBuilder.CreateTable(
                name: "AnalysisTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Format = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnalysisTypes", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_AnalysisTypes_AnalysisTypeId",
                table: "Directions",
                column: "AnalysisTypeId",
                principalTable: "AnalysisTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
