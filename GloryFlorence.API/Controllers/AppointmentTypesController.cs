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
    [Route("api/appointment-types")]
    public class AppointmentTypesController : ControllerBase
    {
        private readonly IAppointmentTypeService _appointmentTypeService;
        private readonly IValidator<CreateAppointmentTypeDto> _createValidator;

        public AppointmentTypesController(IAppointmentTypeService appointmentTypeService, IValidator<CreateAppointmentTypeDto> createValidator)
        {
            _appointmentTypeService = appointmentTypeService;
            _createValidator = createValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<AppointmentTypeDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive, CancellationToken cancellationToken)
        {
            var items = await _appointmentTypeService.GetAllAppointmentTypesAsync(isActive, cancellationToken);
            return Ok(ApiResponse<IEnumerable<AppointmentTypeDto>>.SuccessResponse(items, "Appointment types retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<AppointmentTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _appointmentTypeService.GetAppointmentTypeByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<AppointmentTypeDto>.FailureResponse($"Appointment type with ID {id} was not found."));
            }
            return Ok(ApiResponse<AppointmentTypeDto>.SuccessResponse(item, "Appointment type retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<AppointmentTypeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateAppointmentTypeDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _appointmentTypeService.CreateAppointmentTypeAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<AppointmentTypeDto>.SuccessResponse(created, "Appointment type created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAppointmentTypeDto dto, CancellationToken cancellationToken)
        {
            if (id != dto.Id)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("ID in route does not match ID in body."));
            }

            await _appointmentTypeService.UpdateAppointmentTypeAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _appointmentTypeService.DeleteAppointmentTypeAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
