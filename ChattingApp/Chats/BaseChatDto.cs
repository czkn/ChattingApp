using System.ComponentModel.DataAnnotations;

namespace ChattingApp.Chats;

public abstract class BaseChatDto
{
    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Name { get; set; }
}