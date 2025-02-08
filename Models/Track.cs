namespace Sounds_New.Models
{
    public class Track
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public required string Slug { get; set; }
        public string? Description { get; set; }
        public required string[] Genres { get; set; } = [];
        public required string AudioFilePath { get; set; }
        public required string ImageFilePath { get; set; }
        public bool IsPublic { get; set; } = false;
        public bool IsDownloadsEnabled { get; set; } = false;
        public int[] Peaks { get; set; } = [];
        public int Duration { get; set; } = 0;
        public int Listens { get; set; } = 0;
        public int Likes { get; set; } = 0;
        public int Downloads { get; set; } = 0;
        public int UserId { get; set; }
        public required User User { get; set; }
    }
}