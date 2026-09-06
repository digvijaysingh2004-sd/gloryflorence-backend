using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.Common.Exceptions;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;

namespace GloryFlorence.Application.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExerciseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResult<ExerciseDto>> GetExercisesAsync(
            string? search = null,
            int? categoryId = null,
            bool? isActive = null,
            int pageNumber = 1,
            int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var categories = (await _unitOfWork.Categories.GetAllAsync(cancellationToken)).ToDictionary(c => c.Id, c => c.Name);
            var query = _unitOfWork.Exercises.Query();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                query = query.Where(e => e.Name.ToLower().Contains(s) || e.Description.ToLower().Contains(s));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(e => e.CategoryId == categoryId.Value);
            }

            if (isActive.HasValue)
            {
                query = query.Where(e => e.IsActive == isActive.Value);
            }

            var totalCount = query.Count();
            var safePageNumber = pageNumber > 0 ? pageNumber : 1;
            var safePageSize = pageSize > 0 ? pageSize : 10;

            var items = query
                .OrderBy(e => e.Name)
                .Skip((safePageNumber - 1) * safePageSize)
                .Take(safePageSize)
                .ToList();

            var dtos = items.Select(e => MapToDto(e, categories.TryGetValue(e.CategoryId, out var cName) ? cName : string.Empty));
            return new PagedResult<ExerciseDto>(dtos, totalCount, safePageNumber, safePageSize);
        }

        public async Task<ExerciseDto?> GetExerciseByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            var item = await _unitOfWork.Exercises.GetByIdAsync(id, cancellationToken);
            if (item == null) return null;

            var category = await _unitOfWork.Categories.GetByIdAsync(item.CategoryId, cancellationToken);
            return MapToDto(item, category?.Name ?? string.Empty);
        }

        public async Task<ExerciseDto> CreateExerciseAsync(CreateExerciseDto dto, CancellationToken cancellationToken = default)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), dto.CategoryId);
            }

            var entity = new Exercise
            {
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                Instructions = dto.Instructions,
                VideoUrl = dto.VideoUrl,
                ImageUrl = dto.ImageUrl,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Exercises.AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return MapToDto(entity, category.Name);
        }

        public async Task UpdateExerciseAsync(UpdateExerciseDto dto, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Exercises.GetByIdAsync(dto.Id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Exercise), dto.Id);
            }

            var category = await _unitOfWork.Categories.GetByIdAsync(dto.CategoryId, cancellationToken);
            if (category == null)
            {
                throw new NotFoundException(nameof(Category), dto.CategoryId);
            }

            entity.CategoryId = dto.CategoryId;
            entity.Name = dto.Name;
            entity.Description = dto.Description;
            entity.Instructions = dto.Instructions;
            entity.VideoUrl = dto.VideoUrl;
            entity.ImageUrl = dto.ImageUrl;
            entity.IsActive = dto.IsActive;
            entity.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Exercises.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteExerciseAsync(int id, CancellationToken cancellationToken = default)
        {
            var entity = await _unitOfWork.Exercises.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                throw new NotFoundException(nameof(Exercise), id);
            }

            entity.IsActive = false;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Exercises.Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        private static ExerciseDto MapToDto(Exercise e, string categoryName)
        {
            return new ExerciseDto
            {
                Id = e.Id,
                CategoryId = e.CategoryId,
                CategoryName = categoryName,
                Name = e.Name,
                Description = e.Description,
                Instructions = e.Instructions,
                VideoUrl = e.VideoUrl,
                ImageUrl = e.ImageUrl,
                IsActive = e.IsActive,
                CreatedAt = e.CreatedAt
            };
        }
    }
}
