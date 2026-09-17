using System;

namespace GloryFlorence.Application.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public string Name => string.IsNullOrWhiteSpace($"{FirstName} {LastName}".Trim()) ? Username : $"{FirstName} {LastName}".Trim();
        public DateTime CreatedAt { get; set; }
    }
}
