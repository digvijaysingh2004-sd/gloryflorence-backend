using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IClinicSettingsService
    {
        Task<ClinicSettingsDto> GetClinicSettingsAsync(CancellationToken cancellationToken);
        Task<ClinicSettingsDto> UpdateClinicSettingsAsync(UpdateClinicSettingsDto dto, CancellationToken cancellationToken);
    }
}
