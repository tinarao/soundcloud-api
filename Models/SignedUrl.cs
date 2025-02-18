namespace Sounds_New.Models
{
    public class SignedUrl
    {
        public int Id { get; set; }
        public required string FilePath { get; set; }
        public required string Url { get; set; }
    }
}