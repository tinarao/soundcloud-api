using System.ComponentModel.DataAnnotations;

namespace Sounds_New.DTO
{
    public class RegisterDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters.")]
        public required string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }
    }

    public class LoginDTO
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 6 characters long.")]
        public required string Password { get; set; }
    }
}