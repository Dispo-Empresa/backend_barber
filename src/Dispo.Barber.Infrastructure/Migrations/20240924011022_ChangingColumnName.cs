using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Dispo.Barber.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangingColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Customer",
                newName: "Name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customer",
                newName: "Nome");
        }
    }
}
