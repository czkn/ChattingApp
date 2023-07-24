using System.ComponentModel.DataAnnotations;

namespace ChattingApp.Messages;

public class BaseMessageDto
{
    [Required]
    [StringLength(1000, MinimumLength = 1)]
    public string Text { get; set; }
}