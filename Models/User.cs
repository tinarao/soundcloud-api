using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Sounds_New.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Slug { get; set; }
        public required string Email { get; set; }
        public string? Bio { get; set; }
        public string? AvatarFilePath { get; set; }
        public string[] Links { get; set; } = [];
        public required string Password { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public ICollection<Track> Tracks { get; } = [];
    }
}