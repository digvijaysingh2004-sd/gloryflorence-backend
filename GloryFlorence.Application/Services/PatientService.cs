using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;

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
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken);
            if (patient == null)
            {
                return null;
            }

            return MapToDto(patient);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
        {
            var patients = await _unitOfWork.Patients.GetAllAsync(cancellationToken);
            return patients.Select(MapToDto);
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto, CancellationToken cancellationToken = default)
        {
            var patient = new Patient
            {
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                DateOfBirth = createPatientDto.DateOfBirth,
                Gender = createPatientDto.Gender,
                Email = createPatientDto.Email,
                PhoneNumber = createPatientDto.PhoneNumber,
                Address = createPatientDto.Address,
                MedicalHistory = createPatientDto.MedicalHistory,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Patients.AddAsync(patient, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(patient);
        }

        public async Task UpdatePatientAsync(UpdatePatientDto updatePatientDto, CancellationToken cancellationToken = default)
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
            patient.Email = updatePatientDto.Email;
            patient.PhoneNumber = updatePatientDto.PhoneNumber;
            patient.Address = updatePatientDto.Address;
            patient.MedicalHistory = updatePatientDto.MedicalHistory;
            patient.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Patients.Update(patient);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeletePatientAsync(int id, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(id, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), id);
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
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                DateOfBirth = patient.DateOfBirth,
                Gender = patient.Gender,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                Address = patient.Address,
                MedicalHistory = patient.MedicalHistory
            };
        }
    }
}
