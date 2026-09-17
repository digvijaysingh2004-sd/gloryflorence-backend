using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IPatientService
    {
        Task<PatientDto?> GetPatientByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<PatientDto?> GetPatientByUserIdAsync(int userId, CancellationToken cancellationToken = default);
        Task<PatientDto?> GetPatientByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
        Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto, CancellationToken cancellationToken = default);
        Task<PatientDto> UpdatePatientAsync(UpdatePatientDto updatePatientDto, CancellationToken cancellationToken = default);
        Task DeletePatientAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientMedicalHistoryDto>> GetMedicalHistoriesAsync(int patientId, CancellationToken cancellationToken = default);
        Task<PatientMedicalHistoryDto> AddMedicalHistoryAsync(int patientId, CreatePatientMedicalHistoryDto historyDto, CancellationToken cancellationToken = default);
        Task DeleteMedicalHistoryAsync(int historyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientDocumentDto>> GetDocumentsAsync(int patientId, CancellationToken cancellationToken = default);
        Task<PatientDocumentDto> AddDocumentAsync(int patientId, CreatePatientDocumentDto documentDto, CancellationToken cancellationToken = default);
        Task DeleteDocumentAsync(int documentId, CancellationToken cancellationToken = default);
        Task<bool> UpdateProfilePictureAsync(int patientId, string? profilePictureUrl, CancellationToken cancellationToken = default);
    }
}
