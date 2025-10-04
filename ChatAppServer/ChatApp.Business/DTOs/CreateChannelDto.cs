namespace ChatApp.Business.DTOs;

public class CreateChannelDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsPrivate { get; set; }
    public int CreatedByUserId { get; set; }
}
