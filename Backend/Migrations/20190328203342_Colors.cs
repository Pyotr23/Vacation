using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Backend.Migrations
{
    public partial class Colors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "Employees");

            migrationBuilder.AddColumn<int>(
                name: "ColorId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    ColorId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ColorNumber = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.ColorId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ColorId",
                table: "Employees",
                column: "ColorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_Colors_ColorId",
                table: "Employees",
                column: "ColorId",
                principalTable: "Colors",
                principalColumn: "ColorId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_Colors_ColorId",
                table: "Employees");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropIndex(
                name: "IX_Employees_ColorId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "ColorId",
                table: "Employees");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Employees",
                nullable: true);
        }
    }
}
