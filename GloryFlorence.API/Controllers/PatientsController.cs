using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using GloryFlorence.API.Common;
using GloryFlorence.API.DTOs;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GloryFlorence.API.Controllers
{
    public class PatientsController : BaseApiController
    {
        private readonly IPatientService _patientService;
        private readonly IValidator<CreatePatientDto> _createValidator;
        private readonly ICurrentUserService _currentUserService;
        private readonly IWebHostEnvironment _env;

        public PatientsController(
            IPatientService patientService,
            IValidator<CreatePatientDto> createValidator,
            ICurrentUserService currentUserService,
            IWebHostEnvironment env)
        {
            _patientService = patientService;
            _createValidator = createValidator;
            _currentUserService = currentUserService;
            _env = env;
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return Unauthorized(ApiResponse<PatientDto>.FailureResponse("User is not authenticated."));
            }

            PatientDto? patient = null;
            if (int.TryParse(_currentUserService.UserId, out var userId) && userId > 0)
            {
                patient = await _patientService.GetPatientByUserIdAsync(userId, cancellationToken);
            }

            var userEmail = _currentUserService.Email ?? _currentUserService.Username;
            if (patient == null && !string.IsNullOrEmpty(userEmail))
            {
                patient = await _patientService.GetPatientByEmailAsync(userEmail, cancellationToken);
            }

            if (patient == null)
            {
                return NotFound(ApiResponse<PatientDto>.FailureResponse("Patient record for current logged in user was not found."));
            }

            return Ok(ApiResponse<PatientDto>.SuccessResponse(patient, "Patient profile retrieved successfully."));
        }

        /// <summary>
        /// Upload or replace the profile picture for the logged-in patient
        /// </summary>
        /// <param name="file">Image file (JPG, PNG, WEBP, GIF up to 5MB)</param>
        [HttpPost("me/profile-picture")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<ProfilePictureResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadMyProfilePicture([FromForm] UploadProfilePictureRequest request, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return Unauthorized(ApiResponse<object>.FailureResponse("User is not authenticated."));
            }

            var file = request?.File ?? (Request.HasFormContentType && Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null);
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.FailureResponse(new[] { "Please select an image file to upload." }, "No image file provided."));
            }

            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                return BadRequest(ApiResponse<object>.FailureResponse(new[] { "Image file must not exceed 5 MB." }, "File size exceeds 5MB limit."));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!System.Linq.Enumerable.Contains(allowedExtensions, extension))
            {
                return BadRequest(ApiResponse<object>.FailureResponse(new[] { "Allowed image formats: .jpg, .jpeg, .png, .webp, .gif" }, "Invalid image extension."));
            }

            PatientDto? patient = null;
            if (int.TryParse(_currentUserService.UserId, out var userId) && userId > 0)
            {
                patient = await _patientService.GetPatientByUserIdAsync(userId, cancellationToken);
            }

            var userEmail = _currentUserService.Email ?? _currentUserService.Username;
            if (patient == null && !string.IsNullOrEmpty(userEmail))
            {
                patient = await _patientService.GetPatientByEmailAsync(userEmail, cancellationToken);
            }

            if (patient == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse("Could not find a patient record linked to your user account.", "Patient profile record not found."));
            }

            var webRoot = _env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
            var uploadDir = System.IO.Path.Combine(webRoot, "uploads", "profiles");

            if (!System.IO.Directory.Exists(uploadDir))
            {
                System.IO.Directory.CreateDirectory(uploadDir);
            }

            if (!string.IsNullOrEmpty(patient.ProfilePictureUrl))
            {
                try
                {
                    var oldFileName = System.IO.Path.GetFileName(patient.ProfilePictureUrl);
                    var oldFilePath = System.IO.Path.Combine(uploadDir, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                catch
                {
                    // Ignore deletion error on missing files
                }
            }

            var uniqueFileName = $"patient_{patient.Id}_{System.Guid.NewGuid():N}{extension}";
            var physicalPath = System.IO.Path.Combine(uploadDir, uniqueFileName);

            using (var stream = new System.IO.FileStream(physicalPath, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var fullUrl = $"{Request.Scheme}://{Request.Host}/uploads/profiles/{uniqueFileName}";

            await _patientService.UpdateProfilePictureAsync(patient.Id, fullUrl, cancellationToken);

            return Ok(ApiResponse<ProfilePictureResponseDto>.SuccessResponse(
                new ProfilePictureResponseDto { ProfilePictureUrl = fullUrl },
                "Profile picture uploaded successfully."));
        }

        /// <summary>
        /// Removes the profile picture for the logged-in patient
        /// </summary>
        [HttpDelete("me/profile-picture")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMyProfilePicture(CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated)
            {
                return Unauthorized(ApiResponse<object>.FailureResponse("User is not authenticated."));
            }

            PatientDto? patient = null;
            if (int.TryParse(_currentUserService.UserId, out var userId) && userId > 0)
            {
                patient = await _patientService.GetPatientByUserIdAsync(userId, cancellationToken);
            }

            var userEmail = _currentUserService.Email ?? _currentUserService.Username;
            if (patient == null && !string.IsNullOrEmpty(userEmail))
            {
                patient = await _patientService.GetPatientByEmailAsync(userEmail, cancellationToken);
            }

            if (patient != null && !string.IsNullOrEmpty(patient.ProfilePictureUrl))
            {
                var webRoot = _env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
                var fileName = System.IO.Path.GetFileName(patient.ProfilePictureUrl);
                var filePath = System.IO.Path.Combine(webRoot, "uploads", "profiles", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    try { System.IO.File.Delete(filePath); } catch { }
                }

                await _patientService.UpdateProfilePictureAsync(patient.Id, null, cancellationToken);
            }

            return Ok(ApiResponse<object?>.SuccessResponse(null, "Profile picture removed successfully."));
        }

        /// <summary>
        /// Upload or replace profile picture for a specific patient by ID (Admin / Doctor / Staff)
        /// </summary>
        [HttpPost("{id:int}/profile-picture")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<ProfilePictureResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UploadPatientProfilePicture(int id, [FromForm] UploadProfilePictureRequest request, CancellationToken cancellationToken)
        {
            var file = request?.File ?? (Request.HasFormContentType && Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null);
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("No image file provided."));
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("File size exceeds 5MB limit."));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!System.Linq.Enumerable.Contains(allowedExtensions, extension))
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Allowed formats: .jpg, .jpeg, .png, .webp, .gif"));
            }

            var patientDto = await _patientService.GetPatientByIdAsync(id, cancellationToken);
            if (patientDto == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse($"Patient with ID {id} not found."));
            }

            var webRoot = _env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = System.IO.Path.Combine(webRoot, "uploads", "profiles");
            if (!System.IO.Directory.Exists(uploadsFolder))
            {
                System.IO.Directory.CreateDirectory(uploadsFolder);
            }

            if (!string.IsNullOrEmpty(patientDto.ProfilePictureUrl))
            {
                try
                {
                    var oldFileName = System.IO.Path.GetFileName(patientDto.ProfilePictureUrl);
                    var oldFilePath = System.IO.Path.Combine(uploadsFolder, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                catch { }
            }

            var uniqueFileName = $"patient_{id}_{System.Guid.NewGuid():N}{extension}";
            var physicalPath = System.IO.Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new System.IO.FileStream(physicalPath, System.IO.FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var fullUrl = $"{Request.Scheme}://{Request.Host}/uploads/profiles/{uniqueFileName}";
            await _patientService.UpdateProfilePictureAsync(id, fullUrl, cancellationToken);

            return Ok(ApiResponse<ProfilePictureResponseDto>.SuccessResponse(new ProfilePictureResponseDto
            {
                ProfilePictureUrl = fullUrl
            }, "Patient profile picture uploaded successfully."));
        }

        /// <summary>
        /// Removes the profile picture for a patient by ID
        /// </summary>
        [HttpDelete("{id:int}/profile-picture")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePatientProfilePicture(int id, CancellationToken cancellationToken)
        {
            var patientDto = await _patientService.GetPatientByIdAsync(id, cancellationToken);
            if (patientDto == null)
            {
                return NotFound(ApiResponse<object>.FailureResponse($"Patient with ID {id} not found."));
            }

            if (!string.IsNullOrEmpty(patientDto.ProfilePictureUrl))
            {
                var webRoot = _env.WebRootPath ?? System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot");
                var fileName = System.IO.Path.GetFileName(patientDto.ProfilePictureUrl);
                var filePath = System.IO.Path.Combine(webRoot, "uploads", "profiles", fileName);
                if (System.IO.File.Exists(filePath))
                {
                    try { System.IO.File.Delete(filePath); } catch { }
                }

                await _patientService.UpdateProfilePictureAsync(id, null, cancellationToken);
            }

            return Ok(ApiResponse<object?>.SuccessResponse(null, "Patient profile picture removed."));
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
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePatientDto updatePatientDto, CancellationToken cancellationToken)
        {
            if (updatePatientDto.Id == 0)
            {
                updatePatientDto.Id = id;
            }
            else if (id != updatePatientDto.Id)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("ID in route does not match ID in body."));
            }

            var updatedPatient = await _patientService.UpdatePatientAsync(updatePatientDto, cancellationToken);
            return Ok(ApiResponse<PatientDto>.SuccessResponse(updatedPatient, "Patient updated successfully."));
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
        [Consumes("multipart/form-data", "application/json")]
        [ProducesResponseType(typeof(ApiResponse<PatientDocumentDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadDocument(int id, [FromForm] UploadDocumentRequest? request, CancellationToken cancellationToken)
        {
            if (!Request.HasFormContentType)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Content-Type must be 'multipart/form-data' with a file payload."));
            }

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
