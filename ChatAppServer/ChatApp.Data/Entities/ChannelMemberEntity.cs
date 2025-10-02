using ChatApp.Data.Enums;

namespace ChatApp.Data.Entities;

public class ChannelMemberEntity
{
    public int UserId { get; set; }
    public UserEntity User { get; set; } = null!;

    public int ChannelId { get; set; }
    public TextChannelEntity Channel { get; set; } = null!;

    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    public ChannelRole Role { get; set; } = ChannelRole.Member;
    public MembershipStatus Status { get; set; } = MembershipStatus.Pending;
}
