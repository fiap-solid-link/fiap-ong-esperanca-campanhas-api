using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Esperanca.Campanha.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InicialCampanha : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campanhas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    DataInicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DataFim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    MetaFinanceira = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ModoEncerramento = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ValorArrecadado = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IdGestor = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campanhas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campanhas_IdGestor",
                table: "campanhas",
                column: "IdGestor");

            migrationBuilder.CreateIndex(
                name: "IX_campanhas_Status",
                table: "campanhas",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campanhas");
        }
    }
}
