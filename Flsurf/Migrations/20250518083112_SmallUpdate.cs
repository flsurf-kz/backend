using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class SmallUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ChangeNotes",
                table: "News",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChangeNotes",
                table: "News");
        }
    }
}
