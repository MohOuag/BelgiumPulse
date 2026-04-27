using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BelgiumPulse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AirQualityMeasurements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StationName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Municipality = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    AqiValue = table.Column<int>(type: "int", nullable: false),
                    PM25 = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    PM10 = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    NO2 = table.Column<decimal>(type: "decimal(6,2)", nullable: false),
                    MeasuredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AirQualityMeasurements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StibLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LineNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    LineName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransportType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsOperational = table.Column<bool>(type: "bit", nullable: false),
                    DisruptionMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StibLines", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AlertType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TargetId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastTriggeredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAlerts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WeatherAlerts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeatherAlerts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AirQualityMeasurements_Municipality",
                table: "AirQualityMeasurements",
                column: "Municipality");

            migrationBuilder.CreateIndex(
                name: "IX_AirQualityMeasurements_StationName_MeasuredAt",
                table: "AirQualityMeasurements",
                columns: new[] { "StationName", "MeasuredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StibLines_LineNumber",
                table: "StibLines",
                column: "LineNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserAlerts_AlertType_TargetId_IsActive",
                table: "UserAlerts",
                columns: new[] { "AlertType", "TargetId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_UserAlerts_UserId",
                table: "UserAlerts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherAlerts_Region",
                table: "WeatherAlerts",
                column: "Region");

            migrationBuilder.CreateIndex(
                name: "IX_WeatherAlerts_ValidFrom_ValidUntil",
                table: "WeatherAlerts",
                columns: new[] { "ValidFrom", "ValidUntil" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AirQualityMeasurements");

            migrationBuilder.DropTable(
                name: "StibLines");

            migrationBuilder.DropTable(
                name: "UserAlerts");

            migrationBuilder.DropTable(
                name: "WeatherAlerts");
        }
    }
}
