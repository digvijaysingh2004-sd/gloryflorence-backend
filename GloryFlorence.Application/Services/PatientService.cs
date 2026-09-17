using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.Query()
                .Include(p => p.MedicalHistories)
                .Include(p => p.Documents)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.Physiotherapist)
                .Include(p => p.Appointments)
                    .ThenInclude(a => a.AppointmentType)
                .Include(p => p.Assessments)
                .Include(p => p.TreatmentPlans)
                .Include(p => p.ExercisePrescriptions)
                    .ThenInclude(ep => ep.PrescriptionDetails)
                .Include(p => p.Invoices)
                    .ThenInclude(inv => inv.InvoiceItems)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            var baseDto = MapToDto(patient);
            var detailsDto = new PatientDetailsDto
            {
                Id = baseDto.Id,
                MRN = baseDto.MRN,
                FirstName = baseDto.FirstName,
                LastName = baseDto.LastName,
                DateOfBirth = baseDto.DateOfBirth,
                Gender = baseDto.Gender,
                BloodGroup = baseDto.BloodGroup,
                Email = baseDto.Email,
                PhoneNumber = baseDto.PhoneNumber,
                Address = baseDto.Address,
                City = baseDto.City,
                State = baseDto.State,
                Country = baseDto.Country,
                EmergencyContactName = baseDto.EmergencyContactName,
                EmergencyContactPhone = baseDto.EmergencyContactPhone,
                RegistrationDate = baseDto.RegistrationDate,
                Status = baseDto.Status,
                MedicalHistory = baseDto.MedicalHistory,
                Vitals = baseDto.Vitals,
                MedicalHistories = patient.MedicalHistories.Select(h => new PatientMedicalHistoryDto
                {
                    Id = h.Id,
                    PatientId = h.PatientId,
                    Diagnosis = h.Diagnosis,
                    Symptoms = h.Symptoms,
                    TreatmentReceived = h.TreatmentReceived,
                    RecordDate = h.RecordDate,
                    Remarks = h.Remarks
                }).ToList(),
                Documents = patient.Documents.Select(d => new PatientDocumentDto
                {
                    Id = d.Id,
                    PatientId = d.PatientId,
                    DocumentName = d.DocumentName,
                    DocumentType = d.DocumentType,
                    FilePath = d.FilePath,
                    FileSize = d.FileSize,
                    UploadedAt = d.UploadedAt
                }).ToList(),
                Appointments = patient.Appointments.Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}".Trim(),
                    PatientPhoneNumber = patient.PhoneNumber,
                    PatientEmail = patient.Email,
                    PhysiotherapistId = a.PhysiotherapistId,
                    PhysiotherapistName = a.Physiotherapist != null ? $"{a.Physiotherapist.FirstName} {a.Physiotherapist.LastName}".Trim() : string.Empty,
                    PhysiotherapistEmail = a.Physiotherapist?.Email ?? string.Empty,
                    AppointmentTypeId = a.AppointmentTypeId,
                    AppointmentTypeName = a.AppointmentType != null ? a.AppointmentType.Name : string.Empty,
                    DurationMinutes = a.AppointmentType != null ? a.AppointmentType.DurationMinutes : 30,
                    AppointmentDate = a.AppointmentDate,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime,
                    Status = a.Status,
                    Reason = a.Reason,
                    Notes = a.Notes,
                    CancellationReason = a.CancellationReason,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                }).ToList(),
                Assessments = patient.Assessments.Select(ast => new PatientAssessmentDto
                {
                    Id = ast.Id,
                    PatientId = ast.PatientId,
                    PhysiotherapistId = ast.PhysiotherapistId,
                    AssessmentDate = ast.AssessmentDate,
                    ChiefComplaint = ast.ChiefComplaint,
                    PainLevel = ast.PainLevel,
                    ClinicalNotes = ast.ClinicalNotes,
                    Diagnosis = ast.Diagnosis,
                    CreatedAt = ast.CreatedAt
                }).ToList(),
                TreatmentPlans = patient.TreatmentPlans.Select(tp => new TreatmentPlanDto
                {
                    Id = tp.Id,
                    PatientId = tp.PatientId,
                    PhysiotherapistId = tp.PhysiotherapistId,
                    StartDate = tp.StartDate,
                    ExpectedEndDate = tp.ExpectedEndDate,
                    Goal = tp.Goal,
                    Status = tp.Status,
                    CreatedAt = tp.CreatedAt
                }).ToList(),
                Prescriptions = patient.ExercisePrescriptions.Select(ep => new ExercisePrescriptionDto
                {
                    Id = ep.Id,
                    PatientId = ep.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}".Trim(),
                    PhysiotherapistId = ep.PhysiotherapistId,
                    PrescriptionDate = ep.PrescriptionDate,
                    Instructions = ep.Instructions,
                    Status = ep.Status,
                    CreatedAt = ep.CreatedAt,
                    UpdatedAt = ep.UpdatedAt,
                    PrescriptionDetails = ep.PrescriptionDetails.Select(pd => new ExercisePrescriptionDetailDto
                    {
                        Id = pd.Id,
                        PrescriptionId = pd.PrescriptionId,
                        ExerciseId = pd.ExerciseId,
                        Sets = pd.Sets,
                        Repetitions = pd.Repetitions,
                        HoldSeconds = pd.HoldSeconds,
                        FrequencyPerDay = pd.FrequencyPerDay,
                        DurationWeeks = pd.DurationWeeks,
                        Instructions = pd.Instructions
                    }).ToList()
                }).ToList(),
                Invoices = patient.Invoices.Select(inv => new InvoiceDto
                {
                    Id = inv.Id,
                    InvoiceNumber = inv.InvoiceNumber,
                    PatientId = inv.PatientId,
                    PatientName = $"{patient.FirstName} {patient.LastName}".Trim(),
                    PatientEmail = patient.Email,
                    PatientPhone = patient.PhoneNumber,
                    SubTotal = inv.SubTotal > 0 ? inv.SubTotal : inv.Amount,
                    TaxAmount = inv.TaxAmount,
                    DiscountAmount = inv.DiscountAmount,
                    TotalAmount = inv.TotalAmount > 0 ? inv.TotalAmount : inv.Amount,
                    Amount = inv.Amount,
                    PaidAmount = inv.PaidAmount,
                    BalanceAmount = inv.BalanceAmount,
                    Status = inv.Status,
                    InvoiceDate = inv.InvoiceDate,
                    DueDate = inv.DueDate,
                    Notes = inv.Notes,
                    CreatedAt = inv.CreatedAt
                }).ToList()
            };

            return detailsDto;
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
            return patients.Select(MapToDto);
        }

        public async Task<PatientDto?> GetPatientByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        {
            var patients = await _unitOfWork.Patients.FindAsync(p => p.UserId == userId, cancellationToken);
            var patient = patients.FirstOrDefault();
            return patient == null ? null : MapToDto(patient);
        }

        public async Task<PatientDto?> GetPatientByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;
            var patients = await _unitOfWork.Patients.FindAsync(p => p.Email.ToLower() == email.Trim().ToLower(), cancellationToken);
            var patient = patients.FirstOrDefault();
            return patient == null ? null : MapToDto(patient);
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto, CancellationToken cancellationToken = default)
        {
            var mrn = $"MRN-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100, 999)}";
            var patient = new Patient
            {
                MRN = mrn,
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                DateOfBirth = createPatientDto.DateOfBirth,
                Gender = createPatientDto.Gender,
                BloodGroup = createPatientDto.BloodGroup ?? string.Empty,
                Email = createPatientDto.Email,
                PhoneNumber = createPatientDto.PhoneNumber,
                Address = createPatientDto.Address ?? string.Empty,
                City = createPatientDto.City ?? string.Empty,
                State = createPatientDto.State ?? string.Empty,
                Country = createPatientDto.Country ?? string.Empty,
                EmergencyContactName = createPatientDto.EmergencyContactName ?? string.Empty,
                EmergencyContactPhone = createPatientDto.EmergencyContactPhone ?? string.Empty,
                Status = "Active",
                MedicalHistory = createPatientDto.MedicalHistory,
                // Default active vitals sample for display compatibility
                BloodPressure = "120/80",
                HeartRate = 72,
                WeightKg = 70.0,
                HeightCm = 170.0,
                Temperature = 98.6,
                OxygenSaturation = 98,
                VitalsUpdatedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(patient);
        }

        public async Task<PatientDto> UpdatePatientAsync(UpdatePatientDto updatePatientDto, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(updatePatientDto.Id, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), updatePatientDto.Id);
            }

            patient.FirstName = updatePatientDto.FirstName;
            patient.LastName = updatePatientDto.LastName;
            patient.DateOfBirth = updatePatientDto.DateOfBirth;
            patient.Gender = updatePatientDto.Gender;
            if (!string.IsNullOrWhiteSpace(updatePatientDto.BloodGroup)) patient.BloodGroup = updatePatientDto.BloodGroup;
            patient.Email = updatePatientDto.Email;
            patient.PhoneNumber = updatePatientDto.PhoneNumber;
            if (updatePatientDto.Address != null) patient.Address = updatePatientDto.Address;
            if (updatePatientDto.City != null) patient.City = updatePatientDto.City;
            if (updatePatientDto.State != null) patient.State = updatePatientDto.State;
            if (updatePatientDto.Country != null) patient.Country = updatePatientDto.Country;
            if (updatePatientDto.EmergencyContactName != null) patient.EmergencyContactName = updatePatientDto.EmergencyContactName;
            if (updatePatientDto.EmergencyContactPhone != null) patient.EmergencyContactPhone = updatePatientDto.EmergencyContactPhone;
            if (!string.IsNullOrWhiteSpace(updatePatientDto.Status)) patient.Status = updatePatientDto.Status;
            patient.MedicalHistory = updatePatientDto.MedicalHistory;

            bool vitalsChanged = false;
            if (updatePatientDto.BloodPressure != null) { patient.BloodPressure = updatePatientDto.BloodPressure; vitalsChanged = true; }
            if (updatePatientDto.HeartRate.HasValue) { patient.HeartRate = updatePatientDto.HeartRate; vitalsChanged = true; }
            if (updatePatientDto.WeightKg.HasValue) { patient.WeightKg = updatePatientDto.WeightKg; vitalsChanged = true; }
            if (updatePatientDto.HeightCm.HasValue) { patient.HeightCm = updatePatientDto.HeightCm; vitalsChanged = true; }
            if (updatePatientDto.Temperature.HasValue) { patient.Temperature = updatePatientDto.Temperature; vitalsChanged = true; }
            if (updatePatientDto.OxygenSaturation.HasValue) { patient.OxygenSaturation = updatePatientDto.OxygenSaturation; vitalsChanged = true; }

            if (vitalsChanged)
            {
                patient.VitalsUpdatedAt = DateTime.UtcNow;
            }

            patient.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(patient);
        }

        public async Task DeletePatientAsync(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), id);
            }

            var medicalHistories = await _unitOfWork.PatientMedicalHistories.FindAsync(h => h.PatientId == id, cancellationToken);
            foreach (var h in medicalHistories) _unitOfWork.PatientMedicalHistories.Delete(h);

            var documents = await _unitOfWork.PatientDocuments.FindAsync(d => d.PatientId == id, cancellationToken);
            foreach (var d in documents) _unitOfWork.PatientDocuments.Delete(d);

            var prescriptions = await _unitOfWork.ExercisePrescriptions.Query()
                .Include(ep => ep.PrescriptionDetails)
                .Where(ep => ep.PatientId == id)
                .ToListAsync(cancellationToken);
            foreach (var ep in prescriptions)
            {
                foreach (var detail in ep.PrescriptionDetails.ToList()) _unitOfWork.ExercisePrescriptionDetails.Delete(detail);
                _unitOfWork.ExercisePrescriptions.Delete(ep);
            }

            var sessions = await _unitOfWork.TreatmentSessions.FindAsync(s => s.PatientId == id, cancellationToken);
            foreach (var s in sessions) _unitOfWork.TreatmentSessions.Delete(s);

            var plans = await _unitOfWork.TreatmentPlans.FindAsync(p => p.PatientId == id, cancellationToken);
            foreach (var p in plans) _unitOfWork.TreatmentPlans.Delete(p);

            var assessments = await _unitOfWork.PatientAssessments.FindAsync(a => a.PatientId == id, cancellationToken);
            foreach (var a in assessments) _unitOfWork.PatientAssessments.Delete(a);

            var appointments = await _unitOfWork.Appointments.FindAsync(a => a.PatientId == id, cancellationToken);
            foreach (var a in appointments) _unitOfWork.Appointments.Delete(a);

            var invoices = await _unitOfWork.Invoices.Query()
                .Include(i => i.InvoiceItems)
                .Include(i => i.Payments)
                .Where(i => i.PatientId == id)
                .ToListAsync(cancellationToken);
            foreach (var inv in invoices)
            {
                foreach (var item in inv.InvoiceItems.ToList()) _unitOfWork.InvoiceItems.Delete(item);
                foreach (var payment in inv.Payments.ToList()) _unitOfWork.Payments.Delete(payment);
                _unitOfWork.Invoices.Delete(inv);
            }

            _unitOfWork.Patients.Delete(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<PatientMedicalHistoryDto>> GetMedicalHistoriesAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var histories = await _unitOfWork.PatientMedicalHistories.FindAsync(h => h.PatientId == patientId, cancellationToken);
            return histories.Select(h => new PatientMedicalHistoryDto
            {
                Id = h.Id,
                PatientId = h.PatientId,
                Diagnosis = h.Diagnosis,
                Symptoms = h.Symptoms,
                TreatmentReceived = h.TreatmentReceived,
                RecordDate = h.RecordDate,
                Remarks = h.Remarks
            });
        }

        public async Task<PatientMedicalHistoryDto> AddMedicalHistoryAsync(int patientId, CreatePatientMedicalHistoryDto historyDto, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), patientId);
            }

            var history = new PatientMedicalHistory
            {
                PatientId = patientId,
                Diagnosis = historyDto.Diagnosis,
                Symptoms = historyDto.Symptoms,
                TreatmentReceived = historyDto.TreatmentReceived,
                RecordDate = historyDto.RecordDate,
                Remarks = historyDto.Remarks,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PatientMedicalHistories.AddAsync(history, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PatientMedicalHistoryDto
            {
                Id = history.Id,
                PatientId = history.PatientId,
                Diagnosis = history.Diagnosis,
                Symptoms = history.Symptoms,
                TreatmentReceived = history.TreatmentReceived,
                RecordDate = history.RecordDate,
                Remarks = history.Remarks
            };
        }

        public async Task DeleteMedicalHistoryAsync(int historyId, CancellationToken cancellationToken = default)
        {
            var history = await _unitOfWork.PatientMedicalHistories.GetByIdAsync(historyId, cancellationToken);
            if (history == null)
            {
                throw new NotFoundException(nameof(PatientMedicalHistory), historyId);
            }

            _unitOfWork.PatientMedicalHistories.Delete(history);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<PatientDocumentDto>> GetDocumentsAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var docs = await _unitOfWork.PatientDocuments.FindAsync(d => d.PatientId == patientId, cancellationToken);
            return docs.Select(d => new PatientDocumentDto
            {
                Id = d.Id,
                PatientId = d.PatientId,
                DocumentName = d.DocumentName,
                DocumentType = d.DocumentType,
                FilePath = d.FilePath,
                FileSize = d.FileSize,
                UploadedAt = d.UploadedAt
            });
        }

        public async Task<PatientDocumentDto> AddDocumentAsync(int patientId, CreatePatientDocumentDto documentDto, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), patientId);
            }

            var doc = new PatientDocument
            {
                PatientId = patientId,
                DocumentName = documentDto.DocumentName,
                DocumentType = documentDto.DocumentType,
                FilePath = documentDto.FilePath,
                FileSize = documentDto.FileSize,
                UploadedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PatientDocuments.AddAsync(doc, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new PatientDocumentDto
            {
                Id = doc.Id,
                PatientId = doc.PatientId,
                DocumentName = doc.DocumentName,
                DocumentType = doc.DocumentType,
                FilePath = doc.FilePath,
                FileSize = doc.FileSize,
                UploadedAt = doc.UploadedAt
            };
        }

        public async Task DeleteDocumentAsync(int documentId, CancellationToken cancellationToken = default)
        {
            var doc = await _unitOfWork.PatientDocuments.GetByIdAsync(documentId, cancellationToken);
            if (doc == null)
            {
                throw new NotFoundException(nameof(PatientDocument), documentId);
            }

            _unitOfWork.PatientDocuments.Delete(doc);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static PatientDto MapToDto(Patient patient)
        {
            return new PatientDto
            {
                Id = patient.Id,
                MRN = string.IsNullOrWhiteSpace(patient.MRN) ? $"MRN-{patient.CreatedAt:yyyyMMdd}-{patient.Id:D3}" : patient.MRN,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                BloodGroup = patient.BloodGroup,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                City = patient.City,
                State = patient.State,
                Country = patient.Country,
                EmergencyContactName = patient.EmergencyContactName,
                EmergencyContactPhone = patient.EmergencyContactPhone,
                RegistrationDate = patient.CreatedAt,
                Status = string.IsNullOrWhiteSpace(patient.Status) ? "Active" : patient.Status,
                MedicalHistory = patient.MedicalHistory,
                Vitals = new VitalsDto
                {
                    BloodPressure = patient.BloodPressure ?? "120/80",
                    HeartRate = patient.HeartRate ?? 72,
                    WeightKg = patient.WeightKg ?? 70.0,
                    HeightCm = patient.HeightCm ?? 170.0,
                    Temperature = patient.Temperature ?? 98.6,
                    OxygenSaturation = patient.OxygenSaturation ?? 98,
                    UpdatedAt = patient.VitalsUpdatedAt ?? patient.CreatedAt
                }
            };
        }
    }
}
