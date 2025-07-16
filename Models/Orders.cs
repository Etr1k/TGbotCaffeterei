using System.ComponentModel.DataAnnotations;

namespace CoffeBotAPI.Model
{
    public enum StatusOrder
    {
        New,
        Inprogress,
        Done,
        Cancelled
    };
    public class Orders
    {
        
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        public long TelegramId { get; set; }
        [Required]
        public string Username { get; set; }    
        [Required]
        public string Text { get; set; }
        public StatusOrder Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}