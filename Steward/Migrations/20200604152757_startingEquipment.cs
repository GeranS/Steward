using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class startingEquipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasStartingEquipment",
                table: "PlayerCharacters",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.DropColumn(
                name: "HasStartingEquipment",
                table: "PlayerCharacters");
        }
    }
}
