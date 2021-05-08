using Microsoft.EntityFrameworkCore.Migrations;

namespace Securrency.TDS.Web.DataLayer.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Tds");

            migrationBuilder.CreateTable(
                name: "Payments",
                schema: "Tds",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    SourceAccountId = table.Column<string>(maxLength: 60, nullable: false),
                    TransactionSuccessful = table.Column<bool>(nullable: false),
                    From = table.Column<string>(maxLength: 60, nullable: false),
                    To = table.Column<string>(maxLength: 60, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(15,9)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Payments",
                schema: "Tds");
        }
    }
}
