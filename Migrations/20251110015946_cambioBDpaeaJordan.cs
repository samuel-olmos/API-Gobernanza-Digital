using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Gobernanza_Digital.Migrations
{
    /// <inheritdoc />
    public partial class cambioBDpaeaJordan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Periodos",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<bool>(
                name: "Generadas",
                table: "Periodos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaEmision",
                table: "Boletas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Boletas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Generadas",
                table: "Periodos");

            migrationBuilder.DropColumn(
                name: "FechaEmision",
                table: "Boletas");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Boletas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Periodos",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }
    }
}
