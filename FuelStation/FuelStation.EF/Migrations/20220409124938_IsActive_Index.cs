using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FuelStation.EF.Migrations
{
    public partial class IsActive_Index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IsActive",
                table: "Transactions",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_TransactionLines_IsActive",
                table: "TransactionLines",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Items_IsActive",
                table: "Items",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_IsActive",
                table: "Employees",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IsActive",
                table: "Customers",
                column: "IsActive");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_IsActive",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_TransactionLines_IsActive",
                table: "TransactionLines");

            migrationBuilder.DropIndex(
                name: "IX_Items_IsActive",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Employees_IsActive",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Customers_IsActive",
                table: "Customers");
        }
    }
}
