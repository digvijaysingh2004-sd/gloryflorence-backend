using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface ITreatmentTypeService
    {
        Task<IEnumerable<TreatmentTypeDto>> GetAllTreatmentTypesAsync(int? categoryId = null, bool? isActive = null, CancellationToken cancellationToken = default);
        Task<TreatmentTypeDto?> GetTreatmentTypeByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TreatmentTypeDto> CreateTreatmentTypeAsync(CreateTreatmentTypeDto dto, CancellationToken cancellationToken = default);
        Task UpdateTreatmentTypeAsync(UpdateTreatmentTypeDto dto, CancellationToken cancellationToken = default);
        Task DeleteTreatmentTypeAsync(int id, CancellationToken cancellationToken = default);
    }
}
