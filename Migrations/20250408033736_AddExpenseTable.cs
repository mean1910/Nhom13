using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_PurchaseOrders_PurchaseOrderId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_PurchaseOrderId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Notes",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "TotalAmount",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "ExpenseDate",
                table: "Expenses",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Expenses",
                newName: "Type");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 4, 8, 10, 37, 35, 109, DateTimeKind.Local).AddTicks(4832));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Expenses",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Expenses",
                newName: "ExpenseDate");

            migrationBuilder.AddColumn<string>(
                name: "Notes",
                table: "Expenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PurchaseOrderId",
                table: "Expenses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "TotalAmount",
                table: "Expenses",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 4, 8, 10, 9, 47, 594, DateTimeKind.Local).AddTicks(5514));

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PurchaseOrderId",
                table: "Expenses",
                column: "PurchaseOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_PurchaseOrders_PurchaseOrderId",
                table: "Expenses",
                column: "PurchaseOrderId",
                principalTable: "PurchaseOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
