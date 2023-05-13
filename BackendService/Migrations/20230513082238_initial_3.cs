using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendService.Migrations
{
    /// <inheritdoc />
    public partial class initial_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShoppingInstances",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Bought = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShoppingInstances", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingInstances_Bought",
                table: "ShoppingInstances",
                column: "Bought");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingInstances_ProductId",
                table: "ShoppingInstances",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingInstances_UserId",
                table: "ShoppingInstances",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShoppingInstances");
        }
    }
}
