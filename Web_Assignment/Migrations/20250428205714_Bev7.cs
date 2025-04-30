using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Assignment.Migrations
{
    /// <inheritdoc />
    public partial class Bev7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Hash",
                table: "Staffs",
                newName: "Password");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Staffs",
                newName: "Hash");
        }
    }
}
