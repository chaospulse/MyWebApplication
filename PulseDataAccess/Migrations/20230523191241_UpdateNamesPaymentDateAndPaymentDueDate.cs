using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PulseDataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNamesPaymentDateAndPaymentDueDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentDueData",
                table: "OrderHeader",
                newName: "PaymentDueDate");

            migrationBuilder.RenameColumn(
                name: "PaymentData",
                table: "OrderHeader",
                newName: "PaymentDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentDueDate",
                table: "OrderHeader",
                newName: "PaymentDueData");

            migrationBuilder.RenameColumn(
                name: "PaymentDate",
                table: "OrderHeader",
                newName: "PaymentData");
        }
    }
}
