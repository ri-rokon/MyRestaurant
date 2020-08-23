using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MyRestaurant.Data.Migrations
{
    public partial class OrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OrderDate",
                table: "OrderHeader",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderDate",
                table: "OrderHeader");
        }
    }
}
