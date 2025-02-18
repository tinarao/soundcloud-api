using System.ComponentModel.DataAnnotations.Schema;

namespace Sounds_New.Models
{

    [Flags]
    public enum UserRoles
    {
        User = 1,
        Moderator = 2,
        Admin = 4,
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Slug { get; set; }
        public string Email { get; set; }
        public UserRoles Role { get; set; } = UserRoles.User;
        public string? Bio { get; set; }
        public string? AvatarFilePath { get; set; }
        public string? BannerFilePath { get; set; }
        public string[] Links { get; set; } = [];
        public string Password { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public ICollection<Track> Tracks { get; } = [];
        public ICollection<Comment> Comments { get; } = [];

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
    }
}