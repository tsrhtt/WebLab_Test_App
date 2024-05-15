using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    public partial class IssueWithLab : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directions_Laboratories_LaboratoryId",
                table: "Directions");

            migrationBuilder.DropTable(
                name: "Laboratories");

            migrationBuilder.RenameColumn(
                name: "LaboratoryName",
                table: "Directions",
                newName: "Laboratory");

            migrationBuilder.CreateTable(
                name: "LaboratoryDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LaboratoryDatas", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_LaboratoryDatas_LaboratoryId",
                table: "Directions",
                column: "LaboratoryId",
                principalTable: "LaboratoryDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Directions_LaboratoryDatas_LaboratoryId",
                table: "Directions");

            migrationBuilder.DropTable(
                name: "LaboratoryDatas");

            migrationBuilder.RenameColumn(
                name: "Laboratory",
                table: "Directions",
                newName: "LaboratoryName");

            migrationBuilder.CreateTable(
                name: "Laboratories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Directions_Laboratories_LaboratoryId",
                table: "Directions",
                column: "LaboratoryId",
                principalTable: "Laboratories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
