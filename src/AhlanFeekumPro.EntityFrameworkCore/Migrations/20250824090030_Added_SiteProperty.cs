using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Added_SiteProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSiteProperties",
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
                    PropertyTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bedrooms = table.Column<int>(type: "int", nullable: false),
                    Bathrooms = table.Column<int>(type: "int", nullable: false),
                    NumberOfBed = table.Column<int>(type: "int", nullable: false),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    MaximumNumberOfGuest = table.Column<int>(type: "int", nullable: false),
                    Livingrooms = table.Column<int>(type: "int", nullable: false),
                    PropertyDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HourseRules = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImportantInformation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StreetAndBuildingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LandMark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricePerNight = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    PropertyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSiteProperties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppSiteProperties_AppPropertyTypes_PropertyTypeId",
                        column: x => x.PropertyTypeId,
                        principalTable: "AppPropertyTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AppSitePropertyPropertyFeature",
                columns: table => new
                {
                    SitePropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PropertyFeatureId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSitePropertyPropertyFeature", x => new { x.SitePropertyId, x.PropertyFeatureId });
                    table.ForeignKey(
                        name: "FK_AppSitePropertyPropertyFeature_AppPropertyFeatures_PropertyFeatureId",
                        column: x => x.PropertyFeatureId,
                        principalTable: "AppPropertyFeatures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppSitePropertyPropertyFeature_AppSiteProperties_SitePropertyId",
                        column: x => x.SitePropertyId,
                        principalTable: "AppSiteProperties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppSiteProperties_PropertyTypeId",
                table: "AppSiteProperties",
                column: "PropertyTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSitePropertyPropertyFeature_PropertyFeatureId",
                table: "AppSitePropertyPropertyFeature",
                column: "PropertyFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_AppSitePropertyPropertyFeature_SitePropertyId_PropertyFeatureId",
                table: "AppSitePropertyPropertyFeature",
                columns: new[] { "SitePropertyId", "PropertyFeatureId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppSitePropertyPropertyFeature");

            migrationBuilder.DropTable(
                name: "AppSiteProperties");
        }
    }
}
