using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class OederDetailsFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_FoodItem_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_MenuItemId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "MenuItemId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "FoodItemId",
                table: "OrderDetails",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_FoodItemId",
                table: "OrderDetails",
                column: "FoodItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_FoodItem_FoodItemId",
                table: "OrderDetails",
                column: "FoodItemId",
                principalTable: "FoodItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_FoodItem_FoodItemId",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_FoodItemId",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "FoodItemId",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "MenuItemId",
                table: "OrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_FoodItem_MenuItemId",
                table: "OrderDetails",
                column: "MenuItemId",
                principalTable: "FoodItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
