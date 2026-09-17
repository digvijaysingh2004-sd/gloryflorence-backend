using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloryFlorence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRequiredBackendChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""TreatmentSessions"" ADD COLUMN IF NOT EXISTS ""ModalitiesConducted"" text NULL;
                ALTER TABLE ""TreatmentSessions"" ADD COLUMN IF NOT EXISTS ""NextSessionPlan"" text NULL;
                ALTER TABLE ""TreatmentSessions"" ADD COLUMN IF NOT EXISTS ""PatientTolerance"" text NULL;

                ALTER TABLE ""TreatmentPlans"" ADD COLUMN IF NOT EXISTS ""Diagnosis"" text NULL;
                ALTER TABLE ""TreatmentPlans"" ADD COLUMN IF NOT EXISTS ""TreatmentFrequency"" text NULL;

                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""PainLocation"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""PainType"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""AggravatingFactors"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""RelievingFactors"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""RomFindings"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""PostureAndGait"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""FunctionalLimitations"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""Prognosis"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""ShortTermGoals"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""LongTermGoals"" text NULL;
                ALTER TABLE ""PatientAssessments"" ADD COLUMN IF NOT EXISTS ""RecommendedFrequency"" text NULL;

                ALTER TABLE ""ExercisePrescriptions"" ADD COLUMN IF NOT EXISTS ""Diagnosis"" text NULL;
                ALTER TABLE ""ExercisePrescriptions"" ADD COLUMN IF NOT EXISTS ""TargetGoal"" text NULL;

                ALTER TABLE ""Appointments"" ADD COLUMN IF NOT EXISTS ""Room"" text NULL;
                ALTER TABLE ""Appointments"" ADD COLUMN IF NOT EXISTS ""Fee"" numeric NULL;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""TreatmentSessions"" DROP COLUMN IF EXISTS ""ModalitiesConducted"";
                ALTER TABLE ""TreatmentSessions"" DROP COLUMN IF EXISTS ""NextSessionPlan"";
                ALTER TABLE ""TreatmentSessions"" DROP COLUMN IF EXISTS ""PatientTolerance"";

                ALTER TABLE ""TreatmentPlans"" DROP COLUMN IF EXISTS ""Diagnosis"";
                ALTER TABLE ""TreatmentPlans"" DROP COLUMN IF EXISTS ""TreatmentFrequency"";

                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""PainLocation"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""PainType"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""AggravatingFactors"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""RelievingFactors"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""RomFindings"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""PostureAndGait"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""FunctionalLimitations"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""Prognosis"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""ShortTermGoals"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""LongTermGoals"";
                ALTER TABLE ""PatientAssessments"" DROP COLUMN IF EXISTS ""RecommendedFrequency"";

                ALTER TABLE ""ExercisePrescriptions"" DROP COLUMN IF EXISTS ""Diagnosis"";
                ALTER TABLE ""ExercisePrescriptions"" DROP COLUMN IF EXISTS ""TargetGoal"";

                ALTER TABLE ""Appointments"" DROP COLUMN IF EXISTS ""Room"";
                ALTER TABLE ""Appointments"" DROP COLUMN IF EXISTS ""Fee"";
            ");
        }
    }
}
