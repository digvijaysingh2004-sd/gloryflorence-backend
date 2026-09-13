using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<PagedResult<AppointmentDto>> GetAppointmentsAsync(AppointmentFilterDto filter, CancellationToken cancellationToken = default);
        Task<AppointmentDto?> GetAppointmentByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto, CancellationToken cancellationToken = default);
        Task UpdateAppointmentAsync(UpdateAppointmentDto dto, CancellationToken cancellationToken = default);
        Task RescheduleAppointmentAsync(int id, RescheduleAppointmentDto dto, CancellationToken cancellationToken = default);
        Task CancelAppointmentAsync(int id, CancelAppointmentDto? dto, CancellationToken cancellationToken = default);
        Task DeleteAppointmentAsync(int id, CancellationToken cancellationToken = default);
        Task<bool> HasConflictAsync(int physiotherapistId, DateTime date, TimeSpan startTime, TimeSpan endTime, int? excludeAppointmentId = null, CancellationToken cancellationToken = default);
    }
}
