using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using ChattingApp.Chats;
using ChattingApp.Messages;
using Microsoft.AspNetCore.Identity;

namespace ChattingApp.Users;

public class User : IdentityUser<Guid>
{
    public User()
    {
        Chats = new List<Chat>();
        Messages = new List<Message>();
    }
    
    [Required]
    [StringLength(30, MinimumLength = 1)]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(60, MinimumLength = 1)]
    public string LastName { get; set; }

    public IList<Chat> Chats { get; set; } 
    
    public IList<Message> Messages { get; set; }
}