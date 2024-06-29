using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CandyKeeper.DAL.Migrations
{
    /// <inheritdoc />
    public partial class migr11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Packaging_PackagingId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_PackagingId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PackagingId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "DistrictId",
                table: "Stores",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PackagingId",
                table: "ProductForSales",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "ProductDeliveries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Stores_DistrictId",
                table: "Stores",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductForSales_PackagingId",
                table: "ProductForSales",
                column: "PackagingId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductForSales_Packaging_PackagingId",
                table: "ProductForSales",
                column: "PackagingId",
                principalTable: "Packaging",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stores_Districts_DistrictId",
                table: "Stores",
                column: "DistrictId",
                principalTable: "Districts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductForSales_Packaging_PackagingId",
                table: "ProductForSales");

            migrationBuilder.DropForeignKey(
                name: "FK_Stores_Districts_DistrictId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Stores_DistrictId",
                table: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_ProductForSales_PackagingId",
                table: "ProductForSales");

            migrationBuilder.DropColumn(
                name: "DistrictId",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "PackagingId",
                table: "ProductForSales");

            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "ProductDeliveries");

            migrationBuilder.AddColumn<int>(
                name: "PackagingId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_PackagingId",
                table: "Products",
                column: "PackagingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Packaging_PackagingId",
                table: "Products",
                column: "PackagingId",
                principalTable: "Packaging",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
