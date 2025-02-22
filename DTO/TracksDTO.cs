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

        [StringLength(1100, ErrorMessage = "Description cannot be longer than 1100 characters.")]
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

    public class UpdateTrackPrimaryDataDTO
    {
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Длина названия не может быть больше 100 символов и меньше 3 символов.")]
        public required string Title { get; set; }

        [StringLength(1100, ErrorMessage = "Максимальная длина описания - 1100 символов.")]
        public required string Description { get; set; }
        public required string[] Genres { get; set; }
        public required bool IsPublic { get; set; }
        public required bool IsDownloadsEnabled { get; set; }
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