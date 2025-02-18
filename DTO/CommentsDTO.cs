using System.ComponentModel.DataAnnotations;

namespace Sounds_New.DTO
{
    public class CreateCommentDTO
    {
        [Required(ErrorMessage = "Текст комментария не указан")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Размер комментария не должен превышать 500 букв и должен содержать хотя бы один символ")]
        public required string Text { get; set; }

        [Required(ErrorMessage = "Трек не указан")]
        [Range(1, int.MaxValue, ErrorMessage = "Трек указан некорректно")]
        public int TrackId { get; set; }
    }
}