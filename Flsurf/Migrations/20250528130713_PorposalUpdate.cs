using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class PorposalUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProposedRate",
                table: "Proposals",
                newName: "ProposedRate_Amount");

            migrationBuilder.AddColumn<string>(
                name: "BudgetType",
                table: "Proposals",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "EsitimatedDurationDays",
                table: "Proposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProposedRate_Currency",
                table: "Proposals",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SimilarExpriences",
                table: "Proposals",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProposalEntityId",
                table: "PortfolioProjects",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PortfolioProjects_ProposalEntityId",
                table: "PortfolioProjects",
                column: "ProposalEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_PortfolioProjects_Proposals_ProposalEntityId",
                table: "PortfolioProjects",
                column: "ProposalEntityId",
                principalTable: "Proposals",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PortfolioProjects_Proposals_ProposalEntityId",
                table: "PortfolioProjects");

            migrationBuilder.DropIndex(
                name: "IX_PortfolioProjects_ProposalEntityId",
                table: "PortfolioProjects");

            migrationBuilder.DropColumn(
                name: "BudgetType",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "EsitimatedDurationDays",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ProposedRate_Currency",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "SimilarExpriences",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "ProposalEntityId",
                table: "PortfolioProjects");

            migrationBuilder.RenameColumn(
                name: "ProposedRate_Amount",
                table: "Proposals",
                newName: "ProposedRate");
        }
    }
}
