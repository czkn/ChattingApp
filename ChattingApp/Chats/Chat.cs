using System.ComponentModel.DataAnnotations;
using ChattingApp.Messages;
using ChattingApp.Users;

namespace ChattingApp.Chats;

public class Chat
{
    public Chat()
    {
        Messages = new List<Message>();
        Users = new List<User>();
    }
    
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; }

    [Required]
    public IList<User> Users { get; set; } 
    
    public IList<Message> Messages { get; set; }
    
    [Required]
    public Guid OwnerId { get; set; }
    
    [Required]
    public bool CanPeopleJoin { get; set; }
}