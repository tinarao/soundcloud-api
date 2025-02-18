namespace Sounds_New.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public User User { get; set; }
        public Track Track { get; set; }
    }
}