using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IPatientAssessmentService
    {
        Task<PagedResult<PatientAssessmentDto>> GetAssessmentsAsync(PatientAssessmentFilterDto filter, CancellationToken cancellationToken = default);
        Task<PatientAssessmentDto?> GetAssessmentByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PatientAssessmentDto> CreateAssessmentAsync(CreatePatientAssessmentDto dto, CancellationToken cancellationToken = default);
        Task UpdateAssessmentAsync(int id, UpdatePatientAssessmentDto dto, CancellationToken cancellationToken = default);
        Task DeleteAssessmentAsync(int id, CancellationToken cancellationToken = default);
    }
}
