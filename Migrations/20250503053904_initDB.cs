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
                    UserID = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Username = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                }
            );

            migrationBuilder.CreateTable(
                name: "zFields",
                columns: table => new
                {
                    zFieldId = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: false),
                    Longitude = table.Column<double>(type: "double precision", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zFields", x => x.zFieldId);
                }
            );

            migrationBuilder.CreateTable(
                name: "zMaintenanceTypes",
                columns: table => new
                {
                    zMaintenanceTypeId = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zMaintenanceTypes", x => x.zMaintenanceTypeId);
                }
            );

            migrationBuilder.CreateTable(
                name: "zRoles",
                columns: table => new
                {
                    zRoleID = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    Name = table.Column<string>(type: "text", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_zRoles", x => x.zRoleID);
                }
            );

            migrationBuilder.CreateTable(
                name: "ProductionRecords",
                columns: table => new
                {
                    ProductionRecordGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    DateOfProduction = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    ProductionOfCost = table.Column<decimal>(type: "numeric", nullable: false),
                    ProductionRate = table.Column<int>(type: "integer", nullable: false),
                    zFieldId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionRecords", x => x.ProductionRecordGuid);
                    table.ForeignKey(
                        name: "FK_ProductionRecords_zFields_zFieldId",
                        column: x => x.zFieldId,
                        principalTable: "zFields",
                        principalColumn: "zFieldId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "FieldMaintenances",
                columns: table => new
                {
                    FieldMaintenanceGuid = table.Column<Guid>(type: "uuid", nullable: false),
                    Cost = table.Column<decimal>(type: "numeric", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    FieldMaintenanceDate = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: false
                    ),
                    zMaintenanceTypeId = table.Column<int>(type: "integer", nullable: false),
                    zFieldId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(
                        type: "timestamp with time zone",
                        nullable: true
                    ),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FieldMaintenances", x => x.FieldMaintenanceGuid);
                    table.ForeignKey(
                        name: "FK_FieldMaintenances_zFields_zFieldId",
                        column: x => x.zFieldId,
                        principalTable: "zFields",
                        principalColumn: "zFieldId",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_FieldMaintenances_zMaintenanceTypes_zMaintenanceTypeId",
                        column: x => x.zMaintenanceTypeId,
                        principalTable: "zMaintenanceTypes",
                        principalColumn: "zMaintenanceTypeId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserRoleID = table
                        .Column<int>(type: "integer", nullable: false)
                        .Annotation(
                            "Npgsql:ValueGenerationStrategy",
                            NpgsqlValueGenerationStrategy.IdentityByDefaultColumn
                        ),
                    UserID = table.Column<int>(type: "integer", nullable: false),
                    zRoleId = table.Column<int>(type: "integer", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.UserRoleID);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_UserRoles_zRoles_zRoleId",
                        column: x => x.zRoleId,
                        principalTable: "zRoles",
                        principalColumn: "zRoleID",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.InsertData(
                table: "zFields",
                columns: new[] { "zFieldId", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, 21.123460000000001, 55.123449999999998, "Wusta" },
                    { 2, 22.654319999999998, 56.654319999999998, "North Oman" },
                    { 3, 20.987649999999999, 54.987650000000002, "South Block" },
                    { 4, 23.456790000000002, 57.123449999999998, "West Basin" },
                    { 5, 22.123460000000001, 55.654319999999998, "Central Field" },
                    { 6, 19.987649999999999, 53.987650000000002, "Desert Zone" },
                    { 7, 20.654319999999998, 54.654319999999998, "Offshore Block" },
                    { 8, 21.987649999999999, 55.987650000000002, "Eastern Zone" },
                    { 9, 22.123460000000001, 56.123449999999998, "South Field" },
                    { 10, 23.654319999999998, 57.654319999999998, "Border Zone" },
                }
            );

            migrationBuilder.InsertData(
                table: "zMaintenanceTypes",
                columns: new[] { "zMaintenanceTypeId", "Code", "Name" },
                values: new object[,]
                {
                    { 1, "M1", "Maintenance Type 1" },
                    { 2, "M2", "Maintenance Type 2" },
                    { 3, "M3", "Maintenance Type 3" },
                    { 4, "M4", "Maintenance Type 4" },
                    { 5, "M5", "Maintenance Type 5" },
                }
            );

            migrationBuilder.InsertData(
                table: "zRoles",
                columns: new[] { "zRoleID", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Operator" },
                    { 3, "Engineer" },
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_FieldMaintenances_zFieldId",
                table: "FieldMaintenances",
                column: "zFieldId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_FieldMaintenances_zMaintenanceTypeId",
                table: "FieldMaintenances",
                column: "zMaintenanceTypeId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_ProductionRecords_zFieldId",
                table: "ProductionRecords",
                column: "zFieldId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserID",
                table: "UserRoles",
                column: "UserID"
            );

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_zRoleId",
                table: "UserRoles",
                column: "zRoleId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "FieldMaintenances");

            migrationBuilder.DropTable(name: "ProductionRecords");

            migrationBuilder.DropTable(name: "UserRoles");

            migrationBuilder.DropTable(name: "zMaintenanceTypes");

            migrationBuilder.DropTable(name: "zFields");

            migrationBuilder.DropTable(name: "Users");

            migrationBuilder.DropTable(name: "zRoles");
        }
    }
}
