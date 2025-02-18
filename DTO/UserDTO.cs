using System.ComponentModel.DataAnnotations;

namespace Sounds_New.DTO
{
    public class UserPrimaryDataDTO
    {
        public required string Username { get; set; }
        public string? Bio { get; set; }
        public string? Avatar { get; set; }
        public int TracksCount { get; set; }
        public int SubscribersCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserStatisticDTO : DefaultMethodResponseDTO
    {
        public int OverallListens { get; set; }
        public int OverallLikes { get; set; }
        public int TracksCount { get; set; }
        public float PublicTracksCount { get; set; }
        public float LikesPerListen { get; set; }
        public float ListensPerTrack { get; set; }
        public float LikesPerTrack { get; set; }
        public float SubscribersCount { get; set; }
        public float LikesPerSubscriber { get; set; }
        public float ListensPerSubscriber { get; set; }
    }

    public class UpdateUserDTO
    {
        [StringLength(100, ErrorMessage = "Слишком длинный псевдоним")]
        public string Username { get; set; }

        [StringLength(750, ErrorMessage = "Слишком длинное описание")]
        public string Bio { get; set; }

        [EmailAddress(ErrorMessage = "Некорректная электронная почта")]
        public string Email { get; set; }
    }
}