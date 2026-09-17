namespace GloryFlorence.Application.DTOs
{
    public class LoginDto
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
