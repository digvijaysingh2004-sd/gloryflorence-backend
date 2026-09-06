using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;

namespace GloryFlorence.Application.Interfaces
{
    public interface IAuditLogService
    {
        Task LogAsync(CreateAuditLogDto dto, CancellationToken cancellationToken = default);
        Task<PagedResult<AuditLogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = 20, string? entityName = null, CancellationToken cancellationToken = default);
    }
}
