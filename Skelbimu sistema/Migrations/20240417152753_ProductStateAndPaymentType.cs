using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skelbimu_sistema.Migrations
{
    /// <inheritdoc />
    public partial class ProductStateAndPaymentType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Products");
        }
    }
}
