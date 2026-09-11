using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloryFlorence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDay5ClinicalWorkflowAndAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSessions_Appointments_AppointmentId",
                table: "TreatmentSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Recommendations",
                table: "TreatmentSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TreatmentSessions",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "TreatmentSessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "PainLevelAfter",
                table: "TreatmentSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PainLevelBefore",
                table: "TreatmentSessions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PatientId",
                table: "TreatmentSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PhysiotherapistId",
                table: "TreatmentSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "TreatmentSessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TreatmentSessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "Scheduled");

            migrationBuilder.AddColumn<int>(
                name: "TreatmentPlanId",
                table: "TreatmentSessions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    IPAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "PatientAssessments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    PhysiotherapistId = table.Column<int>(type: "int", nullable: false),
                    AssessmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChiefComplaint = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CurrentCondition = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    PainLevel = table.Column<int>(type: "int", nullable: false),
                    Diagnosis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ClinicalNotes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Recommendations = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientAssessments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PatientAssessments_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssessments_TreatmentSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "TreatmentSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientAssessments_Users_PhysiotherapistId",
                        column: x => x.PhysiotherapistId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentPlans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PatientId = table.Column<int>(type: "int", nullable: false),
                    PhysiotherapistId = table.Column<int>(type: "int", nullable: false),
                    AssessmentId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedEndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfSessions = table.Column<int>(type: "int", nullable: false),
                    Goal = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Draft"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentPlans_PatientAssessments_AssessmentId",
                        column: x => x.AssessmentId,
                        principalTable: "PatientAssessments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentPlans_Patients_PatientId",
                        column: x => x.PatientId,
                        principalTable: "Patients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TreatmentPlans_Users_PhysiotherapistId",
                        column: x => x.PhysiotherapistId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TreatmentPlanDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TreatmentPlanId = table.Column<int>(type: "int", nullable: false),
                    TreatmentTypeId = table.Column<int>(type: "int", nullable: false),
                    Frequency = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    NumberOfSessions = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TreatmentPlanDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TreatmentPlanDetails_TreatmentPlans_TreatmentPlanId",
                        column: x => x.TreatmentPlanId,
                        principalTable: "TreatmentPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TreatmentPlanDetails_TreatmentTypes_TreatmentTypeId",
                        column: x => x.TreatmentTypeId,
                        principalTable: "TreatmentTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSessions_PatientId",
                table: "TreatmentSessions",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSessions_PhysiotherapistId",
                table: "TreatmentSessions",
                column: "PhysiotherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentSessions_TreatmentPlanId",
                table: "TreatmentSessions",
                column: "TreatmentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedAt",
                table: "AuditLogs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityName",
                table: "AuditLogs",
                column: "EntityName");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssessments_PatientId",
                table: "PatientAssessments",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssessments_PhysiotherapistId",
                table: "PatientAssessments",
                column: "PhysiotherapistId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientAssessments_SessionId",
                table: "PatientAssessments",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlanDetails_TreatmentPlanId",
                table: "TreatmentPlanDetails",
                column: "TreatmentPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlanDetails_TreatmentTypeId",
                table: "TreatmentPlanDetails",
                column: "TreatmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlans_AssessmentId",
                table: "TreatmentPlans",
                column: "AssessmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlans_PatientId",
                table: "TreatmentPlans",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_TreatmentPlans_PhysiotherapistId",
                table: "TreatmentPlans",
                column: "PhysiotherapistId");

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSessions_Appointments_AppointmentId",
                table: "TreatmentSessions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSessions_Patients_PatientId",
                table: "TreatmentSessions",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSessions_TreatmentPlans_TreatmentPlanId",
                table: "TreatmentSessions",
                column: "TreatmentPlanId",
                principalTable: "TreatmentPlans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSessions_Users_PhysiotherapistId",
                table: "TreatmentSessions",
                column: "PhysiotherapistId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSessions_Appointments_AppointmentId",
                table: "TreatmentSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSessions_Patients_PatientId",
                table: "TreatmentSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSessions_TreatmentPlans_TreatmentPlanId",
                table: "TreatmentSessions");

            migrationBuilder.DropForeignKey(
                name: "FK_TreatmentSessions_Users_PhysiotherapistId",
                table: "TreatmentSessions");

            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "TreatmentPlanDetails");

            migrationBuilder.DropTable(
                name: "TreatmentPlans");

            migrationBuilder.DropTable(
                name: "PatientAssessments");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentSessions_PatientId",
                table: "TreatmentSessions");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentSessions_PhysiotherapistId",
                table: "TreatmentSessions");

            migrationBuilder.DropIndex(
                name: "IX_TreatmentSessions_TreatmentPlanId",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "PainLevelAfter",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "PainLevelBefore",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "PatientId",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "PhysiotherapistId",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "TreatmentSessions");

            migrationBuilder.DropColumn(
                name: "TreatmentPlanId",
                table: "TreatmentSessions");

            migrationBuilder.AlterColumn<string>(
                name: "Recommendations",
                table: "TreatmentSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "TreatmentSessions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddForeignKey(
                name: "FK_TreatmentSessions_Appointments_AppointmentId",
                table: "TreatmentSessions",
                column: "AppointmentId",
                principalTable: "Appointments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
