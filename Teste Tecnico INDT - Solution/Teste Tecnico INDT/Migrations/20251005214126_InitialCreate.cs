using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Teste_Tecnico_INDT.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contratacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    PropostaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    DataContratacaoUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Propostas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NomeCliente = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Produto = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CriadaEmUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Propostas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contratacoes");

            migrationBuilder.DropTable(
                name: "Propostas");
        }
    }
}
