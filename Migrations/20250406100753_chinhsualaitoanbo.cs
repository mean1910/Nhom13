using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class chinhsualaitoanbo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Warehouses_DestinationWarehouseId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryTransactions_Warehouses_SourceWarehouseId",
                table: "InventoryTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseIngredients_Ingredients_IngredientId",
                table: "WarehouseIngredients");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseIngredients_Warehouses_WarehouseId",
                table: "WarehouseIngredients");

            migrationBuilder.DropTable(
                name: "Warehouses");

            migrationBuilder.DropIndex(
                name: "IX_WarehouseIngredients_WarehouseId",
                table: "WarehouseIngredients");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_DestinationWarehouseId",
                table: "InventoryTransactions");

            migrationBuilder.DropIndex(
                name: "IX_InventoryTransactions_SourceWarehouseId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "WarehouseIngredients");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "DestinationWarehouseId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "SourceWarehouseId",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Ingredients");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseIngredients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InventoryTransactions",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "TransactionType",
                table: "InventoryTransactions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Ingredients",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<int>(
                name: "SupplierId1",
                table: "Ingredients",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ingredients_SupplierId1",
                table: "Ingredients",
                column: "SupplierId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId",
                table: "Ingredients",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId1",
                table: "Ingredients",
                column: "SupplierId1",
                principalTable: "Suppliers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseIngredients_Ingredients_IngredientId",
                table: "WarehouseIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId1",
                table: "Ingredients");

            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseIngredients_Ingredients_IngredientId",
                table: "WarehouseIngredients");

            migrationBuilder.DropIndex(
                name: "IX_Ingredients_SupplierId1",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "InventoryTransactions");

            migrationBuilder.DropColumn(
                name: "SupplierId1",
                table: "Ingredients");

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "WarehouseIngredients",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "WarehouseIngredients",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "Quantity",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "InventoryTransactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DestinationWarehouseId",
                table: "InventoryTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceWarehouseId",
                table: "InventoryTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "InventoryTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Price",
                table: "Ingredients",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<double>(
                name: "Quantity",
                table: "Ingredients",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "Warehouses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseIngredients_WarehouseId",
                table: "WarehouseIngredients",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_DestinationWarehouseId",
                table: "InventoryTransactions",
                column: "DestinationWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryTransactions_SourceWarehouseId",
                table: "InventoryTransactions",
                column: "SourceWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Ingredients_Suppliers_SupplierId",
                table: "Ingredients",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Warehouses_DestinationWarehouseId",
                table: "InventoryTransactions",
                column: "DestinationWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryTransactions_Warehouses_SourceWarehouseId",
                table: "InventoryTransactions",
                column: "SourceWarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseIngredients_Ingredients_IngredientId",
                table: "WarehouseIngredients",
                column: "IngredientId",
                principalTable: "Ingredients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseIngredients_Warehouses_WarehouseId",
                table: "WarehouseIngredients",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
