using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class JobChatAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatEntityJobEntity",
                columns: table => new
                {
                    ChatsId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatEntityJobEntity", x => new { x.ChatsId, x.JobsId });
                    table.ForeignKey(
                        name: "FK_ChatEntityJobEntity_Chats_ChatsId",
                        column: x => x.ChatsId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatEntityJobEntity_Jobs_JobsId",
                        column: x => x.JobsId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatEntityJobEntity_JobsId",
                table: "ChatEntityJobEntity",
                column: "JobsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatEntityJobEntity");
        }
    }
}
