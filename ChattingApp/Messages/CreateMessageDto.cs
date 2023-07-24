using System.ComponentModel.DataAnnotations;

namespace ChattingApp.Messages;

public class CreateMessageDto : BaseMessageDto
{
    [Required]
    public Guid ChatId { get; set; }
}