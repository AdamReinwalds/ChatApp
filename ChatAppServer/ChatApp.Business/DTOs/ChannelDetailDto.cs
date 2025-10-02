using ChatApp.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Business.DTOs;

public class ChannelDetailDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public ICollection<MessageEntity> Messages { get; set; } = [];
    public ICollection<ChannelMemberEntity> Members { get; set; } = [];
    public bool IsPrivate { get; set; }
}
