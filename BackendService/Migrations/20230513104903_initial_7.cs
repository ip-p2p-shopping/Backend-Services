using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendService.Migrations
{
    /// <inheritdoc />
    public partial class initial_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingInstances",
                table: "ShoppingInstances");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ShoppingInstances",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingInstances",
                table: "ShoppingInstances",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ShoppingInstances",
                table: "ShoppingInstances");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ShoppingInstances");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShoppingInstances",
                table: "ShoppingInstances",
                column: "UserId");
        }
    }
}
