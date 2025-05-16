using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class notificationmessangingupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_DailySummaryEmailEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_DesktopBadgeCountEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_DesktopNotificationsEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "NotificationSettings_DoNotDisturbEnd",
                table: "Users",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "NotificationSettings_DoNotDisturbStart",
                table: "Users",
                type: "interval",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_EmailNotificationsEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_EmailWhenOfflineEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "NotificationSettings_PreferredLanguage",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_PushNotificationsEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_PushWhenOfflineEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_WebBadgeCountEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "NotificationSettings_WebNotificationsEnabled",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "NewsEntityId",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    IsHidden = table.Column<bool>(type: "boolean", nullable: false),
                    PublishTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedById = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedById = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_NewsEntityId",
                table: "Files",
                column: "NewsEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_News_AuthorId",
                table: "News",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_News_NewsEntityId",
                table: "Files",
                column: "NewsEntityId",
                principalTable: "News",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_News_NewsEntityId",
                table: "Files");

            migrationBuilder.DropTable(
                name: "News");

            migrationBuilder.DropIndex(
                name: "IX_Files_NewsEntityId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_DailySummaryEmailEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_DesktopBadgeCountEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_DesktopNotificationsEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_DoNotDisturbEnd",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_DoNotDisturbStart",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_EmailNotificationsEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_EmailWhenOfflineEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_PreferredLanguage",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_PushNotificationsEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_PushWhenOfflineEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_WebBadgeCountEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NotificationSettings_WebNotificationsEnabled",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "NewsEntityId",
                table: "Files");
        }
    }
}
