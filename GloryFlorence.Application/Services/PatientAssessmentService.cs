using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;

namespace GloryFlorence.Application.Services
{
    public class PatientAssessmentService : IPatientAssessmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public PatientAssessmentService(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<PagedResult<PatientAssessmentDto>> GetAssessmentsAsync(PatientAssessmentFilterDto filter, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.PatientAssessments.Query();

            if (filter.PatientId.HasValue)
            {
                query = query.Where(a => a.PatientId == filter.PatientId.Value);
            }

            if (filter.PhysiotherapistId.HasValue)
            {
                query = query.Where(a => a.PhysiotherapistId == filter.PhysiotherapistId.Value);
            }

            if (filter.DateFrom.HasValue)
            {
                var fromDate = filter.DateFrom.Value.Date;
                query = query.Where(a => a.AssessmentDate.Date >= fromDate);
            }

            if (filter.DateTo.HasValue)
            {
                var toDate = filter.DateTo.Value.Date;
                query = query.Where(a => a.AssessmentDate.Date <= toDate);
            }

            var totalCount = query.Count();
            var safePageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
            var safePageSize = filter.PageSize > 0 ? filter.PageSize : 10;

            query = filter.SortBy?.ToLower() switch
            {
                "painlevel" => filter.SortDescending ? query.OrderByDescending(a => a.PainLevel) : query.OrderBy(a => a.PainLevel),
                "assessmentdate" => filter.SortDescending ? query.OrderByDescending(a => a.AssessmentDate) : query.OrderBy(a => a.AssessmentDate),
                _ => filter.SortDescending ? query.OrderByDescending(a => a.AssessmentDate).ThenByDescending(a => a.Id) : query.OrderBy(a => a.AssessmentDate).ThenBy(a => a.Id)
            };

            var rawItems = query
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var patients = (await _unitOfWork.Patients.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id);
            var users = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id);

            var items = rawItems.Select(a =>
            {
                var patientName = patients.TryGetValue(a.PatientId, out var p) ? $"{p.FirstName} {p.LastName}".Trim() : string.Empty;
                var physioName = users.TryGetValue(a.PhysiotherapistId, out var u) ? $"{u.FirstName} {u.LastName}".Trim() : string.Empty;

                return new PatientAssessmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    PatientName = patientName,
                    SessionId = a.SessionId,
                    PhysiotherapistId = a.PhysiotherapistId,
                    PhysiotherapistName = physioName,
                    AssessmentDate = a.AssessmentDate,
                    ChiefComplaint = a.ChiefComplaint,
                    CurrentCondition = a.CurrentCondition,
                    PainLevel = a.PainLevel,
                    Diagnosis = a.Diagnosis,
                    ClinicalNotes = a.ClinicalNotes,
                    Recommendations = a.Recommendations,
                    CreatedAt = a.CreatedAt,
                    UpdatedAt = a.UpdatedAt
                };
            }).ToList();

