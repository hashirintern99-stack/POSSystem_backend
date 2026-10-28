using System.ComponentModel.DataAnnotations;

namespace POSSystem.DTO.Requests.Auth
{
    public class SignupRequest
    {
        [Required, StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(150)]
        public string FullName { get; set; } = string.Empty;

        public string? Role { get; set; }
    }
}