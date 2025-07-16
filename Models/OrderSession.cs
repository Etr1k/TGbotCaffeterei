using System.ComponentModel.DataAnnotations;

namespace CoffeBotAPI.Model
{
    public class OrderSession
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}