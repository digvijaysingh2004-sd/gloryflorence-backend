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
    public class ExercisePrescriptionService : IExercisePrescriptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuditLogService _auditLogService;

        public ExercisePrescriptionService(IUnitOfWork unitOfWork, IAuditLogService auditLogService)
        {
            _unitOfWork = unitOfWork;
            _auditLogService = auditLogService;
        }

        public async Task<PagedResult<ExercisePrescriptionDto>> GetPrescriptionsAsync(ExercisePrescriptionFilterDto filter, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.ExercisePrescriptions.Query();

            if (filter.PatientId.HasValue)
            {
                query = query.Where(p => p.PatientId == filter.PatientId.Value);
            }

            if (filter.PhysiotherapistId.HasValue)
            {
                query = query.Where(p => p.PhysiotherapistId == filter.PhysiotherapistId.Value);
            }

            if (filter.TreatmentPlanId.HasValue)
            {
                query = query.Where(p => p.TreatmentPlanId == filter.TreatmentPlanId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                query = query.Where(p => p.Status.ToLower() == filter.Status.ToLower());
            }

            if (filter.DateFrom.HasValue)
            {
                var fromDate = filter.DateFrom.Value.Date;
                query = query.Where(p => p.PrescriptionDate.Date >= fromDate);
            }

            if (filter.DateTo.HasValue)
            {
                var toDate = filter.DateTo.Value.Date;
                query = query.Where(p => p.PrescriptionDate.Date <= toDate);
            }

            var totalCount = query.Count();
            var safePageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
            var safePageSize = filter.PageSize > 0 ? filter.PageSize : 10;

            query = filter.SortBy?.ToLower() switch
            {
                "status" => filter.SortDescending ? query.OrderByDescending(p => p.Status) : query.OrderBy(p => p.Status),
                "prescriptiondate" => filter.SortDescending ? query.OrderByDescending(p => p.PrescriptionDate) : query.OrderBy(p => p.PrescriptionDate),
                _ => filter.SortDescending ? query.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id) : query.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id)
            };

            var rawPrescriptions = query
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var prescriptionIds = rawPrescriptions.Select(p => p.Id).ToList();
            var allDetails = _unitOfWork.ExercisePrescriptionDetails.Query()
                .Where(d => prescriptionIds.Contains(d.PrescriptionId))
                .ToList();

            var patients = (await _unitOfWork.Patients.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id);
            var users = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id);
            var treatmentPlans = (await _unitOfWork.TreatmentPlans.GetAllAsync(cancellationToken)).ToDictionary(tp => tp.Id);
            var exercises = (await _unitOfWork.Exercises.GetAllAsync(cancellationToken)).ToDictionary(e => e.Id);
            var categories = (await _unitOfWork.Categories.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id);

            var items = rawPrescriptions.Select(p =>
            {
                var patientName = patients.TryGetValue(p.PatientId, out var patient) ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty;
                var physioName = users.TryGetValue(p.PhysiotherapistId, out var user) ? $"{user.FirstName} {user.LastName}".Trim() : string.Empty;
                var planGoal = treatmentPlans.TryGetValue(p.TreatmentPlanId, out var tp) ? tp.Goal : string.Empty;

                var details = allDetails
                    .Where(d => d.PrescriptionId == p.Id)
                    .Select(d =>
                    {
                        var exerciseFound = exercises.TryGetValue(d.ExerciseId, out var ex);
                        var categoryName = exerciseFound && categories.TryGetValue(ex!.CategoryId, out var cat) ? cat.Name : string.Empty;

                        return new ExercisePrescriptionDetailDto
                        {
                            Id = d.Id,
                            PrescriptionId = d.PrescriptionId,
                            ExerciseId = d.ExerciseId,
                            ExerciseName = exerciseFound ? ex!.Name : string.Empty,
                            CategoryId = exerciseFound ? ex!.CategoryId : 0,
                            CategoryName = categoryName,
                            ExerciseDescription = exerciseFound ? ex!.Description : string.Empty,
                            ExerciseInstructions = exerciseFound ? ex!.Instructions : string.Empty,
                            ExerciseVideoUrl = exerciseFound ? ex!.VideoUrl : string.Empty,
                            ExerciseImageUrl = exerciseFound ? ex!.ImageUrl : string.Empty,
                            Sets = d.Sets,
                            Repetitions = d.Repetitions,
                            HoldSeconds = d.HoldSeconds,
                            FrequencyPerDay = d.FrequencyPerDay,
                            DurationWeeks = d.DurationWeeks,
                            Instructions = d.Instructions
                        };
                    }).ToList();

                return new ExercisePrescriptionDto
                {
                    Id = p.Id,
                    PatientId = p.PatientId,
                    PatientName = patientName,
                    PhysiotherapistId = p.PhysiotherapistId,
                    PhysiotherapistName = physioName,
                    TreatmentPlanId = p.TreatmentPlanId,
                    TreatmentPlanGoal = planGoal,
                    PrescriptionDate = p.PrescriptionDate,
                    Instructions = p.Instructions,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    PrescriptionDetails = details
                };
            }).ToList();

            return new PagedResult<ExercisePrescriptionDto>(items, totalCount, safePageNumber, safePageSize);
        }

        public async Task<ExercisePrescriptionDto?> GetPrescriptionByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var prescription = await _unitOfWork.ExercisePrescriptions.GetByIdAsync(id, cancellationToken);
            if (prescription == null) return null;

            var patient = await _unitOfWork.Patients.GetByIdAsync(prescription.PatientId, cancellationToken);
            var physio = await _unitOfWork.Users.GetByIdAsync(prescription.PhysiotherapistId, cancellationToken);
            var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(prescription.TreatmentPlanId, cancellationToken);

            var details = _unitOfWork.ExercisePrescriptionDetails.Query()
                .Where(d => d.PrescriptionId == prescription.Id)
                .ToList();

            var exercises = (await _unitOfWork.Exercises.GetAllAsync(cancellationToken)).ToDictionary(e => e.Id);
            var categories = (await _unitOfWork.Categories.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id);

            var detailDtos = details.Select(d =>
            {
                var exerciseFound = exercises.TryGetValue(d.ExerciseId, out var ex);
                var categoryName = exerciseFound && categories.TryGetValue(ex!.CategoryId, out var cat) ? cat.Name : string.Empty;

                return new ExercisePrescriptionDetailDto
                {
                    Id = d.Id,
                    PrescriptionId = d.PrescriptionId,
                    ExerciseId = d.ExerciseId,
                    ExerciseName = exerciseFound ? ex!.Name : string.Empty,
                    CategoryId = exerciseFound ? ex!.CategoryId : 0,
                    CategoryName = categoryName,
                    ExerciseDescription = exerciseFound ? ex!.Description : string.Empty,
                    ExerciseInstructions = exerciseFound ? ex!.Instructions : string.Empty,
                    ExerciseVideoUrl = exerciseFound ? ex!.VideoUrl : string.Empty,
                    ExerciseImageUrl = exerciseFound ? ex!.ImageUrl : string.Empty,
                    Sets = d.Sets,
                    Repetitions = d.Repetitions,
                    HoldSeconds = d.HoldSeconds,
                    FrequencyPerDay = d.FrequencyPerDay,
                    DurationWeeks = d.DurationWeeks,
                    Instructions = d.Instructions
                };
            }).ToList();

            return new ExercisePrescriptionDto
            {
                Id = prescription.Id,
                PatientId = prescription.PatientId,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty,
                PhysiotherapistId = prescription.PhysiotherapistId,
                PhysiotherapistName = physio != null ? $"{physio.FirstName} {physio.LastName}".Trim() : string.Empty,
                TreatmentPlanId = prescription.TreatmentPlanId,
                TreatmentPlanGoal = plan?.Goal ?? string.Empty,
                PrescriptionDate = prescription.PrescriptionDate,
                Instructions = prescription.Instructions,
                Status = prescription.Status,
                CreatedAt = prescription.CreatedAt,
                UpdatedAt = prescription.UpdatedAt,
                PrescriptionDetails = detailDtos
            };
        }

        public async Task<ExercisePrescriptionDto> CreatePrescriptionAsync(CreateExercisePrescriptionDto dto, CancellationToken cancellationToken = default)
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

            // 3. Validate TreatmentPlan and ensure it belongs to the patient
            var treatmentPlan = await _unitOfWork.TreatmentPlans.GetByIdAsync(dto.TreatmentPlanId, cancellationToken);
            if (treatmentPlan == null)
            {
                throw new NotFoundException($"Treatment plan with ID {dto.TreatmentPlanId} was not found.");
            }

            if (treatmentPlan.PatientId != dto.PatientId)
            {
                throw new BadRequestException($"Treatment plan with ID {dto.TreatmentPlanId} does not belong to patient {dto.PatientId}.");
            }

            // 4. Validate duplicate exercises in payload
            if (dto.PrescriptionDetails != null && dto.PrescriptionDetails.Any())
            {
                var duplicateExerciseId = dto.PrescriptionDetails
                    .GroupBy(d => d.ExerciseId)
                    .FirstOrDefault(g => g.Count() > 1)?.Key;

                if (duplicateExerciseId.HasValue)
                {
                    throw new BadRequestException($"Prescription cannot contain duplicate exercise ID {duplicateExerciseId.Value}.");
                }

                // 5. Validate that all prescribed exercises exist in catalog
                foreach (var detail in dto.PrescriptionDetails)
                {
                    var exercise = await _unitOfWork.Exercises.GetByIdAsync(detail.ExerciseId, cancellationToken);
                    if (exercise == null)
                    {
                        throw new NotFoundException($"Exercise with ID {detail.ExerciseId} was not found in the catalog.");
                    }
                }
            }

            var prescription = new ExercisePrescription
            {
                PatientId = dto.PatientId,
                PhysiotherapistId = dto.PhysiotherapistId,
                TreatmentPlanId = dto.TreatmentPlanId,
                PrescriptionDate = dto.PrescriptionDate,
                Instructions = dto.Instructions ?? string.Empty,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Active" : dto.Status,
                CreatedAt = DateTime.UtcNow
            };

            if (dto.PrescriptionDetails != null)
            {
                foreach (var d in dto.PrescriptionDetails)
                {
                    prescription.PrescriptionDetails.Add(new ExercisePrescriptionDetail
                    {
                        ExerciseId = d.ExerciseId,
                        Sets = d.Sets,
                        Repetitions = d.Repetitions,
                        HoldSeconds = d.HoldSeconds,
                        FrequencyPerDay = d.FrequencyPerDay,
                        DurationWeeks = d.DurationWeeks,
                        Instructions = d.Instructions ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            await _unitOfWork.ExercisePrescriptions.AddAsync(prescription, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Audit log
            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "CREATE_EXERCISE_PRESCRIPTION",
                EntityName = nameof(ExercisePrescription),
                EntityId = prescription.Id.ToString(),
                NewValue = $"Prescription created for Patient ID {prescription.PatientId}, Plan ID {prescription.TreatmentPlanId}, Exercises: {prescription.PrescriptionDetails.Count}, Status: '{prescription.Status}'"
            }, cancellationToken);

            return (await GetPrescriptionByIdAsync(prescription.Id, cancellationToken))!;
        }

        public async Task UpdatePrescriptionAsync(int id, UpdateExercisePrescriptionDto dto, CancellationToken cancellationToken = default)
        {
            var prescription = await _unitOfWork.ExercisePrescriptions.GetByIdAsync(id, cancellationToken);
            if (prescription == null)
            {
                throw new NotFoundException($"Exercise prescription with ID {id} was not found.");
            }

            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException($"Physiotherapist with ID {dto.PhysiotherapistId} was not found.");
            }

            var treatmentPlan = await _unitOfWork.TreatmentPlans.GetByIdAsync(dto.TreatmentPlanId, cancellationToken);
            if (treatmentPlan == null)
            {
                throw new NotFoundException($"Treatment plan with ID {dto.TreatmentPlanId} was not found.");
            }

            if (treatmentPlan.PatientId != prescription.PatientId)
            {
                throw new BadRequestException($"Treatment plan with ID {dto.TreatmentPlanId} does not belong to patient {prescription.PatientId}.");
            }

            var oldSummary = $"Physio: {prescription.PhysiotherapistId}, Plan: {prescription.TreatmentPlanId}, Status: {prescription.Status}";

            prescription.PhysiotherapistId = dto.PhysiotherapistId;
            prescription.TreatmentPlanId = dto.TreatmentPlanId;
            prescription.PrescriptionDate = dto.PrescriptionDate;
            prescription.Instructions = dto.Instructions ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(dto.Status))
            {
                prescription.Status = dto.Status;
            }
            prescription.UpdatedAt = DateTime.UtcNow;

            if (dto.PrescriptionDetails != null)
            {
                var duplicateExerciseId = dto.PrescriptionDetails
                    .GroupBy(d => d.ExerciseId)
                    .FirstOrDefault(g => g.Count() > 1)?.Key;

                if (duplicateExerciseId.HasValue)
                {
                    throw new BadRequestException($"Prescription cannot contain duplicate exercise ID {duplicateExerciseId.Value}.");
                }

                foreach (var detail in dto.PrescriptionDetails)
                {
                    var exercise = await _unitOfWork.Exercises.GetByIdAsync(detail.ExerciseId, cancellationToken);
                    if (exercise == null)
                    {
                        throw new NotFoundException($"Exercise with ID {detail.ExerciseId} was not found in the catalog.");
                    }
                }

                var existingDetails = _unitOfWork.ExercisePrescriptionDetails.Query()
                    .Where(d => d.PrescriptionId == prescription.Id)
                    .ToList();

                foreach (var existing in existingDetails)
                {
                    _unitOfWork.ExercisePrescriptionDetails.Delete(existing);
                }

                foreach (var d in dto.PrescriptionDetails)
                {
                    await _unitOfWork.ExercisePrescriptionDetails.AddAsync(new ExercisePrescriptionDetail
                    {
                        PrescriptionId = prescription.Id,
                        ExerciseId = d.ExerciseId,
                        Sets = d.Sets,
                        Repetitions = d.Repetitions,
                        HoldSeconds = d.HoldSeconds,
                        FrequencyPerDay = d.FrequencyPerDay,
                        DurationWeeks = d.DurationWeeks,
                        Instructions = d.Instructions ?? string.Empty,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var newSummary = $"Physio: {prescription.PhysiotherapistId}, Plan: {prescription.TreatmentPlanId}, Status: {prescription.Status}";

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "UPDATE_EXERCISE_PRESCRIPTION",
                EntityName = nameof(ExercisePrescription),
                EntityId = prescription.Id.ToString(),
                OldValue = oldSummary,
                NewValue = newSummary
            }, cancellationToken);
        }

        public async Task UpdatePrescriptionStatusAsync(int id, UpdateExercisePrescriptionStatusDto dto, CancellationToken cancellationToken = default)
        {
            var prescription = await _unitOfWork.ExercisePrescriptions.GetByIdAsync(id, cancellationToken);
            if (prescription == null)
            {
                throw new NotFoundException($"Exercise prescription with ID {id} was not found.");
            }

            var oldStatus = prescription.Status;
            prescription.Status = dto.Status;
            prescription.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "STATUS_CHANGE_EXERCISE_PRESCRIPTION",
                EntityName = nameof(ExercisePrescription),
                EntityId = prescription.Id.ToString(),
                OldValue = oldStatus,
                NewValue = dto.Status
            }, cancellationToken);
        }

        public async Task DeletePrescriptionAsync(int id, CancellationToken cancellationToken = default)
        {
            var prescription = await _unitOfWork.ExercisePrescriptions.GetByIdAsync(id, cancellationToken);
            if (prescription == null)
            {
                throw new NotFoundException($"Exercise prescription with ID {id} was not found.");
            }

            var details = _unitOfWork.ExercisePrescriptionDetails.Query()
                .Where(d => d.PrescriptionId == prescription.Id)
                .ToList();

            foreach (var detail in details)
            {
                _unitOfWork.ExercisePrescriptionDetails.Delete(detail);
            }

            _unitOfWork.ExercisePrescriptions.Delete(prescription);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "DELETE_EXERCISE_PRESCRIPTION",
                EntityName = nameof(ExercisePrescription),
                EntityId = id.ToString(),
                OldValue = $"Deleted prescription ID {id} for Patient {prescription.PatientId}"
            }, cancellationToken);
        }

        public async Task<ExercisePrescriptionDetailDto> AddExerciseAsync(int prescriptionId, AddExerciseToPrescriptionDto dto, CancellationToken cancellationToken = default)
        {
            var prescription = await _unitOfWork.ExercisePrescriptions.GetByIdAsync(prescriptionId, cancellationToken);
            if (prescription == null)
            {
                throw new NotFoundException($"Exercise prescription with ID {prescriptionId} was not found.");
            }

            var exercise = await _unitOfWork.Exercises.GetByIdAsync(dto.ExerciseId, cancellationToken);
            if (exercise == null)
            {
                throw new NotFoundException($"Exercise with ID {dto.ExerciseId} was not found.");
            }

            var isDuplicate = _unitOfWork.ExercisePrescriptionDetails.Query()
                .Any(d => d.PrescriptionId == prescriptionId && d.ExerciseId == dto.ExerciseId);

            if (isDuplicate)
            {
                throw new BadRequestException($"Exercise with ID {dto.ExerciseId} is already in this prescription.");
            }

            var detail = new ExercisePrescriptionDetail
            {
                PrescriptionId = prescriptionId,
                ExerciseId = dto.ExerciseId,
                Sets = dto.Sets,
                Repetitions = dto.Repetitions,
                HoldSeconds = dto.HoldSeconds,
                FrequencyPerDay = dto.FrequencyPerDay,
                DurationWeeks = dto.DurationWeeks,
                Instructions = dto.Instructions ?? string.Empty,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExercisePrescriptionDetails.AddAsync(detail, cancellationToken);
            prescription.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "ADD_EXERCISE_TO_PRESCRIPTION",
                EntityName = nameof(ExercisePrescriptionDetail),
                EntityId = detail.Id.ToString(),
                NewValue = $"Added Exercise {dto.ExerciseId} ('{exercise.Name}') to Prescription {prescriptionId}"
            }, cancellationToken);

            var category = await _unitOfWork.Categories.GetByIdAsync(exercise.CategoryId, cancellationToken);

            return new ExercisePrescriptionDetailDto
            {
                Id = detail.Id,
                PrescriptionId = detail.PrescriptionId,
                ExerciseId = detail.ExerciseId,
                ExerciseName = exercise.Name,
                CategoryId = exercise.CategoryId,
                CategoryName = category?.Name ?? string.Empty,
                ExerciseDescription = exercise.Description,
                ExerciseInstructions = exercise.Instructions,
                ExerciseVideoUrl = exercise.VideoUrl,
                ExerciseImageUrl = exercise.ImageUrl,
                Sets = detail.Sets,
                Repetitions = detail.Repetitions,
                HoldSeconds = detail.HoldSeconds,
                FrequencyPerDay = detail.FrequencyPerDay,
                DurationWeeks = detail.DurationWeeks,
                Instructions = detail.Instructions
            };
        }

        public async Task RemoveExerciseAsync(int prescriptionId, int detailId, CancellationToken cancellationToken = default)
        {
            var detail = await _unitOfWork.ExercisePrescriptionDetails.GetByIdAsync(detailId, cancellationToken);
            if (detail == null || detail.PrescriptionId != prescriptionId)
            {
                throw new NotFoundException($"Prescription detail item with ID {detailId} was not found in prescription {prescriptionId}.");
            }

            _unitOfWork.ExercisePrescriptionDetails.Delete(detail);

            var prescription = await _unitOfWork.ExercisePrescriptions.GetByIdAsync(prescriptionId, cancellationToken);
            if (prescription != null)
            {
                prescription.UpdatedAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(new CreateAuditLogDto
            {
                Action = "REMOVE_EXERCISE_FROM_PRESCRIPTION",
                EntityName = nameof(ExercisePrescriptionDetail),
                EntityId = detailId.ToString(),
                OldValue = $"Removed Exercise ID {detail.ExerciseId} from Prescription {prescriptionId}"
            }, cancellationToken);
        }

        public async Task<IEnumerable<ExercisePrescriptionDto>> GetPatientPrescriptionHistoryAsync(int patientId, CancellationToken cancellationToken = default)
        {
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException($"Patient with ID {patientId} was not found.");
            }

            var filter = new ExercisePrescriptionFilterDto
            {
                PatientId = patientId,
                PageNumber = 1,
                PageSize = 1000,
                SortBy = "prescriptiondate",
                SortDescending = true
            };

            var paged = await GetPrescriptionsAsync(filter, cancellationToken);
            return paged.Items;
        }

        public async Task<IEnumerable<ExercisePrescriptionDto>> GetPrescriptionsByTreatmentPlanAsync(int treatmentPlanId, CancellationToken cancellationToken = default)
        {
            var plan = await _unitOfWork.TreatmentPlans.GetByIdAsync(treatmentPlanId, cancellationToken);
            if (plan == null)
            {
                throw new NotFoundException($"Treatment plan with ID {treatmentPlanId} was not found.");
            }

            var filter = new ExercisePrescriptionFilterDto
            {
                TreatmentPlanId = treatmentPlanId,
                PageNumber = 1,
                PageSize = 1000,
                SortBy = "prescriptiondate",
                SortDescending = true
            };

            var paged = await GetPrescriptionsAsync(filter, cancellationToken);
            return paged.Items;
        }
    }
}
