using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendances.Database.ExternalEvents.Migrations
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
                name: "EventInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TimeStamp = table.Column<long>(type: "bigint", nullable: false),
                    IsHandled = table.Column<bool>(type: "boolean", nullable: false),
                    Payload = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventInfo", x => x.Uuid);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventInfo_Uuid",
                schema: "public",
                table: "EventInfo",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventInfo",
                schema: "public");
        }
    }
}
