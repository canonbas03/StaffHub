using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StaffHub.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityLinkToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Employees",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Employees");
        }
    }
}
