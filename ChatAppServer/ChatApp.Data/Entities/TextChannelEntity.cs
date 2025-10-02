using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChatApp.Data.Entities;

public class TextChannelEntity
{
    [Key]
    public int Id { get; set; }
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    [Required]
    public bool IsPrivate { get; set; } = false;
    [Required]
    public int CreatedByUserId { get; set; }
    [ForeignKey("CreatedByUserId")]
    public UserEntity CreatedByUser { get; set; } = null!;
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<MessageEntity> Messages { get; set; } = [];
    public ICollection<ChannelMemberEntity> Members { get; set; } = [];
}
