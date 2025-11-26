using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BilleteraCriptoProg3.Migrations
{
    /// <inheritdoc />
    public partial class ActualizarTransaccionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Clientes_idCliente",
                table: "Transacciones");

            migrationBuilder.RenameColumn(
                name: "idCliente",
                table: "Transacciones",
                newName: "ClienteId");

            migrationBuilder.RenameIndex(
                name: "IX_Transacciones_idCliente",
                table: "Transacciones",
                newName: "IX_Transacciones_ClienteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Clientes_ClienteId",
                table: "Transacciones",
                column: "ClienteId",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transacciones_Clientes_ClienteId",
                table: "Transacciones");

            migrationBuilder.RenameColumn(
                name: "ClienteId",
                table: "Transacciones",
                newName: "idCliente");

            migrationBuilder.RenameIndex(
                name: "IX_Transacciones_ClienteId",
                table: "Transacciones",
                newName: "IX_Transacciones_idCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Transacciones_Clientes_idCliente",
                table: "Transacciones",
                column: "idCliente",
                principalTable: "Clientes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
