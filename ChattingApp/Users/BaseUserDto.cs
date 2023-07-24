using System.ComponentModel.DataAnnotations;

namespace ChattingApp.Users;

public abstract class BaseUserDto
{
    [Required(ErrorMessage = "Email is required")]
    [StringLength(60, MinimumLength = 6, ErrorMessage = "Email has to be 6-60 letters long")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.(com|pl)$", ErrorMessage = "Invalid Email format")]
    public string? Email { get; set; }
}