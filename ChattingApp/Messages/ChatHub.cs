using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using ChattingApp.Chats;
using static System.Guid;

namespace ChattingApp.Messages;

public class ChatHub : Hub
{
    private readonly ChattingAppDbContext _context;

    public ChatHub(ChattingAppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    public async Task JoinRoom(string roomName)
    {
        var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        TryParse(userId, out var userIdGuid);
        TryParse(roomName, out var roomNameGuid);
        
        var user = await _context.Users.FindAsync(userIdGuid);

        var chat = await _context.Chats.FindAsync(roomNameGuid);

        if (chat == null) return;

        if (user == null) return;

        var isUserInChat = _context.Set<Chat>()
            .Any(c => c.Users.Any(u => u.Id == userIdGuid) && c.Id == chat.Id);

        if (!isUserInChat) return;

        await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
    }
}