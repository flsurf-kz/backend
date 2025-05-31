using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations.EventStore
{
    /// <inheritdoc />
    public partial class EventStoreAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorData",
                table: "Events",
                type: "jsonb",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FailedCounter",
                table: "Events",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "ProcessError",
                table: "Events",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorData",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FailedCounter",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ProcessError",
                table: "Events");
        }
    }
}
