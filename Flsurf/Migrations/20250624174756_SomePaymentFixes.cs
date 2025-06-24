using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class SomePaymentFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Props_ClientSecret",
                table: "Transactions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Props_ClientSecret",
                table: "Transactions");
        }
    }
}
