using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;

namespace GloryFlorence.Application.Services
{
    public class AppointmentTypeService : IAppointmentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<IEnumerable<AppointmentTypeDto>> GetAllAppointmentTypesAsync(bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var query = _unitOfWork.AppointmentTypes.Query();

            if (isActive.HasValue)
            {
                query = query.Where(a => a.IsActive == isActive.Value);
            }

            var items = query.OrderBy(a => a.Name).ToList();
            var dtos = items.Select(MapToDto);
            return Task.FromResult<IEnumerable<AppointmentTypeDto>>(dtos);
        }

        public async Task<AppointmentTypeDto?> GetAppointmentTypeByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.AppointmentTypes.GetByIdAsync(id, cancellationToken);
            return item == null ? null : MapToDto(item);
        }

        public async Task<AppointmentTypeDto> CreateAppointmentTypeAsync(CreateAppointmentTypeDto dto, CancellationToken cancellationToken = default)
        {
            var entity = new AppointmentType
            {
                Name = dto.Name,
                DurationMinutes = dto.DurationMinutes,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AppointmentTypes.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(entity);
        }

        public async Task UpdateAppointmentTypeAsync(UpdateAppointmentTypeDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.AppointmentTypes.GetByIdAsync(dto.Id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(AppointmentType), dto.Id);
            }

            entity.Name = dto.Name;
            entity.DurationMinutes = dto.DurationMinutes;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AppointmentTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAppointmentTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.AppointmentTypes.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(AppointmentType), id);
            }

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.AppointmentTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static AppointmentTypeDto MapToDto(AppointmentType a)
        {
            return new AppointmentTypeDto
            {
                Id = a.Id,
                Name = a.Name,
                DurationMinutes = a.DurationMinutes,
                Description = a.Description,
                IsActive = a.IsActive,
                CreatedAt = a.CreatedAt
            };
        }
    }
}
