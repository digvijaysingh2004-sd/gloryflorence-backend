using Microsoft.AspNetCore.Http;

namespace GloryFlorence.API.DTOs
{
    public class UploadProfilePictureRequest
    {
        public IFormFile File { get; set; } = null!;
    }
}
