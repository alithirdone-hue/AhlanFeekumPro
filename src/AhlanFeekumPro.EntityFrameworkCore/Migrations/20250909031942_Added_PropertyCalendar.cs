using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Added_PropertyCalendar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPropertyCalendars",
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
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SitePropertyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPropertyCalendars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPropertyCalendars_AppSiteProperties_SitePropertyId",
                        column: x => x.SitePropertyId,
                        principalTable: "AppSiteProperties",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPropertyCalendars_SitePropertyId",
                table: "AppPropertyCalendars",
                column: "SitePropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPropertyCalendars");
        }
    }
}
