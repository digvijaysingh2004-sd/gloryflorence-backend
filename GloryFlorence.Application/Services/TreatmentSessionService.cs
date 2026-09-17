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
    public class TreatmentSessionService : ITreatmentSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public TreatmentSessionService(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<PagedResult<TreatmentSessionDto>> GetTreatmentSessionsAsync(TreatmentSessionFilterDto filter, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.TreatmentSessions.Query();

            if (filter.PatientId.HasValue)
            {
                query = query.Where(ts => ts.PatientId == filter.PatientId.Value);
            }

            if (filter.AppointmentId.HasValue)
            {
                query = query.Where(ts => ts.AppointmentId == filter.AppointmentId.Value);
            }

            if (filter.PhysiotherapistId.HasValue)
            {
                query = query.Where(ts => ts.PhysiotherapistId == filter.PhysiotherapistId.Value);
            }

            if (filter.TreatmentPlanId.HasValue)
            {
                query = query.Where(ts => ts.TreatmentPlanId == filter.TreatmentPlanId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(ts => ts.Status.ToLower() == filter.Status.ToLower());
            }

            if (filter.DateFrom.HasValue)
            {
                var fromDate = filter.DateFrom.Value.Date;
                query = query.Where(ts => ts.SessionDate.Date >= fromDate);
            }

            if (filter.DateTo.HasValue)
            {
                var toDate = filter.DateTo.Value.Date;
                query = query.Where(ts => ts.SessionDate.Date <= toDate);
            }

            var totalCount = query.Count();
            var safePageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
            var safePageSize = filter.PageSize > 0 ? filter.PageSize : 10;

            query = filter.SortBy?.ToLower() switch
            {
                "status" => filter.SortDescending ? query.OrderByDescending(ts => ts.Status) : query.OrderBy(ts => ts.Status),
                "sessiondate" => filter.SortDescending ? query.OrderByDescending(ts => ts.SessionDate).ThenByDescending(ts => ts.StartTime) : query.OrderBy(ts => ts.SessionDate).ThenBy(ts => ts.StartTime),
                _ => filter.SortDescending ? query.OrderByDescending(ts => ts.SessionDate).ThenByDescending(ts => ts.Id) : query.OrderBy(ts => ts.SessionDate).ThenBy(ts => ts.Id)
            };

            var rawSessions = query
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var patients = (await _unitOfWork.Patients.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id);
            var users = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id);

            var items = rawSessions.Select(session =>
            {
                var patientName = patients.TryGetValue(session.PatientId, out var p) ? $"{p.FirstName} {p.LastName}".Trim() : string.Empty;
                var physioName = users.TryGetValue(session.PhysiotherapistId, out var u) ? $"{u.FirstName} {u.LastName}".Trim() : string.Empty;

                return new TreatmentSessionDto
                {
                    Id = session.Id,
                    AppointmentId = session.AppointmentId,
                    PatientId = session.PatientId,
                    PatientName = patientName,
                    PhysiotherapistId = session.PhysiotherapistId,
                    PhysiotherapistName = physioName,
                    TreatmentPlanId = session.TreatmentPlanId,
                    SessionDate = session.SessionDate,
                    StartTime = session.StartTime,
                    EndTime = session.EndTime,
                    PainLevelBefore = session.PainLevelBefore,
                    PainLevelAfter = session.PainLevelAfter,
                    Status = session.Status,
                    Assessment = session.Assessment,
                    TreatmentPerformed = session.TreatmentPerformed,
                    Recommendations = session.Recommendations,
                    Notes = session.Notes,
                    ModalitiesConducted = session.ModalitiesConducted,
                    PatientTolerance = session.PatientTolerance,
                    NextSessionPlan = session.NextSessionPlan,
                    CreatedAt = session.CreatedAt,
                    UpdatedAt = session.UpdatedAt
                };
            }).ToList();

            return new PagedResult<TreatmentSessionDto>(items, totalCount, safePageNumber, safePageSize);
        }

        public async Task<TreatmentSessionDto?> GetTreatmentSessionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TreatmentSessions.GetByIdAsync(id, cancellationToken);
            if (session == null) return null;

            var patient = await _unitOfWork.Patients.GetByIdAsync(session.PatientId, cancellationToken);
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(session.PhysiotherapistId, cancellationToken);

            return new TreatmentSessionDto
            {
                Id = session.Id,
                AppointmentId = session.AppointmentId,
                PatientId = session.PatientId,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty,
                PhysiotherapistId = session.PhysiotherapistId,
                PhysiotherapistName = physiotherapist != null ? $"{physiotherapist.FirstName} {physiotherapist.LastName}".Trim() : string.Empty,
                TreatmentPlanId = session.TreatmentPlanId,
                SessionDate = session.SessionDate,
                StartTime = session.StartTime,
                EndTime = session.EndTime,
                PainLevelBefore = session.PainLevelBefore,
                PainLevelAfter = session.PainLevelAfter,
                Status = session.Status,
                Assessment = session.Assessment,
                TreatmentPerformed = session.TreatmentPerformed,
                Recommendations = session.Recommendations,
                Notes = session.Notes,
                ModalitiesConducted = session.ModalitiesConducted,
                PatientTolerance = session.PatientTolerance,
                NextSessionPlan = session.NextSessionPlan,
                CreatedAt = session.CreatedAt,
                UpdatedAt = session.UpdatedAt
            };
        }

        public async Task<TreatmentSessionDto> CreateTreatmentSessionAsync(CreateTreatmentSessionDto dto, CancellationToken cancellationToken = default)
        {
            // 1. Validate Appointment exists
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(dto.AppointmentId, cancellationToken);
            if (appointment == null)
            {
                throw new NotFoundException($"Appointment with ID {dto.AppointmentId} was not found.");
            }

            // 2. Validate Patient matches Appointment's patient
            if (appointment.PatientId != dto.PatientId)
            {
                throw new InvalidOperationException("A treatment session must not be created for a different patient than its appointment.");
            }

            // 3. Validate Patient exists
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {dto.PatientId} was not found.");
            }

            // 4. Validate Physiotherapist exists
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException($"Physiotherapist with ID {dto.PhysiotherapistId} was not found.");
            }

            // 5. If TreatmentPlanId is provided, validate it belongs to the same patient
            if (dto.TreatmentPlanId.HasValue)
            {
                var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(dto.TreatmentPlanId.Value, cancellationToken);
                if (plan == null)
                {
                    throw new NotFoundException($"Treatment plan with ID {dto.TreatmentPlanId.Value} was not found.");
                }

                if (plan.PatientId != dto.PatientId)
                {
                    throw new InvalidOperationException("The specified treatment plan does not belong to this patient.");
                }
            }

            var session = new TreatmentSession
            {
                AppointmentId = dto.AppointmentId,
                PatientId = dto.PatientId,
                PhysiotherapistId = dto.PhysiotherapistId,
                TreatmentPlanId = dto.TreatmentPlanId,
                SessionDate = dto.SessionDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                PainLevelBefore = dto.PainLevelBefore,
                PainLevelAfter = dto.PainLevelAfter,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Scheduled" : dto.Status,
                Assessment = dto.Assessment,
                TreatmentPerformed = dto.TreatmentPerformed,
                Recommendations = dto.Recommendations,
                Notes = dto.Notes,
                ModalitiesConducted = dto.ModalitiesConducted,
                PatientTolerance = dto.PatientTolerance,
                NextSessionPlan = dto.NextSessionPlan,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TreatmentSessions.AddAsync(session, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "CREATE_TREATMENT_SESSION",
                EntityName = nameof(TreatmentSession),
                EntityId = session.Id.ToString(),
                NewValue = $"Session created for Patient ID {session.PatientId}, Appointment ID {session.AppointmentId}, Status: '{session.Status}'"
            }, cancellationToken);

            return (await GetTreatmentSessionByIdAsync(session.Id, cancellationToken))!;
        }

        public async Task<TreatmentSessionDto> UpdateTreatmentSessionAsync(int id, UpdateTreatmentSessionDto dto, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TreatmentSessions.GetByIdAsync(id, cancellationToken);
            if (session == null)
            {
                throw new NotFoundException($"Treatment session with ID {id} was not found.");
            }

            if (dto.TreatmentPlanId.HasValue)
            {
                var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(dto.TreatmentPlanId.Value, cancellationToken);
                if (plan == null)
                {
                    throw new NotFoundException($"Treatment plan with ID {dto.TreatmentPlanId.Value} was not found.");
                }

                if (plan.PatientId != session.PatientId)
                {
                    throw new InvalidOperationException("The specified treatment plan does not belong to this patient.");
                }
            }

            var oldStatus = session.Status;
            session.TreatmentPlanId = dto.TreatmentPlanId;
            session.SessionDate = dto.SessionDate;
            session.StartTime = dto.StartTime;
            session.EndTime = dto.EndTime;
            session.PainLevelBefore = dto.PainLevelBefore;
            session.PainLevelAfter = dto.PainLevelAfter;
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                session.Status = dto.Status;
            }
            session.Assessment = dto.Assessment;
            session.TreatmentPerformed = dto.TreatmentPerformed;
            session.Recommendations = dto.Recommendations;
            session.Notes = dto.Notes;
            if (dto.ModalitiesConducted != null) session.ModalitiesConducted = dto.ModalitiesConducted;
            if (dto.PatientTolerance != null) session.PatientTolerance = dto.PatientTolerance;
            if (dto.NextSessionPlan != null) session.NextSessionPlan = dto.NextSessionPlan;
            session.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TreatmentSessions.Update(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "UPDATE_TREATMENT_SESSION",
                EntityName = nameof(TreatmentSession),
                EntityId = session.Id.ToString(),
                OldValue = $"Status: {oldStatus}",
                NewValue = $"Status: {session.Status}, PainAfter: {session.PainLevelAfter}"
            }, cancellationToken);

            return (await GetTreatmentSessionByIdAsync(session.Id, cancellationToken))!;
        }

        public async Task<TreatmentSessionDto> CompleteTreatmentSessionAsync(int id, CompleteTreatmentSessionDto? dto, CancellationToken cancellationToken = default)
        {
            dto ??= new CompleteTreatmentSessionDto();
            var session = await _unitOfWork.TreatmentSessions.GetByIdAsync(id, cancellationToken);
            if (session == null)
            {
                throw new NotFoundException($"Treatment session with ID {id} was not found.");
            }

            session.Status = "Completed";
            if (dto.PainLevelAfter.HasValue)
            {
                session.PainLevelAfter = dto.PainLevelAfter.Value;
            }
            if (!string.IsNullOrWhiteSpace(dto.TreatmentPerformed))
            {
                session.TreatmentPerformed = dto.TreatmentPerformed;
            }
            if (!string.IsNullOrWhiteSpace(dto.Recommendations))
            {
                session.Recommendations = dto.Recommendations;
            }
            if (!string.IsNullOrWhiteSpace(dto.Notes))
            {
                session.Notes = dto.Notes;
            }
            session.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TreatmentSessions.Update(session);

            // Also update the linked appointment status to Completed
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(session.AppointmentId, cancellationToken);
            if (appointment != null && appointment.Status != "Completed")
            {
                appointment.Status = "Completed";
                appointment.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Appointments.Update(appointment);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "COMPLETE_TREATMENT_SESSION",
                EntityName = nameof(TreatmentSession),
                EntityId = session.Id.ToString(),
                NewValue = $"Session {session.Id} completed. PainBefore: {session.PainLevelBefore}, PainAfter: {session.PainLevelAfter}"
            }, cancellationToken);

            return (await GetTreatmentSessionByIdAsync(session.Id, cancellationToken))!;
        }

        public async Task DeleteTreatmentSessionAsync(int id, CancellationToken cancellationToken = default)
        {
            var session = await _unitOfWork.TreatmentSessions.GetByIdAsync(id, cancellationToken);
            if (session == null)
            {
                throw new NotFoundException($"Treatment session with ID {id} was not found.");
            }

            _unitOfWork.TreatmentSessions.Delete(session);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "DELETE_TREATMENT_SESSION",
                EntityName = nameof(TreatmentSession),
                EntityId = id.ToString(),
                OldValue = $"Deleted session ID {id} for Patient ID {session.PatientId}"
            }, cancellationToken);
        }
    }
}
