using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendances.Database.Sync.Migrations
{
    /// <inheritdoc />
    public partial class Initialization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "LessonSyncItem",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    Action = table.Column<string>(type: "text", nullable: false),
                    QueuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    ExternalId = table.Column<long>(type: "bigint", nullable: true),
                    EntityVersion = table.Column<long>(type: "bigint", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: true),
                    LessonInfo = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonSyncItem", x => x.Uuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LessonSyncItem_Uuid",
                schema: "public",
                table: "LessonSyncItem",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LessonSyncItem",
                schema: "public");
        }
    }
}
