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
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<bool> HasConflictAsync(
            int physiotherapistId,
            DateTime date,
            TimeSpan startTime,
            TimeSpan endTime,
            int? excludeAppointmentId = null,
            CancellationToken cancellationToken = default)
        {
            var appointmentDate = date.Date;

            var conflict = _unitOfWork.Appointments.Query()
                .Where(a => a.PhysiotherapistId == physiotherapistId)
                .Where(a => a.AppointmentDate.Date == appointmentDate)
                .Where(a => a.Status != "Cancelled")
                .Where(a => !excludeAppointmentId.HasValue || a.Id != excludeAppointmentId.Value)
                .Any(a => startTime < a.EndTime && endTime > a.StartTime);

            return Task.FromResult(conflict);
        }

        public async Task<PagedResult<AppointmentDto>> GetAppointmentsAsync(AppointmentFilterDto filter, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.Appointments.Query();

            if (filter.DateFrom.HasValue)
            {
                var fromDate = filter.DateFrom.Value.Date;
                query = query.Where(a => a.AppointmentDate.Date >= fromDate);
            }

            if (filter.DateTo.HasValue)
            {
                var toDate = filter.DateTo.Value.Date;
                query = query.Where(a => a.AppointmentDate.Date <= toDate);
            }

            if (filter.PatientId.HasValue)
            {
                query = query.Where(a => a.PatientId == filter.PatientId.Value);
            }

            if (filter.PhysiotherapistId.HasValue)
            {
                query = query.Where(a => a.PhysiotherapistId == filter.PhysiotherapistId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.Status))
            {
                var status = filter.Status.Trim();
                query = query.Where(a => a.Status.ToLower() == status.ToLower());
            }

            var totalCount = query.Count();
            var safePageNumber = filter.PageNumber > 0 ? filter.PageNumber : 1;
            var safePageSize = filter.PageSize > 0 ? filter.PageSize : 10;

            var items = query
                .OrderByDescending(a => a.AppointmentDate)
                .ThenBy(a => a.StartTime)
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var patients = (await _unitOfWork.Patients.GetAllAsync(cancellationToken)).ToDictionary(p => p.Id);
            var users = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id);
            var appTypes = (await _unitOfWork.AppointmentTypes.GetAllAsync(cancellationToken)).ToDictionary(t => t.Id);

            var dtos = items.Select(a => MapToDto(
                a,
                patients.TryGetValue(a.PatientId, out var p) ? p : null,
                users.TryGetValue(a.PhysiotherapistId, out var u) ? u : null,
                appTypes.TryGetValue(a.AppointmentTypeId, out var at) ? at : null
            ));

            return new PagedResult<AppointmentDto>(dtos, totalCount, safePageNumber, safePageSize);
        }

        public async Task<AppointmentDto?> GetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
            if (item == null) return null;

            var patient = await _unitOfWork.Patients.GetByIdAsync(item.PatientId, cancellationToken);
            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(item.PhysiotherapistId, cancellationToken);
            var appointmentType = await _unitOfWork.AppointmentTypes.GetByIdAsync(item.AppointmentTypeId, cancellationToken);

            return MapToDto(item, patient, physiotherapist, appointmentType);
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.StartTime >= dto.EndTime)
            {
                throw new InvalidOperationException("Start time must be earlier than end time.");
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), dto.PatientId);
            }

            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException("Physiotherapist (User)", dto.PhysiotherapistId);
            }

            var appointmentType = await _unitOfWork.AppointmentTypes.GetByIdAsync(dto.AppointmentTypeId, cancellationToken);
            if (appointmentType == null)
            {
                throw new NotFoundException(nameof(AppointmentType), dto.AppointmentTypeId);
            }

            var hasConflict = await HasConflictAsync(
                dto.PhysiotherapistId,
                dto.AppointmentDate,
                dto.StartTime,
                dto.EndTime,
                null,
                cancellationToken);

            if (hasConflict)
            {
                throw new InvalidOperationException($"Scheduling conflict detected. Physiotherapist already has an active appointment in the slot {dto.StartTime:hh\\:mm} - {dto.EndTime:hh\\:mm} on {dto.AppointmentDate:yyyy-MM-dd}.");
            }

            var entity = new Appointment
            {
                PatientId = dto.PatientId,
                PhysiotherapistId = dto.PhysiotherapistId,
                AppointmentTypeId = dto.AppointmentTypeId,
                AppointmentDate = dto.AppointmentDate.Date,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = "Scheduled",
                Reason = dto.Reason,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Appointments.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(entity, patient, physiotherapist, appointmentType);
        }

        public async Task UpdateAppointmentAsync(UpdateAppointmentDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.StartTime >= dto.EndTime)
            {
                throw new InvalidOperationException("Start time must be earlier than end time.");
            }

            var entity = await _unitOfWork.Appointments.GetByIdAsync(dto.Id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Appointment), dto.Id);
            }

            var patient = await _unitOfWork.Patients.GetByIdAsync(dto.PatientId, cancellationToken);
            if (patient == null)
            {
                throw new NotFoundException(nameof(Patient), dto.PatientId);
            }

            var physiotherapist = await _unitOfWork.Users.GetByIdAsync(dto.PhysiotherapistId, cancellationToken);
            if (physiotherapist == null)
            {
                throw new NotFoundException("Physiotherapist (User)", dto.PhysiotherapistId);
            }

            var appointmentType = await _unitOfWork.AppointmentTypes.GetByIdAsync(dto.AppointmentTypeId, cancellationToken);
            if (appointmentType == null)
            {
                throw new NotFoundException(nameof(AppointmentType), dto.AppointmentTypeId);
            }

            if (dto.Status != "Cancelled")
            {
                var hasConflict = await HasConflictAsync(
                    dto.PhysiotherapistId,
                    dto.AppointmentDate,
                    dto.StartTime,
                    dto.EndTime,
                    dto.Id,
                    cancellationToken);

                if (hasConflict)
                {
                    throw new InvalidOperationException($"Scheduling conflict detected. Physiotherapist already has an active appointment in the slot {dto.StartTime:hh\\:mm} - {dto.EndTime:hh\\:mm} on {dto.AppointmentDate:yyyy-MM-dd}.");
                }
            }

            entity.PatientId = dto.PatientId;
            entity.PhysiotherapistId = dto.PhysiotherapistId;
            entity.AppointmentTypeId = dto.AppointmentTypeId;
            entity.AppointmentDate = dto.AppointmentDate.Date;
            entity.StartTime = dto.StartTime;
            entity.EndTime = dto.EndTime;
            entity.Status = string.IsNullOrWhiteSpace(dto.Status) ? entity.Status : dto.Status;
            entity.Reason = dto.Reason;
            entity.Notes = dto.Notes;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Appointments.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task RescheduleAppointmentAsync(int id, RescheduleAppointmentDto dto, CancellationToken cancellationToken = default)
        {
            if (dto.NewStartTime >= dto.NewEndTime)
            {
                throw new InvalidOperationException("New start time must be earlier than new end time.");
            }

            var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Appointment), id);
            }

            var hasConflict = await HasConflictAsync(
                entity.PhysiotherapistId,
                dto.NewAppointmentDate,
                dto.NewStartTime,
                dto.NewEndTime,
                entity.Id,
                cancellationToken);

            if (hasConflict)
            {
                throw new InvalidOperationException($"Scheduling conflict detected. Physiotherapist already has an active appointment in the slot {dto.NewStartTime:hh\\:mm} - {dto.NewEndTime:hh\\:mm} on {dto.NewAppointmentDate:yyyy-MM-dd}.");
            }

            entity.AppointmentDate = dto.NewAppointmentDate.Date;
            entity.StartTime = dto.NewStartTime;
            entity.EndTime = dto.NewEndTime;
            entity.Status = "Rescheduled";

            if (!string.IsNullOrWhiteSpace(dto.Reason))
            {
                entity.Notes = string.IsNullOrWhiteSpace(entity.Notes)
                    ? $"Rescheduled: {dto.Reason}"
                    : $"{entity.Notes}\n[Rescheduled: {dto.Reason}]";
            }

            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Appointments.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task CancelAppointmentAsync(int id, CancelAppointmentDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Appointment), id);
            }

            entity.Status = "Cancelled";
            entity.CancellationReason = dto.CancellationReason;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Appointments.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAppointmentAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Appointments.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Appointment), id);
            }

            _unitOfWork.Appointments.Delete(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static AppointmentDto MapToDto(Appointment a, Patient? patient, User? physiotherapist, AppointmentType? appointmentType)
        {
            return new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                PatientName = patient != null ? $"{patient.FirstName} {patient.LastName}".Trim() : string.Empty,
                PatientPhoneNumber = patient?.PhoneNumber ?? string.Empty,
                PatientEmail = patient?.Email ?? string.Empty,
                PhysiotherapistId = a.PhysiotherapistId,
                PhysiotherapistName = physiotherapist != null ? $"{physiotherapist.FirstName} {physiotherapist.LastName}".Trim() : string.Empty,
                PhysiotherapistEmail = physiotherapist?.Email ?? string.Empty,
                AppointmentTypeId = a.AppointmentTypeId,
                AppointmentTypeName = appointmentType?.Name ?? string.Empty,
                DurationMinutes = appointmentType?.DurationMinutes ?? (int)(a.EndTime - a.StartTime).TotalMinutes,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                Status = a.Status,
                Reason = a.Reason,
                Notes = a.Notes,
                CancellationReason = a.CancellationReason,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            };
        }
    }
}
