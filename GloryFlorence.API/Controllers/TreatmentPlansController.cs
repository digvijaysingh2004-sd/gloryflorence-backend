using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using GloryFlorence.API.Common;
using GloryFlorence.Application.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GloryFlorence.API.Controllers
{
    [ApiController]
    [Route("api/treatment-plans")]
    public class TreatmentPlansController : ControllerBase
    {
        private readonly ITreatmentPlanService _planService;
        private readonly IValidator<CreateTreatmentPlanDto> _createValidator;
        private readonly IValidator<UpdateTreatmentPlanDto> _updateValidator;
        private readonly IValidator<UpdateTreatmentPlanStatusDto> _statusValidator;

        public TreatmentPlansController(
            ITreatmentPlanService planService,
            IValidator<CreateTreatmentPlanDto> createValidator,
            IValidator<UpdateTreatmentPlanDto> updateValidator,
            IValidator<UpdateTreatmentPlanStatusDto> statusValidator)
        {
            _planService = planService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _statusValidator = statusValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TreatmentPlanDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] TreatmentPlanFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _planService.GetTreatmentPlansAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<TreatmentPlanDto>>.SuccessResponse(result, "Treatment plans retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _planService.GetTreatmentPlanByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<TreatmentPlanDto>.FailureResponse($"Treatment plan with ID {id} was not found."));
            }
            return Ok(ApiResponse<TreatmentPlanDto>.SuccessResponse(item, "Treatment plan retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TreatmentPlanDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTreatmentPlanDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _planService.CreateTreatmentPlanAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TreatmentPlanDto>.SuccessResponse(created, "Treatment plan created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTreatmentPlanDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var updated = await _planService.UpdateTreatmentPlanAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<TreatmentPlanDto>.SuccessResponse(updated, "Treatment plan updated successfully."));
        }

        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateTreatmentPlanStatusDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _statusValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _planService.UpdateTreatmentPlanStatusAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _planService.DeleteTreatmentPlanAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("~/api/patients/{patientId:int}/treatment-plans")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TreatmentPlanDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] TreatmentPlanFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PatientId = patientId;
            var result = await _planService.GetTreatmentPlansAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<TreatmentPlanDto>>.SuccessResponse(result, "Treatment plans retrieved successfully."));
        }

        [HttpPost("~/api/patients/{patientId:int}/treatment-plans")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentPlanDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateForPatient(int patientId, [FromBody] CreateTreatmentPlanDto dto, CancellationToken cancellationToken)
        {
            dto.PatientId = patientId;
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _planService.CreateTreatmentPlanAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TreatmentPlanDto>.SuccessResponse(created, "Treatment plan created successfully."));
        }

        [HttpPut("~/api/patients/{patientId:int}/treatment-plans/{planId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateForPatient(int patientId, int planId, [FromBody] UpdateTreatmentPlanDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _planService.UpdateTreatmentPlanAsync(planId, dto, cancellationToken);
            return NoContent();
        }
    }
}
