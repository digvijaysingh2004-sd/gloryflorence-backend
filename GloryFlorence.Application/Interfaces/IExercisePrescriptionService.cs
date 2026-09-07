using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IExercisePrescriptionService
    {
        Task<PagedResult<ExercisePrescriptionDto>> GetPrescriptionsAsync(ExercisePrescriptionFilterDto filter, CancellationToken cancellationToken = default);
        Task<ExercisePrescriptionDto?> GetPrescriptionByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<ExercisePrescriptionDto> CreatePrescriptionAsync(CreateExercisePrescriptionDto dto, CancellationToken cancellationToken = default);
        Task UpdatePrescriptionAsync(int id, UpdateExercisePrescriptionDto dto, CancellationToken cancellationToken = default);
        Task UpdatePrescriptionStatusAsync(int id, UpdateExercisePrescriptionStatusDto dto, CancellationToken cancellationToken = default);
        Task DeletePrescriptionAsync(int id, CancellationToken cancellationToken = default);
        Task<ExercisePrescriptionDetailDto> AddExerciseAsync(int prescriptionId, AddExerciseToPrescriptionDto dto, CancellationToken cancellationToken = default);
        Task RemoveExerciseAsync(int prescriptionId, int detailId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExercisePrescriptionDto>> GetPatientPrescriptionHistoryAsync(int patientId, CancellationToken cancellationToken = default);
        Task<IEnumerable<ExercisePrescriptionDto>> GetPrescriptionsByTreatmentPlanAsync(int treatmentPlanId, CancellationToken cancellationToken = default);
    }
}
