using System.Collections.Generic;
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
    [Route("api/exercise-prescriptions")]
    public class ExercisePrescriptionsController : ControllerBase
    {
        private readonly IExercisePrescriptionService _prescriptionService;
        private readonly IValidator<CreateExercisePrescriptionDto> _createValidator;
        private readonly IValidator<UpdateExercisePrescriptionDto> _updateValidator;
        private readonly IValidator<AddExerciseToPrescriptionDto> _addExerciseValidator;
        private readonly IValidator<UpdateExercisePrescriptionStatusDto> _statusValidator;

        public ExercisePrescriptionsController(
            IExercisePrescriptionService prescriptionService,
            IValidator<CreateExercisePrescriptionDto> createValidator,
            IValidator<UpdateExercisePrescriptionDto> updateValidator,
            IValidator<AddExerciseToPrescriptionDto> addExerciseValidator,
            IValidator<UpdateExercisePrescriptionStatusDto> statusValidator)
        {
            _prescriptionService = prescriptionService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _addExerciseValidator = addExerciseValidator;
            _statusValidator = statusValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ExercisePrescriptionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] ExercisePrescriptionFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _prescriptionService.GetPrescriptionsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<ExercisePrescriptionDto>>.SuccessResponse(result, "Exercise prescriptions retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ExercisePrescriptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _prescriptionService.GetPrescriptionByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<ExercisePrescriptionDto>.FailureResponse($"Exercise prescription with ID {id} was not found."));
            }
            return Ok(ApiResponse<ExercisePrescriptionDto>.SuccessResponse(item, "Exercise prescription retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExercisePrescriptionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateExercisePrescriptionDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _prescriptionService.CreatePrescriptionAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ExercisePrescriptionDto>.SuccessResponse(created, "Exercise prescription created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ExercisePrescriptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExercisePrescriptionDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var updated = await _prescriptionService.UpdatePrescriptionAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<ExercisePrescriptionDto>.SuccessResponse(updated, "Exercise prescription updated successfully."));
        }

        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateExercisePrescriptionStatusDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _statusValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            await _prescriptionService.UpdatePrescriptionStatusAsync(id, dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _prescriptionService.DeletePrescriptionAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:int}/exercises")]
        [ProducesResponseType(typeof(ApiResponse<ExercisePrescriptionDetailDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddExercise(int id, [FromBody] AddExerciseToPrescriptionDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _addExerciseValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _prescriptionService.AddExerciseAsync(id, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = id }, ApiResponse<ExercisePrescriptionDetailDto>.SuccessResponse(created, "Exercise added to prescription successfully."));
        }

        [HttpDelete("{id:int}/exercises/{detailId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveExercise(int id, int detailId, CancellationToken cancellationToken)
        {
            await _prescriptionService.RemoveExerciseAsync(id, detailId, cancellationToken);
            return NoContent();
        }

        // Nested convenience routes matching requirements
        [HttpGet("~/api/patients/{patientId:int}/exercise-prescriptions")]
        [HttpGet("~/api/patients/{patientId:int}/prescriptions")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExercisePrescriptionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId, CancellationToken cancellationToken)
        {
            var history = await _prescriptionService.GetPatientPrescriptionHistoryAsync(patientId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<ExercisePrescriptionDto>>.SuccessResponse(history, "Patient exercise prescription history retrieved successfully."));
        }

        [HttpPost("~/api/patients/{patientId:int}/exercise-prescriptions")]
        [HttpPost("~/api/patients/{patientId:int}/prescriptions")]
        [ProducesResponseType(typeof(ApiResponse<ExercisePrescriptionDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateForPatient(int patientId, [FromBody] CreateExercisePrescriptionDto dto, CancellationToken cancellationToken)
        {
            dto.PatientId = patientId;
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _prescriptionService.CreatePrescriptionAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ExercisePrescriptionDto>.SuccessResponse(created, "Exercise prescription created successfully."));
        }

        [HttpGet("~/api/treatment-plans/{treatmentPlanId:int}/exercise-prescriptions")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ExercisePrescriptionDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByTreatmentPlan(int treatmentPlanId, CancellationToken cancellationToken)
        {
            var prescriptions = await _prescriptionService.GetPrescriptionsByTreatmentPlanAsync(treatmentPlanId, cancellationToken);
            return Ok(ApiResponse<IEnumerable<ExercisePrescriptionDto>>.SuccessResponse(prescriptions, "Treatment plan exercise prescriptions retrieved successfully."));
        }
    }
}
