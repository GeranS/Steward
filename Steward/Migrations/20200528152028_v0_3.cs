using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class v0_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEducation",
                table: "Traits",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "CharacterDeathTimers",
                columns: table => new
                {
                    CharacterDeathTimerId = table.Column<string>(nullable: false),
                    PlayerCharacterId = table.Column<string>(nullable: true),
                    YearOfDeath = table.Column<int>(nullable: false),
                    DeathTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterDeathTimers", x => x.CharacterDeathTimerId);
                    table.ForeignKey(
                        name: "FK_CharacterDeathTimers_PlayerCharacters_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterDeathTimers_PlayerCharacterId",
                table: "CharacterDeathTimers",
                column: "PlayerCharacterId",
                unique: true,
                filter: "[PlayerCharacterId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterDeathTimers");

            migrationBuilder.DropColumn(
                name: "IsEducation",
                table: "Traits");
        }
    }
}
