using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.API.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Constants;
using GloryFlorence.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.API.Controllers
{
    public class MasterDataController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;

        public MasterDataController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("countries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCountries(CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.Countries.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<Country>>.SuccessResponse(countries, "Countries retrieved successfully."));
        }

        [HttpGet("states")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStates(CancellationToken cancellationToken)
        {
            var states = await _unitOfWork.States.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<State>>.SuccessResponse(states, "States retrieved successfully."));
        }

        [HttpGet("cities")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCities(CancellationToken cancellationToken)
        {
            var cities = await _unitOfWork.Cities.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<City>>.SuccessResponse(cities, "Cities retrieved successfully."));
        }

        [HttpGet("genders")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetGenders(CancellationToken cancellationToken)
        {
            var genders = await _unitOfWork.Genders.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<Gender>>.SuccessResponse(genders, "Genders retrieved successfully."));
        }

        [HttpGet("bloodgroups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBloodGroups(CancellationToken cancellationToken)
        {
            var bloodGroups = await _unitOfWork.BloodGroups.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<BloodGroup>>.SuccessResponse(bloodGroups, "Blood groups retrieved successfully."));
        }

        [HttpGet("specializations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetSpecializations(CancellationToken cancellationToken)
        {
            var specializations = await _unitOfWork.Specializations.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<Specialization>>.SuccessResponse(specializations, "Specializations retrieved successfully."));
        }

        [HttpGet("categories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
        {
            var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
            return Ok(ApiResponse<IEnumerable<Category>>.SuccessResponse(categories, "Categories retrieved successfully."));
        }

        [HttpGet("statuses")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<StatusDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStatuses([FromQuery] string? type, CancellationToken cancellationToken)
        {
            var query = _unitOfWork.Statuses.Query().Where(s => s.IsActive);

            if (!string.IsNullOrWhiteSpace(type))
            {
                var t = type.Trim().ToLower();
                query = query.Where(s => s.Type.ToLower() == t);
            }

            var items = await query.OrderBy(s => s.Type).ThenBy(s => s.Name).ToListAsync(cancellationToken);
            var dtos = items.Select(s => new StatusDto
            {
                Id = s.Id,
                Type = s.Type,
                Code = s.Code,
                Name = s.Name,
                Description = s.Description,
                IsActive = s.IsActive
            });

            return Ok(ApiResponse<IEnumerable<StatusDto>>.SuccessResponse(dtos, "Statuses retrieved successfully."));
        }

        [HttpGet("physiotherapists")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPhysiotherapists(CancellationToken cancellationToken)
        {
            var therapists = await _unitOfWork.Users.Query()
                .Where(u => u.IsActive && (u.Role == Roles.Physiotherapist || u.Role == Roles.Doctor || u.Role == Roles.Admin || u.Role == Roles.SuperAdmin))
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync(cancellationToken);

            var dtos = therapists.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive,
                FirstName = u.FirstName,
                LastName = u.LastName,
                CreatedAt = u.CreatedAt
            });

            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(dtos, "Physiotherapists retrieved successfully."));
        }
    }
}
