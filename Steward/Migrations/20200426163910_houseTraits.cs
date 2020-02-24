using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class houseTraits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AbilityPointBonus",
                table: "Traits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorClassBonus",
                table: "Traits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HealthPoolBonus",
                table: "Traits",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "DefaultMeleeWeaponId",
                table: "PlayerCharacters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DefaultRangedWeaponId",
                table: "PlayerCharacters",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AbilityPointBonus",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ArmorClassBonus",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DEX",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "END",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HealthPoolBonus",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "HouseDescription",
                table: "Houses",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HouseOwnerId",
                table: "Houses",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "INT",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PER",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "STR",
                table: "Houses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Houses",
                keyColumn: "HouseId",
                keyValue: "123",
                column: "HouseDescription",
                value: "Empty.");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_DefaultMeleeWeaponId",
                table: "PlayerCharacters",
                column: "DefaultMeleeWeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_DefaultRangedWeaponId",
                table: "PlayerCharacters",
                column: "DefaultRangedWeaponId");

            migrationBuilder.CreateIndex(
                name: "IX_Houses_HouseOwnerId",
                table: "Houses",
                column: "HouseOwnerId",
                unique: true,
                filter: "[HouseOwnerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Houses_PlayerCharacters_HouseOwnerId",
                table: "Houses",
                column: "HouseOwnerId",
                principalTable: "PlayerCharacters",
                principalColumn: "CharacterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultMeleeWeaponId",
                table: "PlayerCharacters",
                column: "DefaultMeleeWeaponId",
                principalTable: "ValkFinderWeapons",
                principalColumn: "WeaponName",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultRangedWeaponId",
                table: "PlayerCharacters",
                column: "DefaultRangedWeaponId",
                principalTable: "ValkFinderWeapons",
                principalColumn: "WeaponName",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Houses_PlayerCharacters_HouseOwnerId",
                table: "Houses");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultMeleeWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultRangedWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCharacters_DefaultMeleeWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCharacters_DefaultRangedWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropIndex(
                name: "IX_Houses_HouseOwnerId",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "AbilityPointBonus",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "ArmorClassBonus",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "HealthPoolBonus",
                table: "Traits");

            migrationBuilder.DropColumn(
                name: "DefaultMeleeWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropColumn(
                name: "DefaultRangedWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropColumn(
                name: "AbilityPointBonus",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "ArmorClassBonus",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "DEX",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "END",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "HealthPoolBonus",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "HouseDescription",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "HouseOwnerId",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "INT",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "PER",
                table: "Houses");

            migrationBuilder.DropColumn(
                name: "STR",
                table: "Houses");
        }
    }
}
