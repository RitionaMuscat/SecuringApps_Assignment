using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SecuringApps.Data.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    Email = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    isStudent = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentTask",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Deadline = table.Column<DateTime>(nullable: false),
                    DocumentOwner = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentTask", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StudentWork",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    filePath = table.Column<string>(nullable: true),
                    workOwner = table.Column<string>(nullable: true),
                    submittedOn = table.Column<DateTime>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentWork", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentWork_StudentTask_TaskId",
                        column: x => x.TaskId,
                        principalTable: "StudentTask",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false, defaultValueSql: "NEWID()"),
                    comment = table.Column<string>(nullable: true),
                    writtenBy = table.Column<string>(nullable: true),
                    writtenOn = table.Column<DateTime>(nullable: false),
                    WorkId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_StudentWork_WorkId",
                        column: x => x.WorkId,
                        principalTable: "StudentWork",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_WorkId",
                table: "Comments",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentWork_TaskId",
                table: "StudentWork",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "Members");

            migrationBuilder.DropTable(
                name: "StudentWork");

            migrationBuilder.DropTable(
                name: "StudentTask");
        }
    }
}
