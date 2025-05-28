using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyBasket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class changedNamefieldNameApplicationRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDelted",
                table: "AspNetRoles",
                newName: "IsDeleted");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetRoles",
                newName: "IsDelted");
        }
    }
}
