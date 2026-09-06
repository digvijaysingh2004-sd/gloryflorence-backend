using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using GloryFlorence.API.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GloryFlorence.API.Controllers
{
    [ApiController]
    [Route("api/treatment-types")]
    public class TreatmentTypesController : ControllerBase
    {
        private readonly ITreatmentTypeService _treatmentTypeService;
        private readonly IValidator<CreateTreatmentTypeDto> _createValidator;

        public TreatmentTypesController(ITreatmentTypeService treatmentTypeService, IValidator<CreateTreatmentTypeDto> createValidator)
        {
            _treatmentTypeService = treatmentTypeService;
            _createValidator = createValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<TreatmentTypeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? categoryId, [FromQuery] bool? isActive, CancellationToken cancellationToken)
        {
            var items = await _treatmentTypeService.GetAllTreatmentTypesAsync(categoryId, isActive, cancellationToken);
            return Ok(ApiResponse<IEnumerable<TreatmentTypeDto>>.SuccessResponse(items, "Treatment types retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _treatmentTypeService.GetTreatmentTypeByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<TreatmentTypeDto>.FailureResponse($"Treatment type with ID {id} was not found."));
            }
            return Ok(ApiResponse<TreatmentTypeDto>.SuccessResponse(item, "Treatment type retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TreatmentTypeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTreatmentTypeDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _treatmentTypeService.CreateTreatmentTypeAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TreatmentTypeDto>.SuccessResponse(created, "Treatment type created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTreatmentTypeDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("ID in route does not match ID in body."));
            }

            await _treatmentTypeService.UpdateTreatmentTypeAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _treatmentTypeService.DeleteTreatmentTypeAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
