using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Updated_OnlyForYouSection_25091406240182 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPhoto",
                table: "AppOnlyForYouSections");

            migrationBuilder.DropColumn(
                name: "SecondPhoto",
                table: "AppOnlyForYouSections");

            migrationBuilder.DropColumn(
                name: "ThirdPhoto",
                table: "AppOnlyForYouSections");

            migrationBuilder.AddColumn<Guid>(
                name: "FirstPhotoId",
                table: "AppOnlyForYouSections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SecondPhotoId",
                table: "AppOnlyForYouSections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ThirdPhotoId",
                table: "AppOnlyForYouSections",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPhotoId",
                table: "AppOnlyForYouSections");

            migrationBuilder.DropColumn(
                name: "SecondPhotoId",
                table: "AppOnlyForYouSections");

            migrationBuilder.DropColumn(
                name: "ThirdPhotoId",
                table: "AppOnlyForYouSections");

            migrationBuilder.AddColumn<string>(
                name: "FirstPhoto",
                table: "AppOnlyForYouSections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SecondPhoto",
                table: "AppOnlyForYouSections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ThirdPhoto",
                table: "AppOnlyForYouSections",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
