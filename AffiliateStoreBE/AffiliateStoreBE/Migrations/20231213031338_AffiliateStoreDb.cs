using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AffiliateStoreBE.Migrations
{
    /// <inheritdoc />
    public partial class AffiliateStoreDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cartproduct-data_cart-data_CartId",
                table: "cartproduct-data");

            migrationBuilder.DropTable(
                name: "cart-data");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cartproduct-data",
                table: "cartproduct-data");

            migrationBuilder.DropIndex(
                name: "IX_cartproduct-data_CartId",
                table: "cartproduct-data");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d92d2ed5-636a-4836-adcb-544d91927e9d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f2ea9ec8-ea54-4e5f-ae04-45048f4180b2");

            migrationBuilder.DropColumn(
                name: "CartProductId",
                table: "cartproduct-data");

            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "cartproduct-data",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "cartproduct-data",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "cartproduct-data",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cartproduct-data",
                table: "cartproduct-data",
                column: "Id");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "10aa317d-d8bb-4c64-bd57-e0a9a9756359", "1", "Admin", "Admin" },
                    { "1a21fe61-7184-4e7d-8903-d8b700fd5093", "2", "User", "User" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartproduct-data_AccountId",
                table: "cartproduct-data",
                column: "AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_cartproduct-data_account-data_AccountId",
                table: "cartproduct-data",
                column: "AccountId",
                principalTable: "account-data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cartproduct-data_account-data_AccountId",
                table: "cartproduct-data");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cartproduct-data",
                table: "cartproduct-data");

            migrationBuilder.DropIndex(
                name: "IX_cartproduct-data_AccountId",
                table: "cartproduct-data");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "10aa317d-d8bb-4c64-bd57-e0a9a9756359");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a21fe61-7184-4e7d-8903-d8b700fd5093");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "cartproduct-data");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "cartproduct-data");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cartproduct-data",
                newName: "CartId");

            migrationBuilder.AddColumn<Guid>(
                name: "CartProductId",
                table: "cartproduct-data",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_cartproduct-data",
                table: "cartproduct-data",
                column: "CartProductId");

            migrationBuilder.CreateTable(
                name: "cart-data",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cart-data", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cart-data_account-data_AccountId",
                        column: x => x.AccountId,
                        principalTable: "account-data",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "d92d2ed5-636a-4836-adcb-544d91927e9d", "2", "User", "User" },
                    { "f2ea9ec8-ea54-4e5f-ae04-45048f4180b2", "1", "Admin", "Admin" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_cartproduct-data_CartId",
                table: "cartproduct-data",
                column: "CartId");

            migrationBuilder.CreateIndex(
                name: "IX_cart-data_AccountId",
                table: "cart-data",
                column: "AccountId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_cartproduct-data_cart-data_CartId",
                table: "cartproduct-data",
                column: "CartId",
                principalTable: "cart-data",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
