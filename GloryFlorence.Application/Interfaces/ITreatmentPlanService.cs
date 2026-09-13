using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface ITreatmentPlanService
    {
        Task<PagedResult<TreatmentPlanDto>> GetTreatmentPlansAsync(TreatmentPlanFilterDto filter, CancellationToken cancellationToken = default);
        Task<TreatmentPlanDto?> GetTreatmentPlanByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TreatmentPlanDto> CreateTreatmentPlanAsync(CreateTreatmentPlanDto dto, CancellationToken cancellationToken = default);
        Task<TreatmentPlanDto> UpdateTreatmentPlanAsync(int id, UpdateTreatmentPlanDto dto, CancellationToken cancellationToken = default);
        Task UpdateTreatmentPlanStatusAsync(int id, UpdateTreatmentPlanStatusDto dto, CancellationToken cancellationToken = default);
        Task DeleteTreatmentPlanAsync(int id, CancellationToken cancellationToken = default);
    }
}
