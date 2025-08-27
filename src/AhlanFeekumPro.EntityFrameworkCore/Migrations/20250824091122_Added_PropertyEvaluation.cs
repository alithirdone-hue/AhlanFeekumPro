using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Added_PropertyEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPropertyEvaluations",
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
                    Cleanliness = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    PriceAndValue = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    Location = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    Accuracy = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    Attitude = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    RatingComment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SitePropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPropertyEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPropertyEvaluations_AppSiteProperties_SitePropertyId",
                        column: x => x.SitePropertyId,
                        principalTable: "AppSiteProperties",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppPropertyEvaluations_AppUserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPropertyEvaluations_SitePropertyId",
                table: "AppPropertyEvaluations",
                column: "SitePropertyId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPropertyEvaluations_UserProfileId",
                table: "AppPropertyEvaluations",
                column: "UserProfileId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPropertyEvaluations");
        }
    }
}
