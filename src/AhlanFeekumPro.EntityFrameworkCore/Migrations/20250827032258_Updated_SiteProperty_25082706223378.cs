using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Updated_SiteProperty_25082706223378 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "GovernorateId",
                table: "AppSiteProperties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "HotelName",
                table: "AppSiteProperties",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppSiteProperties_GovernorateId",
                table: "AppSiteProperties",
                column: "GovernorateId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSiteProperties_AppGovernorates_GovernorateId",
                table: "AppSiteProperties",
                column: "GovernorateId",
                principalTable: "AppGovernorates",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSiteProperties_AppGovernorates_GovernorateId",
                table: "AppSiteProperties");

            migrationBuilder.DropIndex(
                name: "IX_AppSiteProperties_GovernorateId",
                table: "AppSiteProperties");

            migrationBuilder.DropColumn(
                name: "GovernorateId",
                table: "AppSiteProperties");

            migrationBuilder.DropColumn(
                name: "HotelName",
                table: "AppSiteProperties");
        }
    }
}
