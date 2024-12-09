using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NextStopAPIs.Migrations
{
    /// <inheritdoc />
    public partial class DbSecond : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeatNumber",
                table: "ScheduleSeats",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeatNumber",
                table: "ScheduleSeats");
        }
    }
}
