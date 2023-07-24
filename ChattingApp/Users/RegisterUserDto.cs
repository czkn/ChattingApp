using System.ComponentModel.DataAnnotations;

namespace ChattingApp.Users;

public class RegisterUserDto : BaseUserDto
{
    [Required]
    [StringLength(100,MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+~-])[a-zA-Z\d!@#$%^&*()_+~-]{8,}$", 
        ErrorMessage = "Password must contain at least one lowercase letter, one uppercase letter, one digit, and one special character.")]
    public string? Password { get; set; }
    
    [Required]
    [StringLength(30, MinimumLength = 1, ErrorMessage = "First Name has to be 1-30 letters long")]
    public string FirstName { get; set; }
    
    [Required]
    [StringLength(60, MinimumLength = 1, ErrorMessage = "Last Name has to be 1-60 letters long")]
    public string LastName { get; set; }
}