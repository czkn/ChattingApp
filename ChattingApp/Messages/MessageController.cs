using System.Diagnostics;
using System.Security.Claims;
using ChattingApp.Chats;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using static System.Guid;

namespace ChattingApp.Messages;

[Route("api/[controller]")]
[ApiController]
public class MessageController : Controller
{
    private readonly ChattingAppDbContext _context;
    private readonly IHubContext<ChatHub> _hubContext;

    public MessageController(ChattingAppDbContext context, IHubContext<ChatHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }
    
    [Authorize]
    [HttpGet("{chatId}")]
    public async Task<ActionResult<IEnumerable<GetMessageDto>>> GetMessages(Guid chatId)
    {
        var messages = await _context.Messages.ToListAsync();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        TryParse(userId, out var userIdGuid);

        var user = await _context.Users.FindAsync(userIdGuid);

        if (user == null)
        {
            return NotFound("User Not Found");
        }
        
        var isUserInChat = _context.Set<Chat>()
            .Any(c => c.Users.Any(u => u.Id == userIdGuid) && c.Id == chatId);

        if (!isUserInChat)
        {
            return BadRequest("User is not a part of the given Chat");
        }

        var chatsMessages = messages.Where(message => message.ChatId == chatId);

        return chatsMessages.Select(x => x.MessageToGetMessageDto()).ToList();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CreateMessageDto>> PostMessage(CreateMessageDto createMessageDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest("The model state is not valid");
            
        var chat = await _context.Chats.FindAsync(createMessageDto.ChatId);

        if (chat == null)
        {
            return BadRequest("You can not create a Message without a Chat");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        TryParse(userId, out var userIdGuid);

        var user = await _context.Users.FindAsync(userIdGuid);

        if (user == null)
        {
            return NotFound("Application user not found");
        }

        var isUserInChat = _context.Set<Chat>()
            .Any(c => c.Users.Any(u => u.Id == userIdGuid) && c.Id == chat.Id);

        if (!isUserInChat)
        {
            return BadRequest("User is not a part of the given Chat");
        }
        
        var message = createMessageDto.CreateMessageDtoToMessage();
        
        message.SentById = userIdGuid;
        message.SentByName = user.FirstName + " " + user.LastName;
        
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        var getMessageDto = message.MessageToGetMessageDto();

        await _hubContext.Clients.Group(chat.Id.ToString()).SendAsync("ReceiveMessage", getMessageDto);

        return Created("Created a new Message", createMessageDto);
    }
}