using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Guid;

namespace ChattingApp.Users;

[Route("api/[controller]")]
[ApiController]
public class UserController : Controller
{
    private readonly ChattingAppDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserController(UserManager<User> userManager, ChattingAppDbContext context, SignInManager<User> signInManager)
    {
        _userManager = userManager;
        _context = context;
        _signInManager = signInManager;
    }
    
    [Authorize]
    [HttpGet("{chatId}")]
    public async Task<ActionResult<IEnumerable<GetUserDto>>> GetChatUsers(Guid chatId)
    {
        var users = await _context.Users
            .Include(u => u.Chats)
            .ToListAsync();

        var chat = await _context.Chats.FindAsync(chatId);

        if (chat == null)
        {
            return NotFound("Chat not found");
        }
        
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        TryParse(userId, out var userIdGuid);

        var loggedInUser = await _context.Users.FindAsync(userIdGuid);

        if (loggedInUser == null)
        {
            return NotFound("User not found");
        }

        if (!chat.Users.Contains(loggedInUser))
        {
            return BadRequest("User is not a part of given chat");
        }

        var usersInChat = users.Where(user => user.Chats.Contains(chat));

        return usersInChat.Select(u => u.UserToGetUserDto()).ToList();
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerUserDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest("The model state is not valid");
        
        var userExists = await _userManager.FindByEmailAsync(registerUserDto.Email);
        if (userExists != null)
            return StatusCode(409, new { message = "User with same email already exists" });

        var user = registerUserDto.RegisterUserDtoToUser();

        var result = await _userManager.CreateAsync(user, registerUserDto.Password);

        if (!result.Succeeded) 
            return StatusCode(500, new { message = "Failed to create user" });

        return Created("", "User created successfully");
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto loginUserDto)
    {
        if (!ModelState.IsValid) 
            return BadRequest("The model state is not valid");
    
        var user = await _userManager.FindByEmailAsync(loginUserDto.Email);
        
        if (user == null)
            return Unauthorized(new { message = "Invalid email or password" });

        var isValidPassword = await _userManager.CheckPasswordAsync(user, loginUserDto.Password);
        
        if (!isValidPassword)
            return Unauthorized(new { message = "Invalid email or password" });
        
        await _signInManager.PasswordSignInAsync(user, loginUserDto.Password, false, false);
        
        return Ok(new { message = "Login successful" });
    }

    [Authorize]
    [HttpGet]
    [Route("IsUserLoggedIn")]
    public async Task<IActionResult> IsUserLoggedIn()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        TryParse(userId, out var userIdGuid);

        var loggedInUser = await _context.Users.FindAsync(userIdGuid);

        if (loggedInUser == null)
        {
            return NotFound("User not found");
        }

        return Ok();
    }
    
    [Authorize]
    [HttpPost]
    [Route("/logout")]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return Ok("Logged out successfully");
    }
}