using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Entities;

public class UserEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Username { get; set; } = null!;
    [Required]
    public string PasswordHash { get; set; } = null!;
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<TextChannelEntity> CreatedChannels { get; set; } = [];
    public ICollection<MessageEntity> Messages { get; set; } = [];
    public ICollection<ChannelMemberEntity> ChannelMemberships { get; set; } = [];

}
