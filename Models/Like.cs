using System.ComponentModel.DataAnnotations.Schema;

namespace Sounds_New.Models
{
    public class Like
    {
        public int Id { get; set; }
        public required int TrackId { get; set; }
        public required int UserId { get; set; }
        public Track Track { get; set; }
        public User User { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }
    }
}