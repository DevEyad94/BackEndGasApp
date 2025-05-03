using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace project.Migrations
{
    /// <inheritdoc />
    public partial class initDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "zFields",
                columns: table => new
                {
                    zFieldId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric", nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zFields", x => x.zFieldId);
                });

            migrationBuilder.CreateTable(
                name: "zMaintenanceTypes",
                columns: table => new
                {
                    zMaintenanceTypeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zMaintenanceTypes", x => x.zMaintenanceTypeId);
                });

            migrationBuilder.CreateTable(
                name: "zRoles",
                columns: table => new
                {
                    zRoleID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zRoles", x => x.zRoleID);
                });

            migrationBuilder.CreateTable(
                name: "ProductionRecords",
                columns: table => new
                {
                    ProductionRecordGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfProduction = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ProductionOfCost = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductionRate = table.Column<int>(type: "integer", nullable: false),
                    zFieldId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionRecords", x => x.ProductionRecordGuid);
                    table.ForeignKey(
                        name: "FK_ProductionRecords_zFields_zFieldId",
                        column: x => x.zFieldId,
                        principalTable: "zFields",
                        principalColumn: "zFieldId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FieldMaintenances",
                columns: table => new
                {
                    FieldMaintenanceGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FieldMaintenanceDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    zMaintenanceTypeId = table.Column<int>(type: "integer", nullable: false),
                    zFieldId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldMaintenances", x => x.FieldMaintenanceGuid);
                    table.ForeignKey(
                        name: "FK_FieldMaintenances_zFields_zFieldId",
                        column: x => x.zFieldId,
                        principalTable: "zFields",
                        principalColumn: "zFieldId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FieldMaintenances_zMaintenanceTypes_zMaintenanceTypeId",
                        column: x => x.zMaintenanceTypeId,
                        principalTable: "zMaintenanceTypes",
                        principalColumn: "zMaintenanceTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    zRoleId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_zRoles_zRoleId",
                        column: x => x.zRoleId,
                        principalTable: "zRoles",
                        principalColumn: "zRoleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "zFields",
                columns: new[] { "zFieldId", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, 21.12346m, 55.12345m, "Wusta" },
                    { 2, 22.65432m, 56.65432m, "North Oman" },
                    { 3, 20.98765m, 54.98765m, "South Block" },
                    { 4, 23.45679m, 57.12345m, "West Basin" },
                    { 5, 22.12346m, 55.65432m, "Central Field" },
                    { 6, 19.98765m, 53.98765m, "Desert Zone" },
                    { 7, 20.65432m, 54.65432m, "Offshore Block" },
                    { 8, 21.98765m, 55.98765m, "Eastern Zone" },
                    { 9, 22.12346m, 56.12345m, "South Field" },
                    { 10, 23.65432m, 57.65432m, "Border Zone" }
                });

            migrationBuilder.InsertData(
                table: "zMaintenanceTypes",
                columns: new[] { "zMaintenanceTypeId", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "M1", "Maintenance Type 1" },
                    { 2, "M2", "Maintenance Type 2" },
                    { 3, "M3", "Maintenance Type 3" },
                    { 4, "M4", "Maintenance Type 4" },
                    { 5, "M5", "Maintenance Type 5" }
                });

            migrationBuilder.InsertData(
                table: "zRoles",
                columns: new[] { "zRoleID", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Operator" },
                    { 3, "Engineer" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FieldMaintenances_zFieldId",
                table: "FieldMaintenances",
                column: "zFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_FieldMaintenances_zMaintenanceTypeId",
                table: "FieldMaintenances",
                column: "zMaintenanceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionRecords_zFieldId",
                table: "ProductionRecords",
                column: "zFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserID",
                table: "UserRoles",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_zRoleId",
                table: "UserRoles",
                column: "zRoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FieldMaintenances");

            migrationBuilder.DropTable(
                name: "ProductionRecords");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "zMaintenanceTypes");

            migrationBuilder.DropTable(
                name: "zFields");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "zRoles");
        }
    }
}
