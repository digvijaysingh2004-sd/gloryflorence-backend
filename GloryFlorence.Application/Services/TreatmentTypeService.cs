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
    public class TreatmentTypeService : ITreatmentTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TreatmentTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TreatmentTypeDto>> GetAllTreatmentTypesAsync(int? categoryId = null, bool? isActive = null, CancellationToken cancellationToken = default)
        {
            var categories = (await _unitOfWork.Categories.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);
            var query = _unitOfWork.TreatmentTypes.Query();

            if (categoryId.HasValue)
            {
                query = query.Where(t => t.CategoryId == categoryId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(t => t.IsActive == isActive.Value);
            }

            var items = query.OrderBy(t => t.Name).ToList();
            return items.Select(t => MapToDto(t, categories.TryGetValue(t.CategoryId, out var cName) ? cName : string.Empty));
        }

        public async Task<TreatmentTypeDto?> GetTreatmentTypeByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.TreatmentTypes.GetByIdAsync(id, cancellationToken);
            if (item == null) return null;

            var category = await _unitOfWork.Categories.GetByIdAsync(item.CategoryId, cancellationToken);
            return MapToDto(item, category?.Name ?? string.Empty);
        }

        public async Task<TreatmentTypeDto> CreateTreatmentTypeAsync(CreateTreatmentTypeDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), dto.CategoryId);
            }

            var entity = new TreatmentType
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                DefaultDurationMinutes = dto.DefaultDurationMinutes,
                DefaultPrice = dto.DefaultPrice,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.TreatmentTypes.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(entity, category.Name);
        }

        public async Task UpdateTreatmentTypeAsync(UpdateTreatmentTypeDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.TreatmentTypes.GetByIdAsync(dto.Id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(TreatmentType), dto.Id);
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), dto.CategoryId);
            }

            entity.CategoryId = dto.CategoryId;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.DefaultDurationMinutes = dto.DefaultDurationMinutes;
            entity.DefaultPrice = dto.DefaultPrice;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.TreatmentTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteTreatmentTypeAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.TreatmentTypes.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(TreatmentType), id);
            }

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.TreatmentTypes.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static TreatmentTypeDto MapToDto(TreatmentType t, string categoryName)
        {
            return new TreatmentTypeDto
            {
                Id = t.Id,
                CategoryId = t.CategoryId,
                CategoryName = categoryName,
                Name = t.Name,
                Description = t.Description,
                DefaultDurationMinutes = t.DefaultDurationMinutes,
                DefaultPrice = t.DefaultPrice,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt
            };
        }
    }
}
