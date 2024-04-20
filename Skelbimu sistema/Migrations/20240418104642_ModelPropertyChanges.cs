using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skelbimu_sistema.Migrations
{
    /// <inheritdoc />
    public partial class ModelPropertyChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suspensions_ProductId",
                table: "Suspensions");

            migrationBuilder.AddColumn<bool>(
                name: "Reviewed",
                table: "Suspensions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Suspensions_ProductId",
                table: "Suspensions",
                column: "ProductId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Suspensions_ProductId",
                table: "Suspensions");

            migrationBuilder.DropColumn(
                name: "Reviewed",
                table: "Suspensions");

            migrationBuilder.CreateIndex(
                name: "IX_Suspensions_ProductId",
                table: "Suspensions",
                column: "ProductId");
        }
    }
}
