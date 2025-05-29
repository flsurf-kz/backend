using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class SomeUpdateWork : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ContractEntityId",
                table: "Files",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ContractEntityId",
                table: "Files",
                column: "ContractEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Contracts_ContractEntityId",
                table: "Files",
                column: "ContractEntityId",
                principalTable: "Contracts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Contracts_ContractEntityId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ContractEntityId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ContractEntityId",
                table: "Files");
        }
    }
}
