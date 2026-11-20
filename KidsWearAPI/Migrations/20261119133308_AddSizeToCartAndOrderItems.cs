using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KidsWearAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSizeToCartAndOrderItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId_ProductId",
                table: "Carts");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "OrderItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Size",
                table: "Carts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId_ProductId_Size",
                table: "Carts",
                columns: new[] { "UserId", "ProductId", "Size" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId_ProductId_Size",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "OrderItems");

            migrationBuilder.DropColumn(
                name: "Size",
                table: "Carts");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId_ProductId",
                table: "Carts",
                columns: new[] { "UserId", "ProductId" },
                unique: true);
        }
    }
}
