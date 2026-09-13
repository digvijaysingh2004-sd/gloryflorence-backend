using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Therapist> Therapists => Set<Therapist>();
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<TreatmentSession> TreatmentSessions => Set<TreatmentSession>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Country> Countries => Set<Country>();
        public DbSet<State> States => Set<State>();
        public DbSet<City> Cities => Set<City>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Gender> Genders => Set<Gender>();
        public DbSet<BloodGroup> BloodGroups => Set<BloodGroup>();
        public DbSet<Specialization> Specializations => Set<Specialization>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<PatientMedicalHistory> PatientMedicalHistories => Set<PatientMedicalHistory>();
        public DbSet<PatientDocument> PatientDocuments => Set<PatientDocument>();
        public DbSet<TreatmentType> TreatmentTypes => Set<TreatmentType>();
        public DbSet<Exercise> Exercises => Set<Exercise>();
        public DbSet<AppointmentType> AppointmentTypes => Set<AppointmentType>();
        public DbSet<Status> Statuses => Set<Status>();
        public DbSet<PatientAssessment> PatientAssessments => Set<PatientAssessment>();
        public DbSet<TreatmentPlan> TreatmentPlans => Set<TreatmentPlan>();
        public DbSet<TreatmentPlanDetail> TreatmentPlanDetails => Set<TreatmentPlanDetail>();
        public DbSet<ExercisePrescription> ExercisePrescriptions => Set<ExercisePrescription>();
        public DbSet<ExercisePrescriptionDetail> ExercisePrescriptionDetails => Set<ExercisePrescriptionDetail>();
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<ClinicSettings> ClinicSettings => Set<ClinicSettings>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply all configurations defined in this assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
