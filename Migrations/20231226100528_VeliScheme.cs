using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mebsoftwareApi.Migrations
{
    /// <inheritdoc />
    public partial class VeliScheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VeliName",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VeliPhoneNumber",
                table: "Student",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VeliName",
                table: "Student");

            migrationBuilder.DropColumn(
                name: "VeliPhoneNumber",
                table: "Student");
        }
    }
}
