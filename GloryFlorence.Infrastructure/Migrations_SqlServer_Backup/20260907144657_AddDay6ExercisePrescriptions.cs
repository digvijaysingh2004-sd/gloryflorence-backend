using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloryFlorence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDay6ExercisePrescriptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExercisePrescriptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PhysiotherapistId = table.Column<int>(type: "int", nullable: false),
                    TreatmentPlanId = table.Column<int>(type: "int", nullable: false),
                    PrescriptionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Active"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercisePrescriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExercisePrescriptions_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExercisePrescriptions_TreatmentPlans_TreatmentPlanId",
                        column: x => x.TreatmentPlanId,
                        principalTable: "TreatmentPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ExercisePrescriptions_Users_PhysiotherapistId",
                        column: x => x.PhysiotherapistId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExercisePrescriptionDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PrescriptionId = table.Column<int>(type: "int", nullable: false),
                    ExerciseId = table.Column<int>(type: "int", nullable: false),
                    Sets = table.Column<int>(type: "int", nullable: false),
                    Repetitions = table.Column<int>(type: "int", nullable: false),
                    HoldSeconds = table.Column<int>(type: "int", nullable: false),
                    FrequencyPerDay = table.Column<int>(type: "int", nullable: false),
                    DurationWeeks = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExercisePrescriptionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExercisePrescriptionDetails_ExercisePrescriptions_PrescriptionId",
                        column: x => x.PrescriptionId,
                        principalTable: "ExercisePrescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ExercisePrescriptionDetails_Exercises_ExerciseId",
                        column: x => x.ExerciseId,
                        principalTable: "Exercises",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExercisePrescriptionDetails_ExerciseId",
                table: "ExercisePrescriptionDetails",
                column: "ExerciseId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercisePrescriptionDetails_PrescriptionId",
                table: "ExercisePrescriptionDetails",
                column: "PrescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercisePrescriptions_PatientId",
                table: "ExercisePrescriptions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercisePrescriptions_PhysiotherapistId",
                table: "ExercisePrescriptions",
                column: "PhysiotherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_ExercisePrescriptions_TreatmentPlanId",
                table: "ExercisePrescriptions",
                column: "TreatmentPlanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExercisePrescriptionDetails");

            migrationBuilder.DropTable(
                name: "ExercisePrescriptions");
        }
    }
}
