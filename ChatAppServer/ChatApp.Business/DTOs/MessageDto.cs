namespace ChatApp.Business.DTOs;

public class MessageDto
{
    public int Id { get; set; }
    public string Text { get; set; } = null!;
    public string Username { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}
