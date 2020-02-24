using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscordUsers",
                columns: table => new
                {
                    DiscordId = table.Column<string>(nullable: false),
                    CanUseAdminCommands = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscordUsers", x => x.DiscordId);
                });

            migrationBuilder.CreateTable(
                name: "Graveyards",
                columns: table => new
                {
                    ChannelId = table.Column<string>(nullable: false),
                    ServerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Graveyards", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "Houses",
                columns: table => new
                {
                    HouseId = table.Column<string>(nullable: false),
                    HouseName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Houses", x => x.HouseId);
                });

            migrationBuilder.CreateTable(
                name: "Traits",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    STR = table.Column<int>(nullable: false),
                    END = table.Column<int>(nullable: false),
                    DEX = table.Column<int>(nullable: false),
                    PER = table.Column<int>(nullable: false),
                    INT = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Traits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ValkFinderWeapons",
                columns: table => new
                {
                    WeaponName = table.Column<string>(nullable: false),
                    IsRanged = table.Column<bool>(nullable: false),
                    DieSize = table.Column<int>(nullable: false),
                    DamageModifier = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValkFinderWeapons", x => x.WeaponName);
                });

            migrationBuilder.CreateTable(
                name: "Year",
                columns: table => new
                {
                    CurrentYear = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Year", x => x.CurrentYear);
                });

            migrationBuilder.CreateTable(
                name: "MessageRecords",
                columns: table => new
                {
                    RecordId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServerId = table.Column<long>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageRecords", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_MessageRecords_DiscordUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StaffActions",
                columns: table => new
                {
                    StaffActionId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionDescription = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    SubmitterId = table.Column<string>(nullable: true),
                    AssignedToId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffActions", x => x.StaffActionId);
                    table.ForeignKey(
                        name: "FK_StaffActions_DiscordUsers_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StaffActions_DiscordUsers_SubmitterId",
                        column: x => x.SubmitterId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharacters",
                columns: table => new
                {
                    CharacterId = table.Column<string>(nullable: false),
                    CharacterName = table.Column<string>(nullable: true),
                    Bio = table.Column<string>(nullable: true),
                    STR = table.Column<int>(nullable: false),
                    END = table.Column<int>(nullable: false),
                    DEX = table.Column<int>(nullable: false),
                    PER = table.Column<int>(nullable: false),
                    INT = table.Column<int>(nullable: false),
                    InitialAge = table.Column<int>(nullable: false),
                    YearOfBirth = table.Column<int>(nullable: false),
                    YearOfDeath = table.Column<string>(nullable: true),
                    DiscordUserId = table.Column<string>(nullable: true),
                    HouseId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacters", x => x.CharacterId);
                    table.ForeignKey(
                        name: "FK_PlayerCharacters_DiscordUsers_DiscordUserId",
                        column: x => x.DiscordUserId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlayerCharacters_Houses_HouseId",
                        column: x => x.HouseId,
                        principalTable: "Houses",
                        principalColumn: "HouseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CharacterTraits",
                columns: table => new
                {
                    PlayerCharacterId = table.Column<string>(nullable: false),
                    TraitId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterTraits", x => new { x.PlayerCharacterId, x.TraitId });
                    table.ForeignKey(
                        name: "FK_CharacterTraits_PlayerCharacters_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CharacterTraits_Traits_TraitId",
                        column: x => x.TraitId,
                        principalTable: "Traits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "DiscordUsers",
                columns: new[] { "DiscordId", "CanUseAdminCommands" },
                values: new object[] { "75968535074967552", true });

            migrationBuilder.InsertData(
                table: "Houses",
                columns: new[] { "HouseId", "HouseName" },
                values: new object[] { "123", "Bob" });

            migrationBuilder.InsertData(
                table: "Traits",
                columns: new[] { "Id", "DEX", "Description", "END", "INT", "PER", "STR" },
                values: new object[] { "123", 0, "Court Education - You have been educated in the writ of law, and the book of justice or whatever.", 0, 0, 1, -1 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "WeaponName", "DamageModifier", "DieSize", "IsRanged" },
                values: new object[] { "Sword", 0, 8, false });

            migrationBuilder.InsertData(
                table: "Year",
                column: "CurrentYear",
                value: 2000);

            migrationBuilder.InsertData(
                table: "PlayerCharacters",
                columns: new[] { "CharacterId", "Bio", "CharacterName", "DEX", "DiscordUserId", "END", "HouseId", "INT", "InitialAge", "PER", "STR", "YearOfBirth", "YearOfDeath" },
                values: new object[] { "123", "Gotta go fast!", "Maurice Wentworth", 14, "75968535074967552", 8, "123", 8, 20, 8, 12, 1980, null });

            migrationBuilder.InsertData(
                table: "CharacterTraits",
                columns: new[] { "PlayerCharacterId", "TraitId" },
                values: new object[] { "123", "123" });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterTraits_TraitId",
                table: "CharacterTraits",
                column: "TraitId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageRecords_UserId",
                table: "MessageRecords",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_DiscordUserId",
                table: "PlayerCharacters",
                column: "DiscordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_HouseId",
                table: "PlayerCharacters",
                column: "HouseId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffActions_AssignedToId",
                table: "StaffActions",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffActions_SubmitterId",
                table: "StaffActions",
                column: "SubmitterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterTraits");

            migrationBuilder.DropTable(
                name: "Graveyards");

            migrationBuilder.DropTable(
                name: "MessageRecords");

            migrationBuilder.DropTable(
                name: "StaffActions");

            migrationBuilder.DropTable(
                name: "ValkFinderWeapons");

            migrationBuilder.DropTable(
                name: "Year");

            migrationBuilder.DropTable(
                name: "PlayerCharacters");

            migrationBuilder.DropTable(
                name: "Traits");

            migrationBuilder.DropTable(
                name: "DiscordUsers");

            migrationBuilder.DropTable(
                name: "Houses");
        }
    }
}
