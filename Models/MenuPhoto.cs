using System.ComponentModel.DataAnnotations;

namespace CoffeBotAPI.Model
{
    public class MenuPhoto
    {
        [Key]
        public int Id { get; set; }
        public string FileId { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}