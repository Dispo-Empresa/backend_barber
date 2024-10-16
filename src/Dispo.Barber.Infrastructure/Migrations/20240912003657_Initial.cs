using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Dispo.Barber.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Logo = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BusinessUnities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CNPJ = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Country = table.Column<string>(type: "text", nullable: false),
                    City = table.Column<string>(type: "text", nullable: false),
                    District = table.Column<string>(type: "text", nullable: false),
                    CEP = table.Column<string>(type: "text", nullable: false),
                    Street = table.Column<string>(type: "text", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Complement = table.Column<string>(type: "text", nullable: false),
                    CompanyId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessUnities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessUnities_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompanyServices",
                columns: table => new
                {
                    CompanyId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyServices", x => new { x.CompanyId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_CompanyServices_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Surname = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Password = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<int>(type: "integer", nullable: false),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    BusinessUnityId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_BusinessUnities_BusinessUnityId",
                        column: x => x.BusinessUnityId,
                        principalTable: "BusinessUnities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccomplishedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CustomerObservation = table.Column<string>(type: "text", nullable: true),
                    AcceptedUserObservation = table.Column<string>(type: "text", nullable: true),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    BusinessUnityId = table.Column<long>(type: "bigint", nullable: false),
                    CustomerId = table.Column<long>(type: "bigint", nullable: false),
                    AcceptedUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_BusinessUnities_BusinessUnityId",
                        column: x => x.BusinessUnityId,
                        principalTable: "BusinessUnities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Appointments_Users_AcceptedUserId",
                        column: x => x.AcceptedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_Users_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserServices",
                columns: table => new
                {
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    ServiceId = table.Column<long>(type: "bigint", nullable: false),
                    Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserServices", x => new { x.UserId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_UserServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserServices_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AcceptedUserId",
                table: "Appointments",
                column: "AcceptedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BusinessUnityId",
                table: "Appointments",
                column: "BusinessUnityId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CustomerId",
                table: "Appointments",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_ServiceId",
                table: "Appointments",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessUnities_CompanyId",
                table: "BusinessUnities",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyServices_ServiceId",
                table: "CompanyServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_BusinessUnityId",
                table: "Users",
                column: "BusinessUnityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserServices_ServiceId",
                table: "UserServices",
                column: "ServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "CompanyServices");

            migrationBuilder.DropTable(
                name: "UserServices");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BusinessUnities");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
