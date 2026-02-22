using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Datalagring_Rasmus_Pieplow.Migrations
{
    /// <inheritdoc />
    public partial class AddMigrationUniqeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Registrations_CourseInstanceId",
                table: "Registrations");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_CourseInstanceId_ParticipantId",
                table: "Registrations",
                columns: new[] { "CourseInstanceId", "ParticipantId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Registrations_CourseInstanceId_ParticipantId",
                table: "Registrations");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_CourseInstanceId",
                table: "Registrations",
                column: "CourseInstanceId");
        }
    }
}
