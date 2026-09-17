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
    [Route("api/[controller]")]
    public class AssessmentsController : ControllerBase
    {
        private readonly IPatientAssessmentService _assessmentService;
        private readonly IValidator<CreatePatientAssessmentDto> _createValidator;
        private readonly IValidator<UpdatePatientAssessmentDto> _updateValidator;

        public AssessmentsController(
            IPatientAssessmentService assessmentService,
            IValidator<CreatePatientAssessmentDto> createValidator,
            IValidator<UpdatePatientAssessmentDto> updateValidator)
        {
            _assessmentService = assessmentService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientAssessmentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] PatientAssessmentFilterDto filter, CancellationToken cancellationToken)
        {
            var result = await _assessmentService.GetAssessmentsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<PatientAssessmentDto>>.SuccessResponse(result, "Patient assessments retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PatientAssessmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _assessmentService.GetAssessmentByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<PatientAssessmentDto>.FailureResponse($"Assessment with ID {id} was not found."));
            }
            return Ok(ApiResponse<PatientAssessmentDto>.SuccessResponse(item, "Assessment retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PatientAssessmentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePatientAssessmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _assessmentService.CreateAssessmentAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<PatientAssessmentDto>.SuccessResponse(created, "Assessment created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PatientAssessmentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientAssessmentDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var updated = await _assessmentService.UpdateAssessmentAsync(id, dto, cancellationToken);
            return Ok(ApiResponse<PatientAssessmentDto>.SuccessResponse(updated, "Assessment updated successfully."));
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _assessmentService.DeleteAssessmentAsync(id, cancellationToken);
            return NoContent();
        }

        [HttpGet("~/api/patients/{patientId:int}/assessments")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientAssessmentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByPatient(int patientId, [FromQuery] PatientAssessmentFilterDto filter, CancellationToken cancellationToken)
        {
            filter.PatientId = patientId;
            var result = await _assessmentService.GetAssessmentsAsync(filter, cancellationToken);
            return Ok(ApiResponse<PagedResult<PatientAssessmentDto>>.SuccessResponse(result, "Patient assessments retrieved successfully."));
        }

        [HttpPost("~/api/patients/{patientId:int}/assessments")]
        [ProducesResponseType(typeof(ApiResponse<PatientAssessmentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateForPatient(int patientId, [FromBody] CreatePatientAssessmentDto dto, CancellationToken cancellationToken)
        {
            dto.PatientId = patientId;
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _assessmentService.CreateAssessmentAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<PatientAssessmentDto>.SuccessResponse(created, "Assessment created successfully."));
        }

        [HttpDelete("~/api/patients/{patientId:int}/assessments/{assessmentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteForPatient(int patientId, int assessmentId, CancellationToken cancellationToken)
        {
            await _assessmentService.DeleteAssessmentAsync(assessmentId, cancellationToken);
            return NoContent();
        }
    }
}
