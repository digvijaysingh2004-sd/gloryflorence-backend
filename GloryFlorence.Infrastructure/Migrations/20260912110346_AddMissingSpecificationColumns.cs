using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloryFlorence.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingSpecificationColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""MRN"" character varying(50) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""BloodGroup"" character varying(20) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""City"" character varying(100) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""State"" character varying(100) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""Country"" character varying(100) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""EmergencyContactName"" character varying(100) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""EmergencyContactPhone"" character varying(50) NOT NULL DEFAULT '';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""Status"" character varying(50) NOT NULL DEFAULT 'Active';
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""BloodPressure"" character varying(20);
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""HeartRate"" integer;
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""WeightKg"" double precision;
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""HeightCm"" double precision;
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""Temperature"" double precision;
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""OxygenSaturation"" double precision;
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""VitalsUpdatedAt"" timestamp with time zone;

                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'Invoices') THEN
                        ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""SubTotal"" numeric NOT NULL DEFAULT 0;
                        ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""TaxAmount"" numeric NOT NULL DEFAULT 0;
                        ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""DiscountAmount"" numeric NOT NULL DEFAULT 0;
                        ALTER TABLE ""Invoices"" ADD COLUMN IF NOT EXISTS ""TotalAmount"" numeric NOT NULL DEFAULT 0;
                    END IF;

                    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'InvoiceItems') THEN
                        ALTER TABLE ""InvoiceItems"" ADD COLUMN IF NOT EXISTS ""TreatmentTypeId"" integer;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""MRN"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""BloodGroup"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""City"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""State"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""Country"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""EmergencyContactName"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""EmergencyContactPhone"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""Status"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""BloodPressure"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""HeartRate"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""WeightKg"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""HeightCm"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""Temperature"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""OxygenSaturation"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""VitalsUpdatedAt"";

                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'Invoices') THEN
                        ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""SubTotal"";
                        ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""TaxAmount"";
                        ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""DiscountAmount"";
                        ALTER TABLE ""Invoices"" DROP COLUMN IF EXISTS ""TotalAmount"";
                    END IF;

                    IF EXISTS (SELECT 1 FROM pg_tables WHERE schemaname = 'public' AND tablename = 'InvoiceItems') THEN
                        ALTER TABLE ""InvoiceItems"" DROP COLUMN IF EXISTS ""TreatmentTypeId"";
                    END IF;
                END $$;
            ");
        }
    }
}
