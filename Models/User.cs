    namespace CoffeBotAPI.Model;
    using System.ComponentModel.DataAnnotations;

    public class User
        {
            public int Id { get; set; }
            [Required]
            public string Username { get; set; }
            public long TelegramId { get; set; } 
            public bool IsBarista { get; set; } = false;
            public bool IsAproved { get; set; } = false;
        }

