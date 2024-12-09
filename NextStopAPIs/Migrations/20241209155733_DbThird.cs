using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStopAPIs.Migrations
{
    /// <inheritdoc />
    public partial class DbThird : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "ScheduleSeats");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "ScheduleSeats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
