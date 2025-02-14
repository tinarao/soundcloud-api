using System.ComponentModel.DataAnnotations;
using Sounds_New.Models;

#nullable disable

namespace Sounds_New.DTO
{
    public class CreateTrackDTO
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot be longer than 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Artwork file is required.")]
        [DataType(DataType.Upload)]
        public IFormFile ArtworkFile { get; set; }

        [Required(ErrorMessage = "Audio file is required.")]
        [DataType(DataType.Upload)]
        public IFormFile AudioFile { get; set; }

        [Required(ErrorMessage = "At least one genre is required.")]
        [MinLength(1, ErrorMessage = "At least one genre is required.")]
        public string[] Genres { get; set; } = [];
    }

    public class CreateTrackResultDTO
    {
        public required int Status { get; set; }
        public required string Message { get; set; }
        public Track Track { get; set; }
    }

    public class UpdateTrackDataDTO
    {
        [Required(ErrorMessage = "Peaks is required.")]
        public float[] Peaks { get; set; }
    }
}