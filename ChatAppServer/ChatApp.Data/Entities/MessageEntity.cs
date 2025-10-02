using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Data.Entities;

public class MessageEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Content { get; set; } = null!;
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Required]
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public UserEntity User { get; set; } = null!;
    [Required]
    public int ChannelId { get; set; }
    [ForeignKey("ChannelId")]
    public TextChannelEntity Channel { get; set; } = null!;

}
