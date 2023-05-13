using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendService.Migrations
{
    /// <inheritdoc />
    public partial class initial_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "StoreId",
                table: "Users",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StoreId",
                table: "Products",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    StoreName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Lat = table.Column<double>(type: "float", nullable: false),
                    Long = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_StoreId",
                table: "Users",
                column: "StoreId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_StoreId",
                table: "Products",
                column: "StoreId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropIndex(
                name: "IX_Users_StoreId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Products_StoreId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StoreId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
