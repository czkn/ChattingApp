using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ChattingApp.Chats;
using ChattingApp.Users;

namespace ChattingApp.Messages;

public class Message
{
    public Guid Id { get; set; }
    
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; }
    
    [Required]
    public DateTime SentAt { get; set; }

    [Required]
    [ForeignKey(nameof(SentById))]
    public Guid SentById { get; set; } 
    
    public User SentBy { get; set; }
    
    [Required]
    public string SentByName { get; set; }

    [Required]
    [ForeignKey(nameof(ChatId))]
    public Guid ChatId { get; set; } 
    
    public Chat Chat { get; set; } 
}