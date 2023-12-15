using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MusicStoreApi.Migrations
{
    /// <inheritdoc />
    public partial class UserRecordChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SecondName",
                table: "Users",
                newName: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "SecondName");
        }
    }
}
