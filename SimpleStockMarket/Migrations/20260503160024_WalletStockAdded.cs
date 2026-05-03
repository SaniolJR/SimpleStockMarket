using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleStockMarket.Migrations
{
    /// <inheritdoc />
    public partial class WalletStockAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletStock_Stocks_StockId",
                table: "WalletStock");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletStock_Wallets_WalletId",
                table: "WalletStock");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletStock",
                table: "WalletStock");

            migrationBuilder.RenameTable(
                name: "WalletStock",
                newName: "WalletStocks");

            migrationBuilder.RenameIndex(
                name: "IX_WalletStock_WalletId",
                table: "WalletStocks",
                newName: "IX_WalletStocks_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletStock_StockId",
                table: "WalletStocks",
                newName: "IX_WalletStocks_StockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletStocks",
                table: "WalletStocks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletStocks_Stocks_StockId",
                table: "WalletStocks",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletStocks_Wallets_WalletId",
                table: "WalletStocks",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletStocks_Stocks_StockId",
                table: "WalletStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_WalletStocks_Wallets_WalletId",
                table: "WalletStocks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WalletStocks",
                table: "WalletStocks");

            migrationBuilder.RenameTable(
                name: "WalletStocks",
                newName: "WalletStock");

            migrationBuilder.RenameIndex(
                name: "IX_WalletStocks_WalletId",
                table: "WalletStock",
                newName: "IX_WalletStock_WalletId");

            migrationBuilder.RenameIndex(
                name: "IX_WalletStocks_StockId",
                table: "WalletStock",
                newName: "IX_WalletStock_StockId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WalletStock",
                table: "WalletStock",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WalletStock_Stocks_StockId",
                table: "WalletStock",
                column: "StockId",
                principalTable: "Stocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WalletStock_Wallets_WalletId",
                table: "WalletStock",
                column: "WalletId",
                principalTable: "Wallets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
