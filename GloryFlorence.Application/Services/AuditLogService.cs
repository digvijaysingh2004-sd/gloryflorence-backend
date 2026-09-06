using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;

namespace GloryFlorence.Application.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public AuditLogService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task LogAsync(CreateAuditLogDto dto, CancellationToken cancellationToken = default)
        {
            int? userId = dto.UserId;
            if (!userId.HasValue && int.TryParse(_currentUserService.UserId, out var parsedId))
            {
                userId = parsedId;
            }

            var log = new AuditLog
            {
                UserId = userId,
                Action = dto.Action,
                EntityName = dto.EntityName,
                EntityId = dto.EntityId,
                OldValue = dto.OldValue,
                NewValue = dto.NewValue,
                IPAddress = dto.IPAddress,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AuditLogs.AddAsync(log, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<PagedResult<AuditLogDto>> GetLogsAsync(int pageNumber = 1, int pageSize = 20, string? entityName = null, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.AuditLogs.Query();

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(l => l.EntityName == entityName);
            }

            var totalCount = query.Count();
            var safePageNumber = pageNumber > 0 ? pageNumber : 1;
            var safePageSize = pageSize > 0 ? pageSize : 20;

            var rawItems = query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var users = (await _unitOfWork.Users.GetAllAsync(cancellationToken)).ToDictionary(u => u.Id);

            var items = rawItems.Select(l => new AuditLogDto
            {
                Id = l.Id,
                UserId = l.UserId,
                UserName = l.UserId.HasValue && users.TryGetValue(l.UserId.Value, out var u) ? $"{u.FirstName} {u.LastName}".Trim() : null,
                Action = l.Action,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                OldValue = l.OldValue,
                NewValue = l.NewValue,
                IPAddress = l.IPAddress,
                CreatedAt = l.CreatedAt
            }).ToList();

            return new PagedResult<AuditLogDto>(items, totalCount, safePageNumber, safePageSize);
        }
    }
}
