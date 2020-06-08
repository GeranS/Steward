using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class v0_4_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "ValkFinderWeaponId",
                keyValue: "31412e04-220e-4a15-8ffc-751b6828d4c9");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "ValkFinderWeaponId",
                keyValue: "4983588c-c22e-4e9d-9448-1bc021d590cc");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "ValkFinderWeaponId",
                keyValue: "d2019564-523d-42b6-b3bf-f2ef47f305c0");

            migrationBuilder.CreateTable(
                name: "MarriageChannels",
                columns: table => new
                {
                    ChannelId = table.Column<string>(nullable: false),
                    ServerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MarriageChannels", x => x.ChannelId);
                });

            migrationBuilder.CreateTable(
                name: "Proposals",
                columns: table => new
                {
                    ProposalId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProposerId = table.Column<string>(nullable: true),
                    ProposedId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proposals", x => x.ProposalId);
                    table.ForeignKey(
                        name: "FK_Proposals_DiscordUsers_ProposedId",
                        column: x => x.ProposedId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Proposals_DiscordUsers_ProposerId",
                        column: x => x.ProposerId,
                        principalTable: "DiscordUsers",
                        principalColumn: "DiscordId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "52d3bee2-a79a-425d-bff2-9a3a5972aa72", 0, 0, 8, 0, false, false, null, "Sword", 0 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "8045ceb5-5974-458f-95f0-2a9aded90a32", 0, 0, 6, 0, false, false, null, "Dagger", 0 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "418b7d5c-da51-4dc4-9edd-e1d346856d11", 0, 0, 8, 0, true, false, null, "Shortbow", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_ProposedId",
                table: "Proposals",
                column: "ProposedId");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_ProposerId",
                table: "Proposals",
                column: "ProposerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MarriageChannels");

            migrationBuilder.DropTable(
                name: "Proposals");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "ValkFinderWeaponId",
                keyValue: "418b7d5c-da51-4dc4-9edd-e1d346856d11");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "ValkFinderWeaponId",
                keyValue: "52d3bee2-a79a-425d-bff2-9a3a5972aa72");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "ValkFinderWeaponId",
                keyValue: "8045ceb5-5974-458f-95f0-2a9aded90a32");

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "d2019564-523d-42b6-b3bf-f2ef47f305c0", 0, 0, 8, 0, false, false, null, "Sword", 0 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "31412e04-220e-4a15-8ffc-751b6828d4c9", 0, 0, 6, 0, false, false, null, "Dagger", 0 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "4983588c-c22e-4e9d-9448-1bc021d590cc", 0, 0, 8, 0, true, false, null, "Shortbow", 0 });
        }
    }
}
