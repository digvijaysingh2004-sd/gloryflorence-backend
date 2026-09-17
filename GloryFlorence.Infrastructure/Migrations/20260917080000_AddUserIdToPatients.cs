using GloryFlorence.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GloryFlorence.Infrastructure.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917080000_AddUserIdToPatients")]
    public partial class AddUserIdToPatients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Patients"" ADD COLUMN IF NOT EXISTS ""UserId"" integer NULL;
                
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 FROM pg_constraint WHERE conname = 'FK_Patients_Users_UserId'
                    ) THEN
                        ALTER TABLE ""Patients"" ADD CONSTRAINT ""FK_Patients_Users_UserId"" 
                        FOREIGN KEY (""UserId"") REFERENCES ""Users"" (""Id"") ON DELETE SET NULL;
                    END IF;
                END $$;

                CREATE INDEX IF NOT EXISTS ""IX_Patients_UserId"" ON ""Patients"" (""UserId"");
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Patients"" DROP CONSTRAINT IF EXISTS ""FK_Patients_Users_UserId"";
                DROP INDEX IF EXISTS ""IX_Patients_UserId"";
                ALTER TABLE ""Patients"" DROP COLUMN IF EXISTS ""UserId"";
            ");
        }
    }
}
