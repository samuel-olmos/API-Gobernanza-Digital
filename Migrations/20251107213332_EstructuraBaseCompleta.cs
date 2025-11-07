using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api_Gobernanza_Digital.Migrations
{
    /// <inheritdoc />
    public partial class EstructuraBaseCompleta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boletas_Contribuyentes_ContribuyenteId",
                table: "Boletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Boletas_Servicios_ServicioId",
                table: "Boletas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContribuyenteServicios",
                table: "ContribuyenteServicios");

            migrationBuilder.DropColumn(
                name: "FechaAlta",
                table: "ContribuyenteServicios");

            migrationBuilder.DropColumn(
                name: "FechaVencimiento",
                table: "Boletas");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Boletas");

            migrationBuilder.RenameColumn(
                name: "Frecuencia",
                table: "Servicios",
                newName: "FrecuenciaId");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Contribuyentes",
                newName: "TipoId");

            migrationBuilder.RenameColumn(
                name: "ServicioId",
                table: "Boletas",
                newName: "PeriodoId");

            migrationBuilder.RenameColumn(
                name: "Estado",
                table: "Boletas",
                newName: "EstadoId");

            migrationBuilder.RenameColumn(
                name: "ContribuyenteId",
                table: "Boletas",
                newName: "ContribuyenteServicioId");

            migrationBuilder.RenameIndex(
                name: "IX_Boletas_ServicioId",
                table: "Boletas",
                newName: "IX_Boletas_PeriodoId");

            migrationBuilder.RenameIndex(
                name: "IX_Boletas_ContribuyenteId",
                table: "Boletas",
                newName: "IX_Boletas_ContribuyenteServicioId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ContribuyenteServicios",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "ContribuyenteServicios",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaInicio",
                table: "ContribuyenteServicios",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContribuyenteServicios",
                table: "ContribuyenteServicios",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Estados",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estados", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Frecuencias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MesesIntervalo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frecuencias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Periodos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodoFiscal = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    FechaVencimiento = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Periodos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TiposContribuyente",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TiposContribuyente", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_FrecuenciaId",
                table: "Servicios",
                column: "FrecuenciaId");

            migrationBuilder.CreateIndex(
                name: "IX_ContribuyenteServicios_ContribuyenteId_ServicioId",
                table: "ContribuyenteServicios",
                columns: new[] { "ContribuyenteId", "ServicioId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contribuyentes_TipoId",
                table: "Contribuyentes",
                column: "TipoId");

            migrationBuilder.CreateIndex(
                name: "IX_Boletas_EstadoId",
                table: "Boletas",
                column: "EstadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Periodos_PeriodoFiscal",
                table: "Periodos",
                column: "PeriodoFiscal",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Boletas_ContribuyenteServicios_ContribuyenteServicioId",
                table: "Boletas",
                column: "ContribuyenteServicioId",
                principalTable: "ContribuyenteServicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boletas_Estados_EstadoId",
                table: "Boletas",
                column: "EstadoId",
                principalTable: "Estados",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boletas_Periodos_PeriodoId",
                table: "Boletas",
                column: "PeriodoId",
                principalTable: "Periodos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Contribuyentes_TiposContribuyente_TipoId",
                table: "Contribuyentes",
                column: "TipoId",
                principalTable: "TiposContribuyente",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Servicios_Frecuencias_FrecuenciaId",
                table: "Servicios",
                column: "FrecuenciaId",
                principalTable: "Frecuencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Boletas_ContribuyenteServicios_ContribuyenteServicioId",
                table: "Boletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Boletas_Estados_EstadoId",
                table: "Boletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Boletas_Periodos_PeriodoId",
                table: "Boletas");

            migrationBuilder.DropForeignKey(
                name: "FK_Contribuyentes_TiposContribuyente_TipoId",
                table: "Contribuyentes");

            migrationBuilder.DropForeignKey(
                name: "FK_Servicios_Frecuencias_FrecuenciaId",
                table: "Servicios");

            migrationBuilder.DropTable(
                name: "Estados");

            migrationBuilder.DropTable(
                name: "Frecuencias");

            migrationBuilder.DropTable(
                name: "Periodos");

            migrationBuilder.DropTable(
                name: "TiposContribuyente");

            migrationBuilder.DropIndex(
                name: "IX_Servicios_FrecuenciaId",
                table: "Servicios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContribuyenteServicios",
                table: "ContribuyenteServicios");

            migrationBuilder.DropIndex(
                name: "IX_ContribuyenteServicios_ContribuyenteId_ServicioId",
                table: "ContribuyenteServicios");

            migrationBuilder.DropIndex(
                name: "IX_Contribuyentes_TipoId",
                table: "Contribuyentes");

            migrationBuilder.DropIndex(
                name: "IX_Boletas_EstadoId",
                table: "Boletas");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ContribuyenteServicios");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "ContribuyenteServicios");

            migrationBuilder.DropColumn(
                name: "FechaInicio",
                table: "ContribuyenteServicios");

            migrationBuilder.RenameColumn(
                name: "FrecuenciaId",
                table: "Servicios",
                newName: "Frecuencia");

            migrationBuilder.RenameColumn(
                name: "TipoId",
                table: "Contribuyentes",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "PeriodoId",
                table: "Boletas",
                newName: "ServicioId");

            migrationBuilder.RenameColumn(
                name: "EstadoId",
                table: "Boletas",
                newName: "Estado");

            migrationBuilder.RenameColumn(
                name: "ContribuyenteServicioId",
                table: "Boletas",
                newName: "ContribuyenteId");

            migrationBuilder.RenameIndex(
                name: "IX_Boletas_PeriodoId",
                table: "Boletas",
                newName: "IX_Boletas_ServicioId");

            migrationBuilder.RenameIndex(
                name: "IX_Boletas_ContribuyenteServicioId",
                table: "Boletas",
                newName: "IX_Boletas_ContribuyenteId");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaAlta",
                table: "ContribuyenteServicios",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaVencimiento",
                table: "Boletas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "Periodo",
                table: "Boletas",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContribuyenteServicios",
                table: "ContribuyenteServicios",
                columns: new[] { "ContribuyenteId", "ServicioId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Boletas_Contribuyentes_ContribuyenteId",
                table: "Boletas",
                column: "ContribuyenteId",
                principalTable: "Contribuyentes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Boletas_Servicios_ServicioId",
                table: "Boletas",
                column: "ServicioId",
                principalTable: "Servicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
