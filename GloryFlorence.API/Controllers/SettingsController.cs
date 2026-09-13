using System.Threading;
using System.Threading.Tasks;
using GloryFlorence.API.Common;
using GloryFlorence.Application.DTOs;
using GloryFlorence.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GloryFlorence.API.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : BaseApiController
    {
        private readonly IClinicSettingsService _clinicSettingsService;

        public SettingsController(IClinicSettingsService clinicSettingsService)
        {
            _clinicSettingsService = clinicSettingsService;
        }

        [HttpGet("clinic")]
        [ProducesResponseType(typeof(ApiResponse<ClinicSettingsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClinicSettings(CancellationToken cancellationToken)
        {
            var settings = await _clinicSettingsService.GetClinicSettingsAsync(cancellationToken);
            return Ok(ApiResponse<ClinicSettingsDto>.SuccessResponse(settings, "Clinic settings retrieved successfully."));
        }

        [HttpPut("clinic")]
        [ProducesResponseType(typeof(ApiResponse<ClinicSettingsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateClinicSettings([FromBody] UpdateClinicSettingsDto dto, CancellationToken cancellationToken)
        {
            var updated = await _clinicSettingsService.UpdateClinicSettingsAsync(dto, cancellationToken);
            return Ok(ApiResponse<ClinicSettingsDto>.SuccessResponse(updated, "Clinic settings updated successfully."));
        }
    }
}
