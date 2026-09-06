using System;
using System.Collections.Generic;
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
    public class TreatmentPlanService : ITreatmentPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public TreatmentPlanService(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<PagedResult<TreatmentPlanDto>> GetTreatmentPlansAsync(TreatmentPlanFilterDto filter, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.TreatmentPlans.Query();

            if (filter.PatientId.HasValue)
            {
                query = query.Where(tp => tp.PatientId == filter.PatientId.Value);
            }

            if (filter.PhysiotherapistId.HasValue)
            {
                query = query.Where(tp => tp.PhysiotherapistId == filter.PhysiotherapistId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(tp => tp.Status.ToLower() == filter.Status.ToLower());
            }

            if (filter.DateFrom.HasValue)
            {
                var fromDate = filter.DateFrom.Value.Date;
                query = query.Where(tp => tp.StartDate.Date >= fromDate);
            }

            if (filter.DateTo.HasValue)
            {
                var toDate = filter.DateTo.Value.Date;
                query = query.Where(tp => tp.StartDate.Date <= toDate);
            }

            var totalCount = query.Count();
            var safePageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
            var safePageSize = filter.PageSize > 0 ? filter.PageSize : 10;

            query = filter.SortBy?.ToLower() switch
            {
                "status" => filter.SortDescending ? query.OrderByDescending(tp => tp.Status) : query.OrderBy(tp => tp.Status),
                "startdate" => filter.SortDescending ? query.OrderByDescending(tp => tp.StartDate) : query.OrderBy(tp => tp.StartDate),
                "numberofsessions" => filter.SortDescending ? query.OrderByDescending(tp => tp.NumberOfSessions) : query.OrderBy(tp => tp.NumberOfSessions),
                _ => filter.SortDescending ? query.OrderByDescending(tp => tp.CreatedAt).ThenByDescending(tp => tp.Id) : query.OrderBy(tp => tp.CreatedAt).ThenBy(tp => tp.Id)
            };

            var rawPlans = query
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var planIds = rawPlans.Select(p => p.Id).ToList();
            var allDetails = _unitOfWork.TreatmentPlanDetails.Query()
                .Where(d => planIds.Contains(d.TreatmentPlanId))
                .ToList();

            var patients = (await _unitOfWork.Patients.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id);
            var users = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id);
            var treatmentTypes = (await _unitOfWork.TreatmentTypes.GetAllAsync(cancellationToken)).ToDictionary(t => t.Id);

            var items = rawPlans.Select(plan =>
            {
                var patientName = patients.TryGetValue(plan.PatientId, out var p) ? $"{p.FirstName} {p.LastName}".Trim() : string.Empty;
                var physioName = users.TryGetValue(plan.PhysiotherapistId, out var u) ? $"{u.FirstName} {u.LastName}".Trim() : string.Empty;

                var details = allDetails
                    .Where(d => d.TreatmentPlanId == plan.Id)
                    .Select(d => new TreatmentPlanDetailDto
                    {
                        Id = d.Id,
                        TreatmentPlanId = d.TreatmentPlanId,
                        TreatmentTypeId = d.TreatmentTypeId,
                        TreatmentTypeName = treatmentTypes.TryGetValue(d.TreatmentTypeId, out var tt) ? tt.Name : string.Empty,
                        Frequency = d.Frequency,
                        DurationMinutes = d.DurationMinutes,
                        Instructions = d.Instructions,
                        NumberOfSessions = d.NumberOfSessions
                    }).ToList();

                return new TreatmentPlanDto
                {
                    Id = plan.Id,
                    PatientId = plan.PatientId,
                    PatientName = patientName,
                    PhysiotherapistId = plan.PhysiotherapistId,
                    PhysiotherapistName = physioName,
                    AssessmentId = plan.AssessmentId,
                    StartDate = plan.StartDate,
                    ExpectedEndDate = plan.ExpectedEndDate,
                    NumberOfSessions = plan.NumberOfSessions,
                    Goal = plan.Goal,
                    Notes = plan.Notes,
                    Status = plan.Status,
                    CreatedAt = plan.CreatedAt,
                    UpdatedAt = plan.UpdatedAt,
                    Details = details
                };
            }).ToList();

            return new PagedResult<TreatmentPlanDto>(items, totalCount, safePageNumber, safePageSize);
        }

        public async Task<TreatmentPlanDto?> GetTreatmentPlanByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(id, cancellationToken);
            if (plan == null) return null;

            var patient = await _unitOfWork.Patients.GetByIdAsync(plan.PatientId, cancellationToken);
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(plan.PhysiotherapistId, cancellationToken);

            var details = _unitOfWork.TreatmentPlanDetails.Query()
                .Where(d => d.TreatmentPlanId == plan.Id)
                .ToList();

            var treatmentTypes = (await _unitOfWork.TreatmentTypes.GetAllAsync(cancellationToken)).ToDictionary(t => t.Id);

            return new TreatmentPlanDto
            {
                Id = plan.Id,
                PatientId = plan.PatientId,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty,
                PhysiotherapistId = plan.PhysiotherapistId,
                PhysiotherapistName = physiotherapist != null ? $"{physiotherapist.FirstName} {physiotherapist.LastName}".Trim() : string.Empty,
                AssessmentId = plan.AssessmentId,
                StartDate = plan.StartDate,
                ExpectedEndDate = plan.ExpectedEndDate,
                NumberOfSessions = plan.NumberOfSessions,
                Goal = plan.Goal,
                Notes = plan.Notes,
                Status = plan.Status,
                CreatedAt = plan.CreatedAt,
                UpdatedAt = plan.UpdatedAt,
                Details = details.Select(d => new TreatmentPlanDetailDto
                {
                    Id = d.Id,
                    TreatmentPlanId = d.TreatmentPlanId,
                    TreatmentTypeId = d.TreatmentTypeId,
                    TreatmentTypeName = treatmentTypes.TryGetValue(d.TreatmentTypeId, out var tt) ? tt.Name : string.Empty,
                    Frequency = d.Frequency,
                    DurationMinutes = d.DurationMinutes,
                    Instructions = d.Instructions,
                    NumberOfSessions = d.NumberOfSessions
                }).ToList()
            };
        }

        public async Task<TreatmentPlanDto> CreateTreatmentPlanAsync(CreateTreatmentPlanDto dto, CancellationToken cancellationToken = default)
        {
            // 1. Validate Patient
            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {dto.PatientId} was not found.");
            }

            // 2. Validate Physiotherapist
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException($"Physiotherapist with ID {dto.PhysiotherapistId} was not found.");
            }

            // 3. Validate Assessment & ensure assessment belongs to this patient
            var assessment = await _unitOfWork.PatientAssessments.GetByIdAsync(dto.AssessmentId, cancellationToken);
            if (assessment == null)
            {
                throw new NotFoundException($"Assessment with ID {dto.AssessmentId} was not found.");
            }

            if (assessment.PatientId != dto.PatientId)
            {
                throw new InvalidOperationException("The specified clinical assessment does not belong to this patient.");
            }

            // 4. Validate TreatmentTypes in details
            if (dto.Details != null && dto.Details.Any())
            {
                foreach (var detail in dto.Details)
                {
                    var treatmentType = await _unitOfWork.TreatmentTypes.GetByIdAsync(detail.TreatmentTypeId, cancellationToken);
                    if (treatmentType == null)
                    {
                        throw new NotFoundException($"Treatment type with ID {detail.TreatmentTypeId} was not found.");
                    }
                }
            }

            var plan = new TreatmentPlan
            {
                PatientId = dto.PatientId,
                PhysiotherapistId = dto.PhysiotherapistId,
                AssessmentId = dto.AssessmentId,
                StartDate = dto.StartDate,
                ExpectedEndDate = dto.ExpectedEndDate,
                NumberOfSessions = dto.NumberOfSessions,
                Goal = dto.Goal,
                Notes = dto.Notes,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Draft" : dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.Details != null)
            {
                foreach (var d in dto.Details)
                {
                    plan.TreatmentPlanDetails.Add(new TreatmentPlanDetail
                    {
                        TreatmentTypeId = d.TreatmentTypeId,
                        Frequency = d.Frequency,
                        DurationMinutes = d.DurationMinutes,
                        Instructions = d.Instructions,
                        NumberOfSessions = d.NumberOfSessions,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _unitOfWork.TreatmentPlans.AddAsync(plan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "CREATE_TREATMENT_PLAN",
                EntityName = nameof(TreatmentPlan),
                EntityId = plan.Id.ToString(),
                NewValue = $"Plan created for Patient ID {plan.PatientId}, Sessions: {plan.NumberOfSessions}, Status: '{plan.Status}'"
            }, cancellationToken);

            return (await GetTreatmentPlanByIdAsync(plan.Id, cancellationToken))!;
        }

        public async Task UpdateTreatmentPlanAsync(int id, UpdateTreatmentPlanDto dto, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(id, cancellationToken);
            if (plan == null)
            {
                throw new NotFoundException($"Treatment plan with ID {id} was not found.");
            }

            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException($"Physiotherapist with ID {dto.PhysiotherapistId} was not found.");
            }

            var oldStatus = plan.Status;
            plan.PhysiotherapistId = dto.PhysiotherapistId;
            plan.StartDate = dto.StartDate;
            plan.ExpectedEndDate = dto.ExpectedEndDate;
            plan.NumberOfSessions = dto.NumberOfSessions;
            plan.Goal = dto.Goal;
            plan.Notes = dto.Notes;
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                plan.Status = dto.Status;
            }
            plan.UpdatedAt = DateTime.UtcNow;

            if (dto.Details != null)
            {
                foreach (var detail in dto.Details)
                {
                    var treatmentType = await _unitOfWork.TreatmentTypes.GetByIdAsync(detail.TreatmentTypeId, cancellationToken);
                    if (treatmentType == null)
                    {
                        throw new NotFoundException($"Treatment type with ID {detail.TreatmentTypeId} was not found.");
                    }
                }

                var existingDetails = _unitOfWork.TreatmentPlanDetails.Query()
                    .Where(d => d.TreatmentPlanId == plan.Id)
                    .ToList();

                foreach (var existing in existingDetails)
                {
                    _unitOfWork.TreatmentPlanDetails.Delete(existing);
                }

                foreach (var d in dto.Details)
                {
                    await _unitOfWork.TreatmentPlanDetails.AddAsync(new TreatmentPlanDetail
                    {
                        TreatmentPlanId = plan.Id,
                        TreatmentTypeId = d.TreatmentTypeId,
                        Frequency = d.Frequency,
                        DurationMinutes = d.DurationMinutes,
                        Instructions = d.Instructions,
                        NumberOfSessions = d.NumberOfSessions,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }

            _unitOfWork.TreatmentPlans.Update(plan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "UPDATE_TREATMENT_PLAN",
                EntityName = nameof(TreatmentPlan),
                EntityId = plan.Id.ToString(),
                OldValue = $"Status: {oldStatus}",
                NewValue = $"Status: {plan.Status}, Sessions: {plan.NumberOfSessions}"
            }, cancellationToken);
        }

        public async Task UpdateTreatmentPlanStatusAsync(int id, UpdateTreatmentPlanStatusDto dto, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(id, cancellationToken);
            if (plan == null)
            {
                throw new NotFoundException($"Treatment plan with ID {id} was not found.");
            }

            var oldStatus = plan.Status;
            plan.Status = dto.Status;
            plan.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TreatmentPlans.Update(plan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "STATUS_CHANGE_TREATMENT_PLAN",
                EntityName = nameof(TreatmentPlan),
                EntityId = plan.Id.ToString(),
                OldValue = oldStatus,
                NewValue = plan.Status
            }, cancellationToken);
        }

        public async Task DeleteTreatmentPlanAsync(int id, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(id, cancellationToken);
            if (plan == null)
            {
                throw new NotFoundException($"Treatment plan with ID {id} was not found.");
            }

            var existingDetails = _unitOfWork.TreatmentPlanDetails.Query()
                .Where(d => d.TreatmentPlanId == plan.Id)
                .ToList();

            foreach (var detail in existingDetails)
            {
                _unitOfWork.TreatmentPlanDetails.Delete(detail);
            }

            _unitOfWork.TreatmentPlans.Delete(plan);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "DELETE_TREATMENT_PLAN",
                EntityName = nameof(TreatmentPlan),
                EntityId = id.ToString(),
                OldValue = $"Deleted Treatment Plan for Patient ID {plan.PatientId}"
            }, cancellationToken);
        }
    }
}
