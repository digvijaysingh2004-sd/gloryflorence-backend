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
    [Route("api/exercises")]
    public class ExercisesController : ControllerBase
    {
        private readonly IExerciseService _exerciseService;
        private readonly IValidator<CreateExerciseDto> _createValidator;

        public ExercisesController(IExerciseService exerciseService, IValidator<CreateExerciseDto> createValidator)
        {
            _exerciseService = exerciseService;
            _createValidator = createValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ExerciseDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int? categoryId,
            [FromQuery] bool? isActive,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _exerciseService.GetExercisesAsync(search, categoryId, isActive, pageNumber, pageSize, cancellationToken);
            return Ok(ApiResponse<PagedResult<ExerciseDto>>.SuccessResponse(result, "Exercises retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<ExerciseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var item = await _exerciseService.GetExerciseByIdAsync(id, cancellationToken);
            if (item == null)
            {
                return NotFound(ApiResponse<ExerciseDto>.FailureResponse($"Exercise with ID {id} was not found."));
            }
            return Ok(ApiResponse<ExerciseDto>.SuccessResponse(item, "Exercise retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ExerciseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateExerciseDto dto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var created = await _exerciseService.CreateExerciseAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, ApiResponse<ExerciseDto>.SuccessResponse(created, "Exercise created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciseDto dto, CancellationToken cancellationToken)
        {
            if (dto.Id == 0)
            {
                dto.Id = id;
            }
            else if (id != dto.Id)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("ID in route does not match ID in body."));
            }

            await _exerciseService.UpdateExerciseAsync(dto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _exerciseService.DeleteExerciseAsync(id, cancellationToken);
            return NoContent();
        }
    }
}
