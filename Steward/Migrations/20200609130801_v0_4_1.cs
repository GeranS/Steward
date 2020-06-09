using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class v0_4_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                        name: "FK_Proposals_PlayerCharacters_ProposedId",
                        column: x => x.ProposedId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Proposals_PlayerCharacters_ProposerId",
                        column: x => x.ProposerId,
                        principalTable: "PlayerCharacters",
                        principalColumn: "CharacterId",
                        onDelete: ReferentialAction.Restrict);
                });

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
        }
    }
}
