namespace ChatApp.Business.DTOs;

public class ChannelListDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public bool IsPrivate { get; set; }
    public int MemberCount { get; set; }
}
