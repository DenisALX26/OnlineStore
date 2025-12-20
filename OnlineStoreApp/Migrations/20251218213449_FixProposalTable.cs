using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class FixProposalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectctionReason",
                table: "Proposals",
                newName: "RejectionReason");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RejectionReason",
                table: "Proposals",
                newName: "RejectctionReason");
        }
    }
}