            return new PagedResult<PatientAssessmentDto>(items, totalCount, safePageNumber, safePageSize);
        }

        public async Task<PatientAssessmentDto?> GetAssessmentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var assessment = await _unitOfWork.PatientAssessments.GetByIdAsync(id, cancellationToken);
            if (assessment == null)
            {
                return null;
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(assessment.PatientId, cancellationToken);
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(assessment.PhysiotherapistId, cancellationToken);

            return new PatientAssessmentDto
            {
                Id = assessment.Id,
                PatientId = assessment.PatientId,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty,
                SessionId = assessment.SessionId,
                PhysiotherapistId = assessment.PhysiotherapistId,
                PhysiotherapistName = physiotherapist != null ? $"{physiotherapist.FirstName} {physiotherapist.LastName}".Trim() : string.Empty,
                AssessmentDate = assessment.AssessmentDate,
                ChiefComplaint = assessment.ChiefComplaint,
                CurrentCondition = assessment.CurrentCondition,
                PainLevel = assessment.PainLevel,
                Diagnosis = assessment.Diagnosis,
                ClinicalNotes = assessment.ClinicalNotes,
                Recommendations = assessment.Recommendations,
                CreatedAt = assessment.CreatedAt,
                UpdatedAt = assessment.UpdatedAt
            };
        }

        public async Task<PatientAssessmentDto> CreateAssessmentAsync(CreatePatientAssessmentDto dto, CancellationToken cancellationToken = default)
        {
            // 1. Validate Patient exists
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {dto.PatientId} was not found.");
            }

            // 2. Validate Physiotherapist exists
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException($"Physiotherapist with ID {dto.PhysiotherapistId} was not found.");
            }

            // 3. If SessionId provided, validate session belongs to patient
            if (dto.SessionId.HasValue)
            {
                var session = await _unitOfWork.TreatmentSessions.GetByIdAsync(dto.SessionId.Value, cancellationToken);
                if (session == null)
                {
                    throw new NotFoundException($"Treatment session with ID {dto.SessionId.Value} was not found.");
                }

                if (session.PatientId != dto.PatientId)
                {
                    throw new InvalidOperationException("The specified treatment session does not belong to this patient.");
                }
            }

            var assessment = new PatientAssessment
            {
                PatientId = dto.PatientId,
                SessionId = dto.SessionId,
                PhysiotherapistId = dto.PhysiotherapistId,
                AssessmentDate = dto.AssessmentDate,
                ChiefComplaint = dto.ChiefComplaint,
                CurrentCondition = dto.CurrentCondition,
                PainLevel = dto.PainLevel,
                Diagnosis = dto.Diagnosis,
                ClinicalNotes = dto.ClinicalNotes,
                Recommendations = dto.Recommendations,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PatientAssessments.AddAsync(assessment, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "CREATE_ASSESSMENT",
                EntityName = nameof(PatientAssessment),
                EntityId = assessment.Id.ToString(),
                NewValue = $"Assessment for Patient ID {assessment.PatientId}: Diagnosis: '{assessment.Diagnosis}', PainLevel: {assessment.PainLevel}"
            }, cancellationToken);

            return new PatientAssessmentDto
            {
                Id = assessment.Id,
                PatientId = assessment.PatientId,
                PatientName = $"{patient.FirstName} {patient.LastName}".Trim(),
                SessionId = assessment.SessionId,
                PhysiotherapistId = assessment.PhysiotherapistId,
                PhysiotherapistName = $"{physiotherapist.FirstName} {physiotherapist.LastName}".Trim(),
                AssessmentDate = assessment.AssessmentDate,
                ChiefComplaint = assessment.ChiefComplaint,
                CurrentCondition = assessment.CurrentCondition,
                PainLevel = assessment.PainLevel,
                Diagnosis = assessment.Diagnosis,
                ClinicalNotes = assessment.ClinicalNotes,
                Recommendations = assessment.Recommendations,
                CreatedAt = assessment.CreatedAt
            };
        }

        public async Task UpdateAssessmentAsync(int id, UpdatePatientAssessmentDto dto, CancellationToken cancellationToken = default)
        {
            var assessment = await _unitOfWork.PatientAssessments.GetByIdAsync(id, cancellationToken);
            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {id} was not found.");
            }

            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException($"Physiotherapist with ID {dto.PhysiotherapistId} was not found.");
            }

            if (dto.SessionId.HasValue)
            {
                var session = await _unitOfWork.TreatmentSessions.GetByIdAsync(dto.SessionId.Value, cancellationToken);
                if (session == null)
                {
                    throw new NotFoundException($"Treatment session with ID {dto.SessionId.Value} was not found.");
                }

                if (session.PatientId != assessment.PatientId)
                {
                    throw new InvalidOperationException("The specified treatment session does not belong to this patient.");
                }
            }

            var oldValue = $"PainLevel: {assessment.PainLevel}, Diagnosis: '{assessment.Diagnosis}', Recommendations: '{assessment.Recommendations}'";

            assessment.SessionId = dto.SessionId;
            assessment.PhysiotherapistId = dto.PhysiotherapistId;
            assessment.AssessmentDate = dto.AssessmentDate;
            assessment.ChiefComplaint = dto.ChiefComplaint;
            assessment.CurrentCondition = dto.CurrentCondition;
            assessment.PainLevel = dto.PainLevel;
            assessment.Diagnosis = dto.Diagnosis;
            assessment.ClinicalNotes = dto.ClinicalNotes;
            assessment.Recommendations = dto.Recommendations;
            assessment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.PatientAssessments.Update(assessment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var newValue = $"PainLevel: {assessment.PainLevel}, Diagnosis: '{assessment.Diagnosis}', Recommendations: '{assessment.Recommendations}'";

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "UPDATE_ASSESSMENT",
                EntityName = nameof(PatientAssessment),
                EntityId = assessment.Id.ToString(),
                OldValue = oldValue,
                NewValue = newValue
            }, cancellationToken);
        }

        public async Task DeleteAssessmentAsync(int id, CancellationToken cancellationToken = default)
        {
            var assessment = await _unitOfWork.PatientAssessments.GetByIdAsync(id, cancellationToken);
            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {id} was not found.");
            }

            _unitOfWork.PatientAssessments.Delete(assessment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "DELETE_ASSESSMENT",
                EntityName = nameof(PatientAssessment),
                EntityId = id.ToString(),
                OldValue = $"Deleted assessment for Patient ID {assessment.PatientId}"
            }, cancellationToken);
        }
    }
}
