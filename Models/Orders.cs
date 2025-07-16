using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int SessionId { get; set; }                    // 🔗 связь с OrderSession
        public int LocalNumber { get; set; }                  // 📌 номер заказа в рамках сессии

        [ForeignKey("SessionId")]
        public OrderSession? session { get; set; }

    }
}