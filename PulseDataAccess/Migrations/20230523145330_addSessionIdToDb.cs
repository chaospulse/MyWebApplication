using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addSessionIdToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumer",
                table: "OrderHeader",
                newName: "PhoneNumber");

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "OrderHeader",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "OrderHeader");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "OrderHeader",
                newName: "PhoneNumer");
        }
    }
}
