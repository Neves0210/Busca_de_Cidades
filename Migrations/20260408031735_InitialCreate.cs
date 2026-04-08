using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaCidadesRaio.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cidades",
                columns: table => new
                {
                    CidadeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CodigoIbge = table.Column<long>(type: "bigint", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    UF = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                    Ddd = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cidades", x => x.CidadeId);
                });

            migrationBuilder.CreateTable(
                name: "CidadesEnviadas",
                columns: table => new
                {
                    CidadeEnviadaId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CidadeId = table.Column<int>(type: "integer", nullable: false),
                    NomeDocumento = table.Column<string>(type: "text", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CidadesEnviadas", x => x.CidadeEnviadaId);
                });

            migrationBuilder.CreateTable(
                name: "DocumentosGerados",
                columns: table => new
                {
                    DocumentoGeradoId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeArquivo = table.Column<string>(type: "text", nullable: false),
                    DataGeracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    QuantidadeCidades = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosGerados", x => x.DocumentoGeradoId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cidades");

            migrationBuilder.DropTable(
                name: "CidadesEnviadas");

            migrationBuilder.DropTable(
                name: "DocumentosGerados");
        }
    }
}
