namespace Sounds_New.DTO
{
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
}