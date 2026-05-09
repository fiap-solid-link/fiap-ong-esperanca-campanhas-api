using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Esperanca.Campanha.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ArrecadacoesProcessadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "arrecadacoes_processadas",
                columns: table => new
                {
                    IdDoacao = table.Column<Guid>(type: "uuid", nullable: false),
                    IdCampanha = table.Column<Guid>(type: "uuid", nullable: false),
                    Valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DataProcessamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_arrecadacoes_processadas", x => x.IdDoacao);
                });

            migrationBuilder.CreateIndex(
                name: "IX_arrecadacoes_processadas_IdCampanha",
                table: "arrecadacoes_processadas",
                column: "IdCampanha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "arrecadacoes_processadas");
        }
    }
}
