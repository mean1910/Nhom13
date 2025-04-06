using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class themkhoangoai : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Ingredients");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "Ingredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_SupplierId",
                table: "Ingredients",
                column: "SupplierId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId",
                table: "Ingredients",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId",
                table: "Ingredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_SupplierId",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "Ingredients");

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "Ingredients",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
