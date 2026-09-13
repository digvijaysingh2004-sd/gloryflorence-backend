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
    [Route("api/treatment-sessions")]
    public class TreatmentSessionsController : ControllerBase
    {
        private readonly ITreatmentSessionService _sessionService;
        private readonly IValidator<CreateTreatmentSessionDto> _createValidator;
        private readonly IValidator<UpdateTreatmentSessionDto> _updateValidator;
        private readonly IValidator<CompleteTreatmentSessionDto> _completeValidator;

        public TreatmentSessionsController(
            ITreatmentSessionService sessionService,
            IValidator<CreateTreatmentSessionDto> createValidator,
            IValidator<UpdateTreatmentSessionDto> updateValidator,
            IValidator<CompleteTreatmentSessionDto> completeValidator)
        {
            _sessionService = sessionService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _completeValidator = completeValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TreatmentSessionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] TreatmentSessionFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _sessionService.GetTreatmentSessionsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<TreatmentSessionDto>>.SuccessResponse(result, "Treatment sessions retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentSessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _sessionService.GetTreatmentSessionByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<TreatmentSessionDto>.FailureResponse($"Treatment session with ID {id} was not found."));
            }
            return Ok(ApiResponse<TreatmentSessionDto>.SuccessResponse(item, "Treatment session retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<TreatmentSessionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateTreatmentSessionDto? dto, CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Request body is required."));
            }

            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _sessionService.CreateTreatmentSessionAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TreatmentSessionDto>.SuccessResponse(created, "Treatment session created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentSessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTreatmentSessionDto? dto, CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Request body is required."));
            }

            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var updated = await _sessionService.UpdateTreatmentSessionAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<TreatmentSessionDto>.SuccessResponse(updated, "Treatment session updated successfully."));
        }

        [HttpPost("{id:int}/complete")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentSessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Complete(int id, [FromBody] CompleteTreatmentSessionDto? dto, CancellationToken cancellationToken)
        {
            dto ??= new CompleteTreatmentSessionDto();
            var validationResult = await _completeValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var result = await _sessionService.CompleteTreatmentSessionAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<TreatmentSessionDto>.SuccessResponse(result, "Treatment session marked as completed."));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _sessionService.DeleteTreatmentSessionAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("~/api/patients/{patientId:int}/treatment-sessions")]
        [HttpGet("~/api/patients/{patientId:int}/sessions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TreatmentSessionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] TreatmentSessionFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PatientId = patientId;
            var result = await _sessionService.GetTreatmentSessionsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<TreatmentSessionDto>>.SuccessResponse(result, "Treatment sessions retrieved successfully."));
        }

        [HttpPost("~/api/patients/{patientId:int}/treatment-sessions")]
        [HttpPost("~/api/patients/{patientId:int}/sessions")]
        [ProducesResponseType(typeof(ApiResponse<TreatmentSessionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateForPatient(int patientId, [FromBody] CreateTreatmentSessionDto? dto, CancellationToken cancellationToken)
        {
            if (dto == null)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Request body is required."));
            }

            dto.PatientId = patientId;
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _sessionService.CreateTreatmentSessionAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<TreatmentSessionDto>.SuccessResponse(created, "Treatment session created successfully."));
        }

        [HttpGet("~/api/appointments/{appointmentId:int}/treatment-sessions")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<TreatmentSessionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByAppointment(int appointmentId, [FromQuery] TreatmentSessionFilterDto filter, CancellationToken cancellationToken)
        {
            filter.AppointmentId = appointmentId;
            var result = await _sessionService.GetTreatmentSessionsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<TreatmentSessionDto>>.SuccessResponse(result, "Treatment sessions retrieved successfully."));
        }
    }
}
