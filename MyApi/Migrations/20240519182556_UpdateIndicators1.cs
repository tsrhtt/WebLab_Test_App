using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MyApi.Migrations
{
    public partial class UpdateIndicators1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Indicators",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "ReferenceRange",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Indicators");

            migrationBuilder.RenameColumn(
                name: "Value",
                table: "Indicators",
                newName: "ResultStr");

            migrationBuilder.RenameColumn(
                name: "IndicatorId",
                table: "Indicators",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Indicators",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Indicators",
                type: "integer",
                nullable: false,
                defaultValue: 0)
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Indicators",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Group",
                table: "Indicators",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdditional",
                table: "Indicators",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsInReference",
                table: "Indicators",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsNormExist",
                table: "Indicators",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<float>(
                name: "ResultVal",
                table: "Indicators",
                type: "real",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortNumber",
                table: "Indicators",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Units",
                table: "Indicators",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Indicators",
                table: "Indicators",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Indicators",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Group",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "IsAdditional",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "IsInReference",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "IsNormExist",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "ResultVal",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "SortNumber",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "Units",
                table: "Indicators");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Indicators",
                newName: "IndicatorId");

            migrationBuilder.RenameColumn(
                name: "ResultStr",
                table: "Indicators",
                newName: "Value");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Indicators",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ReferenceRange",
                table: "Indicators",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Indicators",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Indicators",
                table: "Indicators",
                columns: new[] { "IndicatorId", "DirectionId" });
        }
    }
}
