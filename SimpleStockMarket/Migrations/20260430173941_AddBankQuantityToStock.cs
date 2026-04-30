using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleStockMarket.Migrations
{
    /// <inheritdoc />
    public partial class AddBankQuantityToStock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankQuantity",
                table: "Stocks",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BankQuantity",
                table: "Stocks");
        }
    }
}
