using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RelayChat_Identity.Migrations
{
    /// <inheritdoc />
    public partial class FriendRequestReadStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Read",
                table: "FriendRequests",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Read",
                table: "FriendRequests");
        }
    }
}
