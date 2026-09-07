using System;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Domain.Entities;

namespace GloryFlorence.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Patient> Patients { get; }
        IRepository<Therapist> Therapists { get; }
        IRepository<Appointment> Appointments { get; }
        IRepository<TreatmentSession> TreatmentSessions { get; }
        IRepository<User> Users { get; }
        IRepository<Country> Countries { get; }
        IRepository<State> States { get; }
        IRepository<City> Cities { get; }
        IRepository<Address> Addresses { get; }
        IRepository<Gender> Genders { get; }
        IRepository<BloodGroup> BloodGroups { get; }
        IRepository<Specialization> Specializations { get; }
        IRepository<Category> Categories { get; }
        IRepository<PatientMedicalHistory> PatientMedicalHistories { get; }
        IRepository<PatientDocument> PatientDocuments { get; }
        IRepository<TreatmentType> TreatmentTypes { get; }
        IRepository<Exercise> Exercises { get; }
        IRepository<AppointmentType> AppointmentTypes { get; }
        IRepository<Status> Statuses { get; }
        IRepository<PatientAssessment> PatientAssessments { get; }
        IRepository<TreatmentPlan> TreatmentPlans { get; }
        IRepository<TreatmentPlanDetail> TreatmentPlanDetails { get; }
        IRepository<ExercisePrescription> ExercisePrescriptions { get; }
        IRepository<ExercisePrescriptionDetail> ExercisePrescriptionDetails { get; }
        IRepository<AuditLog> AuditLogs { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
