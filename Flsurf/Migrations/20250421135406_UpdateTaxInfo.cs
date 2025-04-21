using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Flsurf.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTaxInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxInfo_Country",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_LegalName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_TaxId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_VatValidTo",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_BankDetails_AccountNumber",
                table: "Users",
                type: "character varying(34)",
                maxLength: 34,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_BankDetails_BankName",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_BankDetails_Bic",
                table: "Users",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_CountryIso",
                table: "Users",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxInfo_LegalStatus",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_LocalIdNumber",
                table: "Users",
                type: "character varying(12)",
                maxLength: 12,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TaxInfo_TaxRegime",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TaxInfo_VatRegistered",
                table: "Users",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxInfo_BankDetails_AccountNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_BankDetails_BankName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_BankDetails_Bic",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_CountryIso",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_LegalStatus",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_LocalIdNumber",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_TaxRegime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TaxInfo_VatRegistered",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_Country",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_LegalName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxInfo_TaxId",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "TaxInfo_VatValidTo",
                table: "Users",
                type: "date",
                nullable: true);
        }
    }
}
