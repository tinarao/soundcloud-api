using System.ComponentModel.DataAnnotations;

#nullable disable

namespace Sounds_New.DTO
{
    public class CreateTrackDTO
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; } = String.Empty;

        [Required]
        public IFormFile ArtworkFile { get; set; }
        [Required]
        public IFormFile AudioFile { get; set; }
        public string[] Genres { get; set; } = [];
    }
}