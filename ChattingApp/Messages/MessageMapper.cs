namespace ChattingApp.Messages;

public static class MessageMapper
{
    public static Message CreateMessageDtoToMessage(this CreateMessageDto createMessageDto)
    {
        return new Message
        {
            Text = createMessageDto.Text,
            SentAt = DateTime.Now,
            ChatId = createMessageDto.ChatId,
        };
    }
    
    public static GetMessageDto MessageToGetMessageDto(this Message message)
    {
        return new GetMessageDto
        {
            Text = message.Text,
            SentByName = message.SentByName,
            SentAt = message.SentAt
        };
    }
}