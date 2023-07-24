namespace ChattingApp.Messages;

public class GetMessageDto : BaseMessageDto
{
    public string SentByName { get; set; }
    
    public DateTime SentAt { get; set; }
}