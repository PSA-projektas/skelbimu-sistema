using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skelbimu_sistema.Migrations
{
    /// <inheritdoc />
    public partial class AddSuspensions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Reports",
                newName: "Reason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Reason",
                table: "Reports",
                newName: "Description");
        }
    }
}
