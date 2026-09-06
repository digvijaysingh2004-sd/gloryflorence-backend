using System;
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
    [Route("api/appointments")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IValidator<CreateAppointmentDto> _createValidator;
        private readonly IValidator<UpdateAppointmentDto> _updateValidator;

        public AppointmentsController(
            IAppointmentService appointmentService,
            IValidator<CreateAppointmentDto> createValidator,
            IValidator<UpdateAppointmentDto> updateValidator)
        {
            _appointmentService = appointmentService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<AppointmentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] AppointmentFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _appointmentService.GetAppointmentsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<AppointmentDto>>.SuccessResponse(result, "Appointments retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _appointmentService.GetAppointmentByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<AppointmentDto>.FailureResponse($"Appointment with ID {id} was not found."));
            }
            return Ok(ApiResponse<AppointmentDto>.SuccessResponse(item, "Appointment retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AppointmentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _appointmentService.CreateAppointmentAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<AppointmentDto>.SuccessResponse(created, "Appointment created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("ID in route does not match ID in body."));
            }

            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _appointmentService.UpdateAppointmentAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:int}/reschedule")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reschedule(int id, [FromBody] RescheduleAppointmentDto dto, CancellationToken cancellationToken)
        {
            await _appointmentService.RescheduleAppointmentAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpPut("{id:int}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel(int id, [FromBody] CancelAppointmentDto dto, CancellationToken cancellationToken)
        {
            await _appointmentService.CancelAppointmentAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _appointmentService.DeleteAppointmentAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("check-conflict")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckConflict(
            [FromQuery] int physiotherapistId,
            [FromQuery] DateTime date,
            [FromQuery] TimeSpan startTime,
            [FromQuery] TimeSpan endTime,
            [FromQuery] int? excludeAppointmentId,
            CancellationToken cancellationToken)
        {
            var conflict = await _appointmentService.HasConflictAsync(physiotherapistId, date, startTime, endTime, excludeAppointmentId, cancellationToken);
            return Ok(ApiResponse<bool>.SuccessResponse(conflict, conflict ? "Conflict detected." : "No conflict detected."));
        }
    }
}
