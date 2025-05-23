using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Attendances.Database.University.Migrations
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
                name: "CourseInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<long>(type: "bigint", nullable: false),
                    ShortName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Format = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AttendanceModules = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseInfo", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "RfidMarkerInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RfidValue = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RfidMarkerInfo", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "UserInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<long>(type: "bigint", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ImageUrl = table.Column<string>(type: "text", nullable: true),
                    Roles = table.Column<string>(type: "text", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInfo", x => x.Uuid);
                });

            migrationBuilder.CreateTable(
                name: "GroupInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<long>(type: "bigint", nullable: false),
                    GroupName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    CourseUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupInfo", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_GroupInfo_CourseInfo_CourseUuid",
                        column: x => x.CourseUuid,
                        principalSchema: "public",
                        principalTable: "CourseInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccountInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    UserUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountInfo", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_AccountInfo_UserInfo_UserUuid",
                        column: x => x.UserUuid,
                        principalSchema: "public",
                        principalTable: "UserInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseStudents",
                schema: "public",
                columns: table => new
                {
                    StudentUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseStudents", x => new { x.StudentUuid, x.CourseUuid });
                    table.ForeignKey(
                        name: "FK_CourseStudents_CourseInfo_CourseUuid",
                        column: x => x.CourseUuid,
                        principalSchema: "public",
                        principalTable: "CourseInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseStudents_UserInfo_StudentUuid",
                        column: x => x.StudentUuid,
                        principalSchema: "public",
                        principalTable: "UserInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CourseTeachers",
                schema: "public",
                columns: table => new
                {
                    TeacherUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    CourseUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseTeachers", x => new { x.TeacherUuid, x.CourseUuid });
                    table.ForeignKey(
                        name: "FK_CourseTeachers_CourseInfo_CourseUuid",
                        column: x => x.CourseUuid,
                        principalSchema: "public",
                        principalTable: "CourseInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseTeachers_UserInfo_TeacherUuid",
                        column: x => x.TeacherUuid,
                        principalSchema: "public",
                        principalTable: "UserInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupStudents",
                schema: "public",
                columns: table => new
                {
                    StudentUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupUuid = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupStudents", x => new { x.StudentUuid, x.GroupUuid });
                    table.ForeignKey(
                        name: "FK_GroupStudents_GroupInfo_GroupUuid",
                        column: x => x.GroupUuid,
                        principalSchema: "public",
                        principalTable: "GroupInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupStudents_UserInfo_StudentUuid",
                        column: x => x.StudentUuid,
                        principalSchema: "public",
                        principalTable: "UserInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonInfo",
                schema: "public",
                columns: table => new
                {
                    Uuid = table.Column<Guid>(type: "uuid", nullable: false),
                    ExternalId = table.Column<long>(type: "bigint", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    AttendanceId = table.Column<long>(type: "bigint", nullable: false),
                    CourseUuid = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupUuid = table.Column<Guid>(type: "uuid", nullable: true),
                    Attendances = table.Column<string>(type: "text", nullable: false),
                    CreatedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonInfo", x => x.Uuid);
                    table.ForeignKey(
                        name: "FK_LessonInfo_CourseInfo_CourseUuid",
                        column: x => x.CourseUuid,
                        principalSchema: "public",
                        principalTable: "CourseInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonInfo_GroupInfo_GroupUuid",
                        column: x => x.GroupUuid,
                        principalSchema: "public",
                        principalTable: "GroupInfo",
                        principalColumn: "Uuid",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountInfo_UserUuid",
                schema: "public",
                table: "AccountInfo",
                column: "UserUuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccountInfo_Uuid",
                schema: "public",
                table: "AccountInfo",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseInfo_ExternalId",
                schema: "public",
                table: "CourseInfo",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseInfo_Uuid",
                schema: "public",
                table: "CourseInfo",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseStudents_CourseUuid",
                schema: "public",
                table: "CourseStudents",
                column: "CourseUuid");

            migrationBuilder.CreateIndex(
                name: "IX_CourseTeachers_CourseUuid",
                schema: "public",
                table: "CourseTeachers",
                column: "CourseUuid");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfo_CourseUuid",
                schema: "public",
                table: "GroupInfo",
                column: "CourseUuid");

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfo_ExternalId",
                schema: "public",
                table: "GroupInfo",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupInfo_Uuid",
                schema: "public",
                table: "GroupInfo",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GroupStudents_GroupUuid",
                schema: "public",
                table: "GroupStudents",
                column: "GroupUuid");

            migrationBuilder.CreateIndex(
                name: "IX_LessonInfo_CourseUuid",
                schema: "public",
                table: "LessonInfo",
                column: "CourseUuid");

            migrationBuilder.CreateIndex(
                name: "IX_LessonInfo_ExternalId",
                schema: "public",
                table: "LessonInfo",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonInfo_GroupUuid",
                schema: "public",
                table: "LessonInfo",
                column: "GroupUuid");

            migrationBuilder.CreateIndex(
                name: "IX_LessonInfo_Uuid",
                schema: "public",
                table: "LessonInfo",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RfidMarkerInfo_Uuid",
                schema: "public",
                table: "RfidMarkerInfo",
                column: "Uuid",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_ExternalId",
                schema: "public",
                table: "UserInfo",
                column: "ExternalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Username",
                schema: "public",
                table: "UserInfo",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfo_Uuid",
                schema: "public",
                table: "UserInfo",
                column: "Uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountInfo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CourseStudents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CourseTeachers",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GroupStudents",
                schema: "public");

            migrationBuilder.DropTable(
                name: "LessonInfo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "RfidMarkerInfo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "UserInfo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "GroupInfo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "CourseInfo",
                schema: "public");
        }
    }
}
