using ChatApp.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<TextChannelEntity> TextChannels { get; set; }
    public DbSet<MessageEntity> Messages { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ChannelMemberEntity>()
            .HasKey(cm => new { cm.UserId, cm.ChannelId });

        modelBuilder.Entity<TextChannelEntity>()
            .HasOne(tc => tc.CreatedByUser)
            .WithMany(u => u.CreatedChannels)
            .HasForeignKey(tc => tc.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ChannelMemberEntity>()
            .Property(cm => cm.Role)
            .HasConversion<string>();

        modelBuilder.Entity<ChannelMemberEntity>()
            .Property(cm => cm.Status)
            .HasConversion<string>();
    }
}
