using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddColoumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserEmail",
                table: "AuditLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserEmail",
                table: "AuditLogs");
        }
    }
}
