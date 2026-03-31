using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Story");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Story",
                columns: table => new
                {
                    Pk = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    InstaUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TakenAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Story", x => x.Pk);
                    table.ForeignKey(
                        name: "FK_Story_InstagramUsers_InstaUserId",
                        column: x => x.InstaUserId,
                        principalTable: "InstagramUsers",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Story_InstaUserId",
                table: "Story",
                column: "InstaUserId");
        }
    }
}
