using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Added_FavoriteProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFavoriteProperties",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SitePropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFavoriteProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppFavoriteProperties_AppSiteProperties_SitePropertyId",
                        column: x => x.SitePropertyId,
                        principalTable: "AppSiteProperties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppFavoriteProperties_AppUserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFavoriteProperties_SitePropertyId",
                table: "AppFavoriteProperties",
                column: "SitePropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppFavoriteProperties_UserProfileId",
                table: "AppFavoriteProperties",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFavoriteProperties");
        }
    }
}
