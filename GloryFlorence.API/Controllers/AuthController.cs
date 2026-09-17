using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.API.Common;
using GloryFlorence.API.DTOs;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using GloryFlorence.Domain.Entities;
using GloryFlorence.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GloryFlorence.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AuthController(
            IUserService userService,
            ICurrentUserService currentUserService,
            ApplicationDbContext context,
            IWebHostEnvironment env)
        {
            _userService = userService;
            _currentUserService = currentUserService;
            _context = context;
            _env = env;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var response = await _userService.LoginAsync(loginDto, cancellationToken);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful."));
        }

        [HttpPost("register")]
        [HttpPost("register-patient")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> RegisterPatient(
            [FromBody] RegisterPatientDto registerDto,
            [FromServices] FluentValidation.IValidator<RegisterPatientDto> validator,
            CancellationToken cancellationToken)
        {
            var validationResult = await validator.ValidateAsync(registerDto, cancellationToken);
            if (!validationResult.IsValid)
            {
                throw new FluentValidation.ValidationException(validationResult.Errors);
            }

            var response = await _userService.RegisterPatientAsync(registerDto, cancellationToken);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Patient registered successfully."));
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser(CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.Username))
            {
                return Unauthorized(ApiResponse<UserDto>.FailureResponse("User is not authenticated."));
            }

            var user = await _userService.GetByUsernameAsync(_currentUserService.Username, cancellationToken);
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "Current user profile retrieved successfully."));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateUserProfileDto updateProfileDto, CancellationToken cancellationToken)
        {
            if (!_currentUserService.IsAuthenticated || string.IsNullOrEmpty(_currentUserService.Username))
            {
                return Unauthorized(ApiResponse<UserDto>.FailureResponse("User is not authenticated."));
            }

            var updatedUser = await _userService.UpdateProfileAsync(_currentUserService.Username, updateProfileDto, cancellationToken);
            return Ok(ApiResponse<UserDto>.SuccessResponse(updatedUser, "User profile updated successfully."));
        }

        /// <summary>
        /// Uploads or replaces profile picture for the logged-in user (Staff / Admin / Any Role)
        /// Saves to wwwroot/uploads/profiles/ exactly like documents save to wwwroot/uploads/documents/
        /// </summary>
        [HttpPost("profile-picture")]
        [Authorize]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<ProfilePictureResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadProfilePictureRequest request, CancellationToken cancellationToken)
        {
            var file = request?.File ?? (Request.HasFormContentType && Request.Form.Files.Count > 0 ? Request.Form.Files[0] : null);
            if (file == null || file.Length == 0)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("No image file was provided."));
            }

            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Image size exceeds 5MB limit."));
            }

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(ApiResponse<object>.FailureResponse("Allowed formats: .jpg, .jpeg, .png, .webp, .gif"));
            }

            int userId = 0;
            if (int.TryParse(_currentUserService.UserId, out int parsedUserId) && parsedUserId > 0)
            {
                userId = parsedUserId;
            }

            if (userId <= 0)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdClaim, out userId);
            }

            if (userId <= 0 && !string.IsNullOrEmpty(_currentUserService.Username))
            {
                var user = await _userService.GetByUsernameAsync(_currentUserService.Username, cancellationToken);
                if (user != null) userId = user.Id;
            }

            if (userId <= 0)
            {
                return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user identity token."));
            }

            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var uploadsFolder = Path.Combine(webRoot, "uploads", "profiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = System.DateTime.UtcNow
                };
                _context.UserProfiles.Add(userProfile);
            }

            if (!string.IsNullOrEmpty(userProfile.ProfilePictureUrl))
            {
                try
                {
                    var oldFileName = Path.GetFileName(userProfile.ProfilePictureUrl);
                    var oldFilePath = Path.Combine(uploadsFolder, oldFileName);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                catch { /* Ignore if file already deleted */ }
            }

            var uniqueFileName = $"user_{userId}_{System.Guid.NewGuid():N}{extension}";
            var physicalPath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(physicalPath, FileMode.Create))
            {
                await file.CopyToAsync(stream, cancellationToken);
            }

            var fullUrl = $"{Request.Scheme}://{Request.Host}/uploads/profiles/{uniqueFileName}";

            userProfile.ProfilePictureUrl = fullUrl;
            userProfile.UpdatedAt = System.DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            return Ok(ApiResponse<ProfilePictureResponseDto>.SuccessResponse(new ProfilePictureResponseDto
            {
                ProfilePictureUrl = fullUrl
            }, "Profile picture saved to server successfully."));
        }

        /// <summary>
        /// Removes the profile picture for the logged-in user
        /// </summary>
        [HttpDelete("profile-picture")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteProfilePicture(CancellationToken cancellationToken)
        {
            int userId = 0;
            if (int.TryParse(_currentUserService.UserId, out int parsedUserId) && parsedUserId > 0)
            {
                userId = parsedUserId;
            }

            if (userId <= 0)
            {
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int.TryParse(userIdClaim, out userId);
            }

            if (userId <= 0 && !string.IsNullOrEmpty(_currentUserService.Username))
            {
                var user = await _userService.GetByUsernameAsync(_currentUserService.Username, cancellationToken);
                if (user != null) userId = user.Id;
            }

            if (userId <= 0)
            {
                return Unauthorized(ApiResponse<object>.FailureResponse("Unauthorized."));
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (userProfile != null && !string.IsNullOrEmpty(userProfile.ProfilePictureUrl))
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var fileName = Path.GetFileName(userProfile.ProfilePictureUrl);
                var filePath = Path.Combine(webRoot, "uploads", "profiles", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    try { System.IO.File.Delete(filePath); } catch { }
                }

                userProfile.ProfilePictureUrl = null;
                userProfile.UpdatedAt = System.DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return Ok(ApiResponse<object?>.SuccessResponse(null, "Profile picture removed successfully."));
        }
    }
}
