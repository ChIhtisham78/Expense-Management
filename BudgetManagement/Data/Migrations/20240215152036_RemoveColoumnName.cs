using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveColoumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColoumnName",
                table: "AuditLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColoumnName",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
