using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyApi.Migrations
{
    public partial class UpdateIndicators2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ResultVal",
                table: "Indicators",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResultStr",
                table: "Indicators",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Indicators",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "DynamicValues",
                table: "Indicators",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<int>(
                name: "GroupOrderNumber",
                table: "Indicators",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "MaxStandardValue",
                table: "Indicators",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MinStandardValue",
                table: "Indicators",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<List<string>>(
                name: "PossibleStringValues",
                table: "Indicators",
                type: "text[]",
                nullable: false);

            migrationBuilder.AddColumn<List<string>>(
                name: "TextStandards",
                table: "Indicators",
                type: "text[]",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "IndicatorGroup",
                columns: table => new
                {
                    Key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndicatorGroup");

            migrationBuilder.DropColumn(
                name: "DynamicValues",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "GroupOrderNumber",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "MaxStandardValue",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "MinStandardValue",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "PossibleStringValues",
                table: "Indicators");

            migrationBuilder.DropColumn(
                name: "TextStandards",
                table: "Indicators");

            migrationBuilder.AlterColumn<float>(
                name: "ResultVal",
                table: "Indicators",
                type: "real",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ResultStr",
                table: "Indicators",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Indicators",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
