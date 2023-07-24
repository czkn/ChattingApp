using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using static System.Guid;

namespace ChattingApp.Chats;

[Route("api/[controller]")]
[ApiController]
public class ChatController : Controller
{
    private readonly ChattingAppDbContext _context;

    public ChatController(ChattingAppDbContext context)
    {
        _context = context;
    }
    
    [Authorize]
    [HttpGet]
    [Route("YourChats")]
    public async Task<ActionResult<IEnumerable<GetChatDto>>> GetYourChats()
    {
        var chats = await _context.Chats
            .Include(c => c.Users)
            .ToListAsync();

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        TryParse(userId, out var userIdGuid);

        var user = await _context.Users.FindAsync(userIdGuid);

        if (user == null)
        {
            return NotFound("User Not Found");
        }

        var usersChats = chats.Where(chat => chat.Users.Contains(user));
        
        return usersChats.Select(x => x.ChatToGetChatDto()).ToList();
    }
    
    [Authorize]
    [HttpGet]
    [Route("OpenChats")]
    public async Task<ActionResult<IEnumerable<GetChatDto>>> GetOpenChats()
    {
        var chats = await _context.Chats.ToListAsync();

        var openChats = chats.Where(chat => chat.CanPeopleJoin);
        
        return openChats.Select(x => x.ChatToGetChatDto()).ToList();
    }
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CreateChatDto>> PostChat(CreateChatDto createChatDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest("The model state is not valid");
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        TryParse(userId, out var userIdGuid);

        var chat = createChatDto.CreateChatDtoToChat();

        chat.OwnerId = userIdGuid;

        chat.Users.AddRange(_context.Users.Where(user
           => user.Id == userIdGuid));

        _context.Chats.Add(chat);
        await _context.SaveChangesAsync();

        return Created("Created a new Chat", createChatDto);
    }

    [Authorize]
    [HttpPatch("{chatId}")]
    public async Task<ActionResult<PatchChatDto>> EditChat(Guid chatId)
    {
        if (!ModelState.IsValid) 
            return BadRequest("The model state is not valid");
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        TryParse(userId, out var userIdGuid);

        var chat = await _context.Chats.FindAsync(chatId);

        if (chat == null)
        {
            return NotFound("Chat not found");
        }

        if (userIdGuid != chat.OwnerId)
        {
            return BadRequest("You are not an Owner of this chat");
        }

        chat.CanPeopleJoin = !chat.CanPeopleJoin;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpPatch]
    [Route("JoinChat/{chatId}")]
    public async Task<ActionResult<PatchChatDto>> JoinChat(Guid chatId)
    {
        if (!ModelState.IsValid) 
            return BadRequest("The model state is not valid");
        
        var chat = await _context.Chats.FindAsync(chatId);

        if (chat == null)
        {
            return NotFound("Chat not found");
        }

        if (chat.CanPeopleJoin == false)
        {
            return BadRequest("This chat is private");
        }
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        TryParse(userId, out var userIdGuid);
        
        var isUserAlreadyInChat = _context.Set<Chat>()
            .Any(c => c.Users.Any(u => u.Id == userIdGuid) && c.Id == chat.Id);

        if (isUserAlreadyInChat)
        {
            return BadRequest("This user is already in the chat");
        }
        
        chat.Users.AddRange(_context.Users.Where(user
            => user.Id == userIdGuid));
        
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{chatId}")]
    public async Task<IActionResult> DeleteChat(Guid chatId)
    {
        var chat = await _context.Chats.FindAsync(chatId);

        if (chat == null)
        {
            return NotFound("Chat not found");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        TryParse(userId, out var userIdGuid);

        if (userIdGuid != chat.OwnerId)
        {
            return BadRequest("Only chat owner can delete a chat");
        }

        _context.Chats.Remove(chat);
        await _context.SaveChangesAsync();
        
        return NoContent();
    }
}