using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IExerciseService
    {
        Task<PagedResult<ExerciseDto>> GetExercisesAsync(string? search = null, int? categoryId = null, bool? isActive = null, int pageNumber = 1, int pageSize = 10, CancellationToken cancellationToken = default);
        Task<ExerciseDto?> GetExerciseByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ExerciseDto> CreateExerciseAsync(CreateExerciseDto dto, CancellationToken cancellationToken = default);
        Task UpdateExerciseAsync(UpdateExerciseDto dto, CancellationToken cancellationToken = default);
        Task DeleteExerciseAsync(int id, CancellationToken cancellationToken = default);
    }
}
