using System;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;
using GloryFlorence.Infrastructure.Data;

namespace GloryFlorence.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IRepository<Patient>? _patients;
        private IRepository<Therapist>? _therapists;
        private IRepository<Appointment>? _appointments;
        private IRepository<TreatmentSession>? _treatmentSessions;
        private IRepository<User>? _users;
        private IRepository<Country>? _countries;
        private IRepository<State>? _states;
        private IRepository<City>? _cities;
        private IRepository<Address>? _addresses;
        private IRepository<Gender>? _genders;
        private IRepository<BloodGroup>? _bloodGroups;
        private IRepository<Specialization>? _specializations;
        private IRepository<Category>? _categories;
        private IRepository<PatientMedicalHistory>? _patientMedicalHistories;
        private IRepository<PatientDocument>? _patientDocuments;
        private IRepository<TreatmentType>? _treatmentTypes;
        private IRepository<Exercise>? _exercises;
        private IRepository<AppointmentType>? _appointmentTypes;
        private IRepository<Status>? _statuses;
        private IRepository<PatientAssessment>? _patientAssessments;
        private IRepository<TreatmentPlan>? _treatmentPlans;
        private IRepository<TreatmentPlanDetail>? _treatmentPlanDetails;
        private IRepository<ExercisePrescription>? _exercisePrescriptions;
        private IRepository<ExercisePrescriptionDetail>? _exercisePrescriptionDetails;
        private IRepository<AuditLog>? _auditLogs;
        private IRepository<Invoice>? _invoices;
        private IRepository<InvoiceItem>? _invoiceItems;
        private IRepository<Payment>? _payments;
        private IRepository<ClinicSettings>? _clinicSettings;

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IRepository<Patient> Patients => _patients ??= new Repository<Patient>(_dbContext);
        public IRepository<Therapist> Therapists => _therapists ??= new Repository<Therapist>(_dbContext);
        public IRepository<Appointment> Appointments => _appointments ??= new Repository<Appointment>(_dbContext);
        public IRepository<TreatmentSession> TreatmentSessions => _treatmentSessions ??= new Repository<TreatmentSession>(_dbContext);
        public IRepository<User> Users => _users ??= new Repository<User>(_dbContext);
        public IRepository<Country> Countries => _countries ??= new Repository<Country>(_dbContext);
        public IRepository<State> States => _states ??= new Repository<State>(_dbContext);
        public IRepository<City> Cities => _cities ??= new Repository<City>(_dbContext);
        public IRepository<Address> Addresses => _addresses ??= new Repository<Address>(_dbContext);
        public IRepository<Gender> Genders => _genders ??= new Repository<Gender>(_dbContext);
        public IRepository<BloodGroup> BloodGroups => _bloodGroups ??= new Repository<BloodGroup>(_dbContext);
        public IRepository<Specialization> Specializations => _specializations ??= new Repository<Specialization>(_dbContext);
        public IRepository<Category> Categories => _categories ??= new Repository<Category>(_dbContext);
        public IRepository<PatientMedicalHistory> PatientMedicalHistories => _patientMedicalHistories ??= new Repository<PatientMedicalHistory>(_dbContext);
        public IRepository<PatientDocument> PatientDocuments => _patientDocuments ??= new Repository<PatientDocument>(_dbContext);
        public IRepository<TreatmentType> TreatmentTypes => _treatmentTypes ??= new Repository<TreatmentType>(_dbContext);
        public IRepository<Exercise> Exercises => _exercises ??= new Repository<Exercise>(_dbContext);
        public IRepository<AppointmentType> AppointmentTypes => _appointmentTypes ??= new Repository<AppointmentType>(_dbContext);
        public IRepository<Status> Statuses => _statuses ??= new Repository<Status>(_dbContext);
        public IRepository<PatientAssessment> PatientAssessments => _patientAssessments ??= new Repository<PatientAssessment>(_dbContext);
        public IRepository<TreatmentPlan> TreatmentPlans => _treatmentPlans ??= new Repository<TreatmentPlan>(_dbContext);
        public IRepository<TreatmentPlanDetail> TreatmentPlanDetails => _treatmentPlanDetails ??= new Repository<TreatmentPlanDetail>(_dbContext);
        public IRepository<ExercisePrescription> ExercisePrescriptions => _exercisePrescriptions ??= new Repository<ExercisePrescription>(_dbContext);
        public IRepository<ExercisePrescriptionDetail> ExercisePrescriptionDetails => _exercisePrescriptionDetails ??= new Repository<ExercisePrescriptionDetail>(_dbContext);
        public IRepository<AuditLog> AuditLogs => _auditLogs ??= new Repository<AuditLog>(_dbContext);
        public IRepository<Invoice> Invoices => _invoices ??= new Repository<Invoice>(_dbContext);
        public IRepository<InvoiceItem> InvoiceItems => _invoiceItems ??= new Repository<InvoiceItem>(_dbContext);
        public IRepository<Payment> Payments => _payments ??= new Repository<Payment>(_dbContext);
        public IRepository<ClinicSettings> ClinicSettings => _clinicSettings ??= new Repository<ClinicSettings>(_dbContext);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbContext.SaveChangesAsync(cancellationToken);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
