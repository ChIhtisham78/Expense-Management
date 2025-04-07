using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnInGeneratedSallary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedSalaryMonth",
                table: "GeneratedSallaries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TransactionId",
                table: "GeneratedSallaries",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GeneratedSalaryMonth",
                table: "GeneratedSallaries");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "GeneratedSallaries");
        }
    }
}
