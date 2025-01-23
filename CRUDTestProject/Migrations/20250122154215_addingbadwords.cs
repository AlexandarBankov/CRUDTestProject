using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUDTestProject.Migrations
{
    /// <inheritdoc />
    public partial class addingbadwords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BadWords",
                columns: table => new
                {
                    Word = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadWords", x => x.Word);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BadWords");
        }
    }
}
