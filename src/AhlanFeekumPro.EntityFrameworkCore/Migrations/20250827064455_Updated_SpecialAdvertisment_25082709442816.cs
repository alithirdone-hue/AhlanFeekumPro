using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Updated_SpecialAdvertisment_25082709442816 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppSpecialAdvertisments_AppUserProfiles_UserProfileId",
                table: "AppSpecialAdvertisments");

            migrationBuilder.DropIndex(
                name: "IX_AppSpecialAdvertisments_UserProfileId",
                table: "AppSpecialAdvertisments");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "AppSpecialAdvertisments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "AppSpecialAdvertisments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AppSpecialAdvertisments_UserProfileId",
                table: "AppSpecialAdvertisments",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_AppSpecialAdvertisments_AppUserProfiles_UserProfileId",
                table: "AppSpecialAdvertisments",
                column: "UserProfileId",
                principalTable: "AppUserProfiles",
                principalColumn: "Id");
        }
    }
}
