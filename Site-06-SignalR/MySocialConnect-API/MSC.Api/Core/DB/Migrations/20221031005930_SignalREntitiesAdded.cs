using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MSC.Api.Core.DB.Migrations
{
    public partial class SignalREntitiesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SignalRGroups",
                columns: table => new
                {
                    GroupName = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalRGroups", x => x.GroupName);
                });

            migrationBuilder.CreateTable(
                name: "SignalRConnections",
                columns: table => new
                {
                    ConnectionId = table.Column<string>(type: "TEXT", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    SignalRGroupGroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SignalRConnections", x => x.ConnectionId);
                    table.ForeignKey(
                        name: "FK_SignalRConnections_SignalRGroups_SignalRGroupGroupName",
                        column: x => x.SignalRGroupGroupName,
                        principalTable: "SignalRGroups",
                        principalColumn: "GroupName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SignalRConnections_SignalRGroupGroupName",
                table: "SignalRConnections",
                column: "SignalRGroupGroupName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SignalRConnections");

            migrationBuilder.DropTable(
                name: "SignalRGroups");
        }
    }
}
