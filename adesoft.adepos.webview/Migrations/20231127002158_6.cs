using Microsoft.EntityFrameworkCore.Migrations;

namespace adesoft.adepos.webview.Migrations
{
    public partial class _6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AltName",
                table: "ZoneProducts",
                maxLength: 60,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltName",
                table: "ZoneProducts");
        }
    }
}
