using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Perfumes.Api.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fornecedores",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RazaoSocial = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NomeFantasia = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CpfCnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: true),
                    Telefone = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(254)", maxLength: 254, nullable: true),
                    Site = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fornecedores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Insumos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TipoInsumo = table.Column<int>(type: "int", nullable: false),
                    UnidadeMedida = table.Column<int>(type: "int", nullable: false),
                    Densidade = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Insumos", x => x.Id);
                    table.CheckConstraint("CK_Insumo_Densidade", "[Densidade] IS NULL OR [Densidade] > 0");
                    table.CheckConstraint("CK_Insumo_Tipo", "[TipoInsumo] IN (1,2)");
                    table.CheckConstraint("CK_Insumo_Unidade", "[UnidadeMedida] IN (1,2,3)");
                });

            migrationBuilder.CreateTable(
                name: "FornecedorProdutos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornecedorId = table.Column<long>(type: "bigint", nullable: false),
                    InsumoId = table.Column<long>(type: "bigint", nullable: false),
                    CodigoProdutoFornecedor = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    QuantidadeEmbalagem = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnidadeEmbalagem = table.Column<int>(type: "int", nullable: false),
                    QuantidadeBase = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                    UnidadeBase = table.Column<int>(type: "int", nullable: false),
                    Ativo = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataAtualizacao = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorProdutos", x => x.Id);
                    table.CheckConstraint("CK_Oferta_Quantidade", "[QuantidadeEmbalagem] > 0 AND [QuantidadeBase] > 0");
                    table.CheckConstraint("CK_Oferta_Unidades", "[UnidadeEmbalagem] IN (1,2,3,4,5) AND [UnidadeBase] IN (1,2,3)");
                    table.ForeignKey(
                        name: "FK_FornecedorProdutos_Fornecedores_FornecedorId",
                        column: x => x.FornecedorId,
                        principalTable: "Fornecedores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FornecedorProdutos_Insumos_InsumoId",
                        column: x => x.InsumoId,
                        principalTable: "Insumos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FornecedorProdutoPrecos",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FornecedorProdutoId = table.Column<long>(type: "bigint", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DataInicio = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DataFim = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DataCadastro = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FornecedorProdutoPrecos", x => x.Id);
                    table.CheckConstraint("CK_Preco_Valor", "[Preco] > 0");
                    table.CheckConstraint("CK_Preco_Vigencia", "[DataFim] IS NULL OR [DataFim] > [DataInicio]");
                    table.ForeignKey(
                        name: "FK_FornecedorProdutoPrecos_FornecedorProdutos_FornecedorProdutoId",
                        column: x => x.FornecedorProdutoId,
                        principalTable: "FornecedorProdutos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_CpfCnpj",
                table: "Fornecedores",
                column: "CpfCnpj",
                unique: true,
                filter: "[CpfCnpj] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Fornecedores_RazaoSocial",
                table: "Fornecedores",
                column: "RazaoSocial");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorProdutoPrecos_FornecedorProdutoId",
                table: "FornecedorProdutoPrecos",
                column: "FornecedorProdutoId",
                unique: true,
                filter: "[DataFim] IS NULL");

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorProdutoPrecos_FornecedorProdutoId_DataInicio",
                table: "FornecedorProdutoPrecos",
                columns: new[] { "FornecedorProdutoId", "DataInicio" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorProdutos_FornecedorId_InsumoId_QuantidadeBase_UnidadeBase",
                table: "FornecedorProdutos",
                columns: new[] { "FornecedorId", "InsumoId", "QuantidadeBase", "UnidadeBase" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FornecedorProdutos_InsumoId",
                table: "FornecedorProdutos",
                column: "InsumoId");

            migrationBuilder.CreateIndex(
                name: "IX_Insumos_Nome",
                table: "Insumos",
                column: "Nome");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FornecedorProdutoPrecos");

            migrationBuilder.DropTable(
                name: "FornecedorProdutos");

            migrationBuilder.DropTable(
                name: "Fornecedores");

            migrationBuilder.DropTable(
                name: "Insumos");
        }
    }
}
