using System.ComponentModel.DataAnnotations.Schema;

namespace Sounds_New.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public User User { get; set; }
        public Track Track { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
    }
}