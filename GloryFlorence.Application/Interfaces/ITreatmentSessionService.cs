using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface ITreatmentSessionService
    {
        Task<PagedResult<TreatmentSessionDto>> GetTreatmentSessionsAsync(TreatmentSessionFilterDto filter, CancellationToken cancellationToken = default);
        Task<TreatmentSessionDto?> GetTreatmentSessionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<TreatmentSessionDto> CreateTreatmentSessionAsync(CreateTreatmentSessionDto dto, CancellationToken cancellationToken = default);
        Task UpdateTreatmentSessionAsync(int id, UpdateTreatmentSessionDto dto, CancellationToken cancellationToken = default);
        Task<TreatmentSessionDto> CompleteTreatmentSessionAsync(int id, CompleteTreatmentSessionDto dto, CancellationToken cancellationToken = default);
        Task DeleteTreatmentSessionAsync(int id, CancellationToken cancellationToken = default);
    }
}
