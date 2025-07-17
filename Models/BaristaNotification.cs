namespace CoffeBotAPI.Model;

public class BaristaNotification
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public long ChatId { get; set; }
    public int MessageId { get; set; }
}
