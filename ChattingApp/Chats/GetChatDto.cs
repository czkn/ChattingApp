namespace ChattingApp.Chats;

public class GetChatDto : BaseChatDto
{
    public Guid Id { get; set; }

    public bool CanPeopleJoin { get; set; }
}