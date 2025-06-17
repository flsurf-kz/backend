using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class SecretShit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecurityPhraseTypes",
                table: "Users",
                newName: "SecurityPhraseType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecurityPhraseType",
                table: "Users",
                newName: "SecurityPhraseTypes");
        }
    }
}
