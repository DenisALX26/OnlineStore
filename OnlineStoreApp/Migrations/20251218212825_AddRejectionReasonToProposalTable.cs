using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineStoreApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonToProposalTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RejectctionReason",
                table: "Proposals",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectctionReason",
                table: "Proposals");
        }
    }
}
