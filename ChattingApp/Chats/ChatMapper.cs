using ChattingApp.Users;

namespace ChattingApp.Chats;

public static class ChatMapper
{
    public static Chat CreateChatDtoToChat(this CreateChatDto createChatDto)
    {
        return new Chat
        {
            Name = createChatDto.Name,
            CanPeopleJoin = false,
        };
    }
    
    public static GetChatDto ChatToGetChatDto(this Chat chat)
    {
        return new GetChatDto
        {
            Id = chat.Id,
            Name = chat.Name,
            CanPeopleJoin = chat.CanPeopleJoin
        };
    }
}