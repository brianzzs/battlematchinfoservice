using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MatchInfoService.Migrations
{
    public partial class UrlMatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UrlMatch",
                table: "Match",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UrlMatch",
                table: "Match");
        }
    }
}
