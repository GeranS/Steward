using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class v0_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DamageModifier",
                table: "ValkFinderWeapons");

            migrationBuilder.AddColumn<bool>(
                name: "IsSecret",
                table: "Traits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "MessageId",
                table: "StaffActions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseRoleId",
                table: "Houses",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "StaffActionChannels",
                columns: table => new
                {
                    ChannelId = table.Column<string>(nullable: false),
                    ServerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffActionChannels", x => x.ChannelId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StaffActionChannels");

            migrationBuilder.DropColumn(
                name: "IsSecret",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "MessageId",
                table: "StaffActions");

            migrationBuilder.DropColumn(
                name: "HouseRoleId",
                table: "Houses");

            migrationBuilder.AddColumn<int>(
                name: "DamageModifier",
                table: "ValkFinderWeapons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
