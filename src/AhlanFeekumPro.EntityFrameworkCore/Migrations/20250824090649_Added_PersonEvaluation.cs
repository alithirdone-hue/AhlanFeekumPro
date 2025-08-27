using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AhlanFeekumPro.Migrations
{
    /// <inheritdoc />
    public partial class Added_PersonEvaluation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppPersonEvaluations",
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
                    Rate = table.Column<int>(type: "int", maxLength: 10, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EvaluatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EvaluatedPersonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppPersonEvaluations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppPersonEvaluations_AppUserProfiles_EvaluatedPersonId",
                        column: x => x.EvaluatedPersonId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppPersonEvaluations_AppUserProfiles_EvaluatorId",
                        column: x => x.EvaluatorId,
                        principalTable: "AppUserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonEvaluations_EvaluatedPersonId",
                table: "AppPersonEvaluations",
                column: "EvaluatedPersonId");

            migrationBuilder.CreateIndex(
                name: "IX_AppPersonEvaluations_EvaluatorId",
                table: "AppPersonEvaluations",
                column: "EvaluatorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppPersonEvaluations");
        }
    }
}
