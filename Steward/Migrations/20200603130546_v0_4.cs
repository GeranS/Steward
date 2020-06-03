using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class v0_4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultMeleeWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultRangedWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValkFinderWeapons",
                table: "ValkFinderWeapons");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "WeaponName",
                keyValue: "Dagger");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "WeaponName",
                keyValue: "Shortbow");

            migrationBuilder.DeleteData(
                table: "ValkFinderWeapons",
                keyColumn: "WeaponName",
                keyValue: "Sword");

            migrationBuilder.DropColumn(
                name: "DieSize",
                table: "ValkFinderWeapons");

            migrationBuilder.AlterColumn<string>(
                name: "WeaponName",
                table: "ValkFinderWeapons",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "ValkFinderWeaponId",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DamageBonus",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DamageDieAmount",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DamageDieSize",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HitBonus",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnique",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "WeaponDescription",
                table: "ValkFinderWeapons",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WeaponTrait",
                table: "ValkFinderWeapons",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "EquippedArmourId",
                table: "PlayerCharacters",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpouseId",
                table: "PlayerCharacters",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValkFinderWeapons",
                table: "ValkFinderWeapons",
                column: "ValkFinderWeaponId");

            migrationBuilder.CreateTable(
                name: "ValkFinderArmours",
                columns: table => new
                {
                    ValkFinderArmourId = table.Column<string>(nullable: false),
                    ArmourName = table.Column<string>(nullable: true),
                    ArmourClassBonus = table.Column<int>(nullable: false),
                    DexCost = table.Column<int>(nullable: false),
                    IsUnique = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValkFinderArmours", x => x.ValkFinderArmourId);
                });

            migrationBuilder.CreateTable(
                name: "ValkFinderItems",
                columns: table => new
                {
                    ValkFinderItemId = table.Column<string>(nullable: false),
                    ItemName = table.Column<string>(nullable: true),
                    ItemDescription = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValkFinderItems", x => x.ValkFinderItemId);
                });

            migrationBuilder.CreateTable(
                name: "CharacterInventories",
                columns: table => new
                {
                    InventoryId = table.Column<string>(nullable: false),
                    PlayerCharacterId = table.Column<string>(nullable: true),
                    ValkFinderWeaponId = table.Column<string>(nullable: true),
                    ValkFinderArmourId = table.Column<string>(nullable: true),
                    ValkFinderItemId = table.Column<string>(nullable: true),
                    Amount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterInventories", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_CharacterInventories_PlayerCharacters_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterInventories_ValkFinderArmours_ValkFinderArmourId",
                        column: x => x.ValkFinderArmourId,
                        principalTable: "ValkFinderArmours",
                        principalColumn: "ValkFinderArmourId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterInventories_ValkFinderItems_ValkFinderItemId",
                        column: x => x.ValkFinderItemId,
                        principalTable: "ValkFinderItems",
                        principalColumn: "ValkFinderItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CharacterInventories_ValkFinderWeapons_ValkFinderWeaponId",
                        column: x => x.ValkFinderWeaponId,
                        principalTable: "ValkFinderWeapons",
                        principalColumn: "ValkFinderWeaponId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "ac289074-bdbf-4408-9b65-ba4716adc0ed", 0, 0, 8, 0, false, false, null, "Sword", 0 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "6ad61f8b-5ef3-4ec9-bdd2-08116a43db34", 0, 0, 6, 0, false, false, null, "Dagger", 0 });

            migrationBuilder.InsertData(
                table: "ValkFinderWeapons",
                columns: new[] { "ValkFinderWeaponId", "DamageBonus", "DamageDieAmount", "DamageDieSize", "HitBonus", "IsRanged", "IsUnique", "WeaponDescription", "WeaponName", "WeaponTrait" },
                values: new object[] { "a56ada89-e274-4e99-acb2-597acfa6cc1e", 0, 0, 8, 0, true, false, null, "Shortbow", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerCharacters_EquippedArmourId",
                table: "PlayerCharacters",
                column: "EquippedArmourId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventories_PlayerCharacterId",
                table: "CharacterInventories",
                column: "PlayerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventories_ValkFinderArmourId",
                table: "CharacterInventories",
                column: "ValkFinderArmourId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventories_ValkFinderItemId",
                table: "CharacterInventories",
                column: "ValkFinderItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CharacterInventories_ValkFinderWeaponId",
                table: "CharacterInventories",
                column: "ValkFinderWeaponId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultMeleeWeapon",
                table: "PlayerCharacters",
                column: "DefaultMeleeWeaponId",
                principalTable: "ValkFinderWeapons",
                principalColumn: "ValkFinderWeaponId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultRangedWeapon",
                table: "PlayerCharacters",
                column: "DefaultRangedWeaponId",
                principalTable: "ValkFinderWeapons",
                principalColumn: "ValkFinderWeaponId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerCharacters_ValkFinderArmours_EquippedArmourId",
                table: "PlayerCharacters",
                column: "EquippedArmourId",
                principalTable: "ValkFinderArmours",
                principalColumn: "ValkFinderArmourId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultMeleeWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderWeapons_DefaultRangedWeaponId",
                table: "PlayerCharacters");

            migrationBuilder.DropForeignKey(
                name: "FK_PlayerCharacters_ValkFinderArmours_EquippedArmourId",
                table: "PlayerCharacters");

            migrationBuilder.DropTable(
                name: "CharacterInventories");

            migrationBuilder.DropTable(
                name: "ValkFinderArmours");

            migrationBuilder.DropTable(
                name: "ValkFinderItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ValkFinderWeapons",
                table: "ValkFinderWeapons");

            migrationBuilder.DropIndex(
                name: "IX_PlayerCharacters_EquippedArmourId",
                table: "PlayerCharacters");

            migrationBuilder.DropColumn(
                name: "ValkFinderWeaponId",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "DamageBonus",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "DamageDieAmount",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "DamageDieSize",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "HitBonus",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "IsUnique",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "WeaponDescription",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "WeaponTrait",
                table: "ValkFinderWeapons");

            migrationBuilder.DropColumn(
                name: "EquippedArmourId",
                table: "PlayerCharacters");

            migrationBuilder.DropColumn(
                name: "SpouseId",
                table: "PlayerCharacters");

            migrationBuilder.AlterColumn<string>(
                name: "WeaponName",
                table: "ValkFinderWeapons",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "DieSize",
                table: "ValkFinderWeapons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ValkFinderWeapons",
                table: "ValkFinderWeapons",
                column: "WeaponName");

            migrationBuilder.UpdateData(
                table: "ValkFinderWeapons",
                keyColumn: "WeaponName",
                keyValue: "Dagger",
                column: "DieSize",
                value: 6);

            migrationBuilder.UpdateData(
                table: "ValkFinderWeapons",
                keyColumn: "WeaponName",
                keyValue: "Shortbow",
                column: "DieSize",
                value: 8);

            migrationBuilder.UpdateData(
                table: "ValkFinderWeapons",
                keyColumn: "WeaponName",
                keyValue: "Sword",
                column: "DieSize",
                value: 8);

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
    }
}
