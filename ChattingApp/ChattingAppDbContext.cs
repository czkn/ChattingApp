using ChattingApp.Chats;
using ChattingApp.Messages;
using ChattingApp.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChattingApp;

public class ChattingAppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public ChattingAppDbContext(DbContextOptions options) : base(options)
    {
        
    }
    
    public DbSet<Chat> Chats { get; set; }
    
    public DbSet<Message> Messages { get; set; }
}