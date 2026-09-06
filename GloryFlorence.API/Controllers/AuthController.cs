using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.API.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GloryFlorence.API.Controllers
{
    public class AuthController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IUserService userService, ICurrentUserService currentUserService)
        {
            _userService = userService;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            var response = await _userService.LoginAsync(loginDto, cancellationToken);
            return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(response, "Login successful."));
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
    }
}
