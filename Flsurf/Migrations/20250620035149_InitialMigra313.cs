using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigra313 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RawAmount_Currency",
                table: "Transactions",
                newName: "RawSum_Currency");

            migrationBuilder.RenameColumn(
                name: "RawAmount_Amount",
                table: "Transactions",
                newName: "RawSum_Amount");

            migrationBuilder.RenameColumn(
                name: "NetAmount_Currency",
                table: "Transactions",
                newName: "NetSum_Currency");

            migrationBuilder.RenameColumn(
                name: "NetAmount_Amount",
                table: "Transactions",
                newName: "NetSum_Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RawSum_Currency",
                table: "Transactions",
                newName: "RawAmount_Currency");

            migrationBuilder.RenameColumn(
                name: "RawSum_Amount",
                table: "Transactions",
                newName: "RawAmount_Amount");

            migrationBuilder.RenameColumn(
                name: "NetSum_Currency",
                table: "Transactions",
                newName: "NetAmount_Currency");

            migrationBuilder.RenameColumn(
                name: "NetSum_Amount",
                table: "Transactions",
                newName: "NetAmount_Amount");
        }
    }
}
