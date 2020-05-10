using Microsoft.EntityFrameworkCore.Migrations;

namespace Steward.Migrations
{
    public partial class number2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
	        migrationBuilder.CreateTable(
		        name: "Year",
		        columns: table => new
		        {
			        StupidId = table.Column<int>(nullable: false)
				        .Annotation("SqlServer:Identity", "1, 1"),
			        CurrentYear = table.Column<int>(nullable: true)
                },
		        constraints: table =>
		        {
			        table.PrimaryKey("PK_Year", x => x.StupidId);
		        });

            migrationBuilder.InsertData(
                table: "Year",
                columns: new[] { "StupidId", "CurrentYear" },
                values: new object[] { 1, 368 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Year",
                table: "Year");

            migrationBuilder.DropColumn(
                name: "StupidId",
                table: "Year");

            migrationBuilder.AlterColumn<int>(
                name: "CurrentYear",
                table: "Year",
                type: "int",
                nullable: false,
                oldClrType: typeof(int))
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Year",
                table: "Year",
                column: "CurrentYear");
        }
    }
}
