using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IAppointmentTypeService
    {
        Task<IEnumerable<AppointmentTypeDto>> GetAllAppointmentTypesAsync(bool? isActive = null, CancellationToken cancellationToken = default);
        Task<AppointmentTypeDto?> GetAppointmentTypeByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<AppointmentTypeDto> CreateAppointmentTypeAsync(CreateAppointmentTypeDto dto, CancellationToken cancellationToken = default);
        Task UpdateAppointmentTypeAsync(UpdateAppointmentTypeDto dto, CancellationToken cancellationToken = default);
        Task DeleteAppointmentTypeAsync(int id, CancellationToken cancellationToken = default);
    }
}
