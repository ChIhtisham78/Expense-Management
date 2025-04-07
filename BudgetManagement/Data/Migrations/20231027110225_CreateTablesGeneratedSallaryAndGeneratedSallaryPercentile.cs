using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExpenseManagment.Data.Migrations
{
    /// <inheritdoc />
    public partial class CreateTablesGeneratedSallaryAndGeneratedSallaryPercentile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectivePercentProjectMappings_Projects_ProjectId",
                table: "EffectivePercentProjectMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectivePercentProjectMappings_SallaryMappings_SallaryMappingId",
                table: "EffectivePercentProjectMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Expences_AccountEntities_AccountId",
                table: "Expences");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AccountEntities_AccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AccountEntities_ClientId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SallaryMappings_AccountEntities_AccountId",
                table: "SallaryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_SallaryMappings_Projects_ProjectId",
                table: "SallaryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Invoices_InvoiceId",
                table: "Transactions");

            migrationBuilder.CreateTable(
                name: "GeneratedSallaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    BasicAmount = table.Column<long>(type: "bigint", nullable: false),
                    BonusAmount = table.Column<long>(type: "bigint", nullable: true),
                    GrossPercentAmount = table.Column<long>(type: "bigint", nullable: false),
                    GrossTotal = table.Column<long>(type: "bigint", nullable: false),
                    InsertionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedSallaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedSallaries_AccountEntities_AccountId",
                        column: x => x.AccountId,
                        principalTable: "AccountEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedSallaries_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedSallaryPercentiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    GeneratedSallaryId = table.Column<int>(type: "int", nullable: false),
                    PercentAmount = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedSallaryPercentiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedSallaryPercentiles_GeneratedSallaries_GeneratedSallaryId",
                        column: x => x.GeneratedSallaryId,
                        principalTable: "GeneratedSallaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedSallaryPercentiles_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedSallaries_AccountId",
                table: "GeneratedSallaries",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedSallaries_ProjectId",
                table: "GeneratedSallaries",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedSallaryPercentiles_GeneratedSallaryId",
                table: "GeneratedSallaryPercentiles",
                column: "GeneratedSallaryId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedSallaryPercentiles_ProjectId",
                table: "GeneratedSallaryPercentiles",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectivePercentProjectMappings_Projects_ProjectId",
                table: "EffectivePercentProjectMappings",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectivePercentProjectMappings_SallaryMappings_SallaryMappingId",
                table: "EffectivePercentProjectMappings",
                column: "SallaryMappingId",
                principalTable: "SallaryMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expences_AccountEntities_AccountId",
                table: "Expences",
                column: "AccountId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AccountEntities_AccountId",
                table: "Invoices",
                column: "AccountId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AccountEntities_ClientId",
                table: "Projects",
                column: "ClientId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SallaryMappings_AccountEntities_AccountId",
                table: "SallaryMappings",
                column: "AccountId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SallaryMappings_Projects_ProjectId",
                table: "SallaryMappings",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Invoices_InvoiceId",
                table: "Transactions",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectivePercentProjectMappings_Projects_ProjectId",
                table: "EffectivePercentProjectMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_EffectivePercentProjectMappings_SallaryMappings_SallaryMappingId",
                table: "EffectivePercentProjectMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Expences_AccountEntities_AccountId",
                table: "Expences");

            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_AccountEntities_AccountId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_AccountEntities_ClientId",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_SallaryMappings_AccountEntities_AccountId",
                table: "SallaryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_SallaryMappings_Projects_ProjectId",
                table: "SallaryMappings");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Invoices_InvoiceId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "GeneratedSallaryPercentiles");

            migrationBuilder.DropTable(
                name: "GeneratedSallaries");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectivePercentProjectMappings_Projects_ProjectId",
                table: "EffectivePercentProjectMappings",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_EffectivePercentProjectMappings_SallaryMappings_SallaryMappingId",
                table: "EffectivePercentProjectMappings",
                column: "SallaryMappingId",
                principalTable: "SallaryMappings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Expences_AccountEntities_AccountId",
                table: "Expences",
                column: "AccountId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_AccountEntities_AccountId",
                table: "Invoices",
                column: "AccountId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_AccountEntities_ClientId",
                table: "Projects",
                column: "ClientId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SallaryMappings_AccountEntities_AccountId",
                table: "SallaryMappings",
                column: "AccountId",
                principalTable: "AccountEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SallaryMappings_Projects_ProjectId",
                table: "SallaryMappings",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Invoices_InvoiceId",
                table: "Transactions",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
