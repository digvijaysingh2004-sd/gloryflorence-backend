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
    public class PatientsController : BaseApiController
    {
        private readonly IPatientService _patientService;
        private readonly IValidator<CreatePatientDto> _createValidator;

        public PatientsController(IPatientService patientService, IValidator<CreatePatientDto> createValidator)
        {
            _patientService = patientService;
            _createValidator = createValidator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<System.Collections.Generic.IEnumerable<PatientDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var patients = await _patientService.GetAllPatientsAsync(cancellationToken);
            return Ok(ApiResponse<System.Collections.Generic.IEnumerable<PatientDto>>.SuccessResponse(patients, "Patients retrieved successfully."));
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetPatientByIdAsync(id, cancellationToken);
            if (patient == null)
            {
                return NotFound(ApiResponse<PatientDto>.FailureResponse($"Patient with ID {id} was not found."));
            }
            return Ok(ApiResponse<PatientDto>.SuccessResponse(patient, "Patient retrieved successfully."));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreatePatientDto createPatientDto, CancellationToken cancellationToken)
        {
            var validationResult = await _createValidator.ValidateAsync(createPatientDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var createdPatient = await _patientService.CreatePatientAsync(createPatientDto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = createdPatient.Id }, ApiResponse<PatientDto>.SuccessResponse(createdPatient, "Patient created successfully."));
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto updatePatientDto, CancellationToken cancellationToken)
        {
            if (id != updatePatientDto.Id)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("ID in route does not match ID in body."));
            }

            await _patientService.UpdatePatientAsync(updatePatientDto, cancellationToken);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _patientService.DeletePatientAsync(id, cancellationToken);
            return NoContent();
        }

        // --- Patient Medical History Endpoints ---

        [HttpGet("{id:int}/medical-history")]
        [ProducesResponseType(typeof(ApiResponse<System.Collections.Generic.IEnumerable<PatientMedicalHistoryDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMedicalHistory(int id, CancellationToken cancellationToken)
        {
            var history = await _patientService.GetMedicalHistoriesAsync(id, cancellationToken);
            return Ok(ApiResponse<System.Collections.Generic.IEnumerable<PatientMedicalHistoryDto>>.SuccessResponse(history, "Patient medical history retrieved successfully."));
        }

        [HttpPost("{id:int}/medical-history")]
        [ProducesResponseType(typeof(ApiResponse<PatientMedicalHistoryDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMedicalHistory(int id, [FromBody] CreatePatientMedicalHistoryDto historyDto, CancellationToken cancellationToken)
        {
            var created = await _patientService.AddMedicalHistoryAsync(id, historyDto, cancellationToken);
            return CreatedAtAction(nameof(GetMedicalHistory), new { id = id }, ApiResponse<PatientMedicalHistoryDto>.SuccessResponse(created, "Medical history record added successfully."));
        }

        [HttpDelete("medical-history/{historyId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteMedicalHistory(int historyId, CancellationToken cancellationToken)
        {
            await _patientService.DeleteMedicalHistoryAsync(historyId, cancellationToken);
            return NoContent();
        }

        // --- Patient Documents Endpoints ---

        [HttpGet("{id:int}/documents")]
        [ProducesResponseType(typeof(ApiResponse<System.Collections.Generic.IEnumerable<PatientDocumentDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDocuments(int id, CancellationToken cancellationToken)
        {
            var documents = await _patientService.GetDocumentsAsync(id, cancellationToken);
            return Ok(ApiResponse<System.Collections.Generic.IEnumerable<PatientDocumentDto>>.SuccessResponse(documents, "Patient documents metadata retrieved successfully."));
        }


        [HttpDelete("documents/{documentId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDocument(int documentId, CancellationToken cancellationToken)
        {
            await _patientService.DeleteDocumentAsync(documentId, cancellationToken);
            return NoContent();
        }

        [HttpPost("{id:int}/documents/upload")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<PatientDocumentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument(int id, [FromForm] UploadDocumentRequest request, CancellationToken cancellationToken)
        {
            if (request?.File == null || request.File.Length == 0)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("No file was uploaded or the file is empty."));
            }

            var file = request.File;
            var documentType = request.DocumentType;

            var uploadsFolder = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
            if (!System.IO.Directory.Exists(uploadsFolder))
            {
                System.IO.Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{System.Guid.NewGuid()}_{System.IO.Path.GetFileName(file.FileName)}";
            var filePath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var relativePath = $"/uploads/documents/{uniqueFileName}";

            var documentDto = new CreatePatientDocumentDto
            {
                DocumentName = file.FileName,
                DocumentType = string.IsNullOrWhiteSpace(documentType) ? "Attachment" : documentType,
                FilePath = relativePath,
                FileSize = file.Length
            };

            var created = await _patientService.AddDocumentAsync(id, documentDto, cancellationToken);
            return CreatedAtAction(nameof(GetDocuments), new { id = id }, ApiResponse<PatientDocumentDto>.SuccessResponse(created, "Document uploaded and saved successfully."));
        }
    }

    public class UploadDocumentRequest
    {
        public IFormFile File { get; set; } = null!;
        public string? DocumentType { get; set; }
    }
}
